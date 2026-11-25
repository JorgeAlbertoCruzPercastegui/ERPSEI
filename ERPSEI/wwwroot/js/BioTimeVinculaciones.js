
(() => {
    "use strict";

    const $ = id => document.getElementById(id);

    const estado = {
        registros: [],
        filtrados: [],
        pagina: 1,
        tamano: 10,
        cargado: false,
        cargando: false
    };

    const token = () =>
        document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value || "";

    function urlHandler(nombre) {
        const url = new URL(window.location.href);
        url.searchParams.set("handler", nombre);
        return url.toString();
    }

    function mostrarMensaje(mensaje, tipo = "info") {
        const el = $("vbMensaje");
        if (!el) return;

        el.className = `alert alert-${tipo} small`;
        el.textContent = mensaje;
    }

    function texto(valor) {
        return String(valor ?? "");
    }

    function escapeHtml(valor) {
        return texto(valor)
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#39;");
    }

    function fechaCorta(valor) {
        if (!valor) return "Sin límite";
        return texto(valor).substring(0, 10);
    }

    function formatoVigencia(item) {
        return `${fechaCorta(item.fechaInicio)} — ` +
            `${fechaCorta(item.fechaFin)}`;
    }

    function estadoHtml(item) {
        if (item.estado === "Pendiente") {
            return '<span class="badge bg-warning text-dark">Pendiente</span>';
        }

        if (item.estado === "Revocada") {
            return '<span class="badge bg-danger">Revocada</span>';
        }

        if (item.estado === "Validada") {
            const hoy = fechaHoyMexico();
            const vencida = item.fechaFin &&
                fechaCorta(item.fechaFin) < hoy;

            return vencida
                ? '<span class="badge bg-secondary">Vigencia cerrada</span>'
                : '<span class="badge bg-success">Validada</span>';
        }

        return escapeHtml(item.estado);
    }

    function fechaHoyMexico() {
        // Formato YYYY-MM-DD sin depender del UTC
        // ni de la zona horaria del navegador.
        const partes = new Intl.DateTimeFormat("en-CA", {
            timeZone: "America/Mexico_City",
            year: "numeric",
            month: "2-digit",
            day: "2-digit"
        }).formatToParts(new Date());

        const valor = tipo =>
            partes.find(x => x.type === tipo)?.value || "";

        return `${valor("year")}-${valor("month")}-${valor("day")}`;
    }

    function actualizarIndicadores() {
        const items = estado.registros;

        $("vbTotal").textContent = items.length;
        $("vbPendientes").textContent =
            items.filter(x => x.estado === "Pendiente").length;
        $("vbValidadas").textContent =
            items.filter(x => x.estado === "Validada").length;
        $("vbSinHorario").textContent =
            items.filter(x => !x.horarioId).length;
    }

    function aplicarFiltros(reiniciar = true) {
        if (reiniciar) estado.pagina = 1;

        const buscar = texto($("vbBuscar").value)
            .trim().toLocaleLowerCase();

        const filtroEstado = $("vbEstado").value;
        const filtroHorario = $("vbHorario").value;

        estado.tamano = Number($("vbTamano").value) || 10;

        estado.filtrados = estado.registros.filter(x => {
            if (filtroEstado && x.estado !== filtroEstado)
                return false;

            if (filtroHorario === "con" && !x.horarioId)
                return false;

            if (filtroHorario === "sin" && x.horarioId)
                return false;

            if (!buscar) return true;

            const cadena = [
                x.nombreIntranet,
                x.nombreBioTime,
                x.empCode,
                x.departamentoBioTime,
                x.horario
            ].join(" ").toLocaleLowerCase();

            return cadena.includes(buscar);
        });

        renderizar();
    }

    function renderizar() {
        const total = estado.filtrados.length;
        const paginas = Math.max(
            1,
            Math.ceil(total / estado.tamano)
        );

        estado.pagina = Math.max(
            1,
            Math.min(estado.pagina, paginas)
        );

        const inicio =
            (estado.pagina - 1) * estado.tamano;

        const visibles = estado.filtrados.slice(
            inicio,
            inicio + estado.tamano
        );

        const tbody = $("vbTabla");

        if (!visibles.length) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="7"
                        class="text-center text-muted py-4">
                        No se encontraron vinculaciones.
                    </td>
                </tr>`;
        } else {
            tbody.innerHTML = visibles.map(x => {
                const horario = x.horarioId
                    ? escapeHtml(x.horario)
                    : '<span class="text-warning">Sin horario</span>';

                const puedeCerrar =
                    x.estado === "Validada" &&
                    (!x.fechaFin ||
                        fechaCorta(x.fechaFin) >= fechaHoyMexico());

                let acciones = "";

                if (x.estado === "Pendiente") {
                    acciones += `
                        <button type="button"
                                class="btn btn-sm btn-primary me-1"
                                data-vb-accion="validar"
                                data-vb-id="${x.id}">
                            Validar
                        </button>`;
                }

                if (puedeCerrar) {
                    acciones += `
                        <button type="button"
                                class="btn btn-sm btn-outline-secondary me-1"
                                data-vb-accion="cerrar"
                                data-vb-id="${x.id}">
                            Cerrar
                        </button>`;
                }

                if (x.estado !== "Revocada") {
                    acciones += `
                        <button type="button"
                                class="btn btn-sm btn-outline-danger"
                                data-vb-accion="revocar"
                                data-vb-id="${x.id}">
                            Revocar
                        </button>`;
                }

                if (!acciones) acciones = "—";

                return `
                    <tr>
                        <td>
                            <strong>${escapeHtml(x.nombreIntranet)}</strong>
                            <div class="text-muted small">
                                ID ${x.empleadoId}
                            </div>
                        </td>
                        <td>
                            ${escapeHtml(x.nombreBioTime)}
                            <div class="text-muted small">
                                ${escapeHtml(x.departamentoBioTime)}
                            </div>
                        </td>
                        <td>
                            <code>${escapeHtml(x.empCode)}</code>
                        </td>
                        <td>${horario}</td>
                        <td>${estadoHtml(x)}</td>
                        <td class="small">
                            ${escapeHtml(formatoVigencia(x))}
                        </td>
                        <td class="text-nowrap">${acciones}</td>
                    </tr>`;
            }).join("");
        }

        $("vbInfo").textContent =
            `${total} registros filtrados`;

        $("vbPagina").textContent =
            `${estado.pagina} / ${paginas}`;

        $("vbAnterior").disabled =
            estado.pagina <= 1;

        $("vbSiguiente").disabled =
            estado.pagina >= paginas;
    }

    async function cargar() {
        if (estado.cargando) return;

        estado.cargando = true;
        mostrarMensaje("Consultando vinculaciones...");

        try {
            const respuesta = await fetch(
                urlHandler("VinculacionesBioTime"),
                {
                    credentials: "same-origin",
                    headers: { "Accept": "application/json" }
                }
            );

            if (!respuesta.ok)
                throw new Error(
                    `Error HTTP ${respuesta.status}`
                );

            const json = await respuesta.json();

            if (!json.success)
                throw new Error(
                    json.message || "Error en la consulta."
                );

            estado.registros = Array.isArray(json.registros)
                ? json.registros : [];

            estado.cargado = true;

            actualizarIndicadores();
            aplicarFiltros();

            mostrarMensaje(
                `${estado.registros.length} vinculaciones consultadas.`,
                "success"
            );
        } catch (error) {
            mostrarMensaje(
                error.message || "No fue posible consultar.",
                "danger"
            );
        } finally {
            estado.cargando = false;
        }
    }

    function abrirModal(idModal) {
        const elemento = $(idModal);
        if (!elemento || !window.bootstrap?.Modal)
            throw new Error(
                "Bootstrap Modal no está disponible."
            );

        window.bootstrap.Modal
            .getOrCreateInstance(elemento)
            .show();
    }

    function cerrarModal(idModal) {
        const elemento = $(idModal);
        if (!elemento) return;

        window.bootstrap?.Modal
            ?.getInstance(elemento)
            ?.hide();
    }

    function obtenerRegistro(id) {
        return estado.registros.find(
            x => Number(x.id) === Number(id)
        );
    }

    function abrirValidacion(item) {
        $("vbFormValidar").reset();
        $("vbValidarId").value = item.id;

        $("vbFechaInicio").value =
            fechaCorta(item.fechaInicio);

        $("vbFechaFin").value = item.fechaFin
            ? fechaCorta(item.fechaFin)
            : "";

        $("vbIdentidad").innerHTML = `
            <div class="mb-2">
                <strong>INTRANET:</strong>
                ${escapeHtml(item.nombreIntranet)}
            </div>
            <div class="mb-2">
                <strong>BioTime:</strong>
                ${escapeHtml(item.nombreBioTime)}
            </div>
            <div>
                <strong>Código:</strong>
                ${escapeHtml(item.empCode)}
            </div>`;

        abrirModal("vbModalValidar");
    }

    function abrirCierre(item) {
        $("vbFormCerrar").reset();
        $("vbCerrarId").value = item.id;

        $("vbCerrarFecha").value =
            item.fechaFin
                ? fechaCorta(item.fechaFin)
                : "";

        abrirModal("vbModalCerrar");
    }

    async function post(nombreHandler, datos) {
        const body = new URLSearchParams();

        for (const [clave, valor] of Object.entries(datos)) {
            body.append(clave, String(valor ?? ""));
        }

        const respuesta = await fetch(
            urlHandler(nombreHandler),
            {
                method: "POST",
                credentials: "same-origin",
                headers: {
                    "Content-Type":
                        "application/x-www-form-urlencoded; charset=UTF-8",
                    "RequestVerificationToken": token(),
                    "Accept": "application/json"
                },
                body: body.toString()
            }
        );

        let json;

        try {
            json = await respuesta.json();
        } catch {
            throw new Error(
                `No se pudo procesar la respuesta HTTP ${respuesta.status}.`
            );
        }

        if (!respuesta.ok || !json.success) {
            throw new Error(
                json.message ||
                `La operación falló (HTTP ${respuesta.status}).`
            );
        }

        return json;
    }

    async function ejecutarFormulario(
        boton,
        accion,
        modalId
    ) {
        boton.disabled = true;

        try {
            const respuesta = await accion();

            cerrarModal(modalId);

            await cargar();

            mostrarMensaje(
                respuesta.message || "Operación completada.",
                "success"
            );
        } catch (error) {
            mostrarMensaje(
                error.message || "No se pudo completar.",
                "danger"
            );
        } finally {
            boton.disabled = false;
        }
    }

    async function revocar(item) {
        const pregunta =
            `¿Deseas revocar la vinculación ${item.empCode} ` +
            `de ${item.nombreIntranet}?\n\n` +
            "Esta acción también excluirá sus checadas históricas " +
            "del proveedor consolidado.";

        if (!window.confirm(pregunta)) return;

        try {
            const respuesta = await post(
                "RevocarVinculacionBioTime",
                {
                    id: item.id,
                    confirmarRevocacion: true
                }
            );

            await cargar();

            mostrarMensaje(
                respuesta.message,
                "success"
            );
        } catch (error) {
            mostrarMensaje(error.message, "danger");
        }
    }

    function iniciarEventos() {
        $("vbActualizar").addEventListener("click", cargar);

        for (const id of [
            "vbBuscar",
            "vbEstado",
            "vbHorario",
            "vbTamano"
        ]) {
            $(id).addEventListener(
                id === "vbBuscar" ? "input" : "change",
                () => aplicarFiltros()
            );
        }

        $("vbAnterior").addEventListener("click", () => {
            estado.pagina--;
            renderizar();
        });

        $("vbSiguiente").addEventListener("click", () => {
            estado.pagina++;
            renderizar();
        });

        $("vbTabla").addEventListener("click", event => {
            const boton = event.target.closest(
                "button[data-vb-accion]"
            );

            if (!boton) return;

            const item = obtenerRegistro(
                boton.dataset.vbId
            );

            if (!item) return;

            switch (boton.dataset.vbAccion) {
                case "validar":
                    abrirValidacion(item);
                    break;
                case "cerrar":
                    abrirCierre(item);
                    break;
                case "revocar":
                    revocar(item);
                    break;
            }
        });

        $("vbFormValidar").addEventListener(
            "submit",
            async event => {
                event.preventDefault();

                if (!$("vbConfirmar").checked) {
                    mostrarMensaje(
                        "Debes confirmar la identidad.",
                        "warning"
                    );
                    return;
                }

                await ejecutarFormulario(
                    $("vbGuardar"),
                    () => post(
                        "ValidarVinculacionBioTime",
                        {
                            id: $("vbValidarId").value,
                            fechaInicio: $("vbFechaInicio").value,
                            fechaFin: $("vbFechaFin").value,
                            confirmarIdentidad: true
                        }
                    ),
                    "vbModalValidar"
                );
            }
        );

        $("vbFormCerrar").addEventListener(
            "submit",
            async event => {
                event.preventDefault();

                const boton = event.target
                    .querySelector('button[type="submit"]');

                await ejecutarFormulario(
                    boton,
                    () => post(
                        "CerrarVinculacionBioTime",
                        {
                            id: $("vbCerrarId").value,
                            fechaFin: $("vbCerrarFecha").value
                        }
                    ),
                    "vbModalCerrar"
                );
            }
        );

        // La página ya controla la navegación por tabs.
        // Cargamos únicamente cuando se selecciona
        // Administración del Reloj.
        const tabAdministracion = document.querySelector(
            '[data-bio-tab="administracion"]'
        );

        tabAdministracion?.addEventListener(
            "click",
            () => {
                if (!estado.cargado)
                    cargar();
            }
        );

        if (!$("bioPaneAdministracion")?.hidden) {
            cargar();
        }
    }

    document.addEventListener(
        "DOMContentLoaded",
        () => {
            if ($("vbPanel"))
                iniciarEventos();
        }
    );
})();
