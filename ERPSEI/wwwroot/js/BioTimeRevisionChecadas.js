
(() => {
    "use strict";

    // =========================================================
    // INTRANET - REVISIÓN DE CHECADAS BIOTIME
    // =========================================================

    const $ = id => document.getElementById(id);

    const estado = {
        registros: [],
        empleadoId: null,
        fecha: null,
        cargando: false,
        guardando: false,
        consultaVersion: 0
    };

    // =========================================================
    // UTILIDADES
    // =========================================================

    function endpoint(handler) {
        const url = new URL(window.location.href);

        url.searchParams.set("handler", handler);

        return url.toString();
    }

    function tokenAntiforgery() {
        return document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value || "";
    }

    function escaparHtml(valor) {
        return String(valor ?? "")
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#39;");
    }

    function fechaHoraLocal(valor) {
        if (!valor) return "—";

        return String(valor)
            .replace("T", " ")
            .substring(0, 19);
    }

    function fechaHoyMexico() {
        const partes = new Intl.DateTimeFormat(
            "en-US",
            {
                timeZone: "America/Mexico_City",
                year: "numeric",
                month: "2-digit",
                day: "2-digit"
            }
        ).formatToParts(new Date());

        const valor = tipo =>
            partes.find(x => x.type === tipo)?.value || "";

        return (
            valor("year") + "-" +
            valor("month") + "-" +
            valor("day")
        );
    }

    function mostrarMensaje(texto, tipo = "info") {
        const elemento = $("rcRevMensaje");

        if (!elemento) return;

        elemento.className =
            `alert alert-${tipo} small`;

        elemento.textContent = texto;
    }

    function mostrarErrorModal(id, texto = "") {
        const elemento = $(id);

        if (!elemento) return;

        elemento.textContent = texto;

        elemento.classList.toggle(
            "d-none",
            !texto
        );
    }

    function abrirModal(id) {
        const elemento = $(id);

        if (!elemento || !window.bootstrap?.Modal) {
            throw new Error(
                "No está disponible Bootstrap Modal."
            );
        }

        bootstrap.Modal
            .getOrCreateInstance(elemento)
            .show();
    }

    function cerrarModal(id) {
        const elemento = $(id);

        if (!elemento) return;

        bootstrap.Modal
            .getInstance(elemento)
            ?.hide();
    }

    function obtenerRegistro(id) {
        return estado.registros.find(
            x => String(x.id) === String(id)
        );
    }

    function estadoHtml(valor) {
        if (valor === "Confirmada") {
            return `
                <span class="badge bg-success">
                    Confirmada
                </span>`;
        }

        if (valor === "Revocada") {
            return `
                <span class="badge bg-secondary">
                    Revocada
                </span>`;
        }

        return `
            <span class="badge bg-warning text-dark">
                Pendiente
            </span>`;
    }

    // =========================================================
    // INDICADORES
    // =========================================================

    function actualizarIndicadores() {
        const total = estado.registros.length;

        const confirmadas = estado.registros.filter(
            x => x.estado === "Confirmada"
        ).length;

        $("rcRevTotal").textContent = total;

        $("rcRevConfirmadas").textContent =
            confirmadas;

        $("rcRevPendientes").textContent =
            total - confirmadas;
    }

    // =========================================================
    // RENDERIZAR TABLA
    // =========================================================

    function renderizarTabla() {
        actualizarIndicadores();

        const tbody = $("rcRevTabla");

        if (!estado.registros.length) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="6"
                        class="text-center text-muted py-4">
                        No existen checadas para
                        el empleado y fecha seleccionados.
                    </td>
                </tr>`;

            return;
        }

        tbody.innerHTML = estado.registros.map(
            registro => {

                const confirmada =
                    registro.estado === "Confirmada";

                const tipo =
                    registro.tipoMovimiento ||
                    "Sin clasificar";

                const textoAccion =
                    confirmada
                        ? "Corregir"
                        : "Clasificar";

                const botonRevocar = confirmada
                    ? `
                        <button type="button"
                                class="btn btn-sm btn-outline-danger"
                                data-rc-accion="revocar"
                                data-rc-id="${escaparHtml(registro.id)}">

                            <i class="bi bi-arrow-counterclockwise me-1"></i>
                            Revocar

                        </button>`
                    : "";

                return `
                    <tr>

                        <td>
                            <code>
                                ${escaparHtml(registro.id)}
                            </code>
                        </td>

                        <td>
                            ${escaparHtml(
                    fechaHoraLocal(
                        registro.fechaHoraMexico
                    )
                )}
                        </td>

                        <td>
                            ${escaparHtml(
                    registro.origen || "BIOTIME"
                )}
                        </td>

                        <td>
                            ${estadoHtml(registro.estado)}
                        </td>

                        <td>
                            ${escaparHtml(tipo)}
                        </td>

                        <td class="text-nowrap">

                            <button type="button"
                                    class="btn btn-sm btn-outline-primary me-1"
                                    data-rc-accion="clasificar"
                                    data-rc-id="${escaparHtml(registro.id)}">

                                <i class="bi bi-pencil-square me-1"></i>
                                ${textoAccion}

                            </button>

                            ${botonRevocar}

                        </td>

                    </tr>`;
            }
        ).join("");
    }

    // =========================================================
    // CONSULTAR CHECADAS
    // =========================================================

    async function consultarChecadas({
        mensajeExito = true
    } = {}) {

        if (estado.cargando) return false;

        const empleadoId =
            Number($("rcRevEmpleado").value);

        const fecha =
            $("rcRevFecha").value;

        if (
            !Number.isInteger(empleadoId) ||
            empleadoId <= 0 ||
            !fecha
        ) {
            mostrarMensaje(
                "Selecciona un empleado y una fecha válida.",
                "warning"
            );

            return false;
        }

        estado.cargando = true;

        $("rcRevConsultar").disabled = true;

        const version = ++estado.consultaVersion;

        mostrarMensaje(
            "Consultando checadas biométricas..."
        );

        try {
            const url = new URL(
                endpoint("RevisionChecadasBioTime")
            );

            url.searchParams.set(
                "empleadoId",
                empleadoId
            );

            url.searchParams.set(
                "fecha",
                fecha
            );

            const respuesta = await fetch(
                url.toString(),
                {
                    method: "GET",
                    credentials: "same-origin",
                    headers: {
                        "Accept": "application/json"
                    }
                }
            );

            let json;

            try {
                json = await respuesta.json();
            } catch {
                throw new Error(
                    `Respuesta inválida del servidor (HTTP ${respuesta.status}).`
                );
            }

            if (!respuesta.ok || !json.success) {
                throw new Error(
                    json.message ||
                    `No fue posible consultar (HTTP ${respuesta.status}).`
                );
            }

            if (version !== estado.consultaVersion) {
                return false;
            }

            estado.registros =
                Array.isArray(json.registros)
                    ? json.registros
                    : [];

            estado.empleadoId = empleadoId;
            estado.fecha = fecha;

            renderizarTabla();

            if (mensajeExito) {
                mostrarMensaje(
                    `${estado.registros.length} checadas consultadas.`,
                    "success"
                );
            }

            return true;

        } catch (error) {
            estado.registros = [];
            estado.empleadoId = null;
            estado.fecha = null;

            renderizarTabla();

            mostrarMensaje(
                error.message ||
                "No se pudieron consultar las checadas.",
                "danger"
            );

            return false;

        } finally {
            estado.cargando = false;
            $("rcRevConsultar").disabled = false;
        }
    }

    // =========================================================
    // PREPARAR MODAL DE CONFIRMACIÓN O CORRECCIÓN
    // =========================================================

    function abrirClasificacion(id) {
        if (estado.guardando) return;

        const registro = obtenerRegistro(id);

        if (!registro) {
            mostrarMensaje(
                "No se encontró la checada seleccionada.",
                "warning"
            );
            return;
        }

        if (!estado.empleadoId) {
            mostrarMensaje(
                "Consulta nuevamente el empleado.",
                "warning"
            );
            return;
        }

        $("rcRevForm").reset();

        mostrarErrorModal(
            "rcRevModalMensaje"
        );

        $("rcRevChecadaId").value =
            registro.id;

        $("rcRevEmpleadoModal").value =
            estado.empleadoId;

        $("rcRevTipo").value =
            registro.estado === "Confirmada"
                ? registro.tipoMovimiento
                : "";

        $("rcRevDetalle").textContent =
            `Checada ${registro.id} · ` +
            fechaHoraLocal(
                registro.fechaHoraMexico
            );

        const esCorreccion =
            registro.estado === "Confirmada";

        $("rcRevModalTitulo").textContent =
            esCorreccion
                ? "Corregir clasificación biométrica"
                : "Confirmar movimiento biométrico";

        $("rcRevGuardar").textContent =
            esCorreccion
                ? "Guardar corrección"
                : "Confirmar movimiento";

        abrirModal("rcRevModal");
    }

    // =========================================================
    // PREPARAR MODAL DE REVOCACIÓN
    // =========================================================

    function abrirRevocacion(id) {
        if (estado.guardando) return;

        const registro = obtenerRegistro(id);

        if (
            !registro ||
            registro.estado !== "Confirmada"
        ) {
            mostrarMensaje(
                "Solo pueden revocarse clasificaciones confirmadas.",
                "warning"
            );
            return;
        }

        if (!estado.empleadoId) {
            mostrarMensaje(
                "Consulta nuevamente el empleado.",
                "warning"
            );
            return;
        }

        $("rcRevFormRevocar").reset();

        mostrarErrorModal(
            "rcRevRevocarMensaje"
        );

        $("rcRevRevocarChecadaId").value =
            registro.id;

        $("rcRevRevocarEmpleadoId").value =
            estado.empleadoId;

        $("rcRevRevocarEmpleado").textContent =
            estado.empleadoId;

        $("rcRevRevocarIdentificador").textContent =
            registro.id;

        $("rcRevRevocarFecha").textContent =
            fechaHoraLocal(
                registro.fechaHoraMexico
            );

        $("rcRevRevocarTipo").textContent =
            registro.tipoMovimiento ||
            "Sin clasificar";

        $("rcRevRevocarMotivo").value = "";

        $("rcRevRevocarConfirmacion").checked =
            false;

        abrirModal("rcRevModalRevocar");
    }

    // =========================================================
    // ENVÍO AJAX SEGURO
    // =========================================================

    async function enviarPost(
        handler,
        datos
    ) {
        const token =
            tokenAntiforgery();

        if (!token) {
            throw new Error(
                "No se encontró el token de seguridad. Actualiza la página."
            );
        }

        const cuerpo =
            new URLSearchParams();

        for (
            const [clave, valor]
            of Object.entries(datos)
        ) {
            cuerpo.append(
                clave,
                String(valor ?? "")
            );
        }

        const respuesta = await fetch(
            endpoint(handler),
            {
                method: "POST",
                credentials: "same-origin",
                headers: {
                    "Content-Type":
                        "application/x-www-form-urlencoded; charset=UTF-8",

                    "RequestVerificationToken":
                        token,

                    "Accept":
                        "application/json"
                },

                body: cuerpo.toString()
            }
        );

        let json;

        try {
            json = await respuesta.json();
        } catch {
            throw new Error(
                `El servidor devolvió HTTP ${respuesta.status} sin un JSON válido.`
            );
        }

        if (!respuesta.ok || !json.success) {
            throw new Error(
                json.message ||
                `La operación fue rechazada (HTTP ${respuesta.status}).`
            );
        }

        return json;
    }

    // =========================================================
    // GUARDAR CONFIRMACIÓN O CORRECCIÓN
    // =========================================================

    async function guardarClasificacion(event) {
        event.preventDefault();

        if (estado.guardando) return;

        const formulario =
            $("rcRevForm");

        if (!formulario.reportValidity()) {
            return;
        }

        const motivo =
            $("rcRevMotivo").value.trim();

        if (
            motivo.length < 10 ||
            motivo.length > 1000
        ) {
            mostrarErrorModal(
                "rcRevModalMensaje",
                "La justificación debe contener entre 10 y 1000 caracteres."
            );
            return;
        }

        estado.guardando = true;

        const boton =
            $("rcRevGuardar");

        boton.disabled = true;

        mostrarErrorModal(
            "rcRevModalMensaje"
        );

        try {
            const resultado = await enviarPost(
                "ConfirmarChecadaBioTime",
                {
                    checadaId:
                        $("rcRevChecadaId").value,

                    empleadoId:
                        $("rcRevEmpleadoModal").value,

                    tipoMovimiento:
                        $("rcRevTipo").value,

                    motivo
                }
            );

            cerrarModal("rcRevModal");

            const actualizada =
                await consultarChecadas({
                    mensajeExito: false
                });

            if (actualizada) {
                mostrarMensaje(
                    resultado.message ||
                    "Movimiento confirmado correctamente.",
                    "success"
                );
            }

        } catch (error) {
            mostrarErrorModal(
                "rcRevModalMensaje",
                error.message ||
                "No fue posible guardar la clasificación."
            );

        } finally {
            estado.guardando = false;
            boton.disabled = false;
        }
    }

    // =========================================================
    // GUARDAR REVOCACIÓN
    // =========================================================

    async function guardarRevocacion(event) {
        event.preventDefault();

        if (estado.guardando) return;

        const formulario =
            $("rcRevFormRevocar");

        if (!formulario.reportValidity()) {
            return;
        }

        const motivo =
            $("rcRevRevocarMotivo").value.trim();

        if (
            motivo.length < 10 ||
            motivo.length > 1000
        ) {
            mostrarErrorModal(
                "rcRevRevocarMensaje",
                "El motivo de revocación debe contener entre 10 y 1000 caracteres."
            );
            return;
        }

        if (
            !$("rcRevRevocarConfirmacion").checked
        ) {
            mostrarErrorModal(
                "rcRevRevocarMensaje",
                "Debes confirmar que deseas revocar la clasificación."
            );
            return;
        }

        estado.guardando = true;

        const boton =
            $("rcRevRevocarGuardar");

        boton.disabled = true;

        mostrarErrorModal(
            "rcRevRevocarMensaje"
        );

        try {
            const resultado = await enviarPost(
                "RevocarChecadaBioTime",
                {
                    checadaId:
                        $("rcRevRevocarChecadaId").value,

                    empleadoId:
                        $("rcRevRevocarEmpleadoId").value,

                    motivo
                }
            );

            cerrarModal(
                "rcRevModalRevocar"
            );

            const actualizada =
                await consultarChecadas({
                    mensajeExito: false
                });

            if (actualizada) {
                mostrarMensaje(
                    resultado.message ||
                    "La clasificación fue revocada correctamente.",
                    "success"
                );
            }

        } catch (error) {
            mostrarErrorModal(
                "rcRevRevocarMensaje",
                error.message ||
                "No fue posible revocar la clasificación."
            );

        } finally {
            estado.guardando = false;
            boton.disabled = false;
        }
    }

    // =========================================================
    // EVENTOS
    // =========================================================

    function registrarEventos() {
        const panel =
            $("rcRevisionPanel");

        if (!panel) return;

        $("rcRevConsultar").addEventListener(
            "click",
            () => consultarChecadas()
        );

        // Invalidar los resultados de otra consulta
        // cuando se cambian los filtros.
        for (const id of [
            "rcRevEmpleado",
            "rcRevFecha"
        ]) {
            $(id).addEventListener(
                "change",
                () => {
                    estado.consultaVersion++;
                    estado.registros = [];
                    estado.empleadoId = null;
                    estado.fecha = null;
                    renderizarTabla();

                    mostrarMensaje(
                        "Los filtros cambiaron. Consulta nuevamente las checadas.",
                        "info"
                    );
                }
            );
        }

        // Delegación de eventos en la tabla.
        $("rcRevTabla").addEventListener(
            "click",
            event => {

                const boton =
                    event.target.closest(
                        "button[data-rc-accion]"
                    );

                if (!boton) return;

                const id =
                    boton.dataset.rcId;

                switch (
                boton.dataset.rcAccion
                ) {
                    case "clasificar":
                        abrirClasificacion(id);
                        break;

                    case "revocar":
                        abrirRevocacion(id);
                        break;
                }
            }
        );

        $("rcRevForm").addEventListener(
            "submit",
            guardarClasificacion
        );

        $("rcRevFormRevocar").addEventListener(
            "submit",
            guardarRevocacion
        );

        // Fecha inicial de la pantalla:
        // día actual de Ciudad de México.
        $("rcRevFecha").value =
            fechaHoyMexico();

        renderizarTabla();
    }

    // =========================================================
    // INICIALIZACIÓN
    // =========================================================

    if (
        document.readyState === "loading"
    ) {
        document.addEventListener(
            "DOMContentLoaded",
            registrarEventos
        );
    } else {
        registrarEventos();
    }

})();
