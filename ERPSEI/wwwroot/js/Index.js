console.log("ADQUISICIONES JS CARGADO");

document.addEventListener(
    "DOMContentLoaded",
    function () {

        // =========================================================
        // DECISIÓN DEL GERENTE
        // =========================================================

        const modalDecisionGerenteElement =
            document.getElementById(
                "modalDecisionGerenteAdq"
            );

        const inputSolicitudDecision =
            document.getElementById(
                "SolicitudDecisionId"
            );

        const inputComentarioDecision =
            document.getElementById(
                "ComentarioDecision"
            );

        const tituloDecisionGerente =
            document.getElementById(
                "tituloDecisionGerenteAdq"
            );

        const folioDecisionGerente =
            document.getElementById(
                "folioDecisionGerenteAdq"
            );

        const mensajeDecisionGerente =
            document.getElementById(
                "mensajeDecisionGerenteAdq"
            );

        const labelComentarioDecision =
            document.getElementById(
                "labelComentarioDecisionAdq"
            );

        const ayudaComentarioDecision =
            document.getElementById(
                "ayudaComentarioDecisionAdq"
            );

        const btnConfirmarAprobacion =
            document.getElementById(
                "btnConfirmarAprobacionAdq"
            );

        const btnConfirmarRechazo =
            document.getElementById(
                "btnConfirmarRechazoAdq"
            );

        const btnEditarDesdeDetalle =
            document.getElementById(
                "btnEditarDesdeDetalleAdq"
            );


        const btnCancelarDesdeDetalle =
            document.getElementById(
                "btnCancelarDesdeDetalleAdq"
            );


        const btnEnviarDesdeDetalle =
            document.getElementById(
                "btnEnviarDesdeDetalleAdq"
            );

        let solicitudDetalleActualAdq =
            null;

        const badgeMensajesPendientesAdq =
            document.getElementById(
                "badgeMensajesPendientesAdq"
            );

        let intervaloSeguimientoAdq =
            null;

        // =========================================================
        // CHAT / HISTORIAL
        // =========================================================

        const itemTabSeguimientoAdq =
            document.getElementById(
                "itemTabSeguimientoAdq"
            );


        const itemTabHistorialAdq =
            document.getElementById(
                "itemTabHistorialAdq"
            );


        const listaComentariosAdq =
            document.getElementById(
                "listaComentariosAdq"
            );


        const listaHistorialAdq =
            document.getElementById(
                "listaHistorialAdq"
            );


        const inputSolicitudComentarioAdq =
            document.getElementById(
                "SolicitudComentarioId"
            );


        const inputNuevoComentarioAdq =
            document.getElementById(
                "NuevoComentarioAdq"
            );


        const btnEnviarComentarioAdq =
            document.getElementById(
                "btnEnviarComentarioAdq"
            );


        const seguimientoEstatusAdq =
            document.getElementById(
                "seguimientoEstatusAdq"
            );


        const formComentarioAdq =
            document.getElementById(
                "formComentarioAdq"
            );

        // =========================================================
        // ARCHIVOS DEL CHAT
        // =========================================================

        const btnSeleccionarAdjuntoComentarioAdq =
            document.getElementById(
                "btnSeleccionarAdjuntoComentarioAdq"
            );


        const inputArchivoComentarioAdq =
            document.getElementById(
                "archivoComentarioAdq"
            );


        const listaAdjuntosComentarioAdq =
            document.getElementById(
                "listaAdjuntosComentarioAdq"
            );

        let archivosComentarioSeleccionadosAdq =
            [];

        // =========================================================
        // COTIZACIONES
        // =========================================================

        const modalCotizacionElement =
            document.getElementById(
                "modalCotizacionAdq"
            );


        const formCotizacionAdq =
            document.getElementById(
                "formCotizacionAdq"
            );


        const cotizacionSolicitudIdAdq =
            document.getElementById(
                "cotizacionSolicitudIdAdq"
            );


        const folioCotizacionAdq =
            document.getElementById(
                "folioCotizacionAdq"
            );


        const contenedorDetallesCotizacionAdq =
            document.getElementById(
                "contenedorDetallesCotizacionAdq"
            );


        const cotizacionAplicaIvaAdq =
            document.getElementById(
                "cotizacionAplicaIvaAdq"
            );


        const cotizacionPorcentajeIvaAdq =
            document.getElementById(
                "cotizacionPorcentajeIvaAdq"
            );


        const cotizacionSubtotalAdq =
            document.getElementById(
                "cotizacionSubtotalAdq"
            );


        const cotizacionIvaAdq =
            document.getElementById(
                "cotizacionIvaAdq"
            );


        const cotizacionTotalAdq =
            document.getElementById(
                "cotizacionTotalAdq"
            );


        const cotizacionIvaLabelAdq =
            document.getElementById(
                "cotizacionIvaLabelAdq"
            );

        const seccionCotizacionesRegistradasAdq =
            document.getElementById(
                "seccionCotizacionesRegistradasAdq"
            );


        const listaCotizacionesRegistradasAdq =
            document.getElementById(
                "listaCotizacionesRegistradasAdq"
            );


        const contadorCotizacionesAdq =
            document.getElementById(
                "contadorCotizacionesAdq"
            );

        const accionesSeleccionCotizacionesAdq =
            document.getElementById(
                "accionesSeleccionCotizacionesAdq"
            );


        const contadorSeleccionCotizacionesAdq =
            document.getElementById(
                "contadorSeleccionCotizacionesAdq"
            );


        const btnEditarCotizacionSeleccionadaAdq =
            document.getElementById(
                "btnEditarCotizacionSeleccionadaAdq"
            );


        const btnEliminarCotizacionesSeleccionadasAdq =
            document.getElementById(
                "btnEliminarCotizacionesSeleccionadasAdq"
            );


        const cotizacionEditarIdAdq =
            document.getElementById(
                "cotizacionEditarIdAdq"
            );


        const tituloCapturaCotizacionAdq =
            document.getElementById(
                "tituloCapturaCotizacionAdq"
            );


        const subtituloCapturaCotizacionAdq =
            document.getElementById(
                "subtituloCapturaCotizacionAdq"
            );


        const badgeEdicionCotizacionAdq =
            document.getElementById(
                "badgeEdicionCotizacionAdq"
            );


        const btnCancelarEdicionCotizacionAdq =
            document.getElementById(
                "btnCancelarEdicionCotizacionAdq"
            );


        const textoGuardarCotizacionAdq =
            document.getElementById(
                "textoGuardarCotizacionAdq"
            );


        const btnAgregarProveedorAlternativoAdq =
            document.getElementById(
                "btnAgregarProveedorAlternativoAdq"
            );

        const btnFinalizarCotizacionAdq =
            document.getElementById(
                "btnFinalizarCotizacionAdq"
            );


        const seccionCapturaProveedorAdq =
            document.getElementById(
                "seccionCapturaProveedorAdq"
            );


        let solicitudCotizacionActualAdq =
            null;

        let cotizacionesRegistradasActualesAdq =
            [];


        let cotizacionEditandoActualAdq =
            null;

        const archivosCotizacionAdq =
            document.getElementById(
                "ArchivosCotizacionAdq"
            );


        const listaArchivosCotizacionAdq =
            document.getElementById(
                "listaArchivosCotizacionAdq"
            );


        const contadorArchivosCotizacionAdq =
            document.getElementById(
                "contadorArchivosCotizacionAdq"
            );


        let archivosCotizacionSeleccionadosAdq =
            [];

        let archivosCotizacionEliminadosAdq =
            new Set();


        const archivosCotizacionEliminarInputsAdq =
            document.getElementById(
                "archivosCotizacionEliminarInputsAdq"
            );

        // =========================================================
        // DEFINIR HANDLER AL GUARDAR COTIZACIÓN
        // =========================================================

        formCotizacionAdq
            ?.addEventListener(
                "submit",
                function () {

                    const cotizacionEditarId =
                        Number(
                            cotizacionEditarIdAdq
                                ?.value
                            ??
                            0
                        );


                    if (
                        cotizacionEditarId >
                        0
                    ) {

                        formCotizacionAdq.action =
                            `${window.location.pathname}?handler=EditarCotizacion`;
                    }
                    else {

                        formCotizacionAdq.action =
                            `${window.location.pathname}?handler=GuardarCotizacion`;
                    }
                }
            );

        const modalPresupuestoElementAdq =
            document.getElementById(
                "modalPresupuestoAdq"
            );

        const presupuestoSolicitudIdAdq =
            document.getElementById(
                "presupuestoSolicitudIdAdq"
            );

        const folioPresupuestoAdq =
            document.getElementById(
                "folioPresupuestoAdq"
            );

        const proveedorPresupuestoAdq =
            document.getElementById(
                "proveedorPresupuestoAdq"
            );

        const subtotalPresupuestoAdq =
            document.getElementById(
                "subtotalPresupuestoAdq"
            );

        const ivaPresupuestoAdq =
            document.getElementById(
                "ivaPresupuestoAdq"
            );

        const totalPresupuestoAdq =
            document.getElementById(
                "totalPresupuestoAdq"
            );

        const comentarioPresupuestoAdq =
            document.getElementById(
                "comentarioPresupuestoAdq"
            );

        const btnConfirmarPresupuestoAdq =
            document.getElementById(
                "btnConfirmarPresupuestoAdq"
            );

        let solicitudPresupuestoActualAdq = null;

        // =========================================================
        // APROBACIÓN PRESUPUESTAL
        // =========================================================

        const modalAprobacionPresupuestalElementAdq =
            document.getElementById(
                "modalAprobacionPresupuestalAdq"
            );


        const detalleAprobacionPresupuestalIdAdq =
            document.getElementById(
                "detalleAprobacionPresupuestalIdAdq"
            );


        const solicitudAprobacionPresupuestalIdAdq =
            document.getElementById(
                "solicitudAprobacionPresupuestalIdAdq"
            );


        const folioAprobacionPresupuestalAdq =
            document.getElementById(
                "folioAprobacionPresupuestalAdq"
            );


        const tituloAprobacionPresupuestalAdq =
            document.getElementById(
                "tituloAprobacionPresupuestalAdq"
            );


        const etapaAprobacionPresupuestalAdq =
            document.getElementById(
                "etapaAprobacionPresupuestalAdq"
            );


        const montoAprobacionPresupuestalAdq =
            document.getElementById(
                "montoAprobacionPresupuestalAdq"
            );


        const solicitanteAprobacionPresupuestalAdq =
            document.getElementById(
                "solicitanteAprobacionPresupuestalAdq"
            );


        const areaAprobacionPresupuestalAdq =
            document.getElementById(
                "areaAprobacionPresupuestalAdq"
            );


        const proveedorAprobacionPresupuestalAdq =
            document.getElementById(
                "proveedorAprobacionPresupuestalAdq"
            );


        const comentarioSolicitudPresupuestalAdq =
            document.getElementById(
                "comentarioSolicitudPresupuestalAdq"
            );


        const comentarioDecisionPresupuestalAdq =
            document.getElementById(
                "comentarioDecisionPresupuestalAdq"
            );


        const btnVerSolicitudDesdeAprobacionAdq =
            document.getElementById(
                "btnVerSolicitudDesdeAprobacionAdq"
            );


        const btnVerChatDesdeAprobacionAdq =
            document.getElementById(
                "btnVerChatDesdeAprobacionAdq"
            );


        const btnAprobarAprobacionPresupuestalAdq =
            document.getElementById(
                "btnAprobarAprobacionPresupuestalAdq"
            );


        const btnDeclinarAprobacionPresupuestalAdq =
            document.getElementById(
                "btnDeclinarAprobacionPresupuestalAdq"
            );


        let modalAprobacionPresupuestalAdq =
            null;

        function abrirModalAprobacionPresupuestalAdq(
            boton
        ) {

            if (
                !modalAprobacionPresupuestalElementAdq
            ) {
                return;
            }


            const solicitudId =
                Number(
                    boton.dataset.solicitudId
                    ??
                    0
                );


            const detalleId =
                Number(
                    boton.dataset.detalleId
                    ??
                    0
                );


            if (
                solicitudId <= 0
                ||
                detalleId <= 0
            ) {
                return;
            }


            if (
                detalleAprobacionPresupuestalIdAdq
            ) {
                detalleAprobacionPresupuestalIdAdq.value =
                    String(
                        detalleId
                    );
            }


            if (
                solicitudAprobacionPresupuestalIdAdq
            ) {
                solicitudAprobacionPresupuestalIdAdq.value =
                    String(
                        solicitudId
                    );
            }


            if (
                folioAprobacionPresupuestalAdq
            ) {
                folioAprobacionPresupuestalAdq.textContent =
                    boton.dataset.folio
                    ??
                    "-";
            }


            if (
                tituloAprobacionPresupuestalAdq
            ) {
                tituloAprobacionPresupuestalAdq.textContent =
                    boton.dataset.titulo
                    ??
                    "-";
            }


            if (
                etapaAprobacionPresupuestalAdq
            ) {

                const orden =
                    boton.dataset.orden
                    ??
                    "";


                const etapa =
                    boton.dataset.etapa
                    ??
                    "-";


                etapaAprobacionPresupuestalAdq.textContent =
                    orden
                        ? `Nivel ${orden} · ${etapa}`
                        : etapa;
            }


            if (
                montoAprobacionPresupuestalAdq
            ) {

                montoAprobacionPresupuestalAdq.textContent =
                    formatearMonedaAdq(
                        Number(
                            boton.dataset.monto
                            ??
                            0
                        )
                    );
            }


            if (
                solicitanteAprobacionPresupuestalAdq
            ) {
                solicitanteAprobacionPresupuestalAdq.textContent =
                    boton.dataset.solicitante
                    ??
                    "-";
            }


            if (
                areaAprobacionPresupuestalAdq
            ) {
                areaAprobacionPresupuestalAdq.textContent =
                    boton.dataset.area
                    ??
                    "-";
            }


            if (
                proveedorAprobacionPresupuestalAdq
            ) {
                proveedorAprobacionPresupuestalAdq.textContent =
                    boton.dataset.proveedor
                    ??
                    "-";
            }


            if (
                comentarioSolicitudPresupuestalAdq
            ) {

                comentarioSolicitudPresupuestalAdq.textContent =
                    boton.dataset.comentario
                    ??
                    "Sin comentarios.";
            }


            if (
                comentarioDecisionPresupuestalAdq
            ) {
                comentarioDecisionPresupuestalAdq.value =
                    "";
            }


            modalAprobacionPresupuestalAdq =
                bootstrap.Modal.getOrCreateInstance(
                    modalAprobacionPresupuestalElementAdq
                );


            modalAprobacionPresupuestalAdq.show();
        }

        document.addEventListener(
            "click",
            function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnVerAprobacionPresupuestalAdq, " +
                        ".btnAprobarPresupuestoAdq, " +
                        ".btnDeclinarPresupuestoAdq"
                    );


                if (!boton) {
                    return;
                }


                abrirModalAprobacionPresupuestalAdq(
                    boton
                );
            }
        );

        btnVerSolicitudDesdeAprobacionAdq
            ?.addEventListener(
                "click",
                function () {

                    const solicitudId =
                        Number(
                            solicitudAprobacionPresupuestalIdAdq
                                ?.value
                            ??
                            0
                        );


                    if (
                        solicitudId <= 0
                    ) {
                        return;
                    }


                    const botonVer =
                        document.querySelector(
                            `.btnVerSolicitudAdq[data-id="${solicitudId}"]`
                        );


                    botonVer?.click();
                }
            );


        btnVerChatDesdeAprobacionAdq
            ?.addEventListener(
                "click",
                function () {

                    const solicitudId =
                        Number(
                            solicitudAprobacionPresupuestalIdAdq
                                ?.value
                            ??
                            0
                        );


                    if (
                        solicitudId <= 0
                    ) {
                        return;
                    }


                    const botonVer =
                        document.querySelector(
                            `.btnVerSolicitudAdq[data-id="${solicitudId}"]`
                        );


                    if (
                        !botonVer
                    ) {
                        return;
                    }


                    botonVer.click();


                    setTimeout(
                        function () {

                            const tabSeguimiento =
                                document.querySelector(
                                    "#tabSeguimientoAdq"
                                );


                            tabSeguimiento?.click();

                        },
                        350
                    );
                }
        );

        // =========================================================
        // ENVIAR DECISIÓN PRESUPUESTAL
        // =========================================================

        async function enviarDecisionPresupuestalAdq(
            decision
        ) {

            const detalleId =
                Number(
                    detalleAprobacionPresupuestalIdAdq
                        ?.value
                    ??
                    0
                );


            if (
                detalleId <= 0
            ) {

                mostrarAdvertenciaAdq(
                    "Aprobación no válida",
                    "No fue posible identificar la aprobación presupuestal."
                );


                return;
            }


            const comentario =
                comentarioDecisionPresupuestalAdq
                    ?.value
                    ?.trim()
                ??
                "";


            if (
                comentario.length >
                3000
            ) {

                mostrarAdvertenciaAdq(
                    "Comentario demasiado largo",
                    "El comentario no puede superar los 3000 caracteres."
                );


                comentarioDecisionPresupuestalAdq
                    ?.focus();


                return;
            }


            const esAprobacion =
                decision ===
                "APROBAR";


            const confirmado =
                await confirmarAccionAdq(
                    {
                        titulo:
                            esAprobacion
                                ? "Aprobar presupuesto"
                                : "Declinar presupuesto",

                        mensaje:
                            esAprobacion
                                ? `
                            <p class="mb-0">
                                ¿Confirmas la aprobación de esta etapa presupuestal?
                            </p>
                          `
                                : `
                            <p class="mb-0">
                                ¿Confirmas que deseas declinar esta etapa presupuestal?
                            </p>
                          `,

                        textoConfirmar:
                            esAprobacion
                                ? "Aprobar"
                                : "Declinar",

                        textoCancelar:
                            "Cancelar",

                        tipo:
                            esAprobacion
                                ? "success"
                                : "danger",

                        icono:
                            esAprobacion
                                ? "bi-check-circle"
                                : "bi-x-circle"
                    }
                );


            if (
                !confirmado
            ) {
                return;
            }


            const boton =
                esAprobacion
                    ? btnAprobarAprobacionPresupuestalAdq
                    : btnDeclinarAprobacionPresupuestalAdq;


            const htmlOriginal =
                boton?.innerHTML;


            if (
                boton
            ) {

                boton.disabled =
                    true;


                boton.innerHTML =
                    `
                <span class="spinner-border spinner-border-sm me-1"></span>
                Procesando...
            `;
            }


            if (
                btnAprobarAprobacionPresupuestalAdq
            ) {
                btnAprobarAprobacionPresupuestalAdq.disabled =
                    true;
            }


            if (
                btnDeclinarAprobacionPresupuestalAdq
            ) {
                btnDeclinarAprobacionPresupuestalAdq.disabled =
                    true;
            }


            try {

                const token =
                    document.querySelector(
                        'input[name="__RequestVerificationToken"]'
                    )
                        ?.value
                    ??
                    "";


                const formData =
                    new FormData();


                formData.append(
                    "__RequestVerificationToken",
                    token
                );


                formData.append(
                    "detalleId",
                    String(
                        detalleId
                    )
                );


                formData.append(
                    "decision",
                    decision
                );


                formData.append(
                    "comentario",
                    comentario
                );


                const respuesta =
                    await fetch(
                        `${window.location.pathname}?handler=DecisionPresupuestal`,
                        {
                            method:
                                "POST",

                            headers:
                            {
                                "X-Requested-With":
                                    "XMLHttpRequest"
                            },

                            body:
                                formData
                        }
                    );


                let resultado =
                    null;


                try {

                    resultado =
                        await respuesta.json();

                }
                catch {

                    resultado =
                    {
                        success:
                            false,

                        message:
                            "El servidor devolvió una respuesta no válida."
                    };
                }


                if (
                    !respuesta.ok
                    ||
                    !resultado?.success
                ) {

                    throw new Error(
                        resultado?.message
                        ??
                        "No fue posible procesar la aprobación presupuestal."
                    );
                }


                modalAprobacionPresupuestalAdq
                    ?.hide();


                mostrarAdvertenciaAdq(
                    esAprobacion
                        ? "Aprobación registrada"
                        : "Aprobación declinada",

                    resultado.message
                    ??
                    (
                        esAprobacion
                            ? "La etapa presupuestal fue aprobada correctamente."
                            : "La etapa presupuestal fue declinada."
                    )
                );


                setTimeout(
                    function () {

                        window.location.reload();

                    },
                    900
                );
            }
            catch (
            error
            ) {

                console.error(
                    error
                );


                mostrarAdvertenciaAdq(
                    "No fue posible procesar la aprobación",
                    error.message
                    ??
                    "Ocurrió un error al procesar la decisión."
                );
            }
            finally {

                if (
                    boton
                    &&
                    htmlOriginal
                ) {

                    boton.innerHTML =
                        htmlOriginal;
                }


                if (
                    btnAprobarAprobacionPresupuestalAdq
                ) {

                    btnAprobarAprobacionPresupuestalAdq.disabled =
                        false;
                }


                if (
                    btnDeclinarAprobacionPresupuestalAdq
                ) {

                    btnDeclinarAprobacionPresupuestalAdq.disabled =
                        false;
                }
            }
        }

        btnAprobarAprobacionPresupuestalAdq
            ?.addEventListener(
                "click",
                function () {

                    enviarDecisionPresupuestalAdq(
                        "APROBAR"
                    );
                }
            );


        btnDeclinarAprobacionPresupuestalAdq
            ?.addEventListener(
                "click",
                function () {

                    enviarDecisionPresupuestalAdq(
                        "DECLINAR"
                    );
                }
            );

        // =========================================================
        // PERMISOS DE ADQUISICIONES
        // =========================================================

        const btnPermisosAdquisicionesAdq =
            document.getElementById(
                "btnPermisosAdquisicionesAdq"
            );


        const modalPermisosAdquisicionesElementAdq =
            document.getElementById(
                "modalPermisosAdquisicionesAdq"
            );


        const tablaPermisosAdquisicionesAdqBody =
            document.getElementById(
                "tablaPermisosAdquisicionesAdqBody"
            );


        const cargandoPermisosAdquisicionesAdq =
            document.getElementById(
                "cargandoPermisosAdquisicionesAdq"
            );


        const contenedorPermisosAdquisicionesAdq =
            document.getElementById(
                "contenedorPermisosAdquisicionesAdq"
            );


        const sinPermisosAdquisicionesAdq =
            document.getElementById(
                "sinPermisosAdquisicionesAdq"
            );


        const mensajePermisosAdquisicionesAdq =
            document.getElementById(
                "mensajePermisosAdquisicionesAdq"
            );


        const buscarPermisoAdquisicionesAdq =
            document.getElementById(
                "buscarPermisoAdquisicionesAdq"
            );


        const btnLimpiarBusquedaPermisosAdq =
            document.getElementById(
                "btnLimpiarBusquedaPermisosAdq"
            );


        const btnGuardarPermisosAdquisicionesAdq =
            document.getElementById(
                "btnGuardarPermisosAdquisicionesAdq"
            );


        const totalUsuariosPermisosAdq =
            document.getElementById(
                "totalUsuariosPermisosAdq"
            );


        let modalPermisosAdquisicionesAdq =
            null;


        let usuariosPermisosAdquisicionesAdq =
            [];


        const usuariosPermisosModificadosAdq =
            new Set();

        // =========================================================
        // MOSTRAR MENSAJE DE PERMISOS
        // =========================================================

        function mostrarMensajePermisosAdq(
            mensaje,
            tipo = "danger"
        ) {

            if (
                !mensajePermisosAdquisicionesAdq
            ) {
                return;
            }


            mensajePermisosAdquisicionesAdq.className =
                `alert alert-${tipo}`;


            mensajePermisosAdquisicionesAdq.textContent =
                mensaje;


            mensajePermisosAdquisicionesAdq.classList.remove(
                "d-none"
            );
        }


        // =========================================================
        // OCULTAR MENSAJE DE PERMISOS
        // =========================================================

        function ocultarMensajePermisosAdq() {

            mensajePermisosAdquisicionesAdq
                ?.classList.add(
                    "d-none"
                );
        }


        // =========================================================
        // TEXTO DEL NIVEL PRESUPUESTAL
        // =========================================================

        function obtenerNombreNivelPresupuestalAdq(
            nivel
        ) {

            switch (
            Number(
                nivel
            )
            ) {

                case 1:
                    return "Gerencia de Adquisiciones";

                case 2:
                    return "Planeación Financiera";

                case 3:
                    return "Dirección de Operaciones Internas";

                case 4:
                    return "Dirección General / Socios";

                default:
                    return "";
            }
        }


        // =========================================================
        // CREAR SELECT DE NIVEL
        // =========================================================

        function crearSelectNivelPresupuestalAdq(
            usuario
        ) {

            const nivel =
                Number(
                    usuario.nivelPresupuestal
                    ??
                    0
                );


            const deshabilitado =
                !usuario.puedeAprobarPresupuesto;


            return `
        <select class="form-select form-select-sm adq-permiso-nivel"
                data-usuario-id="${escapeAttributeAdq(
                usuario.id
            )}"
                ${deshabilitado ? "disabled" : ""}>

            <option value="">
                Seleccionar...
            </option>

            <option value="1"
                ${nivel === 1 ? "selected" : ""}>
                1 - Gerencia de Adquisiciones
            </option>

            <option value="2"
                ${nivel === 2 ? "selected" : ""}>
                2 - Planeación Financiera
            </option>

            <option value="3"
                ${nivel === 3 ? "selected" : ""}>
                3 - Dirección de Operaciones Internas
            </option>

            <option value="4"
                ${nivel === 4 ? "selected" : ""}>
                4 - Dirección General / Socios
            </option>

        </select>
    `;
        }


        // =========================================================
        // CHECKBOX DE PERMISO
        // =========================================================

        function crearCheckboxPermisoAdq(
            usuarioId,
            permiso,
            valor
        ) {

            return `
        <div class="form-check d-flex justify-content-center">

            <input type="checkbox"
                   class="form-check-input adq-permiso-checkbox"
                   data-usuario-id="${escapeAttributeAdq(
                usuarioId
            )}"
                   data-permiso="${escapeAttributeAdq(
                permiso
            )}"
                   ${valor ? "checked" : ""} />

        </div>
    `;
        }


        // =========================================================
        // RENDERIZAR PERMISOS
        // =========================================================

        function renderizarPermisosAdquisicionesAdq(
            usuarios
        ) {

            if (
                !tablaPermisosAdquisicionesAdqBody
            ) {
                return;
            }


            tablaPermisosAdquisicionesAdqBody.innerHTML =
                "";


            if (
                !Array.isArray(
                    usuarios
                )
                ||
                usuarios.length ===
                0
            ) {

                contenedorPermisosAdquisicionesAdq
                    ?.classList.add(
                        "d-none"
                    );


                sinPermisosAdquisicionesAdq
                    ?.classList.remove(
                        "d-none"
                    );


                if (
                    totalUsuariosPermisosAdq
                ) {

                    totalUsuariosPermisosAdq.textContent =
                        "0 usuarios";
                }


                return;
            }


            contenedorPermisosAdquisicionesAdq
                ?.classList.remove(
                    "d-none"
                );


            sinPermisosAdquisicionesAdq
                ?.classList.add(
                    "d-none"
                );


            usuarios.forEach(
                function (
                    usuario
                ) {

                    const fila =
                        document.createElement(
                            "tr"
                        );


                    fila.dataset.usuarioId =
                        usuario.id;


                    fila.dataset.busqueda =
                        (
                            `${usuario.nombre ?? ""} ` +
                            `${usuario.correo ?? ""}`
                        )
                            .toLowerCase();


                    fila.innerHTML = `

                <td>

                    <div class="fw-semibold">
                        ${escapeHtmlAdq(
                        usuario.nombre
                        ??
                        "Usuario"
                    )}
                    </div>

                    <div class="small text-muted">
                        ${escapeHtmlAdq(
                        usuario.correo
                        ??
                        ""
                    )}
                    </div>

                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeVisualizar",
                        usuario.puedeVisualizar
                    )}
                </td>

                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeCrearSolicitud",
                        usuario.puedeCrearSolicitud
                    )}
                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeGestionarSolicitudes",
                        usuario.puedeGestionarSolicitudes
                    )}
                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeAprobar",
                        usuario.puedeAprobar
                    )}
                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeAsignar",
                        usuario.puedeAsignar
                    )}
                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeCotizar",
                        usuario.puedeCotizar
                    )}
                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeGestionarProveedores",
                        usuario.puedeGestionarProveedores
                    )}
                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeGenerarSolicitudPago",
                        usuario.puedeGenerarSolicitudPago
                    )}
                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeVerReportes",
                        usuario.puedeVerReportes
                    )}
                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeAprobarPresupuesto",
                        usuario.puedeAprobarPresupuesto
                    )}
                </td>


                <td>
                    ${crearSelectNivelPresupuestalAdq(
                        usuario
                    )}
                </td>


                <td class="text-center">
                    ${crearCheckboxPermisoAdq(
                        usuario.id,
                        "puedeAdministrar",
                        usuario.puedeAdministrar
                    )}
                </td>
            `;


                    tablaPermisosAdquisicionesAdqBody
                        .appendChild(
                            fila
                        );
                }
            );


            actualizarTotalPermisosAdq();
        }

        // =========================================================
        // MARCAR USUARIO COMO MODIFICADO
        // =========================================================

        function marcarPermisoUsuarioModificadoAdq(
            usuarioId
        ) {

            usuariosPermisosModificadosAdq.add(
                String(
                    usuarioId
                )
            );


            if (
                btnGuardarPermisosAdquisicionesAdq
            ) {

                btnGuardarPermisosAdquisicionesAdq.disabled =
                    usuariosPermisosModificadosAdq.size ===
                    0;
            }
        }


        // =========================================================
        // CAMBIO EN CHECKBOX
        // =========================================================

        tablaPermisosAdquisicionesAdqBody
            ?.addEventListener(
                "change",
                function (
                    event
                ) {

                    const elemento =
                        event.target;


                    if (
                        !(elemento instanceof HTMLElement)
                    ) {
                        return;
                    }


                    const usuarioId =
                        elemento.dataset.usuarioId;


                    if (
                        !usuarioId
                    ) {
                        return;
                    }


                    if (
                        elemento.classList.contains(
                            "adq-permiso-checkbox"
                        )
                    ) {

                        const permiso =
                            elemento.dataset.permiso;


                        if (
                            permiso ===
                            "puedeAprobarPresupuesto"
                        ) {

                            const fila =
                                elemento.closest(
                                    "tr"
                                );


                            const selectNivel =
                                fila?.querySelector(
                                    ".adq-permiso-nivel"
                                );


                            if (
                                selectNivel
                            ) {

                                selectNivel.disabled =
                                    !elemento.checked;


                                if (
                                    !elemento.checked
                                ) {

                                    selectNivel.value =
                                        "";
                                }
                            }
                        }


                        marcarPermisoUsuarioModificadoAdq(
                            usuarioId
                        );


                        return;
                    }


                    if (
                        elemento.classList.contains(
                            "adq-permiso-nivel"
                        )
                    ) {

                        marcarPermisoUsuarioModificadoAdq(
                            usuarioId
                        );
                    }
                }
        );

        // =========================================================
        // CARGAR PERMISOS DESDE BACKEND
        // =========================================================

        async function cargarPermisosAdquisicionesAdq() {

            ocultarMensajePermisosAdq();


            usuariosPermisosModificadosAdq.clear();


            if (
                btnGuardarPermisosAdquisicionesAdq
            ) {

                btnGuardarPermisosAdquisicionesAdq.disabled =
                    true;
            }


            cargandoPermisosAdquisicionesAdq
                ?.classList.remove(
                    "d-none"
                );


            contenedorPermisosAdquisicionesAdq
                ?.classList.add(
                    "d-none"
                );


            sinPermisosAdquisicionesAdq
                ?.classList.add(
                    "d-none"
                );


            try {

                const respuesta =
                    await fetch(
                        `${window.location.pathname}?handler=PermisosUsuariosAdquisiciones`,
                        {
                            method:
                                "GET",

                            headers:
                            {
                                "X-Requested-With":
                                    "XMLHttpRequest"
                            }
                        }
                    );


                const resultado =
                    await respuesta.json();


                if (
                    !respuesta.ok
                    ||
                    !resultado.success
                ) {

                    throw new Error(
                        resultado.message
                        ??
                        "No fue posible cargar los permisos."
                    );
                }


                usuariosPermisosAdquisicionesAdq =
                    Array.isArray(
                        resultado.data
                    )
                        ? resultado.data
                        : [];


                renderizarPermisosAdquisicionesAdq(
                    usuariosPermisosAdquisicionesAdq
                );
            }
            catch (
            error
            ) {

                console.error(
                    error
                );


                mostrarMensajePermisosAdq(
                    error.message
                    ??
                    "Ocurrió un error al cargar los permisos."
                );
            }
            finally {

                cargandoPermisosAdquisicionesAdq
                    ?.classList.add(
                        "d-none"
                    );
            }
        }

        btnPermisosAdquisicionesAdq
            ?.addEventListener(
                "click",
                async function () {

                    if (
                        !modalPermisosAdquisicionesElementAdq
                    ) {
                        return;
                    }


                    modalPermisosAdquisicionesAdq =
                        bootstrap.Modal.getOrCreateInstance(
                            modalPermisosAdquisicionesElementAdq
                        );


                    modalPermisosAdquisicionesAdq.show();


                    await cargarPermisosAdquisicionesAdq();
                }
        );

        function actualizarTotalPermisosAdq() {

            const filasVisibles =
                tablaPermisosAdquisicionesAdqBody
                    ?.querySelectorAll(
                        "tr:not(.d-none)"
                    )
                ??
                [];


            if (
                totalUsuariosPermisosAdq
            ) {

                const total =
                    filasVisibles.length;


                totalUsuariosPermisosAdq.textContent =
                    `${total} usuario${total === 1 ? "" : "s"}`;
            }
        }


        function filtrarPermisosAdquisicionesAdq() {

            const texto =
                (
                    buscarPermisoAdquisicionesAdq
                        ?.value
                    ??
                    ""
                )
                    .trim()
                    .toLowerCase();


            const filas =
                tablaPermisosAdquisicionesAdqBody
                    ?.querySelectorAll(
                        "tr"
                    )
                ??
                [];


            filas.forEach(
                function (
                    fila
                ) {

                    const busqueda =
                        fila.dataset.busqueda
                        ??
                        "";


                    const visible =
                        !texto
                        ||
                        busqueda.includes(
                            texto
                        );


                    fila.classList.toggle(
                        "d-none",
                        !visible
                    );
                }
            );


            actualizarTotalPermisosAdq();
        }


        buscarPermisoAdquisicionesAdq
            ?.addEventListener(
                "input",
                filtrarPermisosAdquisicionesAdq
            );


        btnLimpiarBusquedaPermisosAdq
            ?.addEventListener(
                "click",
                function () {

                    if (
                        buscarPermisoAdquisicionesAdq
                    ) {

                        buscarPermisoAdquisicionesAdq.value =
                            "";
                    }


                    filtrarPermisosAdquisicionesAdq();


                    buscarPermisoAdquisicionesAdq
                        ?.focus();
                }
        );

        // =========================================================
        // OBTENER VALOR DE CHECKBOX DE PERMISO
        // =========================================================

        function obtenerCheckboxPermisoAdq(
            fila,
            permiso
        ) {

            const checkbox =
                fila.querySelector(
                    `.adq-permiso-checkbox[data-permiso="${permiso}"]`
                );


            return checkbox
                ? checkbox.checked
                : false;
        }


        // =========================================================
        // OBTENER PERMISOS MODIFICADOS
        // =========================================================

        function obtenerPermisosAdquisicionesFormularioAdq() {

            const resultado =
                [];


            usuariosPermisosModificadosAdq.forEach(
                function (
                    usuarioId
                ) {

                    const fila =
                        tablaPermisosAdquisicionesAdqBody
                            ?.querySelector(
                                `tr[data-usuario-id="${CSS.escape(usuarioId)}"]`
                            );


                    if (
                        !fila
                    ) {
                        return;
                    }


                    const puedeAprobarPresupuesto =
                        obtenerCheckboxPermisoAdq(
                            fila,
                            "puedeAprobarPresupuesto"
                        );


                    const selectNivel =
                        fila.querySelector(
                            ".adq-permiso-nivel"
                        );


                    const nivelPresupuestal =
                        puedeAprobarPresupuesto
                            &&
                            selectNivel
                            &&
                            selectNivel.value
                            ? Number(
                                selectNivel.value
                            )
                            : null;


                    resultado.push(
                        {
                            usuarioId:
                                usuarioId,

                            puedeVisualizar:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeVisualizar"
                                ),

                            puedeCrearSolicitud:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeCrearSolicitud"
                                ),

                            puedeGestionarSolicitudes:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeGestionarSolicitudes"
                                ),

                            puedeAprobar:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeAprobar"
                                ),

                            puedeAsignar:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeAsignar"
                                ),

                            puedeCotizar:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeCotizar"
                                ),

                            puedeGestionarProveedores:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeGestionarProveedores"
                                ),

                            puedeGenerarSolicitudPago:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeGenerarSolicitudPago"
                                ),

                            puedeVerReportes:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeVerReportes"
                                ),

                            puedeAprobarPresupuesto:
                                puedeAprobarPresupuesto,

                            nivelPresupuestal:
                                nivelPresupuestal,

                            puedeAdministrar:
                                obtenerCheckboxPermisoAdq(
                                    fila,
                                    "puedeAdministrar"
                                )
                        }
                    );
                }
            );


            return resultado;
        }

        // =========================================================
        // VALIDAR PERMISOS PRESUPUESTALES
        // =========================================================

        function validarPermisosPresupuestalesAdq(
            permisos
        ) {

            for (
                const permiso
                of permisos
            ) {

                if (
                    permiso.puedeAprobarPresupuesto
                    &&
                    (
                        !permiso.nivelPresupuestal
                        ||
                        permiso.nivelPresupuestal <
                        1
                        ||
                        permiso.nivelPresupuestal >
                        4
                    )
                ) {

                    mostrarAdvertenciaAdq(
                        "Nivel presupuestal requerido",
                        "Todo usuario con permiso para aprobar presupuesto debe tener asignado un nivel presupuestal."
                    );


                    return false;
                }
            }


            return true;
        }

        // =========================================================
        // GUARDAR PERMISOS DE ADQUISICIONES
        // =========================================================

        async function guardarPermisosAdquisicionesAdq() {

            if (
                !btnGuardarPermisosAdquisicionesAdq
            ) {
                return;
            }


            const permisos =
                obtenerPermisosAdquisicionesFormularioAdq();


            if (
                permisos.length ===
                0
            ) {

                mostrarAdvertenciaAdq(
                    "Sin cambios",
                    "No se detectaron cambios en los permisos."
                );


                return;
            }


            if (
                !validarPermisosPresupuestalesAdq(
                    permisos
                )
            ) {
                return;
            }


            const confirmado =
                await confirmarAccionAdq(
                    {
                        titulo:
                            "Guardar permisos",

                        mensaje:
                            "Se actualizarán los permisos seleccionados del módulo de Adquisiciones.",

                        textoConfirmar:
                            "Guardar",

                        textoCancelar:
                            "Cancelar",

                        tipo:
                            "primary",

                        icono:
                            "bi-shield-check"
                    }
                );


            if (
                !confirmado
            ) {
                return;
            }


            const htmlOriginal =
                btnGuardarPermisosAdquisicionesAdq.innerHTML;


            btnGuardarPermisosAdquisicionesAdq.disabled =
                true;


            btnGuardarPermisosAdquisicionesAdq.innerHTML =
                `
            <span class="spinner-border spinner-border-sm me-1"></span>
            Guardando...
        `;


            ocultarMensajePermisosAdq();


            try {

                const token =
                    document.querySelector(
                        'input[name="__RequestVerificationToken"]'
                    )
                        ?.value;


                const respuesta =
                    await fetch(
                        `${window.location.pathname}?handler=GuardarPermisosUsuariosAdquisiciones`,
                        {
                            method:
                                "POST",

                            headers:
                            {
                                "Content-Type":
                                    "application/json",

                                "X-Requested-With":
                                    "XMLHttpRequest",

                                "RequestVerificationToken":
                                    token
                                    ??
                                    ""
                            },

                            body:
                                JSON.stringify(
                                    {
                                        permisos:
                                            permisos
                                    }
                                )
                        }
                    );


                const resultado =
                    await respuesta.json();


                if (
                    !respuesta.ok
                    ||
                    !resultado.success
                ) {

                    throw new Error(
                        resultado.message
                        ??
                        "No fue posible guardar los permisos."
                    );
                }


                usuariosPermisosModificadosAdq.clear();


                mostrarMensajePermisosAdq(
                    resultado.message
                    ??
                    "Los permisos se guardaron correctamente.",
                    "success"
                );


                await cargarPermisosAdquisicionesAdq();
            }
            catch (
            error
            ) {

                console.error(
                    error
                );


                mostrarMensajePermisosAdq(
                    error.message
                    ??
                    "Ocurrió un error al guardar los permisos."
                );
            }
            finally {

                btnGuardarPermisosAdquisicionesAdq.innerHTML =
                    htmlOriginal;


                btnGuardarPermisosAdquisicionesAdq.disabled =
                    usuariosPermisosModificadosAdq.size ===
                    0;
            }
        }

        btnGuardarPermisosAdquisicionesAdq
            ?.addEventListener(
                "click",
                guardarPermisosAdquisicionesAdq
            );

        // =========================================================
        // MODAL DE CONFIRMACIÓN REUTILIZABLE
        // =========================================================

        function confirmarAccionAdq({
            titulo = "Confirmar acción",
            mensaje = "",
            textoConfirmar = "Confirmar",
            textoCancelar = "Cancelar",
            tipo = "primary",
            icono = "bi-question-circle"
        } = {}) {

            return new Promise(
                function (resolve) {

                    let overlay =
                        document.getElementById(
                            "adqConfirmacionOverlay"
                        );


                    if (overlay) {
                        overlay.remove();
                    }


                    overlay =
                        document.createElement(
                            "div"
                        );


                    overlay.id =
                        "adqConfirmacionOverlay";


                    overlay.className =
                        "adq-confirm-overlay";


                    const clasesBoton =
                    {
                        primary:
                            "btn-primary",

                        success:
                            "btn-success",

                        warning:
                            "btn-warning",

                        danger:
                            "btn-danger"
                    };


                    const claseBoton =
                        clasesBoton[tipo]
                        ??
                        "btn-primary";


                    overlay.innerHTML = `
                <div class="adq-confirm-dialog"
                     role="dialog"
                     aria-modal="true"
                     aria-labelledby="adqConfirmacionTitulo">

                    <div class="adq-confirm-body">

                        <div class="adq-confirm-icon adq-confirm-icon-${tipo}">
                            <i class="bi ${icono}"></i>
                        </div>

                        <div class="adq-confirm-content">

                            <h5 id="adqConfirmacionTitulo"
                                class="adq-confirm-title">
                                ${escapeHtmlAdq(
                        titulo
                    )}
                            </h5>

                            <div class="adq-confirm-message">
                                ${mensaje}
                            </div>

                        </div>

                    </div>


                    <div class="adq-confirm-footer">

                        <button type="button"
                                class="btn btn-light"
                                data-adq-confirmar="cancelar">
                            ${escapeHtmlAdq(
                        textoCancelar
                    )}
                        </button>

                        <button type="button"
                                class="btn ${claseBoton}"
                                data-adq-confirmar="aceptar">

                            <i class="bi bi-check2-circle me-1"></i>

                            ${escapeHtmlAdq(
                        textoConfirmar
                    )}

                        </button>

                    </div>

                </div>
            `;


                    document.body.appendChild(
                        overlay
                    );


                    const btnAceptar =
                        overlay.querySelector(
                            '[data-adq-confirmar="aceptar"]'
                        );


                    const btnCancelar =
                        overlay.querySelector(
                            '[data-adq-confirmar="cancelar"]'
                        );


                    let terminado =
                        false;


                    function cerrar(
                        resultado
                    ) {

                        if (terminado) {
                            return;
                        }


                        terminado =
                            true;


                        overlay.classList.remove(
                            "adq-confirm-overlay-visible"
                        );


                        setTimeout(
                            function () {

                                overlay.remove();

                            },
                            150
                        );


                        document.removeEventListener(
                            "keydown",
                            manejarEscape
                        );


                        resolve(
                            resultado
                        );
                    }


                    function manejarEscape(
                        event
                    ) {

                        if (
                            event.key ===
                            "Escape"
                        ) {

                            cerrar(
                                false
                            );
                        }
                    }


                    btnAceptar
                        ?.addEventListener(
                            "click",
                            function () {

                                cerrar(
                                    true
                                );
                            }
                        );


                    btnCancelar
                        ?.addEventListener(
                            "click",
                            function () {

                                cerrar(
                                    false
                                );
                            }
                        );


                    overlay.addEventListener(
                        "click",
                        function (
                            event
                        ) {

                            if (
                                event.target ===
                                overlay
                            ) {

                                cerrar(
                                    false
                                );
                            }
                        }
                    );


                    document.addEventListener(
                        "keydown",
                        manejarEscape
                    );


                    requestAnimationFrame(
                        function () {

                            overlay.classList.add(
                                "adq-confirm-overlay-visible"
                            );


                            btnCancelar
                                ?.focus();
                        }
                    );
                }
            );
        }

        // =========================================================
        // SINCRONIZAR ARCHIVOS ADICIONALES DE COTIZACIÓN
        // =========================================================

        function sincronizarArchivosCotizacionAdq() {

            if (
                !archivosCotizacionAdq
            ) {
                return;
            }


            const transferencia =
                new DataTransfer();


            archivosCotizacionSeleccionadosAdq
                .forEach(
                    function (
                        archivo
                    ) {

                        transferencia.items.add(
                            archivo
                        );
                    }
                );


            archivosCotizacionAdq.files =
                transferencia.files;
        }

        // =========================================================
        // SINCRONIZAR ARCHIVOS EXISTENTES MARCADOS PARA ELIMINAR
        // =========================================================

        function sincronizarArchivosCotizacionEliminarAdq() {

            if (
                !archivosCotizacionEliminarInputsAdq
            ) {
                return;
            }


            archivosCotizacionEliminarInputsAdq.innerHTML =
                "";


            archivosCotizacionEliminadosAdq
                .forEach(
                    function (
                        archivoId
                    ) {

                        const input =
                            document.createElement(
                                "input"
                            );


                        input.type =
                            "hidden";


                        input.name =
                            "ArchivosCotizacionEliminarIds";


                        input.value =
                            String(
                                archivoId
                            );


                        archivosCotizacionEliminarInputsAdq
                            .appendChild(
                                input
                            );
                    }
                );
        }

        // =========================================================
        // RENDERIZAR ARCHIVOS ADICIONALES DE COTIZACIÓN
        // =========================================================

        function renderizarArchivosCotizacionSeleccionadosAdq() {

            if (
                !listaArchivosCotizacionAdq
            ) {
                return;
            }


            listaArchivosCotizacionAdq.innerHTML =
                "";


            const archivosExistentesTodos =
                cotizacionEditandoActualAdq
                    &&
                    Array.isArray(
                        cotizacionEditandoActualAdq.archivosAdicionales
                    )
                    ? cotizacionEditandoActualAdq.archivosAdicionales
                    : [];


            const archivosExistentes =
                archivosExistentesTodos
                    .filter(
                        function (
                            archivo
                        ) {

                            return !archivosCotizacionEliminadosAdq.has(
                                Number(
                                    archivo.id
                                    ??
                                    0
                                )
                            );
                        }
                    );


            const totalExistentes =
                archivosExistentes.length;


            const totalNuevos =
                archivosCotizacionSeleccionadosAdq.length;


            // =====================================================
            // SIN ARCHIVOS
            // =====================================================

            if (
                totalExistentes === 0
                &&
                totalNuevos === 0
            ) {

                listaArchivosCotizacionAdq.innerHTML = `
            <div class="adq-files-empty">

                <i class="bi bi-paperclip"></i>

                <span>
                    No hay archivos adicionales.
                </span>

            </div>
        `;


                if (
                    contadorArchivosCotizacionAdq
                ) {

                    contadorArchivosCotizacionAdq.textContent =
                        cotizacionEditandoActualAdq
                            ? "Sin archivos adicionales. Este apartado es opcional."
                            : "0 archivos seleccionados.";
                }


                return;
            }


            // =====================================================
            // ARCHIVOS EXISTENTES
            // =====================================================

            if (
                totalExistentes > 0
            ) {

                const tituloExistentes =
                    document.createElement(
                        "div"
                    );


                tituloExistentes.className =
                    "small fw-semibold text-muted mb-2";


                tituloExistentes.innerHTML = `
            <i class="bi bi-folder-check me-1"></i>
            Archivos actuales
        `;


                listaArchivosCotizacionAdq
                    .appendChild(
                        tituloExistentes
                    );


                archivosExistentes.forEach(
                    function (
                        archivo
                    ) {

                        const archivoId =
                            Number(
                                archivo.id
                                ??
                                0
                            );


                        const item =
                            document.createElement(
                                "div"
                            );


                        item.className =
                            "adq-file-item mb-2";


                        item.innerHTML = `
                    <div class="adq-file-item-main">

                        <div class="adq-file-item-icon">
                            <i class="bi bi-file-earmark-check"></i>
                        </div>


                        <div class="adq-file-item-info">

                            <strong title="${escapeAttributeAdq(
                            archivo.nombreOriginal
                            ??
                            "Archivo"
                        )}">

                                ${escapeHtmlAdq(
                            archivo.nombreOriginal
                            ??
                            "Archivo"
                        )}

                            </strong>

                            <span>
                                Archivo existente
                            </span>

                        </div>

                    </div>


                    <div class="d-flex align-items-center gap-2">

                        <a href="${escapeAttributeAdq(
                            archivo.rutaArchivo
                            ??
                            "#"
                        )}"
                           target="_blank"
                           rel="noopener noreferrer"
                           class="btn btn-sm btn-outline-primary"
                           title="Ver archivo">

                            <i class="bi bi-eye"></i>

                        </a>


                        <button type="button"
                                class="btn btn-sm btn-outline-danger btnEliminarArchivoExistenteCotizacionAdq"
                                data-archivo-id="${archivoId}"
                                title="Eliminar archivo">

                            <i class="bi bi-trash"></i>

                        </button>

                    </div>
                `;


                        listaArchivosCotizacionAdq
                            .appendChild(
                                item
                            );
                    }
                );
            }


            // =====================================================
            // ARCHIVOS NUEVOS
            // =====================================================

            if (
                totalNuevos > 0
            ) {

                const tituloNuevos =
                    document.createElement(
                        "div"
                    );


                tituloNuevos.className =
                    totalExistentes > 0
                        ? "small fw-semibold text-muted mt-3 mb-2"
                        : "small fw-semibold text-muted mb-2";


                tituloNuevos.innerHTML = `
            <i class="bi bi-cloud-arrow-up me-1"></i>

            ${cotizacionEditandoActualAdq
                        ? "Nuevos archivos"
                        : "Archivos seleccionados"
                    }
        `;


                listaArchivosCotizacionAdq
                    .appendChild(
                        tituloNuevos
                    );


                archivosCotizacionSeleccionadosAdq.forEach(
                    function (
                        archivo,
                        index
                    ) {

                        const item =
                            document.createElement(
                                "div"
                            );


                        item.className =
                            "adq-file-item mb-2";


                        item.innerHTML = `
                    <div class="adq-file-item-main">

                        <div class="adq-file-item-icon">
                            <i class="bi bi-file-earmark"></i>
                        </div>


                        <div class="adq-file-item-info">

                            <strong title="${escapeAttributeAdq(
                            archivo.name
                        )}">

                                ${escapeHtmlAdq(
                            archivo.name
                        )}

                            </strong>

                            <span>
                                ${formatearTamanoAdq(
                            archivo.size
                        )}
                            </span>

                        </div>

                    </div>


                    <button type="button"
                            class="btn btn-sm btn-outline-danger btnEliminarArchivoCotizacionAdq"
                            data-index="${index}"
                            title="Quitar archivo nuevo">

                        <i class="bi bi-trash"></i>

                    </button>
                `;


                        listaArchivosCotizacionAdq
                            .appendChild(
                                item
                            );
                    }
                );
            }


            // =====================================================
            // CONTADOR
            // =====================================================

            if (
                contadorArchivosCotizacionAdq
            ) {

                if (
                    cotizacionEditandoActualAdq
                ) {

                    contadorArchivosCotizacionAdq.textContent =
                        `${totalExistentes} existentes · ${totalNuevos} nuevos · Opcional al editar.`;
                }
                else {

                    contadorArchivosCotizacionAdq.textContent =
                        totalNuevos === 1
                            ? "1 archivo seleccionado."
                            : `${totalNuevos} archivos seleccionados.`;
                }
            }
        }



        // =========================================================
        // CLAVE ÚNICA DE ARCHIVO
        // =========================================================

        function obtenerClaveArchivoCotizacionAdq(
            archivo
        ) {

            return [
                archivo.name,
                archivo.size,
                archivo.lastModified
            ].join(
                "|"
            );
        }

        // =========================================================
        // AGREGAR ARCHIVOS ADICIONALES
        // =========================================================

        archivosCotizacionAdq
            ?.addEventListener(
                "change",
                function () {

                    const nuevosArchivos =
                        Array.from(
                            archivosCotizacionAdq.files
                            ??
                            []
                        );


                    const clavesExistentes =
                        new Set(
                            archivosCotizacionSeleccionadosAdq
                                .map(
                                    obtenerClaveArchivoCotizacionAdq
                                )
                        );


                    nuevosArchivos.forEach(
                        function (
                            archivo
                        ) {

                            const extension =
                                obtenerExtensionAdq(
                                    archivo.name
                                );


                            if (
                                !extensionesPermitidasAdq.includes(
                                    extension
                                )
                            ) {

                                mostrarAdvertenciaAdq(
                                    "Formato no permitido",
                                    `El archivo ${archivo.name} no tiene un formato permitido.`
                                );

                                return;
                            }


                            if (
                                archivo.size >
                                tamanoMaximoArchivoAdq
                            ) {

                                mostrarAdvertenciaAdq(
                                    "Archivo demasiado grande",
                                    `El archivo ${archivo.name} supera el límite de 15 MB.`
                                );

                                return;
                            }


                            const clave =
                                obtenerClaveArchivoCotizacionAdq(
                                    archivo
                                );


                            if (
                                clavesExistentes.has(
                                    clave
                                )
                            ) {
                                return;
                            }


                            archivosCotizacionSeleccionadosAdq
                                .push(
                                    archivo
                                );


                            clavesExistentes.add(
                                clave
                            );
                        }
                    );


                    sincronizarArchivosCotizacionAdq();

                    renderizarArchivosCotizacionSeleccionadosAdq();
                }
            );

        // =========================================================
        // ELIMINAR ARCHIVO ADICIONAL
        // =========================================================

        document.addEventListener(
            "click",
            function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnEliminarArchivoCotizacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const index =
                    Number(
                        boton.dataset.index
                        ??
                        -1
                    );


                if (
                    index <
                    0
                    ||
                    index >=
                    archivosCotizacionSeleccionadosAdq.length
                ) {
                    return;
                }


                archivosCotizacionSeleccionadosAdq.splice(
                    index,
                    1
                );


                sincronizarArchivosCotizacionAdq();

                renderizarArchivosCotizacionSeleccionadosAdq();
            }
        );

        // =========================================================
        // MARCAR ARCHIVO EXISTENTE PARA ELIMINAR
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnEliminarArchivoExistenteCotizacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const archivoId =
                    Number(
                        boton.dataset.archivoId
                        ??
                        0
                    );


                if (
                    archivoId <= 0
                ) {
                    return;
                }


                const confirmado =
                    await confirmarAccionAdq(
                        {
                            titulo:
                                "Eliminar archivo",

                            mensaje:
                                `
                        <p class="mb-0">
                            El archivo dejará de formar parte de esta cotización
                            cuando guardes los cambios.
                        </p>
                        `,

                            textoConfirmar:
                                "Eliminar",

                            textoCancelar:
                                "Cancelar",

                            tipo:
                                "danger",

                            icono:
                                "bi-trash"
                        }
                    );


                if (!confirmado) {
                    return;
                }


                archivosCotizacionEliminadosAdq.add(
                    archivoId
                );


                sincronizarArchivosCotizacionEliminarAdq();


                renderizarArchivosCotizacionSeleccionadosAdq();
            }
        );

        // =========================================================
        // CONSULTAR COTIZACIONES REGISTRADAS
        // =========================================================

        async function obtenerCotizacionesSolicitudAdq(
            solicitudId
        ) {

            const response =
                await fetch(
                    `?handler=CotizacionesSolicitud&id=${encodeURIComponent(
                        solicitudId
                    )}`,
                    {
                        method:
                            "GET",

                        headers:
                        {
                            "X-Requested-With":
                                "XMLHttpRequest"
                        }
                    }
                );


            if (!response.ok) {

                throw new Error(
                    "No fue posible consultar las cotizaciones registradas."
                );
            }


            const resultado =
                await response.json();


            return Array.isArray(
                resultado.cotizaciones
            )
                ? resultado.cotizaciones
                : [];
        }

        function obtenerCotizacionSeleccionadaAdq(
            cotizaciones
        ) {

            if (
                !Array.isArray(
                    cotizaciones
                )
            ) {
                return null;
            }


            return cotizaciones.find(
                function (
                    cotizacion
                ) {

                    return (
                        cotizacion.esPrincipal ===
                        true
                        &&
                        cotizacion.finalizada ===
                        true
                    );
                }
            )
                ?? null;
        }

        async function solicitarPresupuestoAdq(
            solicitudId,
            comentario
        ) {

            const formData =
                new FormData();


            const token =
                document.querySelector(
                    'input[name="__RequestVerificationToken"]'
                )
                    ?.value
                ?? "";


            formData.append(
                "__RequestVerificationToken",
                token
            );


            formData.append(
                "solicitudId",
                String(
                    solicitudId
                )
            );


            formData.append(
                "comentario",
                comentario
                ??
                ""
            );


            const response =
                await fetch(
                    "?handler=SolicitarPresupuesto",
                    {
                        method:
                            "POST",

                        headers:
                        {
                            "X-Requested-With":
                                "XMLHttpRequest"
                        },

                        body:
                            formData
                    }
                );


            let resultado = null;


            try {

                resultado =
                    await response.json();

            }
            catch {

                resultado =
                {
                    success:
                        false,

                    message:
                        "El servidor devolvió una respuesta no válida."
                };
            }


            if (
                !response.ok
                ||
                !resultado?.success
            ) {

                throw new Error(
                    resultado?.message
                    ??
                    "No fue posible solicitar la aprobación presupuestal."
                );
            }


            return resultado;
        }

        // =========================================================
        // ABRIR SOLICITUD DE PRESUPUESTO
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnSolicitarPresupuestoAdq"
                    );


                if (!boton) {
                    return;
                }


                const solicitudId =
                    Number(
                        boton.dataset.id
                        ??
                        0
                    );


                if (
                    solicitudId <=
                    0
                ) {
                    return;
                }


                try {

                    boton.disabled =
                        true;


                    const [
                        solicitud,
                        cotizaciones
                    ] =
                        await Promise.all(
                            [
                                obtenerSolicitudAdq(
                                    solicitudId
                                ),

                                obtenerCotizacionesSolicitudAdq(
                                    solicitudId
                                )
                            ]
                        );


                    if (!solicitud) {

                        throw new Error(
                            "No fue posible obtener la solicitud."
                        );
                    }


                    const cotizacionSeleccionada =
                        obtenerCotizacionSeleccionadaAdq(
                            cotizaciones
                        );


                    if (!cotizacionSeleccionada) {

                        throw new Error(
                            "No existe una cotización seleccionada y finalizada."
                        );
                    }


                    solicitudPresupuestoActualAdq =
                    {
                        solicitud:
                            solicitud,

                        cotizacion:
                            cotizacionSeleccionada
                    };


                    if (
                        presupuestoSolicitudIdAdq
                    ) {

                        presupuestoSolicitudIdAdq.value =
                            String(
                                solicitud.id
                            );
                    }


                    if (
                        folioPresupuestoAdq
                    ) {

                        folioPresupuestoAdq.textContent =
                            `${solicitud.folio ?? ""} · ${solicitud.titulo ?? ""}`;
                    }


                    if (
                        proveedorPresupuestoAdq
                    ) {

                        proveedorPresupuestoAdq.textContent =
                            cotizacionSeleccionada.nombreProveedor
                            ??
                            "Proveedor";
                    }


                    if (
                        subtotalPresupuestoAdq
                    ) {

                        subtotalPresupuestoAdq.textContent =
                            formatearMonedaAdq(
                                cotizacionSeleccionada.subtotal
                                ??
                                0
                            );
                    }


                    if (
                        ivaPresupuestoAdq
                    ) {

                        ivaPresupuestoAdq.textContent =
                            formatearMonedaAdq(
                                cotizacionSeleccionada.importeIva
                                ??
                                0
                            );
                    }


                    if (
                        totalPresupuestoAdq
                    ) {

                        totalPresupuestoAdq.textContent =
                            formatearMonedaAdq(
                                cotizacionSeleccionada.total
                                ??
                                0
                            );
                    }


                    if (
                        comentarioPresupuestoAdq
                    ) {

                        comentarioPresupuestoAdq.value =
                            "";
                    }


                    bootstrap.Modal
                        .getOrCreateInstance(
                            modalPresupuestoElementAdq
                        )
                        .show();

                }
                catch (
                error
                ) {

                    console.error(
                        "Error al preparar aprobación presupuestal:",
                        error
                    );


                    mostrarAdvertenciaAdq(
                        "No fue posible continuar",
                        error.message
                        ??
                        "No fue posible preparar la solicitud presupuestal."
                    );
                }
                finally {

                    boton.disabled =
                        false;
                }
            }
        );

        function formatearMonedaAdq(
            valor
        ) {

            return Number(
                valor
                ??
                0
            ).toLocaleString(
                "es-MX",
                {
                    style:
                        "currency",

                    currency:
                        "MXN"
                }
            );
        }

        // =========================================================
        // CONFIRMAR SOLICITUD DE PRESUPUESTO
        // =========================================================

        btnConfirmarPresupuestoAdq
            ?.addEventListener(
                "click",
                async function () {

                    const solicitudId =
                        Number(
                            presupuestoSolicitudIdAdq
                                ?.value
                            ??
                            0
                        );


                    if (
                        solicitudId <=
                        0
                    ) {

                        mostrarAdvertenciaAdq(
                            "Solicitud no identificada",
                            "No fue posible identificar la solicitud."
                        );

                        return;
                    }


                    const comentario =
                        comentarioPresupuestoAdq
                            ?.value
                            ?.trim()
                        ??
                        "";


                    const confirmado =
                        await confirmarAccionAdq(
                            {
                                titulo:
                                    "Enviar a aprobación presupuestal",

                                mensaje:
                                    `
                <p class="mb-3">
                    Confirma el envío de esta solicitud para autorización presupuestal.
                </p>

                <div class="adq-confirm-data">

                    <div class="adq-confirm-data-item">

                        <span>
                            Monto solicitado
                        </span>

                        <strong class="text-primary fs-4">
                            ${escapeHtmlAdq(
                                        totalPresupuestoAdq
                                            ?.textContent
                                        ??
                                        "$0.00"
                                    )}
                        </strong>

                    </div>

                </div>

                <div class="small text-muted mt-3">
                    La solicitud avanzará a la etapa
                    <strong>Pendiente de presupuesto</strong>.
                </div>
                `,

                                textoConfirmar:
                                    "Enviar a aprobación",

                                textoCancelar:
                                    "Cancelar",

                                tipo:
                                    "primary",

                                icono:
                                    "bi-cash-stack"
                            }
                        );


                    if (!confirmado) {
                        return;
                    }


                    try {

                        btnConfirmarPresupuestoAdq.disabled =
                            true;


                        btnConfirmarPresupuestoAdq.innerHTML =
                            `
                    <span class="spinner-border spinner-border-sm me-1"
                          role="status"
                          aria-hidden="true">
                    </span>
                    Enviando...
                    `;


                        await solicitarPresupuestoAdq(
                            solicitudId,
                            comentario
                        );


                        bootstrap.Modal
                            .getInstance(
                                modalPresupuestoElementAdq
                            )
                            ?.hide();


                        window.location.reload();

                    }
                    catch (
                    error
                    ) {

                        console.error(
                            "Error al solicitar presupuesto:",
                            error
                        );


                        mostrarAdvertenciaAdq(
                            "No fue posible enviar la solicitud",
                            error.message
                            ??
                            "Ocurrió un error al solicitar la aprobación presupuestal."
                        );
                    }
                    finally {

                        btnConfirmarPresupuestoAdq.disabled =
                            false;


                        btnConfirmarPresupuestoAdq.innerHTML =
                            `
                    <i class="bi bi-send me-1"></i>
                    Enviar a aprobación
                    `;
                    }
                }
            );

        // =========================================================
        // TOKEN ANTIFORGERY - COTIZACIONES
        // =========================================================

        function obtenerTokenAntiforgeryCotizacionAdq() {

            return formCotizacionAdq
                ?.querySelector(
                    'input[name="__RequestVerificationToken"]'
                )
                ?.value
                ??
                "";
        }

        // =========================================================
        // SELECCIONAR COTIZACIÓN
        // =========================================================

        async function seleccionarCotizacionAdq(
            cotizacionId
        ) {

            const token =
                obtenerTokenAntiforgeryCotizacionAdq();


            const datos =
                new FormData();


            datos.append(
                "__RequestVerificationToken",
                token
            );


            datos.append(
                "cotizacionId",
                String(
                    cotizacionId
                )
            );


            const response =
                await fetch(
                    "?handler=SeleccionarCotizacion",
                    {
                        method:
                            "POST",

                        body:
                            datos,

                        headers:
                        {
                            "X-Requested-With":
                                "XMLHttpRequest"
                        }
                    }
                );


            let resultado =
                null;


            try {

                resultado =
                    await response.json();

            }
            catch {

                resultado =
                    null;
            }


            if (
                !response.ok
                ||
                !resultado?.success
            ) {

                throw new Error(
                    resultado?.message
                    ??
                    "No fue posible seleccionar la cotización."
                );
            }


            return resultado;
        }

        // =========================================================
        // FINALIZAR ETAPA DE COTIZACIÓN
        // =========================================================

        async function finalizarCotizacionAdq(
            solicitudId
        ) {

            const token =
                obtenerTokenAntiforgeryCotizacionAdq();


            const datos =
                new FormData();


            datos.append(
                "__RequestVerificationToken",
                token
            );


            datos.append(
                "solicitudId",
                String(
                    solicitudId
                )
            );


            const response =
                await fetch(
                    "?handler=FinalizarCotizacion",
                    {
                        method:
                            "POST",

                        body:
                            datos,

                        headers:
                        {
                            "X-Requested-With":
                                "XMLHttpRequest"
                        }
                    }
                );


            let resultado =
                null;


            try {

                resultado =
                    await response.json();

            }
            catch {

                resultado =
                    null;
            }


            if (
                !response.ok
                ||
                !resultado?.success
            ) {

                throw new Error(
                    resultado?.message
                    ??
                    "No fue posible finalizar la cotización."
                );
            }


            return resultado;
        }

        // =========================================================
        // REABRIR ETAPA DE COTIZACIÓN
        // =========================================================

        async function reabrirCotizacionAdq(
            solicitudId
        ) {

            const token =
                document.querySelector(
                    'input[name="__RequestVerificationToken"]'
                )
                    ?.value
                ??
                "";


            const datos =
                new FormData();


            datos.append(
                "__RequestVerificationToken",
                token
            );


            datos.append(
                "solicitudId",
                String(
                    solicitudId
                )
            );


            const response =
                await fetch(
                    "?handler=ReabrirCotizacion",
                    {
                        method:
                            "POST",

                        body:
                            datos,

                        headers:
                        {
                            "X-Requested-With":
                                "XMLHttpRequest"
                        }
                    }
                );


            let resultado =
                null;


            try {

                resultado =
                    await response.json();

            }
            catch {

                resultado =
                    null;
            }


            if (
                !response.ok
                ||
                !resultado?.success
            ) {

                throw new Error(
                    resultado?.message
                    ??
                    "No fue posible reabrir la cotización."
                );
            }


            return resultado;
        }

        // =========================================================
        // MODIFICAR / REABRIR COTIZACIONES
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnReabrirCotizacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const solicitudId =
                    Number(
                        boton.dataset.id
                        ??
                        0
                    );


                const folio =
                    boton.dataset.folio
                    ??
                    "la solicitud";


                if (
                    solicitudId <=
                    0
                ) {
                    return;
                }


                const confirmado =
                    await confirmarAccionAdq(
                        {
                            titulo:
                                "Modificar cotizaciones",

                            mensaje:
                                `
                        <p class="mb-3">
                            Vas a reabrir la etapa de cotización de
                            <strong>${escapeHtmlAdq(
                                    folio
                                )}</strong>.
                        </p>

                        <div class="alert alert-warning mb-0">

                            <div class="d-flex gap-2">

                                <i class="bi bi-exclamation-triangle-fill"></i>

                                <div>
                                    Podrás agregar nuevas propuestas,
                                    modificar la selección actual y
                                    volver a finalizar la etapa antes
                                    de solicitar presupuesto.
                                </div>

                            </div>

                        </div>
                        `,

                            textoConfirmar:
                                "Modificar cotizaciones",

                            textoCancelar:
                                "Cancelar",

                            tipo:
                                "warning",

                            icono:
                                "bi-pencil-square"
                        }
                    );


                if (!confirmado) {
                    return;
                }


                try {

                    boton.disabled =
                        true;


                    boton.innerHTML = `
                <span class="spinner-border spinner-border-sm me-1"></span>
                Abriendo...
            `;


                    await reabrirCotizacionAdq(
                        solicitudId
                    );


                    window.location.reload();

                }
                catch (
                error
                ) {

                    console.error(
                        "Error al reabrir cotización:",
                        error
                    );


                    mostrarAdvertenciaAdq(
                        "No fue posible modificar las cotizaciones",
                        error.message
                        ??
                        "Ocurrió un error."
                    );


                    boton.disabled =
                        false;


                    boton.innerHTML = `
                <i class="bi bi-pencil-square"></i>

                <span>
                    Modificar cotizaciones
                </span>
            `;
                }
            }
        );

        // =========================================================
        // FORMATEAR TAMAÑO DE ARCHIVO
        // =========================================================

        function formatearTamanoArchivoCotizacionAdq(
            bytes
        ) {

            const tamano =
                Number(
                    bytes ??
                    0
                );


            if (
                tamano <=
                0
            ) {

                return "0 KB";
            }


            if (
                tamano <
                1024 * 1024
            ) {

                return `${(
                    tamano /
                    1024
                ).toFixed(
                    1
                )} KB`;
            }


            return `${(
                tamano /
                (
                    1024 *
                    1024
                )
            ).toFixed(
                1
            )} MB`;
        }

        // =========================================================
        // RENDERIZAR ARCHIVOS DE COTIZACIÓN
        // =========================================================

        function renderizarArchivosCotizacionAdq(
            archivos
        ) {

            if (
                !Array.isArray(
                    archivos
                )
                ||
                archivos.length ===
                0
            ) {

                return `
            <span class="adq-quote-no-files">
                Sin archivos
            </span>
        `;
            }


            return archivos
                .map(
                    function (
                        archivo
                    ) {

                        return `
                    <a href="${escapeHtmlAdq(
                            archivo.rutaArchivo
                            ??
                            "#"
                        )}"
                       target="_blank"
                       rel="noopener noreferrer"
                       class="adq-quote-file-link">

                        <i class="bi bi-paperclip"></i>

                        <span>
                            ${escapeHtmlAdq(
                            archivo.nombreOriginal
                            ??
                            "Archivo"
                        )}
                        </span>

                        <small>
                            ${formatearTamanoArchivoCotizacionAdq(
                            archivo.tamanoBytes
                        )}
                        </small>

                    </a>
                `;
                    }
                )
                .join(
                    ""
                );
        }

        // =========================================================
        // MOSTRAR EVIDENCIAS EXISTENTES DURANTE EDICIÓN
        // =========================================================

        function mostrarEvidenciasExistentesEdicionAdq(
            cotizacion
        ) {

            const detalles =
                Array.isArray(
                    cotizacion?.detalles
                )
                    ? [...cotizacion.detalles]
                    : [];


            detalles.sort(
                function (
                    a,
                    b
                ) {

                    return Number(
                        a.orden
                        ??
                        0
                    )
                        -
                        Number(
                            b.orden
                            ??
                            0
                        );
                }
            );


            const filas =
                Array.from(
                    contenedorDetallesCotizacionAdq
                        ?.querySelectorAll(
                            "tr"
                        )
                    ??
                    []
                );


            filas.forEach(
                function (
                    fila,
                    index
                ) {

                    const detalleCotizacion =
                        detalles[
                        index
                        ];


                    if (
                        !detalleCotizacion
                    ) {
                        return;
                    }


                    const inputEvidencia =
                        fila.querySelector(
                            ".cotizacion-evidencia-adq"
                        );


                    if (
                        !inputEvidencia
                    ) {
                        return;
                    }


                    const evidencias =
                        Array.isArray(
                            detalleCotizacion.evidencias
                        )
                            ? detalleCotizacion.evidencias
                            : [];


                    // =====================================================
                    // EVIDENCIA OBLIGATORIA SÓLO SI NO EXISTE UNA ACTUAL
                    // =====================================================

                    inputEvidencia.required =
                        evidencias.length ===
                        0;


                    const contenedorActual =
                        fila.querySelector(
                            ".adq-evidencia-existente-edicion"
                        );


                    contenedorActual
                        ?.remove();


                    const contenedor =
                        document.createElement(
                            "div"
                        );


                    contenedor.className =
                        "adq-evidencia-existente-edicion mt-2";


                    if (
                        evidencias.length ===
                        0
                    ) {

                        contenedor.innerHTML = `
                        <div class="small text-danger">

                            <i class="bi bi-exclamation-circle me-1"></i>

                            No existe evidencia registrada.
                            Debes seleccionar un archivo antes de guardar.

                        </div>
                    `;
                    }
                    else {

                        const archivosHtml =
                            evidencias
                                .map(
                                    function (
                                        archivo
                                    ) {

                                        return `
                                    <a href="${escapeAttributeAdq(
                                            archivo.rutaArchivo
                                            ??
                                            "#"
                                        )}"
                                       target="_blank"
                                       rel="noopener noreferrer"
                                       class="d-flex align-items-center gap-2 text-decoration-none mb-1">

                                        <i class="bi bi-file-earmark-check text-success"></i>

                                        <span class="text-truncate">
                                            ${escapeHtmlAdq(
                                            archivo.nombreOriginal
                                            ??
                                            "Evidencia"
                                        )}
                                        </span>

                                    </a>
                                `;
                                    }
                                )
                                .join(
                                    ""
                                );


                        contenedor.innerHTML = `
                    <div class="small fw-semibold text-success mb-1">

                        <i class="bi bi-check-circle-fill me-1"></i>

                        Evidencia actual

                    </div>

                    ${archivosHtml}

                    <div class="small text-muted mt-1">

                        El archivo de arriba se conservará.
                        Selecciona uno nuevo únicamente si deseas reemplazarlo.

                    </div>
                `;
                    }


                    inputEvidencia
                        .insertAdjacentElement(
                            "afterend",
                            contenedor
                        );


                    const ayuda =
                        inputEvidencia
                            .parentElement
                            ?.querySelector(
                                "small.text-muted"
                            );


                    if (
                        ayuda
                    ) {

                        ayuda.textContent =
                            evidencias.length >
                                0
                                ? "Opcional al editar · La evidencia actual se conservará si no seleccionas otra."
                                : "Obligatorio · Debes cargar evidencia para este producto. Máximo 15 MB.";
                    }
                }
            );
        }

        // =========================================================
        // DETALLE DE UNA COTIZACIÓN
        // =========================================================

        function renderizarDetalleCotizacionRegistradaAdq(
            cotizacion
        ) {

            const detalles =
                Array.isArray(
                    cotizacion.detalles
                )
                    ? cotizacion.detalles
                    : [];


            const productosHtml =
                detalles.length ===
                    0
                    ? `
                <div class="text-muted small">
                    No existen productos registrados.
                </div>
            `
                    : detalles
                        .map(
                            function (
                                detalle
                            ) {

                                return `
                            <div class="adq-quote-detail-product">

                                <div class="adq-quote-detail-product-main">

                                    <strong>
                                        ${escapeHtmlAdq(
                                    detalle.productoServicio
                                    ??
                                    ""
                                )}
                                    </strong>

                                    ${detalle.descripcion
                                        ? `
                                                <small>
                                                    ${escapeHtmlAdq(
                                            detalle.descripcion
                                        )}
                                                </small>
                                            `
                                        : ""
                                    }

                                </div>


                                <div class="adq-quote-detail-data">

                                    <div>
                                        <span>
                                            Cantidad
                                        </span>

                                        <strong>
                                            ${Number(
                                        detalle.cantidad ??
                                        0
                                    )}
                                            ${escapeHtmlAdq(
                                        detalle.unidad ??
                                        ""
                                    )}
                                        </strong>
                                    </div>


                                    <div>
                                        <span>
                                            Precio unitario
                                        </span>

                                        <strong>
                                            ${formatearMonedaCotizacionAdq(
                                        detalle.precioUnitario
                                    )}
                                        </strong>
                                    </div>


                                    <div>
                                        <span>
                                            Importe
                                        </span>

                                        <strong>
                                            ${formatearMonedaCotizacionAdq(
                                        detalle.importe
                                    )}
                                        </strong>
                                    </div>

                                </div>


                                <div class="adq-quote-detail-files">

                                    <span class="adq-quote-detail-label">
                                        Evidencia
                                    </span>

                                    ${renderizarArchivosCotizacionAdq(
                                        detalle.evidencias
                                    )}

                                </div>

                            </div>
                        `;
                            }
                        )
                        .join(
                            ""
                        );


            const archivosAdicionales =
                renderizarArchivosCotizacionAdq(
                    cotizacion.archivosAdicionales
                );


            return `
        <div class="adq-quote-expanded-detail">

            <div class="adq-quote-expanded-section">

                <div class="adq-quote-expanded-title">
                    Productos y servicios
                </div>

                ${productosHtml}

            </div>


            <div class="adq-quote-expanded-section">

                <div class="adq-quote-expanded-title">
                    Archivos adicionales
                </div>

                <div class="adq-quote-additional-files">
                    ${archivosAdicionales}
                </div>

            </div>


            ${cotizacion.observaciones
                    ? `
                        <div class="adq-quote-expanded-section">

                            <div class="adq-quote-expanded-title">
                                Observaciones
                            </div>

                            <div class="adq-quote-observations">
                                ${escapeHtmlAdq(
                        cotizacion.observaciones
                    )}
                            </div>

                        </div>
                    `
                    : ""
                }

        </div>
    `;
        }

        // =========================================================
        // RENDERIZAR COTIZACIONES REGISTRADAS
        // =========================================================

        function renderizarCotizacionesRegistradasAdq(
            cotizaciones
        ) {

            if (
                !seccionCotizacionesRegistradasAdq
                ||
                !listaCotizacionesRegistradasAdq
            ) {
                return;
            }


            // =====================================================
            // GUARDAR COTIZACIONES EN MEMORIA
            // =====================================================

            cotizacionesRegistradasActualesAdq =
                Array.isArray(
                    cotizaciones
                )
                    ? cotizaciones
                    : [];


            listaCotizacionesRegistradasAdq.innerHTML =
                "";


            // =====================================================
            // SIN COTIZACIONES
            // =====================================================

            if (
                !Array.isArray(
                    cotizaciones
                )
                ||
                cotizaciones.length ===
                0
            ) {

                seccionCotizacionesRegistradasAdq
                    .classList
                    .add(
                        "d-none"
                    );


                if (
                    contadorCotizacionesAdq
                ) {

                    contadorCotizacionesAdq.textContent =
                        "0 cotizaciones";
                }


                btnFinalizarCotizacionAdq
                    ?.classList
                    .add(
                        "d-none"
                    );


                accionesSeleccionCotizacionesAdq
                    ?.classList
                    .add(
                        "d-none"
                    );


                accionesSeleccionCotizacionesAdq
                    ?.classList
                    .remove(
                        "d-flex"
                    );


                actualizarAccionesCotizacionesAdq();


                return;
            }


            // =====================================================
            // MOSTRAR SECCIÓN
            // =====================================================

            seccionCotizacionesRegistradasAdq
                .classList
                .remove(
                    "d-none"
                );


            accionesSeleccionCotizacionesAdq
                ?.classList
                .remove(
                    "d-none"
                );


            accionesSeleccionCotizacionesAdq
                ?.classList
                .add(
                    "d-flex"
                );


            if (
                contadorCotizacionesAdq
            ) {

                contadorCotizacionesAdq.textContent =
                    cotizaciones.length ===
                        1
                        ? "1 cotización"
                        : `${cotizaciones.length} cotizaciones`;
            }


            // =====================================================
            // COMPARATIVO ECONÓMICO
            // =====================================================

            const totales =
                cotizaciones.map(
                    function (
                        cotizacion
                    ) {

                        return Number(
                            cotizacion.total
                            ??
                            0
                        );
                    }
                );


            const totalMenor =
                Math.min(
                    ...totales
                );


            const totalMayor =
                Math.max(
                    ...totales
                );


            const todosIguales =
                cotizaciones.length >
                1
                &&
                totalMenor ===
                totalMayor;


            const existeComparativo =
                cotizaciones.length >
                1
                &&
                !todosIguales;


            const existeSeleccionada =
                cotizaciones.some(
                    function (
                        cotizacion
                    ) {

                        return (
                            cotizacion.esPrincipal ===
                            true
                        );
                    }
                );


            // =====================================================
            // BOTÓN FINALIZAR
            // =====================================================

            if (
                btnFinalizarCotizacionAdq
            ) {

                btnFinalizarCotizacionAdq
                    .classList
                    .toggle(
                        "d-none",
                        !existeSeleccionada
                    );
            }


            // =====================================================
            // RENDERIZAR COTIZACIONES
            // =====================================================

            cotizaciones.forEach(
                function (
                    cotizacion,
                    index
                ) {

                    const cotizacionId =
                        Number(
                            cotizacion.id
                            ??
                            0
                        );


                    const total =
                        Number(
                            cotizacion.total
                            ??
                            0
                        );


                    const esSeleccionada =
                        cotizacion.esPrincipal ===
                        true;


                    const esMenor =
                        existeComparativo
                        &&
                        total ===
                        totalMenor;


                    const esMayor =
                        existeComparativo
                        &&
                        total ===
                        totalMayor;


                    const esIntermedia =
                        existeComparativo
                        &&
                        !esMenor
                        &&
                        !esMayor;


                    const esUnica =
                        cotizaciones.length ===
                        1;


                    // =================================================
                    // CREAR TARJETA
                    // =================================================

                    const tarjeta =
                        document.createElement(
                            "article"
                        );


                    tarjeta.className =
                        "adq-quote-history-card";


                    tarjeta.dataset.cotizacionId =
                        String(
                            cotizacionId
                        );


                    // =================================================
                    // CLASIFICACIÓN DE PRECIO
                    // =================================================

                    if (
                        esMenor
                    ) {

                        tarjeta.classList.add(
                            "adq-quote-price-low"
                        );
                    }


                    if (
                        esIntermedia
                    ) {

                        tarjeta.classList.add(
                            "adq-quote-price-medium"
                        );
                    }


                    if (
                        esMayor
                    ) {

                        tarjeta.classList.add(
                            "adq-quote-price-high"
                        );
                    }


                    if (
                        todosIguales
                        ||
                        esUnica
                    ) {

                        tarjeta.classList.add(
                            "adq-quote-price-equal"
                        );
                    }


                    if (
                        esSeleccionada
                    ) {

                        tarjeta.classList.add(
                            "adq-quote-history-card-selected"
                        );
                    }


                    // =================================================
                    // INFORMACIÓN DEL PROVEEDOR
                    // =================================================

                    const rfc =
                        cotizacion.rfcProveedor
                        ??
                        "";


                    const contacto =
                        cotizacion.contactoProveedor
                        ??
                        "";


                    const correo =
                        cotizacion.emailProveedor
                        ??
                        "";


                    const telefono =
                        cotizacion.telefonoProveedor
                        ??
                        "";


                    // =================================================
                    // BADGE DE PRECIO
                    // =================================================

                    let badgePrecio =
                        "";


                    if (
                        esMenor
                    ) {

                        badgePrecio = `
                    <span class="adq-quote-price-badge adq-quote-price-badge-low">

                        <i class="bi bi-arrow-down-circle-fill"></i>

                        Menor precio

                    </span>
                `;
                    }
                    else if (
                        esMayor
                    ) {

                        badgePrecio = `
                    <span class="adq-quote-price-badge adq-quote-price-badge-high">

                        <i class="bi bi-arrow-up-circle-fill"></i>

                        Mayor precio

                    </span>
                `;
                    }
                    else if (
                        esIntermedia
                    ) {

                        badgePrecio = `
                    <span class="adq-quote-price-badge adq-quote-price-badge-medium">

                        <i class="bi bi-dash-circle-fill"></i>

                        Precio intermedio

                    </span>
                `;
                    }
                    else if (
                        todosIguales
                    ) {

                        badgePrecio = `
                    <span class="adq-quote-price-badge adq-quote-price-badge-equal">

                        <i class="bi bi-arrows-expand"></i>

                        Precio equivalente

                    </span>
                `;
                    }
                    else if (
                        esUnica
                    ) {

                        badgePrecio = `
                    <span class="adq-quote-price-badge adq-quote-price-badge-equal">

                        <i class="bi bi-receipt"></i>

                        Única cotización

                    </span>
                `;
                    }


                    // =================================================
                    // BADGE DE COTIZACIÓN SELECCIONADA
                    // =================================================

                    const badgeSeleccionada =
                        esSeleccionada
                            ? `
                        <span class="adq-quote-selected-badge">

                            <i class="bi bi-check-circle-fill"></i>

                            Cotización seleccionada

                        </span>
                    `
                            : "";


                    // =================================================
                    // BOTÓN SELECCIONAR
                    // =================================================

                    const botonSeleccion =
                        esSeleccionada
                            ? `
                        <button type="button"
                                class="btn btn-sm btn-success adq-btn-selected"
                                disabled>

                            <i class="bi bi-check2-circle me-1"></i>

                            Seleccionada

                        </button>
                    `
                            : `
                        <button type="button"
                                class="btn btn-sm btn-outline-success btnSeleccionarCotizacionAdq"
                                data-cotizacion-id="${cotizacionId}"
                                data-proveedor="${escapeAttributeAdq(
                                cotizacion.nombreProveedor
                                ??
                                "Proveedor"
                            )}"
                                data-total="${total}">

                            <i class="bi bi-check2-circle me-1"></i>

                            Seleccionar

                        </button>
                    `;


                    // =================================================
                    // HTML DE TARJETA
                    // =================================================

                    tarjeta.innerHTML = `
                <div class="adq-quote-provider-header">

                    <div class="adq-quote-card-selector">

                        <input type="checkbox"
                               class="form-check-input checkboxCotizacionAdq"
                               value="${cotizacionId}"
                               data-cotizacion-id="${cotizacionId}"
                               aria-label="Seleccionar cotización de ${escapeAttributeAdq(
                        cotizacion.nombreProveedor
                        ??
                        "Proveedor"
                    )}" />

                    </div>


                    <div class="adq-quote-provider-number">

                        ${index + 1}

                    </div>


                    <div class="adq-quote-provider-content">

                        <div class="adq-quote-provider-top">

                            <div class="adq-quote-provider-identity">

                                <div class="adq-quote-provider-name-row">

                                    <strong class="adq-quote-provider-name">

                                        ${escapeHtmlAdq(
                        cotizacion.nombreProveedor
                        ??
                        "Proveedor"
                    )}

                                    </strong>


                                    ${badgePrecio}


                                    ${badgeSeleccionada}

                                </div>


                                <span class="adq-quote-provider-subtitle">

                                    Información general del proveedor

                                </span>

                            </div>


                            <div class="adq-quote-provider-actions">

                                <div class="adq-quote-history-amount">

                                    <span>
                                        Total
                                    </span>

                                    <strong>

                                        ${formatearMonedaCotizacionAdq(
                        total
                    )}

                                    </strong>

                                </div>


                                <button type="button"
                                        class="btn btn-sm btn-outline-primary btnVerDetalleCotizacionAdq"
                                        data-cotizacion-id="${cotizacionId}">

                                    <i class="bi bi-eye me-1"></i>

                                    Ver detalle

                                </button>


                                ${botonSeleccion}

                            </div>

                        </div>


                        <div class="adq-quote-provider-data-grid">

                            <div class="adq-quote-provider-data-item">

                                <div class="adq-quote-provider-data-icon">

                                    <i class="bi bi-building"></i>

                                </div>


                                <div>

                                    <span>
                                        RFC
                                    </span>

                                    <strong>

                                        ${rfc
                            ? escapeHtmlAdq(
                                rfc
                            )
                            : "No registrado"
                        }

                                    </strong>

                                </div>

                            </div>


                            <div class="adq-quote-provider-data-item">

                                <div class="adq-quote-provider-data-icon">

                                    <i class="bi bi-person"></i>

                                </div>


                                <div>

                                    <span>
                                        Contacto
                                    </span>

                                    <strong>

                                        ${contacto
                            ? escapeHtmlAdq(
                                contacto
                            )
                            : "No registrado"
                        }

                                    </strong>

                                </div>

                            </div>


                            <div class="adq-quote-provider-data-item">

                                <div class="adq-quote-provider-data-icon">

                                    <i class="bi bi-envelope"></i>

                                </div>


                                <div>

                                    <span>
                                        Correo electrónico
                                    </span>

                                    <strong>

                                        ${correo
                            ? escapeHtmlAdq(
                                correo
                            )
                            : "No registrado"
                        }

                                    </strong>

                                </div>

                            </div>


                            <div class="adq-quote-provider-data-item">

                                <div class="adq-quote-provider-data-icon">

                                    <i class="bi bi-telephone"></i>

                                </div>


                                <div>

                                    <span>
                                        Teléfono
                                    </span>

                                    <strong>

                                        ${telefono
                            ? escapeHtmlAdq(
                                telefono
                            )
                            : "No registrado"
                        }

                                    </strong>

                                </div>

                            </div>

                        </div>

                    </div>

                </div>


                <div class="adq-quote-history-detail d-none"
                     data-detalle-cotizacion-id="${cotizacionId}">

                    ${renderizarDetalleCotizacionRegistradaAdq(
                            cotizacion
                        )}

                </div>
            `;


                    listaCotizacionesRegistradasAdq
                        .appendChild(
                            tarjeta
                        );
                }
            );


            // =====================================================
            // REINICIAR ESTADO DE SELECCIÓN
            // =====================================================

            actualizarAccionesCotizacionesAdq();
        }

        // =========================================================
        // OBTENER COTIZACIONES MARCADAS
        // =========================================================

        function obtenerCotizacionesMarcadasAdq() {

            return Array.from(
                document.querySelectorAll(
                    ".checkboxCotizacionAdq:checked"
                )
            )
                .map(
                    function (
                        checkbox
                    ) {

                        return Number(
                            checkbox.dataset.cotizacionId
                            ??
                            0
                        );
                    }
                )
                .filter(
                    function (
                        id
                    ) {

                        return id >
                            0;
                    }
                );
        }


        // =========================================================
        // ACTUALIZAR ACCIONES DE COTIZACIONES
        // =========================================================

        function actualizarAccionesCotizacionesAdq() {

            const seleccionadas =
                obtenerCotizacionesMarcadasAdq();


            const total =
                seleccionadas.length;


            if (
                contadorSeleccionCotizacionesAdq
            ) {

                contadorSeleccionCotizacionesAdq.textContent =
                    total ===
                        1
                        ? "1 seleccionada"
                        : `${total} seleccionadas`;
            }


            if (
                btnEditarCotizacionSeleccionadaAdq
            ) {

                btnEditarCotizacionSeleccionadaAdq.disabled =
                    total ===
                    0;
            }


            if (
                btnEliminarCotizacionesSeleccionadasAdq
            ) {

                btnEliminarCotizacionesSeleccionadasAdq.disabled =
                    total ===
                    0;
            }
        }


        // =========================================================
        // CAMBIO DE SELECCIÓN
        // =========================================================

        document.addEventListener(
            "change",
            function (
                event
            ) {

                const checkbox =
                    event.target.closest(
                        ".checkboxCotizacionAdq"
                    );


                if (!checkbox) {
                    return;
                }


                const tarjeta =
                    checkbox.closest(
                        ".adq-quote-history-card"
                    );


                tarjeta
                    ?.classList
                    .toggle(
                        "adq-quote-card-checked",
                        checkbox.checked
                    );


                actualizarAccionesCotizacionesAdq();
            }
        );

        // =========================================================
        // EDITAR COTIZACIÓN SELECCIONADA
        // =========================================================

        btnEditarCotizacionSeleccionadaAdq
            ?.addEventListener(
                "click",
                async function () {

                    const seleccionadas =
                        obtenerCotizacionesMarcadasAdq();


                    if (
                        seleccionadas.length ===
                        0
                    ) {

                        mostrarAdvertenciaAdq(
                            "Selecciona una cotización",
                            "Debes seleccionar una cotización para poder editarla."
                        );

                        return;
                    }


                    if (
                        seleccionadas.length >
                        1
                    ) {

                        await confirmarAccionAdq(
                            {
                                titulo:
                                    "Solo una cotización",

                                mensaje:
                                    `
                            <p class="mb-3">
                                Para editar una cotización debes seleccionar solamente un registro.
                            </p>

                            <div class="alert alert-info mb-0">

                                <div class="d-flex gap-2">

                                    <i class="bi bi-info-circle-fill"></i>

                                    <div>
                                        Actualmente tienes seleccionadas
                                        <strong>
                                            ${seleccionadas.length}
                                        </strong>
                                        cotizaciones.
                                    </div>

                                </div>

                            </div>
                            `,

                                textoConfirmar:
                                    "Entendido",

                                textoCancelar:
                                    "Cerrar",

                                tipo:
                                    "primary",

                                icono:
                                    "bi-pencil-square"
                            }
                        );

                        return;
                    }


                    const cotizacionId =
                        Number(
                            seleccionadas[0]
                        );


                    // =====================================================
                    // IDENTIFICAR SOLICITUD
                    // =====================================================

                    const solicitudId =
                        Number(
                            solicitudCotizacionActualAdq
                                ?.id
                            ??
                            cotizacionSolicitudIdAdq
                                ?.value
                            ??
                            0
                        );


                    if (
                        solicitudId <=
                        0
                    ) {

                        mostrarAdvertenciaAdq(
                            "Solicitud no identificada",
                            "No fue posible identificar la solicitud de la cotización."
                        );

                        return;
                    }


                    // =====================================================
                    // CONSULTAR INFORMACIÓN ACTUALIZADA DESDE BD
                    // =====================================================

                    let cotizacionesActualizadas =
                        [];


                    try {

                        cotizacionesActualizadas =
                            await obtenerCotizacionesSolicitudAdq(
                                solicitudId
                            );

                    }
                    catch (
                    error
                    ) {

                        console.error(
                            "Error al consultar la cotización para edición:",
                            error
                        );


                        mostrarAdvertenciaAdq(
                            "No fue posible editar",
                            error.message
                            ??
                            "No fue posible consultar la información actualizada de la cotización."
                        );

                        return;
                    }


                    cotizacionesRegistradasActualesAdq =
                        Array.isArray(
                            cotizacionesActualizadas
                        )
                            ? cotizacionesActualizadas
                            : [];


                    const cotizacion =
                        cotizacionesRegistradasActualesAdq
                            .find(
                                function (
                                    item
                                ) {

                                    return Number(
                                        item.id
                                        ??
                                        0
                                    ) ===
                                        cotizacionId;
                                }
                            );


                    if (
                        !cotizacion
                    ) {

                        mostrarAdvertenciaAdq(
                            "No fue posible editar",
                            "No se encontró la información de la cotización seleccionada."
                        );

                        return;
                    }


                    // =====================================================
                    // ACTIVAR MODO EDICIÓN
                    // =====================================================

                    cotizacionEditandoActualAdq =
                        cotizacion;


                    archivosCotizacionEliminadosAdq =
                        new Set();


                    sincronizarArchivosCotizacionEliminarAdq();


                    if (
                        cotizacionEditarIdAdq
                    ) {

                        cotizacionEditarIdAdq.value =
                            String(
                                cotizacionId
                            );
                    }


                    if (
                        formCotizacionAdq
                    ) {

                        formCotizacionAdq.action =
                            `${window.location.pathname}?handler=EditarCotizacion`;
                    }


                    // =====================================================
                    // DATOS DEL PROVEEDOR
                    // =====================================================

                    const nombreProveedor =
                        document.getElementById(
                            "cotizacionNombreProveedorAdq"
                        );


                    const rfc =
                        document.getElementById(
                            "cotizacionRfcProveedorAdq"
                        );


                    const telefono =
                        document.getElementById(
                            "cotizacionTelefonoProveedorAdq"
                        );


                    const contacto =
                        document.getElementById(
                            "cotizacionContactoProveedorAdq"
                        );


                    const email =
                        document.getElementById(
                            "cotizacionEmailProveedorAdq"
                        );


                    const observaciones =
                        document.getElementById(
                            "cotizacionObservacionesAdq"
                        );


                    if (
                        nombreProveedor
                    ) {

                        nombreProveedor.value =
                            cotizacion.nombreProveedor
                            ??
                            "";
                    }


                    if (
                        rfc
                    ) {

                        rfc.value =
                            cotizacion.rfcProveedor
                            ??
                            "";
                    }


                    if (
                        telefono
                    ) {

                        telefono.value =
                            cotizacion.telefonoProveedor
                            ??
                            "";
                    }


                    if (
                        contacto
                    ) {

                        contacto.value =
                            cotizacion.contactoProveedor
                            ??
                            "";
                    }


                    if (
                        email
                    ) {

                        email.value =
                            cotizacion.emailProveedor
                            ??
                            "";
                    }


                    if (
                        observaciones
                    ) {

                        observaciones.value =
                            cotizacion.observaciones
                            ??
                            "";
                    }


                    // =====================================================
                    // IVA
                    // =====================================================

                    if (
                        cotizacionAplicaIvaAdq
                    ) {

                        cotizacionAplicaIvaAdq.checked =
                            cotizacion.aplicaIva ===
                            true;
                    }


                    if (
                        cotizacionPorcentajeIvaAdq
                    ) {

                        cotizacionPorcentajeIvaAdq.value =
                            String(
                                cotizacion.porcentajeIva
                                ??
                                16
                            );
                    }


                    // =====================================================
                    // VOLVER A CREAR PRODUCTOS
                    // =====================================================

                    renderizarDetallesCotizacionAdq(
                        solicitudCotizacionActualAdq
                            ?.detalles
                        ??
                        []
                    );


                    // =====================================================
                    // CARGAR PRECIOS Y DESCRIPCIONES
                    // =====================================================

                    const detallesCotizacion =
                        Array.isArray(
                            cotizacion.detalles
                        )
                            ? [...cotizacion.detalles]
                            : [];


                    detallesCotizacion.sort(
                        function (
                            a,
                            b
                        ) {

                            return Number(
                                a.orden
                                ??
                                0
                            )
                                -
                                Number(
                                    b.orden
                                    ??
                                    0
                                );
                        }
                    );


                    const filasDetalles =
                        Array.from(
                            contenedorDetallesCotizacionAdq
                                ?.querySelectorAll(
                                    "tr"
                                )
                            ??
                            []
                        );


                    filasDetalles.forEach(
                        function (
                            fila,
                            index
                        ) {

                            const detalleCotizacion =
                                detallesCotizacion[
                                index
                                ];


                            if (
                                !detalleCotizacion
                            ) {
                                return;
                            }


                            const inputPrecio =
                                fila.querySelector(
                                    ".cotizacion-precio-unitario-adq"
                                );


                            if (
                                inputPrecio
                            ) {

                                inputPrecio.value =
                                    String(
                                        detalleCotizacion.precioUnitario
                                        ??
                                        0
                                    );
                            }


                            const inputDescripcion =
                                fila.querySelector(
                                    'textarea[name$=".DescripcionProveedor"], input[name$=".DescripcionProveedor"]'
                                );


                            if (
                                inputDescripcion
                            ) {

                                inputDescripcion.value =
                                    detalleCotizacion.descripcion
                                    ??
                                    "";
                            }
                        }
                    );


                    // =====================================================
                    // EVIDENCIAS EXISTENTES
                    // =====================================================

                    mostrarEvidenciasExistentesEdicionAdq(
                        cotizacion
                    );


                    // =====================================================
                    // ARCHIVOS ADICIONALES
                    // =====================================================

                    archivosCotizacionSeleccionadosAdq =
                        [];


                    sincronizarArchivosCotizacionAdq();


                    renderizarArchivosCotizacionSeleccionadosAdq();


                    calcularCotizacionAdq();


                    // =====================================================
                    // INTERFAZ DE EDICIÓN
                    // =====================================================

                    seccionCapturaProveedorAdq
                        ?.classList
                        .add(
                            "adq-quote-editing"
                        );


                    badgeEdicionCotizacionAdq
                        ?.classList
                        .remove(
                            "d-none"
                        );


                    btnCancelarEdicionCotizacionAdq
                        ?.classList
                        .remove(
                            "d-none"
                        );


                    if (
                        tituloCapturaCotizacionAdq
                    ) {

                        tituloCapturaCotizacionAdq.textContent =
                            "Editar cotización";
                    }


                    if (
                        subtituloCapturaCotizacionAdq
                    ) {

                        subtituloCapturaCotizacionAdq.textContent =
                            `Modificando la propuesta de ${cotizacion.nombreProveedor ?? "Proveedor"}.`;
                    }


                    if (
                        textoGuardarCotizacionAdq
                    ) {

                        textoGuardarCotizacionAdq.textContent =
                            "Guardar cambios";
                    }


                    // =====================================================
                    // BAJAR AL FORMULARIO
                    // =====================================================

                    setTimeout(
                        function () {

                            seccionCapturaProveedorAdq
                                ?.scrollIntoView(
                                    {
                                        behavior:
                                            "smooth",

                                        block:
                                            "start"
                                    }
                                );

                        },
                        100
                    );
                }
            );

        // =========================================================
        // ELIMINAR COTIZACIONES SELECCIONADAS
        // =========================================================

        btnEliminarCotizacionesSeleccionadasAdq
            ?.addEventListener(
                "click",
                async function () {

                    const seleccionadas =
                        obtenerCotizacionesMarcadasAdq();


                    if (
                        seleccionadas.length ===
                        0
                    ) {

                        mostrarAdvertenciaAdq(
                            "Selecciona una cotización",
                            "Debes seleccionar al menos una cotización para eliminar."
                        );

                        return;
                    }


                    // =====================================================
                    // OBTENER NOMBRES DE PROVEEDORES
                    // =====================================================

                    const cotizacionesSeleccionadas =
                        cotizacionesRegistradasActualesAdq
                            .filter(
                                function (
                                    cotizacion
                                ) {

                                    return seleccionadas.includes(
                                        Number(
                                            cotizacion.id
                                            ??
                                            0
                                        )
                                    );
                                }
                            );


                    const proveedoresHtml =
                        cotizacionesSeleccionadas
                            .map(
                                function (
                                    cotizacion
                                ) {

                                    return `
                                <li class="mb-1">
                                    <strong>
                                        ${escapeHtmlAdq(
                                        cotizacion.nombreProveedor
                                        ??
                                        "Proveedor"
                                    )}
                                    </strong>

                                    <span class="text-muted ms-1">
                                        ${formatearMonedaCotizacionAdq(
                                        cotizacion.total
                                        ??
                                        0
                                    )}
                                    </span>
                                </li>
                            `;
                                }
                            )
                            .join(
                                ""
                            );


                    // =====================================================
                    // CONFIRMACIÓN
                    // =====================================================

                    const confirmado =
                        await confirmarAccionAdq(
                            {
                                titulo:
                                    seleccionadas.length ===
                                        1
                                        ? "Eliminar cotización"
                                        : "Eliminar cotizaciones",

                                mensaje:
                                    `
                            <p class="mb-3">

                                ${seleccionadas.length ===
                                        1
                                        ? "¿Deseas eliminar la cotización seleccionada?"
                                        : `¿Deseas eliminar las ${seleccionadas.length} cotizaciones seleccionadas?`
                                    }

                            </p>


                            ${proveedoresHtml
                                        ? `
                                        <div class="border rounded p-3 mb-3 bg-light">

                                            <div class="small fw-semibold text-muted mb-2">
                                                Cotizaciones seleccionadas
                                            </div>

                                            <ul class="mb-0 ps-3">
                                                ${proveedoresHtml}
                                            </ul>

                                        </div>
                                    `
                                        : ""
                                    }


                            <div class="alert alert-danger mb-0">

                                <div class="d-flex gap-2">

                                    <i class="bi bi-exclamation-triangle-fill"></i>

                                    <div>

                                        Las cotizaciones seleccionadas serán
                                        retiradas del comparativo.

                                        <div class="small mt-1">
                                            Esta acción se registrará en el historial.
                                        </div>

                                    </div>

                                </div>

                            </div>
                            `,

                                textoConfirmar:
                                    seleccionadas.length ===
                                        1
                                        ? "Eliminar"
                                        : `Eliminar ${seleccionadas.length}`,

                                textoCancelar:
                                    "Cancelar",

                                tipo:
                                    "danger",

                                icono:
                                    "bi-trash3"
                            }
                        );


                    if (
                        !confirmado
                    ) {
                        return;
                    }


                    // =====================================================
                    // PREPARAR REQUEST
                    // =====================================================

                    const token =
                        formCotizacionAdq
                            ?.querySelector(
                                'input[name="__RequestVerificationToken"]'
                            )
                            ?.value
                        ??
                        "";


                    const datos =
                        new FormData();


                    datos.append(
                        "__RequestVerificationToken",
                        token
                    );


                    seleccionadas.forEach(
                        function (
                            cotizacionId
                        ) {

                            datos.append(
                                "cotizacionIds",
                                String(
                                    cotizacionId
                                )
                            );
                        }
                    );


                    // =====================================================
                    // BLOQUEAR BOTÓN
                    // =====================================================

                    const htmlOriginal =
                        btnEliminarCotizacionesSeleccionadasAdq.innerHTML;


                    try {

                        btnEliminarCotizacionesSeleccionadasAdq.disabled =
                            true;


                        btnEditarCotizacionSeleccionadaAdq.disabled =
                            true;


                        btnEliminarCotizacionesSeleccionadasAdq.innerHTML =
                            `
                        <span class="spinner-border spinner-border-sm me-1"
                              role="status"
                              aria-hidden="true">
                        </span>

                        Eliminando...
                    `;


                        // =================================================
                        // ELIMINAR
                        // =================================================

                        const response =
                            await fetch(
                                "?handler=EliminarCotizaciones",
                                {
                                    method:
                                        "POST",

                                    body:
                                        datos,

                                    headers:
                                    {
                                        "X-Requested-With":
                                            "XMLHttpRequest"
                                    }
                                }
                            );


                        let resultado =
                            null;


                        try {

                            resultado =
                                await response.json();

                        }
                        catch {

                            resultado =
                                null;
                        }


                        if (
                            !response.ok
                            ||
                            !resultado?.success
                        ) {

                            throw new Error(
                                resultado?.message
                                ??
                                "No fue posible eliminar las cotizaciones seleccionadas."
                            );
                        }


                        // =================================================
                        // SI SE ESTABA EDITANDO UNA ELIMINADA,
                        // SALIMOS DEL MODO EDICIÓN
                        // =================================================

                        const cotizacionEditandoId =
                            Number(
                                cotizacionEditarIdAdq
                                    ?.value
                                ??
                                0
                            );


                        if (
                            cotizacionEditandoId >
                            0
                            &&
                            seleccionadas.includes(
                                cotizacionEditandoId
                            )
                        ) {

                            cotizacionEditandoActualAdq =
                                null;


                            archivosCotizacionEliminadosAdq =
                                new Set();


                            sincronizarArchivosCotizacionEliminarAdq();


                            if (
                                cotizacionEditarIdAdq
                            ) {

                                cotizacionEditarIdAdq.value =
                                    "0";
                            }


                            if (
                                formCotizacionAdq
                            ) {

                                formCotizacionAdq.reset();


                                formCotizacionAdq.action =
                                    `${window.location.pathname}?handler=GuardarCotizacion`;
                            }


                            seccionCapturaProveedorAdq
                                ?.classList
                                .remove(
                                    "adq-quote-editing"
                                );


                            badgeEdicionCotizacionAdq
                                ?.classList
                                .add(
                                    "d-none"
                                );


                            btnCancelarEdicionCotizacionAdq
                                ?.classList
                                .add(
                                    "d-none"
                                );


                            if (
                                tituloCapturaCotizacionAdq
                            ) {

                                tituloCapturaCotizacionAdq.textContent =
                                    "Datos del proveedor";
                            }


                            if (
                                subtituloCapturaCotizacionAdq
                            ) {

                                subtituloCapturaCotizacionAdq.textContent =
                                    "Información general del proveedor que emite la cotización.";
                            }


                            if (
                                textoGuardarCotizacionAdq
                            ) {

                                textoGuardarCotizacionAdq.textContent =
                                    "Guardar cotización";
                            }


                            archivosCotizacionSeleccionadosAdq =
                                [];


                            sincronizarArchivosCotizacionAdq();


                            renderizarDetallesCotizacionAdq(
                                solicitudCotizacionActualAdq
                                    ?.detalles
                                ??
                                []
                            );


                            renderizarArchivosCotizacionSeleccionadosAdq();


                            calcularCotizacionAdq();
                        }


                        // =================================================
                        // RECARGAR COMPARATIVO SIN CERRAR MODAL
                        // =================================================

                        const solicitudId =
                            Number(
                                resultado.solicitudId
                                ??
                                solicitudCotizacionActualAdq
                                    ?.id
                                ??
                                0
                            );


                        const cotizacionesActualizadas =
                            await obtenerCotizacionesSolicitudAdq(
                                solicitudId
                            );


                        renderizarCotizacionesRegistradasAdq(
                            cotizacionesActualizadas
                        );


                        // =================================================
                        // MENSAJE
                        // =================================================

                        await confirmarAccionAdq(
                            {
                                titulo:
                                    "Cotizaciones actualizadas",

                                mensaje:
                                    `
                                <p class="mb-0">

                                    ${escapeHtmlAdq(
                                        resultado.message
                                        ??
                                        "La eliminación se realizó correctamente."
                                    )}

                                </p>
                            `,

                                textoConfirmar:
                                    "Entendido",

                                textoCancelar:
                                    "Cerrar",

                                tipo:
                                    "success",

                                icono:
                                    "bi-check-circle-fill"
                            }
                        );

                    }
                    catch (
                    error
                    ) {

                        console.error(
                            "Error al eliminar cotizaciones:",
                            error
                        );


                        mostrarAdvertenciaAdq(
                            "No fue posible eliminar",
                            error.message
                            ??
                            "Ocurrió un error al eliminar las cotizaciones seleccionadas."
                        );

                    }
                    finally {

                        btnEliminarCotizacionesSeleccionadasAdq.innerHTML =
                            htmlOriginal;


                        actualizarAccionesCotizacionesAdq();
                    }
                }
            );

        // =========================================================
        // CANCELAR EDICIÓN DE COTIZACIÓN
        // =========================================================

        btnCancelarEdicionCotizacionAdq
            ?.addEventListener(
                "click",
                function () {

                    cotizacionEditandoActualAdq =
                        null;

                    archivosCotizacionEliminadosAdq =
                        new Set();


                    sincronizarArchivosCotizacionEliminarAdq();

                    if (
                        formCotizacionAdq
                    ) {

                        formCotizacionAdq.action =
                            `${window.location.pathname}?handler=GuardarCotizacion`;
                    }


                    if (
                        cotizacionEditarIdAdq
                    ) {

                        cotizacionEditarIdAdq.value =
                            "0";
                    }


                    formCotizacionAdq
                        ?.reset();


                    seccionCapturaProveedorAdq
                        ?.classList
                        .remove(
                            "adq-quote-editing"
                        );


                    badgeEdicionCotizacionAdq
                        ?.classList
                        .add(
                            "d-none"
                        );


                    btnCancelarEdicionCotizacionAdq
                        ?.classList
                        .add(
                            "d-none"
                        );


                    if (
                        tituloCapturaCotizacionAdq
                    ) {

                        tituloCapturaCotizacionAdq.textContent =
                            "Datos del proveedor";
                    }


                    if (
                        subtituloCapturaCotizacionAdq
                    ) {

                        subtituloCapturaCotizacionAdq.textContent =
                            "Información general del proveedor que emite la cotización.";
                    }


                    if (
                        textoGuardarCotizacionAdq
                    ) {

                        textoGuardarCotizacionAdq.textContent =
                            "Guardar cotización";
                    }


                    renderizarDetallesCotizacionAdq(
                        solicitudCotizacionActualAdq
                            ?.detalles
                        ??
                        []
                    );


                    calcularCotizacionAdq();
                }
            );

        // =========================================================
        // VER / OCULTAR DETALLE DE COTIZACIÓN
        // =========================================================

        document.addEventListener(
            "click",
            function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnVerDetalleCotizacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const cotizacionId =
                    Number(
                        boton.dataset.cotizacionId
                        ??
                        0
                    );


                const detalle =
                    document.querySelector(
                        `[data-detalle-cotizacion-id="${cotizacionId}"]`
                    );


                if (!detalle) {
                    return;
                }


                const estaOculto =
                    detalle.classList.contains(
                        "d-none"
                    );


                detalle.classList.toggle(
                    "d-none"
                );


                boton.innerHTML =
                    estaOculto
                        ? `
                    <i class="bi bi-eye-slash me-1"></i>
                    Ocultar
                `
                        : `
                    <i class="bi bi-eye me-1"></i>
                    Ver detalle
                `;
            }
        );

        // =========================================================
        // SELECCIONAR COTIZACIÓN - EVENTO
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnSeleccionarCotizacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const cotizacionId =
                    Number(
                        boton.dataset.cotizacionId
                        ??
                        0
                    );


                const proveedor =
                    boton.dataset.proveedor
                    ??
                    "Proveedor";


                const total =
                    Number(
                        boton.dataset.total
                        ??
                        0
                    );


                if (
                    cotizacionId <=
                    0
                ) {
                    return;
                }

                const confirmado =
                    await confirmarAccionAdq(
                        {
                            titulo:
                                "Seleccionar cotización",

                            mensaje:
                                `
                <p class="mb-3">
                    Confirma que deseas seleccionar esta cotización como la propuesta elegida.
                </p>

                <div class="adq-confirm-data">

                    <div class="adq-confirm-data-item">

                        <span>
                            Proveedor
                        </span>

                        <strong>
                            ${escapeHtmlAdq(
                                    proveedor
                                    ??
                                    "Proveedor"
                                )}
                        </strong>

                    </div>

                    <div class="adq-confirm-data-item">

                        <span>
                            Total
                        </span>

                        <strong class="text-success fs-5">
                            ${formatearMonedaCotizacionAdq(
                                    total
                                )}
                        </strong>

                    </div>

                </div>

                <div class="small text-muted mt-3">
                    Puedes cambiar la selección mientras la etapa de cotización no haya sido finalizada.
                </div>
                `,

                            textoConfirmar:
                                "Seleccionar",

                            textoCancelar:
                                "Cancelar",

                            tipo:
                                "success",

                            icono:
                                "bi-file-earmark-check"
                        }
                    );


                if (!confirmado) {
                    return;
                }

                try {

                    boton.disabled =
                        true;




                    boton.innerHTML = `
                <span class="spinner-border spinner-border-sm me-1"></span>
                Seleccionando...
            `;


                    await seleccionarCotizacionAdq(
                        cotizacionId
                    );


                    const solicitudId =
                        Number(
                            solicitudCotizacionActualAdq?.id
                            ??
                            0
                        );


                    const cotizaciones =
                        await obtenerCotizacionesSolicitudAdq(
                            solicitudId
                        );


                    renderizarCotizacionesRegistradasAdq(
                        cotizaciones
                    );


                }
                catch (
                error
                ) {

                    console.error(
                        "Error al seleccionar cotización:",
                        error
                    );


                    mostrarAdvertenciaAdq(
                        "No fue posible seleccionar",
                        error.message
                        ??
                        "Ocurrió un error."
                    );
                }
            }
        );

        // =========================================================
        // FINALIZAR ETAPA DE COTIZACIÓN - EVENTO
        // =========================================================

        btnFinalizarCotizacionAdq
            ?.addEventListener(
                "click",
                async function () {

                    const solicitudId =
                        Number(
                            solicitudCotizacionActualAdq?.id
                            ??
                            0
                        );


                    if (
                        solicitudId <=
                        0
                    ) {

                        mostrarAdvertenciaAdq(
                            "Solicitud no identificada",
                            "No fue posible identificar la solicitud."
                        );

                        return;
                    }


                    const confirmado =
                        await confirmarAccionAdq(
                            {
                                titulo:
                                    "Finalizar cotización",

                                mensaje:
                                    `
                <p class="mb-3">
                    La cotización seleccionada será utilizada para continuar con el proceso de adquisición.
                </p>

                <div class="alert alert-warning mb-0">

                    <div class="d-flex gap-2">

                        <i class="bi bi-exclamation-triangle-fill"></i>

                        <div>
                            Después de finalizar esta etapa ya no podrás cambiar el proveedor desde la fase de cotización.
                        </div>

                    </div>

                </div>
                `,

                                textoConfirmar:
                                    "Finalizar cotización",

                                textoCancelar:
                                    "Regresar",

                                tipo:
                                    "success",

                                icono:
                                    "bi-check2-circle"
                            }
                        );


                    if (!confirmado) {
                        return;
                    }


                    try {

                        btnFinalizarCotizacionAdq.disabled =
                            true;


                        btnFinalizarCotizacionAdq.innerHTML = `
                    <span class="spinner-border spinner-border-sm me-1"></span>
                    Finalizando...
                `;


                        await finalizarCotizacionAdq(
                            solicitudId
                        );

                        window.location.reload();

                    }
                    catch (
                    error
                    ) {

                        console.error(
                            "Error al finalizar cotización:",
                            error
                        );


                        mostrarAdvertenciaAdq(
                            "No fue posible finalizar",
                            error.message
                            ??
                            "Ocurrió un error."
                        );


                        btnFinalizarCotizacionAdq.disabled =
                            false;


                        btnFinalizarCotizacionAdq.innerHTML = `
                    <i class="bi bi-check2-circle me-1"></i>
                    Finalizar cotización
                `;
                    }
                }
            );

        // =========================================================
        // REABRIR / MODIFICAR COTIZACIONES
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnReabrirCotizacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const solicitudId =
                    Number(
                        boton.dataset.id
                        ??
                        0
                    );


                const folio =
                    boton.dataset.folio
                    ??
                    "la solicitud";


                if (
                    solicitudId <=
                    0
                ) {
                    return;
                }


                const confirmado =
                    await confirmarAccionAdq(
                        {
                            titulo:
                                "Modificar cotizaciones",

                            mensaje:
                                `
                        <p class="mb-3">
                            Vas a reabrir la etapa de cotización de
                            <strong>${escapeHtmlAdq(
                                    folio
                                )}</strong>.
                        </p>

                        <div class="alert alert-warning mb-0">

                            <div class="d-flex gap-2">

                                <i class="bi bi-exclamation-triangle-fill"></i>

                                <div>
                                    La cotización seleccionada dejará de estar finalizada y podrás agregar nuevas propuestas o cambiar la selección antes de solicitar presupuesto.
                                </div>

                            </div>

                        </div>
                        `,

                            textoConfirmar:
                                "Modificar cotizaciones",

                            textoCancelar:
                                "Cancelar",

                            tipo:
                                "warning",

                            icono:
                                "bi-pencil-square"
                        }
                    );


                if (!confirmado) {
                    return;
                }


                try {

                    boton.disabled =
                        true;


                    boton.innerHTML = `
                <span class="spinner-border spinner-border-sm me-1"></span>
                Abriendo...
            `;


                    await reabrirCotizacionAdq(
                        solicitudId
                    );


                    window.location.reload();

                }
                catch (
                error
                ) {

                    console.error(
                        "Error al reabrir cotización:",
                        error
                    );


                    mostrarAdvertenciaAdq(
                        "No fue posible modificar las cotizaciones",
                        error.message
                        ??
                        "Ocurrió un error."
                    );


                    boton.disabled =
                        false;


                    boton.innerHTML = `
                <i class="bi bi-pencil-square"></i>
                <span>
                    Modificar cotizaciones
                </span>
            `;
                }
            }
        );

        // =========================================================
        // PREPARAR NUEVA COTIZACIÓN / PROVEEDOR ALTERNATIVO
        // =========================================================

        function prepararProveedorAlternativoAdq() {

            if (
                !solicitudCotizacionActualAdq
            ) {
                return;
            }

            // =====================================================
            // SALIR COMPLETAMENTE DEL MODO EDICIÓN
            // =====================================================

            cotizacionEditandoActualAdq =
                null;


            archivosCotizacionEliminadosAdq =
                new Set();


            sincronizarArchivosCotizacionEliminarAdq();


            if (
                cotizacionEditarIdAdq
            ) {

                cotizacionEditarIdAdq.value =
                    "0";
            }


            if (
                formCotizacionAdq
            ) {

                formCotizacionAdq.action =
                    `${window.location.pathname}?handler=GuardarCotizacion`;
            }


            seccionCapturaProveedorAdq
                ?.classList
                .remove(
                    "adq-quote-editing"
                );


            badgeEdicionCotizacionAdq
                ?.classList
                .add(
                    "d-none"
                );


            btnCancelarEdicionCotizacionAdq
                ?.classList
                .add(
                    "d-none"
                );


            if (
                tituloCapturaCotizacionAdq
            ) {

                tituloCapturaCotizacionAdq.textContent =
                    "Datos del proveedor";
            }


            if (
                subtituloCapturaCotizacionAdq
            ) {

                subtituloCapturaCotizacionAdq.textContent =
                    "Información general del proveedor que emite la cotización.";
            }


            if (
                textoGuardarCotizacionAdq
            ) {

                textoGuardarCotizacionAdq.textContent =
                    "Guardar cotización";
            }


            const solicitudId =
                Number(
                    solicitudCotizacionActualAdq.id
                    ??
                    0
                );


            /*
             * Limpiar solamente datos de captura.
             */
            const camposLimpiar =
                [
                    "cotizacionNombreProveedorAdq",
                    "cotizacionRfcProveedorAdq",
                    "cotizacionTelefonoProveedorAdq",
                    "cotizacionContactoProveedorAdq",
                    "cotizacionEmailProveedorAdq",
                    "cotizacionObservacionesAdq"
                ];


            camposLimpiar.forEach(
                function (
                    id
                ) {

                    const elemento =
                        document.getElementById(
                            id
                        );


                    if (
                        elemento
                    ) {

                        elemento.value =
                            "";
                    }
                }
            );


            if (
                cotizacionSolicitudIdAdq
            ) {

                cotizacionSolicitudIdAdq.value =
                    String(
                        solicitudId
                    );
            }


            if (
                cotizacionAplicaIvaAdq
            ) {

                cotizacionAplicaIvaAdq.checked =
                    true;
            }


            if (
                cotizacionPorcentajeIvaAdq
            ) {

                cotizacionPorcentajeIvaAdq.value =
                    "16";
            }


            /*
             * Volvemos a generar los productos
             * para limpiar precios y archivos.
             */
            renderizarDetallesCotizacionAdq(
                solicitudCotizacionActualAdq.detalles
            );


            archivosCotizacionSeleccionadosAdq =
                [];


            sincronizarArchivosCotizacionAdq();

            renderizarArchivosCotizacionSeleccionadosAdq();


            calcularCotizacionAdq();


            /*
             * Llevar al usuario directamente
             * a los datos del nuevo proveedor.
             */
            setTimeout(
                function () {

                    seccionCapturaProveedorAdq
                        ?.scrollIntoView(
                            {
                                behavior:
                                    "smooth",

                                block:
                                    "start"
                            }
                        );

                },
                100
            );


            document
                .getElementById(
                    "cotizacionNombreProveedorAdq"
                )
                ?.focus();
        }

        btnAgregarProveedorAlternativoAdq
            ?.addEventListener(
                "click",
                prepararProveedorAlternativoAdq
            );

        // =========================================================
        // FORMATEAR MONEDA
        // =========================================================

        function formatearMonedaCotizacionAdq(
            valor
        ) {

            const numero =
                Number(
                    valor ?? 0
                );


            return numero.toLocaleString(
                "es-MX",
                {
                    style:
                        "currency",

                    currency:
                        "MXN"
                }
            );
        }


        // =========================================================
        // CALCULAR COTIZACIÓN
        // =========================================================

        function calcularCotizacionAdq() {

            let subtotal =
                0;


            document
                .querySelectorAll(
                    ".cotizacion-precio-unitario-adq"
                )
                .forEach(
                    function (input) {

                        const cantidad =
                            Number(
                                input.dataset.cantidad
                                ??
                                0
                            );


                        const precio =
                            Number(
                                input.value
                                ??
                                0
                            );


                        const importe =
                            cantidad *
                            precio;


                        subtotal +=
                            importe;


                        const fila =
                            input.closest(
                                "tr"
                            );


                        const importeElement =
                            fila?.querySelector(
                                ".cotizacion-importe-adq"
                            );


                        if (
                            importeElement
                        ) {

                            importeElement.textContent =
                                formatearMonedaCotizacionAdq(
                                    importe
                                );
                        }
                    }
                );


            const aplicaIva =
                cotizacionAplicaIvaAdq?.checked
                ??
                false;


            const porcentajeIva =
                aplicaIva
                    ? Number(
                        cotizacionPorcentajeIvaAdq?.value
                        ??
                        0
                    )
                    : 0;


            const importeIva =
                subtotal *
                (
                    porcentajeIva /
                    100
                );


            const total =
                subtotal +
                importeIva;


            if (
                cotizacionSubtotalAdq
            ) {

                cotizacionSubtotalAdq.textContent =
                    formatearMonedaCotizacionAdq(
                        subtotal
                    );
            }


            if (
                cotizacionIvaAdq
            ) {

                cotizacionIvaAdq.textContent =
                    formatearMonedaCotizacionAdq(
                        importeIva
                    );
            }


            if (
                cotizacionTotalAdq
            ) {

                cotizacionTotalAdq.textContent =
                    formatearMonedaCotizacionAdq(
                        total
                    );
            }


            if (
                cotizacionIvaLabelAdq
            ) {

                cotizacionIvaLabelAdq.textContent =
                    aplicaIva
                        ? `IVA ${porcentajeIva}%`
                        : "IVA";
            }


            if (
                cotizacionPorcentajeIvaAdq
            ) {

                cotizacionPorcentajeIvaAdq.disabled =
                    !aplicaIva;
            }
        }

        // =========================================================
        // RENDERIZAR PRODUCTOS DE COTIZACIÓN
        // =========================================================

        function renderizarDetallesCotizacionAdq(
            detalles
        ) {

            if (
                !contenedorDetallesCotizacionAdq
            ) {
                return;
            }


            contenedorDetallesCotizacionAdq.innerHTML =
                "";


            if (
                !Array.isArray(
                    detalles
                )
                ||
                detalles.length ===
                0
            ) {

                contenedorDetallesCotizacionAdq.innerHTML = `
            <tr>

                <td colspan="6"
                    class="text-center text-muted py-4">

                    La solicitud no contiene productos.

                </td>

            </tr>
        `;

                return;
            }


            detalles.forEach(
                function (
                    detalle,
                    index
                ) {

                    const fila =
                        document.createElement(
                            "tr"
                        );


                    const cantidad =
                        Number(
                            detalle.cantidad
                            ??
                            0
                        );


                    fila.innerHTML = `
                        <td>

                            <strong>
                                ${escapeHtmlAdq(
                        detalle.productoServicio
                        ??
                        ""
                    )}
                            </strong>

                            ${detalle.descripcion
                            ? `
                                        <small class="d-block text-muted mt-1">
                                            ${escapeHtmlAdq(
                                detalle.descripcion
                            )}
                                        </small>
                                    `
                            : ""
                        }

                            <input type="hidden"
                                   name="InputCotizacion.Detalles[${index}].SolicitudDetalleId"
                                   value="${Number(
                            detalle.id
                            ??
                            0
                        )}" />

                        </td>


                        <td>
                            ${cantidad}
                        </td>


                        <td>
                            ${escapeHtmlAdq(
                            detalle.unidad
                            ??
                            ""
                        )}
                        </td>


                        <td>

                            <div class="input-group input-group-sm">

                                <span class="input-group-text">
                                    $
                                </span>

                                <input type="number"
                                       class="form-control cotizacion-precio-unitario-adq"
                                       name="InputCotizacion.Detalles[${index}].PrecioUnitario"
                                       min="0.01"
                                       step="0.01"
                                       data-cantidad="${cantidad}"
                                       required />

                            </div>

                        </td>


                        <td class="text-end">

                            <strong class="cotizacion-importe-adq">
                                $0.00
                            </strong>

                        </td>


                        <td>

                            <input type="file"
                                   class="form-control form-control-sm cotizacion-evidencia-adq"
                                   name="InputCotizacion.Detalles[${index}].ArchivoEvidencia"
                                   accept=".pdf,.doc,.docx,.xls,.xlsx,.png,.jpg,.jpeg"
                                   required />

                            <small class="text-muted d-block mt-1">
                                Máximo 15 MB
                            </small>

                        </td>
                    `;


                    contenedorDetallesCotizacionAdq
                        .appendChild(
                            fila
                        );
                }
            );


            calcularCotizacionAdq();
        }

        // =========================================================
        // ABRIR COTIZACIÓN
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnGenerarCotizacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const solicitudId =
                    Number(
                        boton.dataset.id
                        ??
                        0
                    );


                if (
                    solicitudId <=
                    0
                ) {
                    return;
                }


                try {

                    boton.disabled =
                        true;


                    const [
                        solicitud,
                        cotizaciones
                    ] =
                        await Promise.all(
                            [
                                obtenerSolicitudAdq(
                                    solicitudId
                                ),

                                obtenerCotizacionesSolicitudAdq(
                                    solicitudId
                                )
                            ]
                        );


                    if (!solicitud) {

                        throw new Error(
                            "No fue posible obtener la solicitud."
                        );
                    }


                    solicitudCotizacionActualAdq =
                        solicitud;


                    if (
                        formCotizacionAdq
                    ) {

                        formCotizacionAdq.reset();
                    }

                    archivosCotizacionSeleccionadosAdq =
                        [];

                    cotizacionEditandoActualAdq =
                        null;


                    archivosCotizacionEliminadosAdq =
                        new Set();


                    sincronizarArchivosCotizacionEliminarAdq();


                    if (
                        cotizacionEditarIdAdq
                    ) {

                        cotizacionEditarIdAdq.value =
                            "0";
                    }


                    sincronizarArchivosCotizacionAdq();

                    renderizarArchivosCotizacionSeleccionadosAdq();


                    if (
                        cotizacionSolicitudIdAdq
                    ) {

                        cotizacionSolicitudIdAdq.value =
                            String(
                                solicitud.id
                            );
                    }


                    if (
                        folioCotizacionAdq
                    ) {

                        folioCotizacionAdq.textContent =
                            `${solicitud.folio ?? ""} · ${solicitud.titulo ?? ""}`;
                    }


                    if (
                        cotizacionAplicaIvaAdq
                    ) {

                        cotizacionAplicaIvaAdq.checked =
                            true;
                    }


                    if (
                        cotizacionPorcentajeIvaAdq
                    ) {

                        cotizacionPorcentajeIvaAdq.value =
                            "16";
                    }


                    renderizarDetallesCotizacionAdq(
                        solicitud.detalles
                    );


                    renderizarCotizacionesRegistradasAdq(
                        cotizaciones
                    );


                    calcularCotizacionAdq();


                    bootstrap.Modal
                        .getOrCreateInstance(
                            modalCotizacionElement
                        )
                        .show();

                }
                catch (
                error
                ) {

                    console.error(
                        "Error al preparar cotización:",
                        error
                    );

                    mostrarAdvertenciaAdq(
                        "No fue posible preparar la cotización.",
                        error.message
                        ??
                        "Ocurrió un error."
                    );

                }
                finally {

                    boton.disabled =
                        false;
                }
            }
        );

        // =========================================================
        // EVENTOS DE CÁLCULO
        // =========================================================

        document.addEventListener(
            "input",
            function (
                event
            ) {

                if (
                    event.target.matches(
                        ".cotizacion-precio-unitario-adq"
                    )
                ) {

                    calcularCotizacionAdq();
                }
            }
        );


        cotizacionPorcentajeIvaAdq
            ?.addEventListener(
                "input",
                calcularCotizacionAdq
            );


        cotizacionAplicaIvaAdq
            ?.addEventListener(
                "change",
                calcularCotizacionAdq
            );

        // =========================================================
        // MIS ÓRDENES ASIGNADAS - ABRIR CHAT
        // =========================================================

        document.addEventListener(
            "click",
            function (event) {

                const botonChat =
                    event.target.closest(
                        ".btnChatOrdenAdq"
                    );

                if (!botonChat) {
                    return;
                }

                const solicitudId =
                    Number(
                        botonChat.dataset.id
                        ??
                        0
                    );

                if (solicitudId <= 0) {
                    return;
                }

                const botonVer =
                    document.querySelector(
                        `.btnVerSolicitudAdq[data-id="${solicitudId}"]`
                    );

                if (!botonVer) {
                    return;
                }

                botonVer.click();


                setTimeout(
                    function () {

                        const tabChat =
                            document.querySelector(
                                "#tabSeguimientoAdq"
                            );

                        if (tabChat) {
                            tabChat.click();
                        }

                    },
                    350
                );
            }
        );

        // =========================================================
        // OBTENER CHAT / HISTORIAL
        // =========================================================

        async function obtenerSeguimientoAdq(
            solicitudId
        ) {

            const response =
                await fetch(
                    `?handler=SeguimientoSolicitud&id=${encodeURIComponent(
                        solicitudId
                    )}`,
                    {
                        method:
                            "GET",

                        headers: {
                            "X-Requested-With":
                                "XMLHttpRequest"
                        }
                    }
                );


            const resultado =
                await response.json();


            if (
                !response.ok
                ||
                !resultado.success
            ) {

                throw new Error(
                    resultado.message
                    ??
                    "No fue posible consultar el seguimiento."
                );
            }


            return resultado.seguimiento;
        }

        // =========================================================
        // RENDERIZAR CHAT
        // =========================================================

        function renderizarComentariosAdq(
            comentarios
        ) {

            if (
                !listaComentariosAdq
            ) {
                return;
            }


            listaComentariosAdq.innerHTML =
                "";


            if (
                !Array.isArray(
                    comentarios
                )
                ||
                comentarios.length ===
                0
            ) {

                listaComentariosAdq.innerHTML = `
            <div class="adq-chat-empty">

                <i class="bi bi-chat-square-dots"></i>

                <strong>
                    Aún no hay mensajes
                </strong>

                <span>
                    Inicia la conversación relacionada
                    con esta solicitud.
                </span>

            </div>
        `;

                return;
            }


            comentarios.forEach(
                function (
                    comentario
                ) {

                    const elemento =
                        document.createElement(
                            "div"
                        );


                    elemento.className =
                        comentario.esUsuarioActual
                            ? "adq-chat-message adq-chat-message-own"
                            : "adq-chat-message";


                    const fecha =
                        new Date(
                            comentario.fechaCreacion
                        );


                    const adjuntos =
                        Array.isArray(
                            comentario.adjuntos
                        )
                            ? comentario.adjuntos
                            : [];


                    let htmlComentario =
                        "";


                    /*
                     * Un mensaje puede contener solamente
                     * archivos, por eso el texto es opcional.
                     */
                    if (
                        comentario.comentario
                        &&
                        comentario.comentario.trim()
                    ) {
                        htmlComentario = `
                    <div class="adq-chat-bubble">
                        ${escapeHtmlAdq(
                            comentario.comentario
                        )}
                    </div>
                `;
                    }


                    let htmlAdjuntos =
                        "";


                    if (
                        adjuntos.length >
                        0
                    ) {

                        htmlAdjuntos = `
                    <div class="adq-chat-attachments">

                        ${adjuntos
                                .map(
                                    function (
                                        archivo
                                    ) {

                                        return `
                                            <a href="${escapeAttributeAdq(
                                            archivo.rutaArchivo
                                        )}"
                                               target="_blank"
                                               rel="noopener noreferrer"
                                               class="adq-chat-attachment"
                                               title="Abrir ${escapeAttributeAdq(
                                            archivo.nombreOriginal
                                        )}">

                                                <div class="adq-chat-file-icon">

                                                    <i class="bi bi-file-earmark"></i>

                                                </div>


                                                <div class="adq-chat-file-name">

                                                    ${escapeHtmlAdq(
                                            archivo.nombreOriginal
                                        )}

                                                </div>


                                                <div class="adq-chat-file-size">

                                                    ${formatearTamanoAdq(
                                            Number(
                                                archivo.tamanoBytes
                                                ??
                                                0
                                            )
                                        )}

                                                </div>


                                                <div class="adq-chat-file-action">

                                                    <i class="bi bi-box-arrow-up-right"></i>

                                                </div>

                                            </a>
                                        `;
                                    }
                                )
                                .join(
                                    ""
                                )
                            }

                    </div>
                `;
                    }


                    elemento.innerHTML = `
                <div class="adq-chat-message-header">

                    <strong>

                        ${comentario.esUsuarioActual
                            ? "Tú"
                            : escapeHtmlAdq(
                                comentario.usuario
                            )
                        }

                    </strong>

                    <span>

                        ${fecha.toLocaleString(
                            "es-MX"
                        )}

                    </span>

                </div>


                ${htmlComentario}

                ${htmlAdjuntos}
            `;


                    listaComentariosAdq
                        .appendChild(
                            elemento
                        );
                }
            );


            listaComentariosAdq.scrollTop =
                listaComentariosAdq.scrollHeight;
        }

        // =========================================================
        // RENDERIZAR HISTORIAL
        // =========================================================

        function renderizarHistorialAdq(
            historial
        ) {

            if (
                !listaHistorialAdq
            ) {
                return;
            }


            listaHistorialAdq.innerHTML =
                "";


            if (
                !Array.isArray(
                    historial
                )
                ||
                historial.length ===
                0
            ) {

                listaHistorialAdq.innerHTML = `
                    <div class="text-muted small">
                        No existen movimientos registrados.
                    </div>
                `;

                return;
            }


            historial.forEach(
                function (
                    evento
                ) {

                    const fecha =
                        new Date(
                            evento.fechaEvento
                        );


                    const item =
                        document.createElement(
                            "div"
                        );


                    item.className =
                        "adq-history-item";


                    item.innerHTML = `
                        <div class="adq-history-marker">

                            <i class="bi bi-circle-fill"></i>

                        </div>


                        <div class="adq-history-content">

                            <strong>
                                ${escapeHtmlAdq(
                        evento.descripcion
                    )}
                            </strong>

                            <span>
                                ${escapeHtmlAdq(
                        evento.usuario
                        ||
                        "Sistema"
                    )
                        }
                                ·
                                ${fecha.toLocaleString(
                            "es-MX"
                        )}
                            </span>

                        </div>
                    `;


                    listaHistorialAdq
                        .appendChild(
                            item
                        );
                }
            );
        }

        // =========================================================
        // CARGAR SEGUIMIENTO
        // =========================================================

        async function cargarSeguimientoAdq(
            solicitudId
        ) {

            const seguimiento =
                await obtenerSeguimientoAdq(
                    solicitudId
                );

            if (
                badgeMensajesPendientesAdq
            ) {

                const pendientes =
                    Number(
                        seguimiento.mensajesPendientes ??
                        0
                    );


                if (
                    pendientes > 0
                ) {

                    badgeMensajesPendientesAdq
                        .classList
                        .remove(
                            "d-none"
                        );


                    badgeMensajesPendientesAdq.textContent =
                        String(
                            pendientes
                        );

                }
                else {

                    badgeMensajesPendientesAdq
                        .classList
                        .add(
                            "d-none"
                        );


                    badgeMensajesPendientesAdq.textContent =
                        "";
                }
            }


            if (
                inputSolicitudComentarioAdq
            ) {

                inputSolicitudComentarioAdq.value =
                    String(
                        solicitudId
                    );
            }


            if (
                seguimientoEstatusAdq
            ) {

                seguimientoEstatusAdq.textContent =
                    seguimiento.estatus
                    ??
                    "";
            }


            /*
             * La escritura del chat la determina el backend.
             * Puede incluir solicitante, gerente/aprobador,
             * Adquisiciones y agente asignado.
             */
            if (
                formComentarioAdq
            ) {

                formComentarioAdq.classList.toggle(
                    "d-none",
                    seguimiento.puedeEscribir !==
                    true
                );
            }


            renderizarComentariosAdq(
                seguimiento.comentarios
            );


            renderizarHistorialAdq(
                seguimiento.historial
            );
        }


        document.addEventListener(
            "click",
            function (event) {

                const boton =
                    event.target.closest(
                        ".btnDecisionAprobacionAdq"
                    );


                if (!boton) {
                    return;
                }


                const id =
                    boton.dataset.id;

                const folio =
                    boton.dataset.folio ??
                    "";

                const accion =
                    boton.dataset.accion;


                inputSolicitudDecision.value =
                    id;

                inputComentarioDecision.value =
                    "";


                folioDecisionGerente.textContent =
                    folio;


                if (
                    accion ===
                    "aprobar"
                ) {
                    tituloDecisionGerente.textContent =
                        "Aprobar solicitud";


                    mensajeDecisionGerente.className =
                        "alert alert-success mb-3";


                    mensajeDecisionGerente.innerHTML = `
                        <i class="bi bi-check-circle me-1"></i>

                        La solicitud será aprobada y enviada
                        al área de Adquisiciones.
                    `;


                    labelComentarioDecision.textContent =
                        "Comentario (opcional)";


                    ayudaComentarioDecision.textContent =
                        "Puedes agregar una observación para el solicitante.";


                    inputComentarioDecision.required =
                        false;


                    btnConfirmarAprobacion
                        .classList.remove(
                            "d-none"
                        );


                    btnConfirmarRechazo
                        .classList.add(
                            "d-none"
                        );
                }
                else {
                    tituloDecisionGerente.textContent =
                        "Rechazar solicitud";


                    mensajeDecisionGerente.className =
                        "alert alert-danger mb-3";


                    mensajeDecisionGerente.innerHTML = `
                        <i class="bi bi-exclamation-triangle me-1"></i>

                        La solicitud quedará marcada
                        como rechazada.
                    `;


                    labelComentarioDecision.textContent =
                        "Motivo del rechazo";


                    ayudaComentarioDecision.textContent =
                        "El motivo es obligatorio y quedará registrado en el historial.";


                    inputComentarioDecision.required =
                        true;


                    btnConfirmarRechazo
                        .classList.remove(
                            "d-none"
                        );


                    btnConfirmarAprobacion
                        .classList.add(
                            "d-none"
                        );
                }


                bootstrap.Modal
                    .getOrCreateInstance(
                        modalDecisionGerenteElement
                    )
                    .show();

            }
        );

        // =========================================================
        // REFERENCIAS GENERALES
        // =========================================================

        const formulario =
            document.getElementById(
                "formNuevaSolicitudAdq"
            );

        const modalNuevaSolicitudElement =
            document.getElementById(
                "modalNuevaSolicitud"
            );

        const modalVerSolicitudElement =
            document.getElementById(
                "modalVerSolicitudAdq"
            );

        const btnNuevaSolicitud =
            document.getElementById(
                "btnNuevaSolicitudAdq"
            );

        const btnEnviar =
            document.getElementById(
                "btnEnviarSolicitudAdq"
            );

        const btnGuardarBorrador =
            document.getElementById(
                "btnGuardarBorradorAdq"
            );

        const btnGuardarCambios =
            document.getElementById(
                "btnGuardarCambiosAdq"
            );

        const btnEnviarBorrador =
            document.getElementById(
                "btnEnviarBorradorAdq"
            );

        const solicitudEditarId =
            document.getElementById(
                "SolicitudEditarId"
            );

        const tituloModal =
            document.getElementById(
                "modalNuevaSolicitudLabel"
            );


        // =========================================================
        // PRODUCTOS
        // =========================================================

        const contenedorDetalles =
            document.getElementById(
                "contenedorDetallesAdq"
            );

        const btnAgregarProducto =
            document.getElementById(
                "btnAgregarDetalleAdq"
            );

        const inputProducto =
            document.getElementById(
                "adqProductoServicio"
            );

        const inputCantidad =
            document.getElementById(
                "adqCantidad"
            );

        const inputUnidad =
            document.getElementById(
                "adqUnidad"
            );

        const inputDescripcionProducto =
            document.getElementById(
                "adqDescripcionProducto"
            );

        const contadorProductos =
            document.getElementById(
                "adqContadorProductos"
            );


        // =========================================================
        // ARCHIVOS
        // =========================================================

        const inputArchivos =
            document.getElementById(
                "ArchivosSolicitud"
            );

        const btnAgregarArchivos =
            document.getElementById(
                "btnAgregarArchivosAdq"
            );

        const listaArchivos =
            document.getElementById(
                "listaArchivosAdq"
            );

        const contadorArchivos =
            document.getElementById(
                "contadorArchivosAdq"
            );


        /*
         * Aquí conservaremos todos los archivos
         * aunque el usuario abra varias veces
         * el selector.
         */
        let archivosSeleccionadosAdq =
            [];

        /*
         * Archivos que ya existen en la BD
         * cuando estamos editando una solicitud.
         */
        let archivosExistentesAdq =
            [];


        /*
         * IDs de archivos existentes que el
         * usuario decidió eliminar.
         */
        let adjuntosEliminarIdsAdq =
            [];


        const extensionesPermitidasAdq =
            [
                ".pdf",
                ".doc",
                ".docx",
                ".xls",
                ".xlsx",
                ".png",
                ".jpg",
                ".jpeg"
            ];

        const tamanoMaximoArchivoAdq =
            15 * 1024 * 1024;


        // =========================================================
        // FILTROS
        // =========================================================

        const filtroBusqueda =
            document.getElementById(
                "adqFiltroBusqueda"
            );

        const filtroEstatus =
            document.getElementById(
                "adqFiltroEstatus"
            );

        const btnLimpiarFiltros =
            document.getElementById(
                "btnLimpiarFiltrosAdq"
            );

        // =========================================================
        // PAGINACIÓN - REFERENCIAS
        // =========================================================

        const paginacionLista =
            document.getElementById(
                "adqPaginacionLista"
            );


        const paginacionContenedor =
            document.getElementById(
                "paginacionSolicitudesAdq"
            );


        const paginaInicio =
            document.getElementById(
                "adqPaginaInicio"
            );


        const paginaFin =
            document.getElementById(
                "adqPaginaFin"
            );


        const paginaTotal =
            document.getElementById(
                "adqPaginaTotal"
            );


        const registrosPorPaginaAdq =
            10;


        let paginaActualAdq =
            1;


        // =========================================================
        // HISTORIAL DE APROBACIONES - FILTROS + PAGINACIÓN
        // =========================================================

        const filtroHistorialAprobacionesAdq =
            document.getElementById(
                "adqFiltroHistorialAprobaciones"
            );

        const filtroDecisionHistorialAdq =
            document.getElementById(
                "adqFiltroDecisionHistorial"
            );

        const btnLimpiarHistorialAprobacionesAdq =
            document.getElementById(
                "btnLimpiarHistorialAprobaciones"
            );

        const paginacionHistorialAdq =
            document.getElementById(
                "paginacionHistorialAprobacionesAdq"
            );

        const listaPaginacionHistorialAdq =
            document.getElementById(
                "adqHistorialPaginacionLista"
            );

        const historialPaginaInicioAdq =
            document.getElementById(
                "adqHistorialPaginaInicio"
            );

        const historialPaginaFinAdq =
            document.getElementById(
                "adqHistorialPaginaFin"
            );

        const historialPaginaTotalAdq =
            document.getElementById(
                "adqHistorialPaginaTotal"
            );

        const registrosPorPaginaHistorialAdq =
            10;

        let paginaActualHistorialAdq =
            1;

        // =========================================================
        // UTILIDADES
        // =========================================================

        function escapeHtmlAdq(
            texto
        ) {

            const elemento =
                document.createElement(
                    "div"
                );

            elemento.textContent =
                texto ?? "";

            return elemento.innerHTML;
        }


        function escapeAttributeAdq(
            texto
        ) {

            return String(
                texto ?? ""
            )
                .replaceAll(
                    "&",
                    "&amp;"
                )
                .replaceAll(
                    "\"",
                    "&quot;"
                )
                .replaceAll(
                    "<",
                    "&lt;"
                )
                .replaceAll(
                    ">",
                    "&gt;"
                );
        }


        function obtenerExtensionAdq(
            nombreArchivo
        ) {

            const indice =
                nombreArchivo.lastIndexOf(
                    "."
                );

            if (indice < 0) {
                return "";
            }

            return nombreArchivo
                .substring(
                    indice
                )
                .toLowerCase();
        }


        function formatearTamanoAdq(
            bytes
        ) {

            if (bytes < 1024) {

                return `${bytes} B`;

            }

            if (
                bytes <
                1024 * 1024
            ) {

                return (
                    bytes /
                    1024
                )
                    .toFixed(2) +
                    " KB";
            }

            return (
                bytes /
                1024 /
                1024
            )
                .toFixed(2) +
                " MB";
        }


        function mostrarAdvertenciaAdq(
            titulo,
            mensaje
        ) {

            const overlayAnterior =
                document.getElementById(
                    "adqMensajeOverlay"
                );


            overlayAnterior
                ?.remove();


            const overlay =
                document.createElement(
                    "div"
                );


            overlay.id =
                "adqMensajeOverlay";


            overlay.className =
                "adq-confirm-overlay";


            overlay.innerHTML = `
        <div class="adq-confirm-dialog"
             role="alertdialog"
             aria-modal="true">

            <div class="adq-confirm-body">

                <div class="adq-confirm-icon adq-confirm-icon-warning">

                    <i class="bi bi-exclamation-triangle"></i>

                </div>


                <div class="adq-confirm-content">

                    <h5 class="adq-confirm-title">

                        ${escapeHtmlAdq(
                titulo
                ??
                "Advertencia"
            )}

                    </h5>

                    <div class="adq-confirm-message">

                        ${escapeHtmlAdq(
                mensaje
                ??
                "Ocurrió un error."
            )}

                    </div>

                </div>

            </div>


            <div class="adq-confirm-footer">

                <button type="button"
                        class="btn btn-primary"
                        data-adq-mensaje-aceptar>

                    <i class="bi bi-check2 me-1"></i>

                    Entendido

                </button>

            </div>

        </div>
    `;


            document.body.appendChild(
                overlay
            );


            const boton =
                overlay.querySelector(
                    "[data-adq-mensaje-aceptar]"
                );


            function cerrar() {

                overlay.classList.remove(
                    "adq-confirm-overlay-visible"
                );


                setTimeout(
                    function () {

                        overlay.remove();

                    },
                    150
                );
            }


            boton
                ?.addEventListener(
                    "click",
                    cerrar
                );


            requestAnimationFrame(
                function () {

                    overlay.classList.add(
                        "adq-confirm-overlay-visible"
                    );


                    boton
                        ?.focus();
                }
            );
        }

        // =========================================================
        // CHAT - RENDERIZAR ARCHIVOS SELECCIONADOS
        // =========================================================

        function renderizarAdjuntosComentarioSeleccionadosAdq() {

            if (
                !listaAdjuntosComentarioAdq
            ) {
                return;
            }


            listaAdjuntosComentarioAdq.innerHTML =
                "";


            if (
                archivosComentarioSeleccionadosAdq.length ===
                0
            ) {
                return;
            }


            archivosComentarioSeleccionadosAdq
                .forEach(
                    function (
                        archivo,
                        indice
                    ) {

                        const item =
                            document.createElement(
                                "div"
                            );


                        item.className =
                            "adq-chat-selected-file";


                        item.innerHTML = `
                            <div class="adq-chat-file-icon">

                                <i class="bi bi-file-earmark"></i>

                            </div>


                            <div class="adq-chat-file-name"
                                 title="${escapeAttributeAdq(
                            archivo.name
                        )}">

                                ${escapeHtmlAdq(
                            archivo.name
                        )}

                            </div>


                            <div class="adq-chat-file-size">

                                ${formatearTamanoAdq(
                            archivo.size
                        )}

                            </div>


                            <button type="button"
                                    class="adq-chat-file-action adq-chat-file-remove btnEliminarAdjuntoComentarioAdq"
                                    data-index="${indice}"
                                    title="Quitar archivo"
                                    aria-label="Quitar archivo">

                                <i class="bi bi-x-lg"></i>

                            </button>
                        `;


                        listaAdjuntosComentarioAdq
                            .appendChild(
                                item
                            );
                    }
                );
        }


        // =========================================================
        // CHAT - AGREGAR ARCHIVOS
        // =========================================================

        function agregarAdjuntosComentarioAdq(
            archivosNuevos
        ) {

            const clavesExistentes =
                new Set(
                    archivosComentarioSeleccionadosAdq
                        .map(
                            obtenerClaveArchivoAdq
                        )
                );


            archivosNuevos.forEach(
                function (
                    archivo
                ) {

                    const clave =
                        obtenerClaveArchivoAdq(
                            archivo
                        );


                    /*
                     * Evitamos agregar el mismo archivo
                     * varias veces.
                     */
                    if (
                        clavesExistentes.has(
                            clave
                        )
                    ) {
                        return;
                    }


                    const validacion =
                        archivoValidoAdq(
                            archivo
                        );


                    if (
                        !validacion.formatoValido
                    ) {
                        mostrarAdvertenciaAdq(
                            "Formato no permitido",
                            `El archivo ${archivo.name} no tiene un formato permitido.`
                        );

                        return;
                    }


                    if (
                        !validacion.tamanoValido
                    ) {
                        mostrarAdvertenciaAdq(
                            "Archivo demasiado grande",
                            `El archivo ${archivo.name} supera el límite de 15 MB.`
                        );

                        return;
                    }


                    archivosComentarioSeleccionadosAdq
                        .push(
                            archivo
                        );


                    clavesExistentes.add(
                        clave
                    );
                }
            );


            renderizarAdjuntosComentarioSeleccionadosAdq();
        }


        // =========================================================
        // CHAT - ELIMINAR ARCHIVO SELECCIONADO
        // =========================================================

        function eliminarAdjuntoComentarioAdq(
            indice
        ) {

            if (
                indice < 0
                ||
                indice >=
                archivosComentarioSeleccionadosAdq.length
            ) {
                return;
            }


            archivosComentarioSeleccionadosAdq
                .splice(
                    indice,
                    1
                );


            renderizarAdjuntosComentarioSeleccionadosAdq();
        }


        // =========================================================
        // CHAT - LIMPIAR ARCHIVOS
        // =========================================================

        function limpiarAdjuntosComentarioAdq() {

            archivosComentarioSeleccionadosAdq =
                [];


            if (
                inputArchivoComentarioAdq
            ) {
                inputArchivoComentarioAdq.value =
                    "";
            }


            renderizarAdjuntosComentarioSeleccionadosAdq();
        }

        // =========================================================
        // CHAT - EVENTOS DE ARCHIVOS
        // =========================================================

        btnSeleccionarAdjuntoComentarioAdq
            ?.addEventListener(
                "click",
                function () {

                    inputArchivoComentarioAdq
                        ?.click();
                }
            );


        inputArchivoComentarioAdq
            ?.addEventListener(
                "change",
                function () {

                    const archivos =
                        Array.from(
                            inputArchivoComentarioAdq.files
                            ??
                            []
                        );


                    agregarAdjuntosComentarioAdq(
                        archivos
                    );


                    /*
                     * Limpiamos el input real para permitir
                     * volver a seleccionar archivos.
                     *
                     * Los File quedan almacenados en nuestro
                     * arreglo independiente.
                     */
                    inputArchivoComentarioAdq.value =
                        "";
                }
            );


        listaAdjuntosComentarioAdq
            ?.addEventListener(
                "click",
                function (
                    event
                ) {

                    const boton =
                        event.target.closest(
                            ".btnEliminarAdjuntoComentarioAdq"
                        );


                    if (
                        !boton
                    ) {
                        return;
                    }


                    const indice =
                        Number(
                            boton.dataset.index
                        );


                    if (
                        Number.isNaN(
                            indice
                        )
                    ) {
                        return;
                    }


                    eliminarAdjuntoComentarioAdq(
                        indice
                    );
                }
            );


        // =========================================================
        // PRODUCTOS - CREAR ELEMENTO
        // =========================================================

        function crearElementoProductoAdq(
            producto,
            cantidad,
            unidad,
            descripcion
        ) {

            const fila =
                document.createElement(
                    "div"
                );

            fila.className =
                "adq-product-item";

            fila.innerHTML = `
                <div class="adq-product-main">

                    <div class="adq-product-icon">

                        <i class="bi bi-box-seam"></i>

                    </div>

                    <div class="adq-product-information">

                        <strong class="adq-product-name">
                            ${escapeHtmlAdq(producto)}
                        </strong>

                        <span class="adq-product-description">
                            ${descripcion
                    ? escapeHtmlAdq(
                        descripcion
                    )
                    : "Sin descripción adicional"
                }
                        </span>

                    </div>

                </div>

                <div class="adq-product-data">

                    <div>

                        <span>
                            Cantidad
                        </span>

                        <strong>
                            ${escapeHtmlAdq(cantidad)}
                        </strong>

                    </div>

                    <div>

                        <span>
                            Unidad
                        </span>

                        <strong>
                            ${escapeHtmlAdq(unidad)}
                        </strong>

                    </div>

                </div>

                <button type="button"
                        class="btn btn-outline-danger btnEliminarDetalleAdq"
                        title="Eliminar producto">

                    <i class="bi bi-trash"></i>

                </button>

                <input type="hidden"
                       data-field="ProductoServicio"
                       value="${escapeAttributeAdq(producto)}" />

                <input type="hidden"
                       data-field="Cantidad"
                       value="${escapeAttributeAdq(cantidad)}" />

                <input type="hidden"
                       data-field="Unidad"
                       value="${escapeAttributeAdq(unidad)}" />

                <input type="hidden"
                       data-field="Descripcion"
                       value="${escapeAttributeAdq(descripcion)}" />
            `;

            return fila;
        }


        // =========================================================
        // PRODUCTOS - AGREGAR
        // =========================================================

        function agregarProductoAdq() {

            if (
                !contenedorDetalles ||
                !inputProducto ||
                !inputCantidad ||
                !inputUnidad
            ) {
                return;
            }

            const producto =
                inputProducto.value
                    .trim();

            const cantidadTexto =
                inputCantidad.value
                    .trim();

            const cantidad =
                Number(
                    cantidadTexto
                );

            const unidad =
                inputUnidad.value
                    .trim();

            const descripcion =
                inputDescripcionProducto
                    ?.value
                    .trim() ??
                "";


            if (!producto) {

                mostrarAdvertenciaAdq(
                    "Producto incompleto",
                    "Captura el producto o servicio."
                );

                inputProducto.focus();

                return;
            }


            if (
                !cantidadTexto ||
                Number.isNaN(
                    cantidad
                ) ||
                cantidad <= 0
            ) {

                mostrarAdvertenciaAdq(
                    "Cantidad incorrecta",
                    "La cantidad debe ser mayor a cero."
                );

                inputCantidad.focus();

                return;
            }


            if (!unidad) {

                mostrarAdvertenciaAdq(
                    "Unidad requerida",
                    "Selecciona una unidad."
                );

                inputUnidad.focus();

                return;
            }


            document
                .getElementById(
                    "adqProductosVacio"
                )
                ?.remove();


            const elemento =
                crearElementoProductoAdq(
                    producto,
                    cantidadTexto,
                    unidad,
                    descripcion
                );


            contenedorDetalles
                .appendChild(
                    elemento
                );


            renumerarProductosAdq();

            actualizarContadorProductosAdq();

            limpiarCapturaProductoAdq();

            actualizarEstadoEnviar();
        }


        // =========================================================
        // PRODUCTOS - CARGAR EXISTENTE
        // =========================================================

        function agregarProductoExistenteAdq(
            detalle
        ) {

            if (!contenedorDetalles) {
                return;
            }


            document
                .getElementById(
                    "adqProductosVacio"
                )
                ?.remove();


            const elemento =
                crearElementoProductoAdq(
                    detalle.productoServicio,
                    detalle.cantidad,
                    detalle.unidad,
                    detalle.descripcion ?? ""
                );


            contenedorDetalles
                .appendChild(
                    elemento
                );


            renumerarProductosAdq();

            actualizarContadorProductosAdq();
        }


        // =========================================================
        // PRODUCTOS - ELIMINAR
        // =========================================================

        function eliminarProductoAdq(
            boton
        ) {

            const item =
                boton.closest(
                    ".adq-product-item"
                );

            if (!item) {
                return;
            }


            item.remove();


            renumerarProductosAdq();

            actualizarContadorProductosAdq();

            verificarListaProductosVaciaAdq();

            actualizarEstadoEnviar();
        }


        // =========================================================
        // PRODUCTOS - RENUMERAR
        // =========================================================

        function renumerarProductosAdq() {

            if (!contenedorDetalles) {
                return;
            }


            const productos =
                contenedorDetalles
                    .querySelectorAll(
                        ".adq-product-item"
                    );


            productos.forEach(
                function (
                    producto,
                    indice
                ) {

                    const campos =
                        producto
                            .querySelectorAll(
                                "[data-field]"
                            );


                    campos.forEach(
                        function (
                            campo
                        ) {

                            const nombre =
                                campo.dataset.field;


                            campo.name =
                                `Input.Detalles[${indice}].${nombre}`;
                        }
                    );

                }
            );
        }


        // =========================================================
        // PRODUCTOS - LIMPIAR CAPTURA
        // =========================================================

        function limpiarCapturaProductoAdq() {

            if (inputProducto) {
                inputProducto.value =
                    "";
            }

            if (inputCantidad) {
                inputCantidad.value =
                    "1";
            }

            if (inputUnidad) {
                inputUnidad.value =
                    "";
            }

            if (
                inputDescripcionProducto
            ) {
                inputDescripcionProducto.value =
                    "";
            }


            inputProducto?.focus();
        }


        // =========================================================
        // PRODUCTOS - CONTADOR
        // =========================================================

        function actualizarContadorProductosAdq() {

            if (
                !contenedorDetalles ||
                !contadorProductos
            ) {
                return;
            }


            const cantidad =
                contenedorDetalles
                    .querySelectorAll(
                        ".adq-product-item"
                    )
                    .length;


            contadorProductos.textContent =
                cantidad === 1
                    ? "1 producto"
                    : `${cantidad} productos`;
        }


        // =========================================================
        // PRODUCTOS - VACÍO
        // =========================================================

        function verificarListaProductosVaciaAdq() {

            if (!contenedorDetalles) {
                return;
            }


            const cantidad =
                contenedorDetalles
                    .querySelectorAll(
                        ".adq-product-item"
                    )
                    .length;


            if (cantidad > 0) {
                return;
            }


            if (
                document.getElementById(
                    "adqProductosVacio"
                )
            ) {
                return;
            }


            const vacio =
                document.createElement(
                    "div"
                );


            vacio.id =
                "adqProductosVacio";

            vacio.className =
                "adq-products-empty";


            vacio.innerHTML = `
                <i class="bi bi-box"></i>

                <strong>
                    Aún no has agregado productos o servicios
                </strong>

                <span>
                    Captura la información superior
                    y presiona el botón +.
                </span>
            `;


            contenedorDetalles
                .appendChild(
                    vacio
                );
        }


        function limpiarProductosAdq() {

            if (!contenedorDetalles) {
                return;
            }


            contenedorDetalles.innerHTML =
                "";


            verificarListaProductosVaciaAdq();

            actualizarContadorProductosAdq();
        }


        // =========================================================
        // EVENTOS DE PRODUCTOS
        // =========================================================

        btnAgregarProducto
            ?.addEventListener(
                "click",
                agregarProductoAdq
            );


        contenedorDetalles
            ?.addEventListener(
                "click",
                function (
                    event
                ) {

                    const boton =
                        event.target.closest(
                            ".btnEliminarDetalleAdq"
                        );


                    if (!boton) {
                        return;
                    }


                    eliminarProductoAdq(
                        boton
                    );
                }
            );


        inputProducto
            ?.addEventListener(
                "keydown",
                function (
                    event
                ) {

                    if (
                        event.key !==
                        "Enter"
                    ) {
                        return;
                    }


                    event.preventDefault();

                    agregarProductoAdq();
                }
            );


        inputDescripcionProducto
            ?.addEventListener(
                "keydown",
                function (
                    event
                ) {

                    if (
                        event.key !==
                        "Enter"
                    ) {
                        return;
                    }


                    event.preventDefault();

                    agregarProductoAdq();
                }
            );


        // =========================================================
        // ARCHIVOS - VALIDACIÓN
        // =========================================================

        function archivoValidoAdq(
            archivo
        ) {

            const extension =
                obtenerExtensionAdq(
                    archivo.name
                );


            const formatoValido =
                extensionesPermitidasAdq
                    .includes(
                        extension
                    );


            const tamanoValido =
                archivo.size <=
                tamanoMaximoArchivoAdq;


            return {
                valido:
                    formatoValido &&
                    tamanoValido,

                formatoValido:
                    formatoValido,

                tamanoValido:
                    tamanoValido
            };
        }


        // =========================================================
        // ARCHIVOS - IDENTIFICADOR
        // =========================================================

        function obtenerClaveArchivoAdq(
            archivo
        ) {

            return [
                archivo.name,
                archivo.size,
                archivo.lastModified
            ]
                .join(
                    "|"
                );
        }


        // =========================================================
        // ARCHIVOS - SINCRONIZAR INPUT
        // =========================================================

        function sincronizarInputArchivosAdq() {

            if (!inputArchivos) {
                return;
            }


            const transferencia =
                new DataTransfer();


            archivosSeleccionadosAdq
                .forEach(
                    function (
                        archivo
                    ) {

                        transferencia.items.add(
                            archivo
                        );

                    }
                );


            inputArchivos.files =
                transferencia.files;
        }

        // =========================================================
        // ARCHIVOS EXISTENTES - SINCRONIZAR ELIMINADOS
        // =========================================================

        function sincronizarAdjuntosEliminarAdq() {

            const contenedor =
                document.getElementById(
                    "contenedorAdjuntosEliminarAdq"
                );


            if (!contenedor) {
                return;
            }


            contenedor.innerHTML =
                "";


            adjuntosEliminarIdsAdq
                .forEach(
                    function (
                        id
                    ) {

                        const input =
                            document.createElement(
                                "input"
                            );


                        input.type =
                            "hidden";


                        input.name =
                            "AdjuntosEliminarIds";


                        input.value =
                            String(
                                id
                            );


                        contenedor.appendChild(
                            input
                        );
                    }
                );
        }

        // =========================================================
        // ARCHIVOS - RENDERIZAR
        // =========================================================

        function renderizarArchivosSeleccionadosAdq() {

            if (!listaArchivos) {
                return;
            }


            listaArchivos.innerHTML =
                "";


            const existentesActivos =
                archivosExistentesAdq
                    .filter(
                        function (
                            archivo
                        ) {

                            return !adjuntosEliminarIdsAdq
                                .includes(
                                    Number(
                                        archivo.id
                                    )
                                );
                        }
                    );


            const cantidadTotal =
                existentesActivos.length +
                archivosSeleccionadosAdq.length;


            if (contadorArchivos) {

                contadorArchivos.textContent =
                    cantidadTotal === 1
                        ? "1 archivo agregado"
                        : `${cantidadTotal} archivos agregados`;
            }


            if (cantidadTotal === 0) {

                const vacio =
                    document.createElement(
                        "div"
                    );


                vacio.className =
                    "adq-files-empty";


                vacio.innerHTML = `
                    <i class="bi bi-folder2-open"></i>

                    <span>
                        Aún no has agregado archivos.
                    </span>
                `;


                listaArchivos.appendChild(
                    vacio
                );


                return;
            }


            // =====================================================
            // ARCHIVOS EXISTENTES
            // =====================================================

            existentesActivos
                .forEach(
                    function (
                        archivo
                    ) {

                        const item =
                            document.createElement(
                                "div"
                            );


                        item.className =
                            "adq-file-item";


                        item.innerHTML = `
                            <div class="adq-file-item-main">

                                <div class="adq-file-item-icon">

                                    <i class="bi bi-file-earmark-check"></i>

                                </div>


                                <div class="adq-file-item-info">

                                    <strong>
                                        ${escapeHtmlAdq(
                            archivo.nombreOriginal
                        )}
                                    </strong>

                                    <span>
                                        ${formatearTamanoAdq(
                            archivo.tamanoBytes ?? 0
                        )}
                                        · Archivo existente
                                    </span>

                                </div>

                            </div>


                            <div class="d-flex gap-2">

                                <a href="${escapeAttributeAdq(
                            archivo.rutaArchivo
                        )}"
                                   target="_blank"
                                   class="btn btn-sm btn-outline-primary"
                                   title="Abrir archivo">

                                    <i class="bi bi-eye"></i>

                                </a>


                                <button type="button"
                                        class="btn btn-sm btn-outline-danger btnEliminarArchivoExistenteAdq"
                                        data-id="${archivo.id}"
                                        title="Eliminar archivo">

                                    <i class="bi bi-trash"></i>

                                </button>

                            </div>
                        `;


                        listaArchivos.appendChild(
                            item
                        );
                    }
                );


            // =====================================================
            // ARCHIVOS NUEVOS
            // =====================================================

            archivosSeleccionadosAdq
                .forEach(
                    function (
                        archivo,
                        indice
                    ) {

                        const validacion =
                            archivoValidoAdq(
                                archivo
                            );


                        const item =
                            document.createElement(
                                "div"
                            );


                        item.className =
                            "adq-file-item";


                        if (!validacion.valido) {

                            item.classList.add(
                                "adq-file-item-error"
                            );
                        }


                        let estado =
                            "Archivo nuevo";


                        if (!validacion.formatoValido) {

                            estado =
                                "Formato no permitido";
                        }
                        else if (!validacion.tamanoValido) {

                            estado =
                                "Supera 15 MB";
                        }


                        item.innerHTML = `
                            <div class="adq-file-item-main">

                                <div class="adq-file-item-icon">

                                    <i class="bi ${validacion.valido
                                ? "bi-file-earmark-plus"
                                : "bi-file-earmark-x"
                            }"></i>

                                </div>


                                <div class="adq-file-item-info">

                                    <strong>
                                        ${escapeHtmlAdq(
                                archivo.name
                            )}
                                    </strong>

                                    <span>
                                        ${formatearTamanoAdq(
                                archivo.size
                            )}
                                        ·
                                        ${escapeHtmlAdq(
                                estado
                            )}
                                    </span>

                                </div>

                            </div>


                            <button type="button"
                                    class="btn btn-sm btn-outline-danger btnEliminarArchivoAdq"
                                    data-index="${indice}"
                                    title="Eliminar archivo">

                                <i class="bi bi-trash"></i>

                            </button>
                        `;


                        listaArchivos.appendChild(
                            item
                        );
                    }
                );
        }


        // =========================================================
        // ARCHIVOS - AGREGAR
        // =========================================================

        function agregarArchivosAdq(
            archivosNuevos
        ) {

            const clavesExistentes =
                new Set(
                    archivosSeleccionadosAdq
                        .map(
                            obtenerClaveArchivoAdq
                        )
                );


            let archivoInvalido =
                false;


            archivosNuevos
                .forEach(
                    function (
                        archivo
                    ) {

                        const clave =
                            obtenerClaveArchivoAdq(
                                archivo
                            );


                        if (
                            clavesExistentes.has(
                                clave
                            )
                        ) {

                            return;
                        }


                        const validacion =
                            archivoValidoAdq(
                                archivo
                            );


                        if (
                            !validacion.valido
                        ) {

                            archivoInvalido =
                                true;

                        }


                        archivosSeleccionadosAdq
                            .push(
                                archivo
                            );


                        clavesExistentes.add(
                            clave
                        );

                    }
                );


            sincronizarInputArchivosAdq();

            renderizarArchivosSeleccionadosAdq();

            actualizarEstadoEnviar();


            if (archivoInvalido) {

                mostrarAdvertenciaAdq(
                    "Revisa los archivos",
                    "Uno o más archivos tienen un formato no permitido o superan el límite de 15 MB."
                );

            }
        }


        // =========================================================
        // ARCHIVOS - ELIMINAR
        // =========================================================

        function eliminarArchivoAdq(
            indice
        ) {

            archivosSeleccionadosAdq
                .splice(
                    indice,
                    1
                );


            sincronizarInputArchivosAdq();

            renderizarArchivosSeleccionadosAdq();

            actualizarEstadoEnviar();
        }


        // =========================================================
        // EVENTOS DE ARCHIVOS
        // =========================================================

        btnAgregarArchivos
            ?.addEventListener(
                "click",
                function () {

                    inputArchivos
                        ?.click();

                }
            );


        inputArchivos
            ?.addEventListener(
                "change",
                function () {

                    const archivos =
                        Array.from(
                            inputArchivos.files ??
                            []
                        );


                    agregarArchivosAdq(
                        archivos
                    );

                }
            );


        listaArchivos
            ?.addEventListener(
                "click",
                function (
                    event
                ) {

                    const boton =
                        event.target.closest(
                            ".btnEliminarArchivoAdq"
                        );


                    if (!boton) {
                        return;
                    }


                    const indice =
                        Number(
                            boton.dataset.index
                        );


                    if (
                        Number.isNaN(
                            indice
                        )
                    ) {
                        return;
                    }


                    eliminarArchivoAdq(
                        indice
                    );
                }
            );

        // =========================================================
        // ARCHIVOS EXISTENTES - ELIMINAR
        // =========================================================

        listaArchivos
            ?.addEventListener(
                "click",
                function (
                    event
                ) {

                    const boton =
                        event.target.closest(
                            ".btnEliminarArchivoExistenteAdq"
                        );


                    if (!boton) {
                        return;
                    }


                    const id =
                        Number(
                            boton.dataset.id
                        );


                    if (
                        !id ||
                        Number.isNaN(
                            id
                        )
                    ) {
                        return;
                    }


                    if (
                        !adjuntosEliminarIdsAdq
                            .includes(
                                id
                            )
                    ) {

                        adjuntosEliminarIdsAdq
                            .push(
                                id
                            );
                    }


                    sincronizarAdjuntosEliminarAdq();

                    renderizarArchivosSeleccionadosAdq();

                    actualizarEstadoEnviar();
                }
            );


        // =========================================================
        // ARCHIVOS - VALIDAR TODOS
        // =========================================================

        function archivosValidosAdq() {

            return archivosSeleccionadosAdq
                .every(
                    function (
                        archivo
                    ) {

                        return archivoValidoAdq(
                            archivo
                        ).valido;

                    }
                );
        }


        // =========================================================
        // FORMULARIO COMPLETO
        // =========================================================

        function formularioCompleto() {

            if (!formulario) {
                return false;
            }


            const titulo =
                formulario.querySelector(
                    '[name="Input.Titulo"]'
                );

            const area =
                formulario.querySelector(
                    '[name="Input.AreaId"]'
                );

            const descripcion =
                formulario.querySelector(
                    '[name="Input.Descripcion"]'
                );

            const justificacion =
                formulario.querySelector(
                    '[name="Input.Justificacion"]'
                );


            if (
                !titulo?.value.trim() ||
                !area?.value ||
                area.value === "0" ||
                !descripcion?.value.trim() ||
                !justificacion?.value.trim()
            ) {
                return false;
            }


            const productos =
                formulario.querySelectorAll(
                    ".adq-product-item"
                );


            if (
                productos.length ===
                0
            ) {
                return false;
            }


            if (
                !archivosValidosAdq()
            ) {
                return false;
            }


            return true;
        }


        function actualizarEstadoEnviar() {

            if (!btnEnviar) {
                return;
            }


            const editando =
                Boolean(
                    solicitudEditarId?.value
                );


            if (editando) {

                btnEnviar.disabled =
                    false;

                return;
            }


            btnEnviar.disabled =
                !formularioCompleto();
        }


        formulario
            ?.addEventListener(
                "input",
                actualizarEstadoEnviar
            );


        formulario
            ?.addEventListener(
                "change",
                actualizarEstadoEnviar
            );


        // =========================================================
        // RESET DE ARCHIVOS
        // =========================================================

        function limpiarArchivosAdq() {

            archivosSeleccionadosAdq =
                [];


            archivosExistentesAdq =
                [];


            adjuntosEliminarIdsAdq =
                [];


            sincronizarInputArchivosAdq();

            sincronizarAdjuntosEliminarAdq();

            renderizarArchivosSeleccionadosAdq();
        }


        // =========================================================
        // NUEVA SOLICITUD
        // =========================================================

        function prepararNuevaSolicitudAdq() {

            if (!formulario) {
                return;
            }


            formulario.reset();


            if (
                solicitudEditarId
            ) {

                solicitudEditarId.value =
                    "";

            }


            limpiarProductosAdq();

            limpiarArchivosAdq();

            limpiarCapturaProductoAdq();


            if (tituloModal) {

                tituloModal.innerHTML = `
                    <i class="bi bi-cart-plus me-2"></i>
                    Nueva solicitud de compra
                `;

            }


            btnGuardarCambios
                ?.classList.add(
                    "d-none"
                );

            btnEnviarBorrador
                ?.classList.add(
                    "d-none"
                );


            btnGuardarBorrador
                ?.classList.remove(
                    "d-none"
                );


            btnEnviar
                ?.classList.remove(
                    "d-none"
                );


            actualizarEstadoEnviar();
        }


        btnNuevaSolicitud
            ?.addEventListener(
                "click",
                prepararNuevaSolicitudAdq
            );


        // =========================================================
        // CONSULTAR SOLICITUD
        // =========================================================

        async function obtenerSolicitudAdq(
            id
        ) {

            const response =
                await fetch(
                    `?handler=DetalleSolicitud&id=${encodeURIComponent(id)}`,
                    {
                        method: "GET",
                        headers: {
                            "X-Requested-With":
                                "XMLHttpRequest"
                        }
                    }
                );


            if (!response.ok) {

                throw new Error(
                    "No fue posible consultar la solicitud."
                );

            }


            const resultado =
                await response.json();


            if (
                !resultado.success ||
                !resultado.solicitud
            ) {

                throw new Error(
                    resultado.message ??
                    "No fue posible consultar la solicitud."
                );

            }


            return resultado.solicitud;
        }


        // =========================================================
        // VER SOLICITUD
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnVerSolicitudAdq"
                    );


                if (!boton) {
                    return;
                }


                try {
                    boton.disabled =
                        true;


                    const solicitud =
                        await obtenerSolicitudAdq(
                            boton.dataset.id
                        );

                    limpiarAdjuntosComentarioAdq();


                    if (
                        inputNuevoComentarioAdq
                    ) {
                        inputNuevoComentarioAdq.value =
                            "";
                    }

                    solicitudDetalleActualAdq =
                        solicitud;


                    /*
                     * Siempre reiniciamos el detalle
                     * en la pestaña Información.
                     */
                    const tabInformacion =
                        document.getElementById(
                            "tabInformacionAdq"
                        );


                    if (
                        tabInformacion
                    ) {

                        bootstrap.Tab
                            .getOrCreateInstance(
                                tabInformacion
                            )
                            .show();
                    }


                    /*
                     * El chat comienza cuando el usuario
                     * ya envió la solicitud.
                     */
                    if (
                        solicitud.estatusId !==
                        1
                    ) {

                        itemTabSeguimientoAdq
                            ?.classList.remove(
                                "d-none"
                            );


                        itemTabHistorialAdq
                            ?.classList.remove(
                                "d-none"
                            );


                        try {

                            await cargarSeguimientoAdq(
                                solicitud.id
                            );

                            if (
                                intervaloSeguimientoAdq
                            ) {

                                clearInterval(
                                    intervaloSeguimientoAdq
                                );
                            }

                            intervaloSeguimientoAdq =
                                setInterval(
                                    async function () {

                                        if (
                                            !solicitudDetalleActualAdq?.id
                                        ) {
                                            return;
                                        }

                                        try {

                                            await cargarSeguimientoAdq(
                                                solicitudDetalleActualAdq.id
                                            );

                                        }
                                        catch (
                                        error
                                        ) {

                                            console.error(
                                                "Error al actualizar seguimiento:",
                                                error
                                            );
                                        }

                                    },
                                    20000
                                );

                        }
                        catch (
                        error
                        ) {

                            console.error(
                                "Error al cargar seguimiento:",
                                error
                            );
                        }

                    }
                    else {

                        itemTabSeguimientoAdq
                            ?.classList.add(
                                "d-none"
                            );


                        itemTabHistorialAdq
                            ?.classList.add(
                                "d-none"
                            );


                        if (
                            inputNuevoComentarioAdq
                        ) {

                            inputNuevoComentarioAdq.value =
                                "";
                        }
                    }

                    document.getElementById(
                        "verSolicitudFolio"
                    ).textContent =
                        solicitud.folio;


                    document.getElementById(
                        "verSolicitudTitulo"
                    ).textContent =
                        solicitud.titulo;

                    document.getElementById(
                        "verSolicitudSolicitante"
                    ).textContent =
                        solicitud.solicitante ??
                        "No disponible";


                    document.getElementById(
                        "verSolicitudArea"
                    ).textContent =
                        solicitud.area;


                    document.getElementById(
                        "verSolicitudEstatus"
                    ).textContent =
                        solicitud.estatus;


                    document.getElementById(
                        "verSolicitudDescripcion"
                    ).textContent =
                        solicitud.descripcion;


                    document.getElementById(
                        "verSolicitudJustificacion"
                    ).textContent =
                        solicitud.justificacion;


                    const fecha =
                        new Date(
                            solicitud.fechaSolicitud
                        );


                    document.getElementById(
                        "verSolicitudFecha"
                    ).textContent =
                        fecha.toLocaleDateString(
                            "es-MX"
                        );


                    const productos =
                        document.getElementById(
                            "verSolicitudProductos"
                        );


                    productos.innerHTML =
                        "";


                    solicitud.detalles
                        .forEach(
                            function (
                                detalle
                            ) {

                                const item =
                                    document.createElement(
                                        "div"
                                    );


                                item.className =
                                    "adq-detail-product-item";


                                item.innerHTML = `
                                    <div class="adq-detail-product-main">

                                        <div class="adq-detail-product-icon">

                                            <i class="bi bi-box-seam"></i>

                                        </div>


                                        <div class="adq-detail-product-info">

                                            <span class="adq-detail-product-title">
                                                ${escapeHtmlAdq(
                                    detalle.productoServicio
                                )}
                                            </span>

                                            <span class="adq-detail-product-description">

                                                ${escapeHtmlAdq(
                                    detalle.descripcion ??
                                    "Sin descripción adicional"
                                )}

                                            </span>

                                        </div>

                                    </div>


                                    <div class="adq-detail-product-meta">

                                        <div>

                                            <span class="adq-detail-meta-label">
                                                Cantidad
                                            </span>

                                            <span class="adq-detail-meta-value">
                                                ${escapeHtmlAdq(
                                    detalle.cantidad
                                )}
                                            </span>

                                        </div>


                                        <div>

                                            <span class="adq-detail-meta-label">
                                                Unidad
                                            </span>

                                            <span class="adq-detail-meta-value">
                                                ${escapeHtmlAdq(
                                    detalle.unidad
                                )}
                                            </span>

                                        </div>

                                    </div>
                                `;


                                productos.appendChild(
                                    item
                                );

                            }
                        );


                    const documentos =
                        document.getElementById(
                            "verSolicitudDocumentos"
                        );


                    documentos.innerHTML =
                        "";


                    if (
                        !solicitud.adjuntos ||
                        solicitud.adjuntos.length ===
                        0
                    ) {

                        documentos.innerHTML = `
                            <div class="text-muted small">
                                No existen documentos adjuntos.
                            </div>
                        `;

                    }
                    else {

                        solicitud.adjuntos
                            .forEach(
                                function (
                                    archivo
                                ) {

                                    const enlace =
                                        document.createElement(
                                            "a"
                                        );


                                    enlace.className =
                                        "adq-file-item text-decoration-none";


                                    enlace.href =
                                        archivo.rutaArchivo;


                                    enlace.target =
                                        "_blank";


                                    enlace.rel =
                                        "noopener noreferrer";


                                    enlace.innerHTML = `
                                        <div class="adq-file-item-main">

                                            <div class="adq-file-item-icon">

                                                <i class="bi bi-file-earmark"></i>

                                            </div>

                                            <div class="adq-file-item-info">

                                                <strong>
                                                    ${escapeHtmlAdq(
                                        archivo.nombreOriginal
                                    )}
                                                </strong>

                                                <span>
                                                    Abrir documento
                                                </span>

                                            </div>

                                        </div>
                                    `;


                                    documentos.appendChild(
                                        enlace
                                    );

                                }
                            );

                    }

                    // =========================================================
                    // ACCIONES DISPONIBLES DESDE EL DETALLE
                    // =========================================================

                    btnEditarDesdeDetalle
                        ?.classList.add(
                            "d-none"
                        );


                    btnCancelarDesdeDetalle
                        ?.classList.add(
                            "d-none"
                        );


                    btnEnviarDesdeDetalle
                        ?.classList.add(
                            "d-none"
                        );


                    /*
                     * Borrador:
                     * editar + cancelar + enviar.
                     */
                    if (
                        solicitud.estatusId ===
                        1
                    ) {
                        btnEditarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );

                        btnCancelarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );

                        btnEnviarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );
                    }


                    /*
                     * Pendiente del gerente:
                     * editar + cancelar.
                     */
                    else if (
                        solicitud.estatusId ===
                        2
                    ) {
                        btnEditarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );

                        btnCancelarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );
                    }


                    /*
                     * Gerente ya aprobó y llegó a Adquisiciones:
                     * ya no puede editar, solamente cancelar.
                     */
                    else if (
                        solicitud.estatusId ===
                        3
                    ) {
                        btnCancelarDesdeDetalle
                            ?.classList.remove(
                                "d-none"
                            );
                    }

                    bootstrap.Modal
                        .getOrCreateInstance(
                            modalVerSolicitudElement
                        )
                        .show();

                }
                catch (
                error
                ) {
                    console.error(
                        error
                    );


                    mostrarAdvertenciaAdq(
                        "No fue posible consultar",
                        error.message
                    );

                }
                finally {
                    boton.disabled =
                        false;
                }
            }
        );

        // =========================================================
        // DETENER ACTUALIZACIÓN DEL CHAT AL CERRAR DETALLE
        // =========================================================

        modalVerSolicitudElement
            ?.addEventListener(
                "hidden.bs.modal",
                function () {

                    if (
                        intervaloSeguimientoAdq
                    ) {

                        clearInterval(
                            intervaloSeguimientoAdq
                        );

                        intervaloSeguimientoAdq =
                            null;

                        limpiarAdjuntosComentarioAdq();


                        if (
                            inputNuevoComentarioAdq
                        ) {
                            inputNuevoComentarioAdq.value =
                                "";
                        }
                    }

                    solicitudDetalleActualAdq =
                        null;

                    if (
                        badgeMensajesPendientesAdq
                    ) {

                        badgeMensajesPendientesAdq
                            .classList
                            .add(
                                "d-none"
                            );

                        badgeMensajesPendientesAdq.textContent =
                            "";
                    }
                }
            );


        // =========================================================
        // EDITAR DESDE DETALLE
        // =========================================================

        btnEditarDesdeDetalle
            ?.addEventListener(
                "click",
                function () {

                    if (
                        !solicitudDetalleActualAdq
                    ) {
                        return;
                    }


                    const id =
                        solicitudDetalleActualAdq.id;


                    bootstrap.Modal
                        .getInstance(
                            modalVerSolicitudElement
                        )
                        ?.hide();


                    const botonEditar =
                        document.querySelector(
                            `.btnEditarSolicitudAdq[data-id="${id}"]`
                        );


                    botonEditar?.click();
                }
            );

        // =========================================================
        // CANCELAR DESDE DETALLE
        // =========================================================

        btnCancelarDesdeDetalle
            ?.addEventListener(
                "click",
                function () {

                    if (
                        !solicitudDetalleActualAdq
                    ) {
                        return;
                    }


                    abrirCancelacionSolicitudUsuarioAdq(
                        solicitudDetalleActualAdq.id,
                        solicitudDetalleActualAdq.folio
                    );


                    bootstrap.Modal
                        .getInstance(
                            modalVerSolicitudElement
                        )
                        ?.hide();
                }
            );

        // =========================================================
        // ABRIR CANCELACIÓN DEL SOLICITANTE
        // =========================================================

        function abrirCancelacionSolicitudUsuarioAdq(
            id,
            folio
        ) {

            const modalElement =
                document.getElementById(
                    "modalCancelarSolicitudUsuarioAdq"
                );


            const inputId =
                document.getElementById(
                    "SolicitudCancelarUsuarioId"
                );


            const folioElemento =
                document.getElementById(
                    "folioCancelarSolicitudUsuarioAdq"
                );


            const motivo =
                document.getElementById(
                    "MotivoCancelacionUsuario"
                );


            if (
                !modalElement ||
                !inputId
            ) {
                return;
            }


            inputId.value =
                id;


            if (folioElemento) {
                folioElemento.textContent =
                    folio ?? "";
            }


            if (motivo) {
                motivo.value =
                    "";
            }


            bootstrap.Modal
                .getOrCreateInstance(
                    modalElement
                )
                .show();
        }

        // =========================================================
        // CANCELAR DESDE MIS SOLICITUDES
        // =========================================================

        document.addEventListener(
            "click",
            function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnCancelarSolicitudUsuarioAdq"
                    );


                if (!boton) {
                    return;
                }


                abrirCancelacionSolicitudUsuarioAdq(
                    boton.dataset.id,
                    boton.dataset.folio
                );
            }
        );

        // =========================================================
        // ENVIAR BORRADOR DESDE DETALLE
        // =========================================================

        btnEnviarDesdeDetalle
            ?.addEventListener(
                "click",
                function () {

                    if (
                        !solicitudDetalleActualAdq ||
                        solicitudDetalleActualAdq.estatusId !== 1
                    ) {
                        return;
                    }


                    const id =
                        solicitudDetalleActualAdq.id;


                    bootstrap.Modal
                        .getInstance(
                            modalVerSolicitudElement
                        )
                        ?.hide();


                    const botonEditar =
                        document.querySelector(
                            `.btnEditarSolicitudAdq[data-id="${id}"]`
                        );


                    botonEditar?.click();
                }
            );

        // =========================================================
        // EDITAR SOLICITUD
        // =========================================================

        document.addEventListener(
            "click",
            async function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnEditarSolicitudAdq"
                    );


                if (!boton) {
                    return;
                }


                try {
                    boton.disabled =
                        true;


                    const solicitud =
                        await obtenerSolicitudAdq(
                            boton.dataset.id
                        );


                    if (
                        solicitud.estatusId !== 1 &&
                        solicitud.estatusId !== 2
                    ) {

                        mostrarAdvertenciaAdq(
                            "Edición no disponible",
                            "La solicitud ya no puede modificarse porque ya fue aprobada por el gerente."
                        );

                        return;
                    }


                    prepararNuevaSolicitudAdq();


                    solicitudEditarId.value =
                        solicitud.id;


                    document.querySelector(
                        '[name="Input.Titulo"]'
                    ).value =
                        solicitud.titulo;


                    document.querySelector(
                        '[name="Input.AreaId"]'
                    ).value =
                        String(
                            solicitud.areaId
                        );


                    document.querySelector(
                        '[name="Input.Descripcion"]'
                    ).value =
                        solicitud.descripcion;


                    document.querySelector(
                        '[name="Input.Justificacion"]'
                    ).value =
                        solicitud.justificacion;


                    limpiarProductosAdq();


                    solicitud.detalles
                        .forEach(
                            function (
                                detalle
                            ) {

                                agregarProductoExistenteAdq(
                                    detalle
                                );

                            }
                        );

                    // =========================================================
                    // CARGAR ARCHIVOS EXISTENTES
                    // =========================================================

                    archivosExistentesAdq =
                        Array.isArray(
                            solicitud.adjuntos
                        )
                            ? solicitud.adjuntos
                                .map(
                                    function (
                                        archivo
                                    ) {

                                        return {
                                            id:
                                                Number(
                                                    archivo.id
                                                ),

                                            nombreOriginal:
                                                archivo.nombreOriginal,

                                            rutaArchivo:
                                                archivo.rutaArchivo,

                                            extension:
                                                archivo.extension,

                                            mimeType:
                                                archivo.mimeType,

                                            tamanoBytes:
                                                Number(
                                                    archivo.tamanoBytes ??
                                                    0
                                                )
                                        };
                                    }
                                )
                            : [];


                    adjuntosEliminarIdsAdq =
                        [];


                    archivosSeleccionadosAdq =
                        [];


                    sincronizarInputArchivosAdq();

                    sincronizarAdjuntosEliminarAdq();

                    renderizarArchivosSeleccionadosAdq();


                    if (tituloModal) {

                        let textoEstado =
                            "";

                        if (solicitud.estatusId === 1) {
                            textoEstado =
                                "Borrador";
                        }
                        else if (solicitud.estatusId === 2) {
                            textoEstado =
                                "Pendiente aprobación gerente";
                        }
                        else if (solicitud.estatusId === 3) {
                            textoEstado =
                                "Solicitud enviada";
                        }


                        tituloModal.innerHTML = `
                            <i class="bi bi-pencil-square me-2"></i>
                            Editar solicitud
                            <small class="d-block mt-1 text-muted">
                                ${escapeHtmlAdq(textoEstado)}
                            </small>
                        `;
                    }


                    btnGuardarCambios
                        ?.classList.remove(
                            "d-none"
                        );

                    if (
                        solicitud.estatusId ===
                        1
                    ) {

                        btnEnviarBorrador
                            ?.classList.remove(
                                "d-none"
                            );
                    }
                    else {

                        btnEnviarBorrador
                            ?.classList.add(
                                "d-none"
                            );
                    }


                    btnGuardarBorrador
                        ?.classList.add(
                            "d-none"
                        );


                    btnEnviar
                        ?.classList.add(
                            "d-none"
                        );


                    actualizarContadorProductosAdq();


                    bootstrap.Modal
                        .getOrCreateInstance(
                            modalNuevaSolicitudElement
                        )
                        .show();

                }
                catch (
                error
                ) {
                    console.error(
                        error
                    );


                    mostrarAdvertenciaAdq(
                        "No fue posible editar",
                        error.message
                    );

                }
                finally {
                    boton.disabled =
                        false;
                }
            }
        );


        // =========================================================
        // FILTROS + PAGINACIÓN - MIS SOLICITUDES
        // =========================================================

        function aplicarFiltros(
            reiniciarPagina = true
        ) {

            const texto =
                filtroBusqueda
                    ?.value
                    .trim()
                    .toLowerCase() ??
                "";


            const estatus =
                filtroEstatus
                    ?.value ??
                "";


            const filas =
                Array.from(
                    document.querySelectorAll(
                        "#tablaSolicitudesAdq .adq-solicitud-row"
                    )
                );


            const filasFiltradas =
                filas.filter(
                    function (
                        fila
                    ) {

                        const folio =
                            fila.dataset.folio ??
                            "";

                        const titulo =
                            fila.dataset.titulo ??
                            "";

                        const area =
                            fila.dataset.area ??
                            "";

                        const filaEstatus =
                            fila.dataset.estatus ??
                            "";


                        const coincideTexto =
                            !texto ||
                            folio.includes(
                                texto
                            ) ||
                            titulo.includes(
                                texto
                            ) ||
                            area.includes(
                                texto
                            );


                        const coincideEstatus =
                            !estatus ||
                            filaEstatus ===
                            estatus;


                        return (
                            coincideTexto &&
                            coincideEstatus
                        );
                    }
                );


            if (
                reiniciarPagina
            ) {
                paginaActualAdq =
                    1;
            }


            renderizarPaginaSolicitudesAdq(
                filas,
                filasFiltradas
            );
        }

        // =========================================================
        // RENDERIZAR PÁGINA DE SOLICITUDES
        // =========================================================

        function renderizarPaginaSolicitudesAdq(
            todasLasFilas,
            filasFiltradas
        ) {

            /*
             * Primero ocultamos absolutamente
             * todas las filas.
             */
            todasLasFilas
                .forEach(
                    function (
                        fila
                    ) {

                        fila.classList.add(
                            "d-none"
                        );
                    }
                );


            const totalRegistros =
                filasFiltradas.length;


            const totalPaginas =
                Math.max(
                    1,
                    Math.ceil(
                        totalRegistros /
                        registrosPorPaginaAdq
                    )
                );


            /*
             * Evitamos quedar en una página
             * que ya no exista después de filtrar.
             */
            if (
                paginaActualAdq >
                totalPaginas
            ) {

                paginaActualAdq =
                    totalPaginas;
            }


            const inicio =
                (
                    paginaActualAdq -
                    1
                ) *
                registrosPorPaginaAdq;


            const fin =
                Math.min(
                    inicio +
                    registrosPorPaginaAdq,
                    totalRegistros
                );


            /*
             * Mostramos únicamente los registros
             * correspondientes a la página.
             */
            filasFiltradas
                .slice(
                    inicio,
                    fin
                )
                .forEach(
                    function (
                        fila
                    ) {

                        fila.classList.remove(
                            "d-none"
                        );
                    }
                );


            actualizarInformacionPaginacionAdq(
                inicio,
                fin,
                totalRegistros
            );


            renderizarControlesPaginacionAdq(
                totalPaginas,
                totalRegistros
            );
        }

        // =========================================================
        // INFORMACIÓN DE PAGINACIÓN
        // =========================================================

        function actualizarInformacionPaginacionAdq(
            inicio,
            fin,
            total
        ) {

            if (
                paginaInicio
            ) {

                paginaInicio.textContent =
                    total === 0
                        ? "0"
                        : String(
                            inicio + 1
                        );
            }


            if (
                paginaFin
            ) {

                paginaFin.textContent =
                    String(
                        fin
                    );
            }


            if (
                paginaTotal
            ) {

                paginaTotal.textContent =
                    String(
                        total
                    );
            }
        }

        // =========================================================
        // CONTROLES DE PAGINACIÓN
        // =========================================================

        function renderizarControlesPaginacionAdq(
            totalPaginas,
            totalRegistros
        ) {

            if (
                !paginacionLista ||
                !paginacionContenedor
            ) {
                return;
            }


            paginacionLista.innerHTML =
                "";


            /*
             * Si no existen registros,
             * ocultamos completamente el paginador.
             */
            if (
                totalRegistros === 0
            ) {

                paginacionContenedor
                    .classList
                    .add(
                        "d-none"
                    );

                return;
            }


            paginacionContenedor
                .classList
                .remove(
                    "d-none"
                );


            // =====================================================
            // ANTERIOR
            // =====================================================

            crearBotonPaginacionAdq(
                "Anterior",
                paginaActualAdq - 1,
                paginaActualAdq === 1,
                false
            );


            // =====================================================
            // NÚMEROS
            // =====================================================

            for (
                let pagina = 1;
                pagina <= totalPaginas;
                pagina++
            ) {

                crearBotonPaginacionAdq(
                    String(
                        pagina
                    ),
                    pagina,
                    false,
                    pagina === paginaActualAdq
                );
            }


            // =====================================================
            // SIGUIENTE
            // =====================================================

            crearBotonPaginacionAdq(
                "Siguiente",
                paginaActualAdq + 1,
                paginaActualAdq ===
                totalPaginas,
                false
            );
        }

        // =========================================================
        // CREAR BOTÓN DE PAGINACIÓN
        // =========================================================

        function crearBotonPaginacionAdq(
            texto,
            pagina,
            deshabilitado,
            activo
        ) {

            if (
                !paginacionLista
            ) {
                return;
            }


            const item =
                document.createElement(
                    "li"
                );


            item.className =
                "page-item";


            if (
                deshabilitado
            ) {

                item.classList.add(
                    "disabled"
                );
            }


            if (
                activo
            ) {

                item.classList.add(
                    "active"
                );
            }


            const boton =
                document.createElement(
                    "button"
                );


            boton.type =
                "button";


            boton.className =
                "page-link";


            boton.textContent =
                texto;


            boton.disabled =
                deshabilitado;


            boton.addEventListener(
                "click",
                function () {

                    if (
                        deshabilitado
                    ) {
                        return;
                    }


                    paginaActualAdq =
                        pagina;


                    aplicarFiltros(
                        false
                    );


                    /*
                     * Regresamos suavemente
                     * al inicio de la tabla.
                     */
                    document
                        .getElementById(
                            "tablaSolicitudesAdq"
                        )
                        ?.scrollIntoView({
                            behavior:
                                "smooth",

                            block:
                                "start"
                        });
                }
            );


            item.appendChild(
                boton
            );


            paginacionLista.appendChild(
                item
            );
        }


        filtroBusqueda
            ?.addEventListener(
                "input",
                function () {

                    aplicarFiltros(
                        true
                    );
                }
            );


        filtroEstatus
            ?.addEventListener(
                "change",
                function () {

                    aplicarFiltros(
                        true
                    );
                }
            );


        btnLimpiarFiltros
            ?.addEventListener(
                "click",
                function () {

                    if (
                        filtroBusqueda
                    ) {

                        filtroBusqueda.value =
                            "";

                    }


                    if (
                        filtroEstatus
                    ) {

                        filtroEstatus.value =
                            "";

                    }

                    paginaActualAdq =
                        1;


                    aplicarFiltros(
                        true
                    );
                }
            );


        // =========================================================
        // EVITAR DOBLE SUBMIT
        // =========================================================

        formulario
            ?.addEventListener(
                "submit",
                function () {

                    sincronizarInputArchivosAdq();

                    renumerarProductosAdq();


                    setTimeout(
                        function () {

                            const botones =
                                formulario.querySelectorAll(
                                    'button[type="submit"]'
                                );


                            botones.forEach(
                                function (
                                    boton
                                ) {

                                    boton.disabled =
                                        true;

                                }
                            );

                        },
                        0
                    );
                }
            );

        // =========================================================
        // ACCIONES DE ADQUISICIONES
        // =========================================================

        document.addEventListener(
            "click",
            function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnAccionAdquisiciones"
                    );


                if (!boton) {
                    return;
                }


                const id =
                    boton.dataset.id;

                const folio =
                    boton.dataset.folio ??
                    "";

                const accion =
                    boton.dataset.accion;


                const inputId =
                    document.getElementById(
                        "SolicitudAdquisicionesId"
                    );


                const titulo =
                    document.getElementById(
                        "tituloAccionAdquisiciones"
                    );


                const folioElemento =
                    document.getElementById(
                        "folioAccionAdquisiciones"
                    );


                const mensaje =
                    document.getElementById(
                        "mensajeAccionAdquisiciones"
                    );


                const comentario =
                    document.getElementById(
                        "ComentarioAdquisiciones"
                    );


                const labelComentario =
                    document.getElementById(
                        "labelComentarioAdquisiciones"
                    );


                const ayudaComentario =
                    document.getElementById(
                        "ayudaComentarioAdquisiciones"
                    );


                const btnAprobar =
                    document.getElementById(
                        "btnAprobarAdquisiciones"
                    );


                const btnCancelar =
                    document.getElementById(
                        "btnCancelarAdquisiciones"
                    );


                inputId.value =
                    id;


                folioElemento.textContent =
                    folio;


                comentario.value =
                    "";


                if (
                    accion ===
                    "aprobar"
                ) {
                    titulo.textContent =
                        "Aprobar solicitud";


                    mensaje.className =
                        "alert alert-success mb-3";


                    mensaje.innerHTML = `
                        <i class="bi bi-check-circle me-1"></i>

                        La solicitud será aprobada por el área
                        de Adquisiciones.
                    `;


                    labelComentario.textContent =
                        "Comentario (opcional)";


                    ayudaComentario.textContent =
                        "Puedes registrar una observación de la revisión.";


                    comentario.required =
                        false;


                    btnAprobar.classList.remove(
                        "d-none"
                    );


                    btnCancelar.classList.add(
                        "d-none"
                    );
                }
                else {
                    titulo.textContent =
                        "Cancelar solicitud";


                    mensaje.className =
                        "alert alert-danger mb-3";


                    mensaje.innerHTML = `
                        <i class="bi bi-exclamation-triangle me-1"></i>

                        La solicitud será cancelada
                        y no continuará con el proceso de compra.
                    `;


                    labelComentario.textContent =
                        "Motivo de cancelación";


                    ayudaComentario.textContent =
                        "Este campo es obligatorio.";


                    comentario.required =
                        true;


                    btnCancelar.classList.remove(
                        "d-none"
                    );


                    btnAprobar.classList.add(
                        "d-none"
                    );
                }


                bootstrap.Modal
                    .getOrCreateInstance(
                        document.getElementById(
                            "modalAccionAdquisiciones"
                        )
                    )
                    .show();

            }
        );


        // =========================================================
        // ASIGNAR AGENTE
        // =========================================================

        document.addEventListener(
            "click",
            function (
                event
            ) {

                const boton =
                    event.target.closest(
                        ".btnAsignarAdquisiciones"
                    );


                if (!boton) {
                    return;
                }


                document.getElementById(
                    "SolicitudAsignarAdqId"
                ).value =
                    boton.dataset.id;


                document.getElementById(
                    "folioAsignarAdq"
                ).textContent =
                    boton.dataset.folio ??
                    "";


                bootstrap.Modal
                    .getOrCreateInstance(
                        document.getElementById(
                            "modalAsignarAdquisiciones"
                        )
                    )
                    .show();

            }
        );

        // =========================================================
        // ENVIAR MENSAJE DEL CHAT
        // =========================================================

        // =========================================================
        // ENVIAR MENSAJE DEL CHAT
        // =========================================================

        btnEnviarComentarioAdq
            ?.addEventListener(
                "click",
                async function () {

                    const solicitudId =
                        Number(
                            inputSolicitudComentarioAdq
                                ?.value
                            ??
                            0
                        );


                    const comentario =
                        inputNuevoComentarioAdq
                            ?.value
                            .trim()
                        ??
                        "";


                    if (
                        !solicitudId
                    ) {
                        return;
                    }


                    /*
                     * Permitimos:
                     *
                     * mensaje
                     * mensaje + archivo
                     * solo archivo
                     */
                    if (
                        !comentario
                        &&
                        archivosComentarioSeleccionadosAdq.length ===
                        0
                    ) {

                        mostrarAdvertenciaAdq(
                            "Mensaje requerido",
                            "Escribe un mensaje o adjunta al menos un archivo."
                        );


                        inputNuevoComentarioAdq
                            ?.focus();


                        return;
                    }


                    const token =
                        document.querySelector(
                            'input[name="__RequestVerificationToken"]'
                        )
                            ?.value;


                    const datos =
                        new FormData();


                    datos.append(
                        "SolicitudComentarioId",
                        String(
                            solicitudId
                        )
                    );


                    datos.append(
                        "NuevoComentarioAdq",
                        comentario
                    );


                    /*
                     * ASP.NET Core enlazará cada elemento
                     * con:
                     *
                     * List<IFormFile> ArchivosComentarioAdq
                     */
                    archivosComentarioSeleccionadosAdq
                        .forEach(
                            function (
                                archivo
                            ) {

                                datos.append(
                                    "ArchivosComentarioAdq",
                                    archivo,
                                    archivo.name
                                );
                            }
                        );


                    try {

                        btnEnviarComentarioAdq.disabled =
                            true;


                        const contenidoOriginal =
                            btnEnviarComentarioAdq.innerHTML;


                        btnEnviarComentarioAdq.innerHTML = `
                    <span class="spinner-border spinner-border-sm me-1"></span>
                    Enviando...
                `;


                        try {

                            const response =
                                await fetch(
                                    "?handler=AgregarComentarioAdq",
                                    {
                                        method:
                                            "POST",

                                        headers: {
                                            "RequestVerificationToken":
                                                token
                                                ??
                                                ""
                                        },

                                        body:
                                            datos
                                    }
                                );


                            const resultado =
                                await response.json();


                            if (
                                !response.ok
                                ||
                                !resultado.success
                            ) {

                                throw new Error(
                                    resultado.message
                                    ??
                                    "No fue posible enviar el mensaje."
                                );
                            }


                            // ==========================================
                            // LIMPIAR MENSAJE
                            // ==========================================

                            if (
                                inputNuevoComentarioAdq
                            ) {

                                inputNuevoComentarioAdq.value =
                                    "";
                            }


                            // ==========================================
                            // LIMPIAR ARCHIVOS
                            // ==========================================

                            limpiarAdjuntosComentarioAdq();


                            // ==========================================
                            // RECARGAR CHAT
                            // ==========================================

                            await cargarSeguimientoAdq(
                                solicitudId
                            );

                        }
                        finally {

                            btnEnviarComentarioAdq.disabled =
                                false;


                            btnEnviarComentarioAdq.innerHTML =
                                contenidoOriginal;
                        }

                    }
                    catch (
                    error
                    ) {

                        mostrarAdvertenciaAdq(
                            "No fue posible enviar",
                            error.message
                            ??
                            "Ocurrió un error al enviar el mensaje."
                        );
                    }
                }
            );
           

        // =========================================================
        // HISTORIAL DE APROBACIONES - FILTROS
        // =========================================================

        function aplicarFiltrosHistorialAdq(
            reiniciarPagina = true
        ) {

            const texto =
                filtroHistorialAprobacionesAdq
                    ?.value
                    .trim()
                    .toLowerCase()
                ??
                "";

            const decision =
                filtroDecisionHistorialAdq
                    ?.value
                    .trim()
                    .toLowerCase()
                ??
                "";

            const filas =
                Array.from(
                    document.querySelectorAll(
                        "#tablaHistorialAprobacionesAdq .adq-historial-aprobacion-row"
                    )
                );

            const filasFiltradas =
                filas.filter(
                    function (
                        fila
                    ) {

                        const folio =
                            fila.dataset.folio ??
                            "";

                        const titulo =
                            fila.dataset.titulo ??
                            "";

                        const solicitante =
                            fila.dataset.solicitante ??
                            "";

                        const area =
                            fila.dataset.area ??
                            "";

                        const decisionFila =
                            fila.dataset.decision ??
                            "";

                        const coincideTexto =
                            !texto
                            ||
                            folio.includes(
                                texto
                            )
                            ||
                            titulo.includes(
                                texto
                            )
                            ||
                            solicitante.includes(
                                texto
                            )
                            ||
                            area.includes(
                                texto
                            );

                        const coincideDecision =
                            !decision
                            ||
                            decisionFila ===
                            decision;

                        return (
                            coincideTexto
                            &&
                            coincideDecision
                        );
                    }
                );

            if (
                reiniciarPagina
            ) {

                paginaActualHistorialAdq =
                    1;
            }

            renderizarPaginaHistorialAdq(
                filas,
                filasFiltradas
            );
        }


        // =========================================================
        // HISTORIAL DE APROBACIONES - RENDERIZAR PÁGINA
        // =========================================================

        function renderizarPaginaHistorialAdq(
            todasLasFilas,
            filasFiltradas
        ) {

            todasLasFilas.forEach(
                function (
                    fila
                ) {

                    fila.classList.add(
                        "d-none"
                    );
                }
            );

            const totalRegistros =
                filasFiltradas.length;

            const totalPaginas =
                Math.max(
                    1,
                    Math.ceil(
                        totalRegistros /
                        registrosPorPaginaHistorialAdq
                    )
                );

            if (
                paginaActualHistorialAdq >
                totalPaginas
            ) {

                paginaActualHistorialAdq =
                    totalPaginas;
            }

            const inicio =
                (
                    paginaActualHistorialAdq -
                    1
                )
                *
                registrosPorPaginaHistorialAdq;

            const fin =
                Math.min(
                    inicio +
                    registrosPorPaginaHistorialAdq,
                    totalRegistros
                );

            filasFiltradas
                .slice(
                    inicio,
                    fin
                )
                .forEach(
                    function (
                        fila
                    ) {

                        fila.classList.remove(
                            "d-none"
                        );
                    }
                );

            actualizarInformacionHistorialAdq(
                inicio,
                fin,
                totalRegistros
            );

            renderizarPaginacionHistorialAdq(
                totalPaginas,
                totalRegistros
            );
        }


        // =========================================================
        // HISTORIAL DE APROBACIONES - INFORMACIÓN
        // =========================================================

        function actualizarInformacionHistorialAdq(
            inicio,
            fin,
            total
        ) {

            if (
                historialPaginaInicioAdq
            ) {

                historialPaginaInicioAdq.textContent =
                    total === 0
                        ? "0"
                        : String(
                            inicio + 1
                        );
            }

            if (
                historialPaginaFinAdq
            ) {

                historialPaginaFinAdq.textContent =
                    String(
                        fin
                    );
            }

            if (
                historialPaginaTotalAdq
            ) {

                historialPaginaTotalAdq.textContent =
                    String(
                        total
                    );
            }
        }


        // =========================================================
        // HISTORIAL DE APROBACIONES - PAGINACIÓN
        // =========================================================

        function renderizarPaginacionHistorialAdq(
            totalPaginas,
            totalRegistros
        ) {

            if (
                !listaPaginacionHistorialAdq
                ||
                !paginacionHistorialAdq
            ) {
                return;
            }

            listaPaginacionHistorialAdq.innerHTML =
                "";

            if (
                totalRegistros === 0
            ) {

                paginacionHistorialAdq
                    .classList
                    .add(
                        "d-none"
                    );

                return;
            }

            paginacionHistorialAdq
                .classList
                .remove(
                    "d-none"
                );

            crearBotonPaginacionHistorialAdq(
                "Anterior",
                paginaActualHistorialAdq - 1,
                paginaActualHistorialAdq === 1,
                false
            );

            for (
                let pagina = 1;
                pagina <= totalPaginas;
                pagina++
            ) {

                crearBotonPaginacionHistorialAdq(
                    String(
                        pagina
                    ),
                    pagina,
                    false,
                    pagina ===
                    paginaActualHistorialAdq
                );
            }

            crearBotonPaginacionHistorialAdq(
                "Siguiente",
                paginaActualHistorialAdq + 1,
                paginaActualHistorialAdq ===
                totalPaginas,
                false
            );
        }


        function crearBotonPaginacionHistorialAdq(
            texto,
            pagina,
            deshabilitado = false,
            activo = false
        ) {

            if (
                !listaPaginacionHistorialAdq
            ) {
                return;
            }

            const item =
                document.createElement(
                    "li"
                );

            item.className =
                "page-item";

            if (
                activo
            ) {

                item.classList.add(
                    "active"
                );
            }

            if (
                deshabilitado
            ) {

                item.classList.add(
                    "disabled"
                );
            }

            const boton =
                document.createElement(
                    "button"
                );

            boton.type =
                "button";

            boton.className =
                "page-link";

            boton.textContent =
                texto;

            boton.disabled =
                deshabilitado;

            boton.addEventListener(
                "click",
                function () {

                    if (
                        deshabilitado
                    ) {
                        return;
                    }

                    paginaActualHistorialAdq =
                        pagina;

                    aplicarFiltrosHistorialAdq(
                        false
                    );


                    document
                        .getElementById(
                            "tablaHistorialAprobacionesAdq"
                        )
                        ?.scrollIntoView({
                            behavior:
                                "smooth",

                            block:
                                "start"
                        });
                }
            );

            item.appendChild(
                boton
            );

            listaPaginacionHistorialAdq
                .appendChild(
                    item
                );
        }


        // =========================================================
        // HISTORIAL DE APROBACIONES - EVENTOS
        // =========================================================

        filtroHistorialAprobacionesAdq
            ?.addEventListener(
                "input",
                function () {

                    aplicarFiltrosHistorialAdq(
                        true
                    );
                }
            );

        filtroDecisionHistorialAdq
            ?.addEventListener(
                "change",
                function () {

                    aplicarFiltrosHistorialAdq(
                        true
                    );
                }
            );

        btnLimpiarHistorialAprobacionesAdq
            ?.addEventListener(
                "click",
                function () {

                    if (
                        filtroHistorialAprobacionesAdq
                    ) {

                        filtroHistorialAprobacionesAdq.value =
                            "";
                    }

                    if (
                        filtroDecisionHistorialAdq
                    ) {

                        filtroDecisionHistorialAdq.value =
                            "";
                    }

                    aplicarFiltrosHistorialAdq(
                        true
                    );
                }
            );


        // =========================================================
        // INICIALIZACIÓN
        // =========================================================

        renumerarProductosAdq();

        actualizarContadorProductosAdq();

        verificarListaProductosVaciaAdq();

        renderizarArchivosSeleccionadosAdq();

        actualizarEstadoEnviar();

        aplicarFiltros(
            true
        );

        aplicarFiltrosHistorialAdq(
            true
        );

        // =========================================================
        // ABRIR SOLICITUD / COTIZACIÓN DESDE URL
        // =========================================================

        const parametrosUrlAdq =
            new URLSearchParams(
                window.location.search
            );


        const solicitudAbrirIdAdq =
            Number(
                parametrosUrlAdq.get(
                    "openId"
                )
                ??
                0
            );


        const cotizacionAbrirIdAdq =
            Number(
                parametrosUrlAdq.get(
                    "openCotizacionId"
                )
                ??
                0
            );


        // =========================================================
        // ABRIR DIRECTAMENTE GESTIÓN DE COTIZACIONES
        // =========================================================

        if (
            cotizacionAbrirIdAdq >
            0
        ) {

            const botonCotizacionAdq =
                document.querySelector(
                    `.btnGenerarCotizacionAdq[data-id="${cotizacionAbrirIdAdq}"]`
                );


            if (
                botonCotizacionAdq
            ) {

                setTimeout(
                    function () {

                        botonCotizacionAdq.click();

                    },
                    250
                );
            }
        }


        // =========================================================
        // ABRIR DETALLE NORMAL DE SOLICITUD
        // =========================================================

        else if (
            solicitudAbrirIdAdq >
            0
        ) {

            const botonSolicitudAdq =
                document.querySelector(
                    `.btnVerSolicitudAdq[data-id="${solicitudAbrirIdAdq}"]`
                );


            if (
                botonSolicitudAdq
            ) {

                setTimeout(
                    function () {

                        botonSolicitudAdq.click();

                    },
                    250
                );
            }
        }

    }
);