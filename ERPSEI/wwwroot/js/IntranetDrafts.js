(function () {

    "use strict";

    const PREFIJO = "ERPSEI_DRAFT";
    const VERSION = 1;

    // 24 horas
    const TIEMPO_EXPIRACION =
        24 * 60 * 60 * 1000;

    // Espera después de escribir antes de guardar
    const DEBOUNCE_MS = 400;

    let timers = new WeakMap();

    // Formularios donde el usuario realmente comenzó a capturar.
    const formulariosTocados = new WeakSet();


    // =========================================================
    // USUARIO
    // =========================================================

    function obtenerUsuarioActual() {

        const elemento =
            document.getElementById(
                "intranetCurrentUser"
            );

        if (!elemento)
            return null;

        const userId =
            elemento.dataset.userId;

        if (!userId)
            return null;

        return userId;
    }


    // =========================================================
    // RUTA
    // =========================================================

    function obtenerRutaActual() {

        return (
            window.location.pathname
                .toLowerCase()
                .replace(/\/+$/, "") ||
            "/"
        );
    }


    // =========================================================
    // IDENTIFICADOR DEL FORMULARIO
    // =========================================================

    function obtenerIdentificadorFormulario(form) {

        if (
            form.dataset.draftKey &&
            form.dataset.draftKey.trim()
        ) {
            return form.dataset.draftKey.trim();
        }

        if (form.id) {
            return form.id;
        }

        return "formulario";
    }


    // =========================================================
    // CLAVE DE LOCAL STORAGE
    // =========================================================

    function construirClave(form) {

        const userId =
            obtenerUsuarioActual();

        if (!userId)
            return null;

        const ruta =
            obtenerRutaActual();

        const formulario =
            obtenerIdentificadorFormulario(
                form
            );

        return (
            `${PREFIJO}:` +
            `${userId}:` +
            `${ruta}:` +
            `${formulario}`
        );
    }


    // =========================================================
    // DETERMINAR SI UN CAMPO DEBE GUARDARSE
    // =========================================================

    function campoValido(campo) {

        if (!campo)
            return false;

        if (
            campo.dataset &&
            campo.dataset.noDraft === "true"
        ) {
            return false;
        }

        if (campo.disabled)
            return false;

        const tipo =
            (campo.type || "")
                .toLowerCase();

        if (
            tipo === "password" ||
            tipo === "file" ||
            tipo === "submit" ||
            tipo === "button" ||
            tipo === "reset" ||
            tipo === "hidden"
        ) {
            return false;
        }

        return !!(
            campo.name ||
            campo.id
        );
    }


    // =========================================================
    // ID DEL CAMPO
    // =========================================================

    function obtenerClaveCampo(campo) {

        return (
            campo.name ||
            campo.id
        );
    }


    // =========================================================
    // SERIALIZAR FORMULARIO
    // =========================================================

    function serializarFormulario(form) {

        const campos = {};

        const elementos =
            form.querySelectorAll(
                "input, select, textarea"
            );

        elementos.forEach(
            function (campo) {

                if (!campoValido(campo))
                    return;

                const clave =
                    obtenerClaveCampo(
                        campo
                    );

                if (!clave)
                    return;

                const tipo =
                    (campo.type || "")
                        .toLowerCase();


                if (
                    tipo === "checkbox" ||
                    tipo === "radio"
                ) {

                    campos[clave] = {
                        tipo: tipo,
                        valor: campo.value,
                        checked: campo.checked
                    };

                    return;
                }


                if (
                    campo.tagName === "SELECT" &&
                    campo.multiple
                ) {

                    campos[clave] = {
                        tipo: "select-multiple",
                        valor: Array.from(
                            campo.selectedOptions
                        ).map(
                            option =>
                                option.value
                        )
                    };

                    return;
                }


                campos[clave] = {

                    tipo:
                        campo.tagName
                            .toLowerCase(),

                    valor:
                        campo.value
                };
            }
        );

        return campos;
    }


    // =========================================================
    // ¿HAY INFORMACIÓN REAL?
    // =========================================================

    function tieneInformacion(campos) {

        return Object.values(
            campos
        ).some(
            function (item) {

                if (!item)
                    return false;


                if (
                    item.tipo === "checkbox" ||
                    item.tipo === "radio"
                ) {
                    return (
                        item.checked === true
                    );
                }


                if (
                    Array.isArray(
                        item.valor
                    )
                ) {
                    return (
                        item.valor.length > 0
                    );
                }


                return (
                    item.valor !== null &&
                    item.valor !== undefined &&
                    String(
                        item.valor
                    ).trim() !== ""
                );
            }
        );
    }


    // =========================================================
    // GUARDAR
    // =========================================================

    function guardarFormulario(form) {

        if (!form)
            return;


        // =====================================================
        // NO CREAR BORRADORES SI EL USUARIO
        // NUNCA HA INTERACTUADO CON EL FORMULARIO
        // =====================================================

        const yaTeniaBorrador =
            form.dataset.draftRestored ===
            "true";


        if (
            !formulariosTocados.has(
                form
            ) &&
            !yaTeniaBorrador
        ) {
            return;
        }


        const clave =
            construirClave(
                form
            );

        if (!clave)
            return;


        const campos =
            serializarFormulario(
                form
            );


        // Si realmente ya no hay información,
        // eliminamos el borrador.

        if (
            !tieneInformacion(
                campos
            )
        ) {

            localStorage.removeItem(
                clave
            );

            return;
        }


        const borrador = {

            version:
                VERSION,

            guardadoEn:
                new Date()
                    .toISOString(),

            expiracion:
                Date.now() +
                TIEMPO_EXPIRACION,

            url:
                window.location.pathname +
                window.location.search,

            formulario:
                obtenerIdentificadorFormulario(
                    form
                ),

            // Este indicador permite distinguir
            // borradores reales de valores iniciales
            // creados automáticamente por JavaScript.
            interaccionUsuario:
                true,

            campos:
                campos
        };


        try {

            localStorage.setItem(
                clave,
                JSON.stringify(
                    borrador
                )
            );

        } catch (error) {

            console.warn(
                "No fue posible guardar el borrador:",
                error
            );
        }
    }


    // =========================================================
    // GUARDAR CON DEBOUNCE
    // =========================================================

    function programarGuardado(form) {

        const anterior =
            timers.get(
                form
            );

        if (anterior) {

            clearTimeout(
                anterior
            );
        }


        const timer =
            setTimeout(
                function () {

                    guardarFormulario(
                        form
                    );

                },
                DEBOUNCE_MS
            );


        timers.set(
            form,
            timer
        );
    }


    // =========================================================
    // LOCALIZAR CAMPO
    // =========================================================

    function buscarCampo(
        form,
        clave
    ) {

        const porNombre =
            Array.from(
                form.elements
            ).filter(
                x =>
                    x.name === clave
            );


        if (
            porNombre.length > 0
        ) {
            return porNombre;
        }


        const porId =
            form.querySelector(
                `#${CSS.escape(clave)}`
            );


        return porId
            ? [porId]
            : [];
    }


    // =========================================================
    // DISPARAR EVENTOS
    // =========================================================

    function dispararEventos(
        campo
    ) {

        campo.dispatchEvent(
            new Event(
                "input",
                {
                    bubbles: true
                }
            )
        );


        campo.dispatchEvent(
            new Event(
                "change",
                {
                    bubbles: true
                }
            )
        );
    }


    // =========================================================
    // RESTAURAR
    // =========================================================

    function restaurarFormulario(form) {

        if (!form)
            return false;


        const clave =
            construirClave(
                form
            );

        if (!clave)
            return false;


        const contenido =
            localStorage.getItem(
                clave
            );

        if (!contenido)
            return false;


        let borrador;


        try {

            borrador =
                JSON.parse(
                    contenido
                );

        } catch {

            localStorage.removeItem(
                clave
            );

            return false;
        }


        // =====================================================
        // DESCARTAR BORRADORES ANTIGUOS / FALSOS
        // =====================================================
        //
        // Antes de esta versión podían existir borradores
        // generados solamente porque el formulario tenía
        // valores predeterminados.
        //
        // Solamente aceptamos borradores que indiquen que
        // existió interacción real del usuario.
        // =====================================================

        if (
            borrador?.interaccionUsuario !==
            true
        ) {

            localStorage.removeItem(
                clave
            );

            return false;
        }


        // =====================================================
        // VALIDAR EXPIRACIÓN
        // =====================================================

        if (
            !borrador ||
            !borrador.expiracion ||
            Date.now() >
            borrador.expiracion
        ) {

            localStorage.removeItem(
                clave
            );

            return false;
        }


        if (!borrador.campos)
            return false;


        // =====================================================
        // RESTAURAR CAMPOS
        // =====================================================

        Object.entries(
            borrador.campos
        ).forEach(
            function (
                [
                    claveCampo,
                    dato
                ]
            ) {

                const campos =
                    buscarCampo(
                        form,
                        claveCampo
                    );


                if (
                    campos.length === 0
                ) {
                    return;
                }


                campos.forEach(
                    function (campo) {

                        if (
                            !campoValido(
                                campo
                            )
                        ) {
                            return;
                        }


                        const tipo =
                            (
                                campo.type ||
                                ""
                            ).toLowerCase();


                        // =============================================
                        // CHECKBOX
                        // =============================================

                        if (
                            tipo ===
                            "checkbox"
                        ) {

                            campo.checked =
                                !!dato.checked;

                            dispararEventos(
                                campo
                            );

                            return;
                        }


                        // =============================================
                        // RADIO
                        // =============================================

                        if (
                            tipo ===
                            "radio"
                        ) {

                            campo.checked =
                                dato.checked &&
                                campo.value ===
                                dato.valor;

                            dispararEventos(
                                campo
                            );

                            return;
                        }


                        // =============================================
                        // SELECT MULTIPLE
                        // =============================================

                        if (
                            campo.tagName ===
                            "SELECT" &&
                            campo.multiple &&
                            Array.isArray(
                                dato.valor
                            )
                        ) {

                            Array.from(
                                campo.options
                            ).forEach(
                                option => {

                                    option.selected =
                                        dato.valor
                                            .includes(
                                                option.value
                                            );
                                }
                            );


                            dispararEventos(
                                campo
                            );

                            return;
                        }


                        // =============================================
                        // INPUT / SELECT / TEXTAREA
                        // =============================================

                        campo.value =
                            dato.valor ??
                            "";


                        dispararEventos(
                            campo
                        );
                    }
                );
            }
        );


        // =====================================================
        // MARCAR COMO RESTAURADO
        // =====================================================

        form.dataset.draftRestored =
            "true";


        return true;
    }


    // =========================================================
    // ELIMINAR BORRADOR
    // =========================================================

    function eliminarFormulario(form) {

        if (!form)
            return;


        const clave =
            construirClave(
                form
            );

        if (!clave)
            return;


        // =====================================================
        // ELIMINAR LOCAL STORAGE
        // =====================================================

        localStorage.removeItem(
            clave
        );


        // =====================================================
        // CANCELAR DEBOUNCE PENDIENTE
        // =====================================================
        //
        // Esto evita que después de guardar correctamente
        // exista todavía un setTimeout pendiente que vuelva
        // a crear el borrador.
        // =====================================================

        const timerPendiente =
            timers.get(
                form
            );


        if (timerPendiente) {

            clearTimeout(
                timerPendiente
            );


            timers.delete(
                form
            );
        }


        // =====================================================
        // EL FORMULARIO DEJA DE CONSIDERARSE MODIFICADO
        // =====================================================
        //
        // Esto también evita que pagehide / saveAll()
        // vuelva a guardar el formulario después de que
        // el servidor confirmó el guardado.
        // =====================================================

        formulariosTocados.delete(
            form
        );


        // =====================================================
        // ELIMINAR BANDERA DE RESTAURACIÓN
        // =====================================================

        delete form.dataset
            .draftRestored;
    }


    // =========================================================
    // GUARDAR TODOS
    // =========================================================

    function guardarTodos() {

        document
            .querySelectorAll(
                'form[data-autosave-draft="true"]'
            )
            .forEach(
                guardarFormulario
            );
    }


    // =========================================================
    // LIMPIAR BORRADORES VENCIDOS / ANTIGUOS
    // =========================================================

    function limpiarExpirados() {

        const userId =
            obtenerUsuarioActual();


        if (!userId)
            return;


        const inicio =
            `${PREFIJO}:${userId}:`;


        for (
            let i =
                localStorage.length - 1;
            i >= 0;
            i--
        ) {

            const clave =
                localStorage.key(
                    i
                );


            if (
                !clave ||
                !clave.startsWith(
                    inicio
                )
            ) {
                continue;
            }


            try {

                const contenido =
                    localStorage.getItem(
                        clave
                    );


                const item =
                    JSON.parse(
                        contenido
                    );


                // =============================================
                // ELIMINAR SI:
                //
                // - Está corrupto
                // - Es un borrador antiguo sin interacción
                // - No tiene expiración
                // - Ya expiró
                // =============================================

                if (
                    !item ||
                    item.interaccionUsuario !==
                    true ||
                    !item.expiracion ||
                    Date.now() >
                    item.expiracion
                ) {

                    localStorage.removeItem(
                        clave
                    );
                }

            } catch {

                localStorage.removeItem(
                    clave
                );
            }
        }
    }


    // =========================================================
    // INICIALIZAR FORMULARIO
    // =========================================================

    function inicializarFormulario(
        form
    ) {

        if (
            form.dataset
                .draftInitialized ===
            "true"
        ) {
            return;
        }


        form.dataset
            .draftInitialized =
            "true";


        // =====================================================
        // REGISTRAR INTERACCIÓN REAL DEL USUARIO
        // =====================================================

        function registrarInteraccionUsuario(
            event
        ) {

            // =============================================
            // Ignorar eventos disparados por JavaScript.
            //
            // Esto es fundamental porque durante:
            //
            // - inicialización del módulo
            // - carga de selects
            // - restauración de borradores
            // - asignación programática
            //
            // pueden producirse input/change artificiales.
            // =============================================

            if (!event.isTrusted) {
                return;
            }


            formulariosTocados.add(
                form
            );


            programarGuardado(
                form
            );
        }


        // =====================================================
        // INPUT
        // =====================================================

        form.addEventListener(
            "input",
            registrarInteraccionUsuario
        );


        // =====================================================
        // CHANGE
        // =====================================================

        form.addEventListener(
            "change",
            registrarInteraccionUsuario
        );


        // =====================================================
        // INTENTAR RESTAURACIÓN
        // =====================================================

        const restaurado =
            restaurarFormulario(
                form
            );


        // =====================================================
        // AVISAR AL MÓDULO
        // =====================================================

        if (restaurado) {

            document.dispatchEvent(
                new CustomEvent(
                    "erpsei:draft-restored",
                    {
                        detail: {
                            form:
                                form
                        }
                    }
                )
            );
        }
    }


    // =========================================================
    // INICIALIZAR TODO
    // =========================================================

    function inicializar() {

        // Primero eliminamos borradores:
        //
        // - vencidos
        // - corruptos
        // - antiguos sin interaccionUsuario

        limpiarExpirados();


        document
            .querySelectorAll(
                'form[data-autosave-draft="true"]'
            )
            .forEach(
                inicializarFormulario
            );
    }


    // =========================================================
    // API GLOBAL
    // =========================================================

    window.IntranetDrafts = {

        // Inicialización manual
        init:
            inicializar,


        // Guardar formulario específico
        save:
            guardarFormulario,


        // Guardar todos
        saveAll:
            guardarTodos,


        // Restaurar formulario específico
        restore:
            restaurarFormulario,


        // Eliminar formulario específico
        clear:
            eliminarFormulario,


        // Eliminar mediante ID
        clearById:
            function (
                formId
            ) {

                const form =
                    document.getElementById(
                        formId
                    );


                if (form) {

                    eliminarFormulario(
                        form
                    );
                }
            }
    };


    // =========================================================
    // ARRANQUE
    // =========================================================

    document.addEventListener(
        "DOMContentLoaded",
        inicializar
    );


    // =========================================================
    // GUARDADO ADICIONAL AL SALIR / ACTUALIZAR
    // =========================================================
    //
    // guardarFormulario() ya verifica que realmente
    // haya existido interacción del usuario.
    //
    // Por lo tanto un F5 sin haber tocado ningún formulario
    // NO crea ningún borrador.
    // =========================================================

    window.addEventListener(
        "pagehide",
        guardarTodos
    );

})();