
(() => {
    "use strict";

    const $ = id => document.getElementById(id);
    const baseUrl = window.location.pathname;

    const TAMANO_PAGINA = 10;

    let oficinas = [];
    let paginaActual = 1;
    let cargadas = false;
    let cargando = false;
    let guardando = false;

    function mensaje(texto, tipo = "info") {
        const elemento = $("rcMensaje");
        if (!elemento) return;

        elemento.className = `alert alert-${tipo}`;
        elemento.textContent = texto;
    }

    function oficinaActual() {
        const id = Number($("rcOficina").value);
        return oficinas.find(x => x.id === id) || null;
    }

    function actualizarFormulario() {
        const oficina = oficinaActual();

        $("rcLatitud").value = oficina?.latitud ?? "";
        $("rcLongitud").value = oficina?.longitud ?? "";
        $("rcRadio").value = oficina?.radioMetros ?? 75;
        $("rcPrecision").value =
            oficina?.precisionMaximaMetros ?? 30;

        $("rcActiva").checked = oficina?.activa === true;

        // Cada activación requiere nueva confirmación.
        $("rcConfirmar").checked = false;

        if (!oficina) {
            mensaje("Selecciona una oficina.");
        } else if (oficina.configurada) {
            mensaje(
                "Puedes actualizar la configuración de esta oficina."
            );
        } else {
            mensaje(
                "Esta oficina todavía no tiene una geocerca configurada."
            );
        }
    }

    function crearCelda(fila, contenido) {
        const td = document.createElement("td");
        td.textContent = contenido ?? "—";
        fila.appendChild(td);
        return td;
    }

    function actualizarPaginacion() {
        const total = oficinas.length;

        const totalPaginas = Math.max(
            1,
            Math.ceil(total / TAMANO_PAGINA)
        );

        paginaActual = Math.min(
            Math.max(1, paginaActual),
            totalPaginas
        );

        const desde = total === 0
            ? 0
            : (paginaActual - 1) * TAMANO_PAGINA + 1;

        const hasta = Math.min(
            paginaActual * TAMANO_PAGINA,
            total
        );

        $("rcInfoPaginacion").textContent =
            `Mostrando ${desde}–${hasta} de ${total} oficinas`;

        $("rcNumeroPagina").textContent =
            `${paginaActual} / ${totalPaginas}`;

        $("rcPaginaAnterior").disabled =
            paginaActual <= 1;

        $("rcPaginaSiguiente").disabled =
            paginaActual >= totalPaginas;
    }

    function pintarTabla() {
        const tbody = $("rcTablaOficinas");
        tbody.replaceChildren();

        const inicio =
            (paginaActual - 1) * TAMANO_PAGINA;

        const oficinasPagina = oficinas.slice(
            inicio,
            inicio + TAMANO_PAGINA
        );

        if (oficinasPagina.length === 0) {
            const fila = document.createElement("tr");
            const td = crearCelda(
                fila,
                "No se encontraron oficinas."
            );

            td.colSpan = 4;
            td.className = "text-center text-muted py-4";

            tbody.appendChild(fila);
            actualizarPaginacion();
            return;
        }

        for (const oficina of oficinasPagina) {
            const tr = document.createElement("tr");

            crearCelda(tr, oficina.nombre);

            crearCelda(
                tr,
                oficina.configurada
                    ? `${oficina.radioMetros} m`
                    : "—"
            );

            const estadoTd = document.createElement("td");
            const badge = document.createElement("span");

            badge.className = oficina.activa
                ? "badge bg-success"
                : oficina.configurada
                    ? "badge bg-secondary"
                    : "badge bg-light text-dark border";

            badge.textContent = oficina.activa
                ? "Activa"
                : oficina.configurada
                    ? "Inactiva"
                    : "Sin configurar";

            estadoTd.appendChild(badge);
            tr.appendChild(estadoTd);

            const accionTd = document.createElement("td");
            const boton = document.createElement("button");

            boton.type = "button";
            boton.className =
                "btn btn-sm btn-outline-primary";
            boton.textContent = "Configurar";

            boton.addEventListener("click", () => {
                $("rcOficina").value = String(oficina.id);
                actualizarFormulario();

                $("rcGeocercaForm").scrollIntoView({
                    behavior: "smooth",
                    block: "start"
                });
            });

            accionTd.appendChild(boton);
            tr.appendChild(accionTd);

            tbody.appendChild(tr);
        }

        actualizarPaginacion();
    }

    function pintarCatalogo() {
        const select = $("rcOficina");
        const seleccionado = select.value;

        select.replaceChildren();

        const inicial = document.createElement("option");
        inicial.value = "";
        inicial.textContent = "Selecciona una oficina";

        select.appendChild(inicial);

        for (const oficina of oficinas) {
            const opcion = document.createElement("option");

            opcion.value = String(oficina.id);
            opcion.textContent = oficina.nombre;

            select.appendChild(opcion);
        }

        if (oficinas.some(
            x => String(x.id) === seleccionado
        )) {
            select.value = seleccionado;
        }

        actualizarFormulario();
    }

    async function cargarGeocercas() {
        if (cargando) return false;

        cargando = true;
        mensaje("Consultando oficinas...");

        try {
            const respuesta = await fetch(
                `${baseUrl}?handler=Geocercas`,
                {
                    method: "GET",
                    credentials: "same-origin",
                    cache: "no-store"
                }
            );

            if (!respuesta.ok) {
                throw new Error(
                    "No fue posible consultar las oficinas."
                );
            }

            const data = await respuesta.json();

            if (!data.success) {
                throw new Error(
                    data.message || "Error al consultar."
                );
            }

            oficinas = data.registros || [];

            $("rcTotalOficinas").textContent =
                data.total;

            $("rcConfiguradas").textContent =
                data.configuradas;

            $("rcActivas").textContent =
                data.activas;

            // Mantener la página actual al recargar datos.
            const totalPaginas = Math.max(
                1,
                Math.ceil(oficinas.length / TAMANO_PAGINA)
            );

            paginaActual = Math.min(
                paginaActual,
                totalPaginas
            );

            pintarCatalogo();
            pintarTabla();

            cargadas = true;

            if (!$("rcOficina").value) {
                mensaje(
                    "Selecciona una oficina para configurar su geocerca."
                );
            }

            return true;
        } catch (error) {
            mensaje(error.message, "danger");
            return false;
        } finally {
            cargando = false;
        }
    }

    function cambiarPagina(direccion) {
        const totalPaginas = Math.max(
            1,
            Math.ceil(oficinas.length / TAMANO_PAGINA)
        );

        const nuevaPagina = paginaActual + direccion;

        if (nuevaPagina < 1 ||
            nuevaPagina > totalPaginas) {
            return;
        }

        paginaActual = nuevaPagina;

        pintarTabla();

        // Mantener visible la tabla al navegar.
        $("rcTablaOficinas").closest(
            ".table-responsive"
        )?.scrollIntoView({
            behavior: "smooth",
            block: "nearest"
        });
    }

    async function guardarGeocerca(event) {
        event.preventDefault();

        if (guardando) return;

        const form = $("rcGeocercaForm");

        if (!form.reportValidity()) return;

        const oficinaId = Number(
            $("rcOficina").value
        );

        const activa = $("rcActiva").checked;

        if (!oficinaId) {
            mensaje(
                "Selecciona una oficina.",
                "warning"
            );
            return;
        }

        if (activa && !$("rcConfirmar").checked) {
            mensaje(
                "Confirma las coordenadas antes de activar la geocerca.",
                "warning"
            );
            return;
        }

        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;

        if (!token) {
            mensaje(
                "No se encontró el token de seguridad.",
                "danger"
            );
            return;
        }

        const datos = new FormData();

        datos.append("OficinaId", oficinaId);
        datos.append("Latitud", $("rcLatitud").value);
        datos.append("Longitud", $("rcLongitud").value);
        datos.append("RadioMetros", $("rcRadio").value);

        datos.append(
            "PrecisionMaximaMetros",
            $("rcPrecision").value
        );

        datos.append("Activa", String(activa));

        datos.append(
            "ConfirmarCoordenadas",
            String($("rcConfirmar").checked)
        );

        guardando = true;
        $("rcGuardar").disabled = true;

        mensaje("Guardando configuración...");

        try {
            const respuesta = await fetch(
                `${baseUrl}?handler=GuardarGeocerca`,
                {
                    method: "POST",
                    credentials: "same-origin",
                    headers: {
                        "RequestVerificationToken": token
                    },
                    body: datos
                }
            );

            const data = await respuesta.json();

            if (!respuesta.ok || !data.success) {
                throw new Error(
                    data.message ||
                    "No fue posible guardar la geocerca."
                );
            }

            const recargada = await cargarGeocercas();

            if (recargada) {
                mensaje(data.message, "success");
            } else {
                mensaje(
                    "La configuración se guardó, pero no fue posible actualizar la tabla.",
                    "warning"
                );
            }
        } catch (error) {
            mensaje(error.message, "danger");
        } finally {
            guardando = false;
            $("rcGuardar").disabled = false;
        }
    }

    function iniciar() {
        if (!$("rcGeocercaForm")) return;

        $("rcOficina").addEventListener(
            "change",
            actualizarFormulario
        );

        $("rcGeocercaForm").addEventListener(
            "submit",
            guardarGeocerca
        );

        $("rcPaginaAnterior").addEventListener(
            "click",
            () => cambiarPagina(-1)
        );

        $("rcPaginaSiguiente").addEventListener(
            "click",
            () => cambiarPagina(1)
        );

        const tab = document.querySelector(
            '[data-bio-tab="administracion"]'
        );

        if (tab) {
            tab.addEventListener("click", () => {
                if (!cargadas) {
                    cargarGeocercas();
                }
            });
        }
    }

    document.addEventListener(
        "DOMContentLoaded",
        iniciar
    );
})();
