
(() => {
    "use strict";

    const $ = id => document.getElementById(id);
    const baseUrl = window.location.pathname;

    let cargado = false;
    let ocupado = false;

    function informar(mensaje, tipo = "info") {
        const elemento = $("rcHoMensaje");
        if (!elemento) return;

        elemento.className = `alert alert-${tipo}`;
        elemento.textContent = mensaje;
    }

    function token() {
        return document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;
    }

    async function solicitar(url, opciones = {}) {
        const respuesta = await fetch(url, {
            credentials: "same-origin",
            cache: "no-store",
            ...opciones
        });

        const resultado = await respuesta.json();

        if (!respuesta.ok || resultado.success === false) {
            throw new Error(
                resultado.message ||
                "No fue posible completar la solicitud."
            );
        }

        return resultado;
    }

    function celda(fila, contenido) {
        const td = document.createElement("td");
        td.textContent = contenido ?? "—";
        fila.appendChild(td);
        return td;
    }

    function fecha(valor) {
        return valor
            ? String(valor).substring(0, 10)
            : "—";
    }

    async function buscarUsuarios() {
        const buscar = $("rcHoBuscar").value.trim();

        if (buscar.length < 2) {
            informar(
                "Escribe al menos dos caracteres para buscar.",
                "warning"
            );
            return;
        }

        $("rcHoBtnBuscar").disabled = true;

        try {
            const parametros = new URLSearchParams({
                handler: "UsuariosReloj",
                buscar
            });

            const data = await solicitar(
                `${baseUrl}?${parametros}`
            );

            const select = $("rcHoUsuario");
            select.replaceChildren();

            const inicial = document.createElement("option");
            inicial.value = "";
            inicial.textContent = "Selecciona un colaborador";
            select.appendChild(inicial);

            for (const usuario of data.usuarios || []) {
                const option = document.createElement("option");
                option.value = usuario.id;
                option.textContent =
                    usuario.usuario || usuario.correo || usuario.id;

                if (!usuario.empleadoId) {
                    option.disabled = true;
                    option.textContent += " (sin empleado vinculado)";
                }

                select.appendChild(option);
            }

            informar(
                data.usuarios?.length
                    ? "Selecciona el colaborador correspondiente."
                    : "No se encontraron usuarios.",
                "info"
            );
        } catch (error) {
            informar(error.message, "danger");
        } finally {
            $("rcHoBtnBuscar").disabled = false;
        }
    }

    function pintarAutorizaciones(registros) {
        const tbody = $("rcHoTabla");
        tbody.replaceChildren();

        if (!registros.length) {
            const fila = document.createElement("tr");
            const td = celda(
                fila,
                "Todavía no existen autorizaciones."
            );
            td.colSpan = 4;
            td.className = "text-center text-muted py-4";
            tbody.appendChild(fila);
            return;
        }

        for (const registro of registros) {
            const fila = document.createElement("tr");

            celda(fila, registro.usuario);

            const rango = celda(
                fila,
                `${fecha(registro.fechaInicio)} al ${fecha(registro.fechaFin)}`
            );
            rango.className = "text-nowrap";

            const estadoTd = document.createElement("td");
            const estado = document.createElement("span");

            estado.className =
                registro.estado === "Aprobada"
                    ? "badge bg-success"
                    : registro.estado === "Revocada"
                        ? "badge bg-danger"
                        : "badge bg-secondary";

            estado.textContent = registro.estado;
            estadoTd.appendChild(estado);
            fila.appendChild(estadoTd);

            const accionTd = document.createElement("td");

            if (registro.estado === "Aprobada") {
                const boton = document.createElement("button");
                boton.type = "button";
                boton.className = "btn btn-sm btn-outline-danger";
                boton.textContent = "Revocar";

                boton.addEventListener("click", () =>
                    revocar(registro.id)
                );

                accionTd.appendChild(boton);
            } else {
                accionTd.textContent = "—";
            }

            fila.appendChild(accionTd);
            tbody.appendChild(fila);
        }
    }

    async function cargarAutorizaciones() {
        const data = await solicitar(
            `${baseUrl}?handler=ModalidadesReloj`
        );

        pintarAutorizaciones(data.autorizaciones || []);
        cargado = true;
    }

    async function guardar(evento) {
        evento.preventDefault();
        if (ocupado) return;

        const formulario = $("rcHoForm");

        if (!formulario.reportValidity()) return;

        const userId = $("rcHoUsuario").value;
        const inicio = $("rcHoInicio").value;
        const fin = $("rcHoFin").value;

        if (!userId || fin < inicio) {
            informar(
                "Selecciona un colaborador y un rango válido.",
                "warning"
            );
            return;
        }

        if (!token()) {
            informar(
                "No se encontró el token de seguridad.",
                "danger"
            );
            return;
        }

        const datos = new FormData();
        datos.append("UserId", userId);
        datos.append("FechaInicio", inicio);
        datos.append("FechaFin", fin);
        datos.append("Motivo", $("rcHoMotivo").value.trim());

        ocupado = true;
        $("rcHoGuardar").disabled = true;

        try {
            const respuesta = await solicitar(
                `${baseUrl}?handler=AutorizarRemoto`,
                {
                    method: "POST",
                    headers: {
                        "RequestVerificationToken": token()
                    },
                    body: datos
                }
            );

            await cargarAutorizaciones();
            formulario.reset();
            informar(respuesta.message, "success");
        } catch (error) {
            informar(error.message, "danger");
        } finally {
            ocupado = false;
            $("rcHoGuardar").disabled = false;
        }
    }

    async function revocar(id) {
        if (ocupado) return;

        if (!window.confirm(
            "¿Confirmas que deseas revocar esta autorización de Home Office?"
        )) return;

        ocupado = true;

        try {
            const datos = new FormData();
            datos.append("autorizacionId", String(id));

            const respuesta = await solicitar(
                `${baseUrl}?handler=RevocarRemoto`,
                {
                    method: "POST",
                    headers: {
                        "RequestVerificationToken": token()
                    },
                    body: datos
                }
            );

            await cargarAutorizaciones();
            informar(respuesta.message, "success");
        } catch (error) {
            informar(error.message, "danger");
        } finally {
            ocupado = false;
        }
    }

    function iniciar() {
        const panel = $("rcModalidadesPanel");
        if (!panel) return;

        $("rcHoBtnBuscar").addEventListener(
            "click",
            buscarUsuarios
        );

        $("rcHoBuscar").addEventListener(
            "keydown",
            evento => {
                if (evento.key === "Enter") {
                    evento.preventDefault();
                    buscarUsuarios();
                }
            }
        );

        $("rcHoForm").addEventListener("submit", guardar);

        const tab = document.querySelector(
            '[data-bio-tab="administracion"]'
        );

        tab?.addEventListener("click", async () => {
            if (cargado) return;

            try {
                await cargarAutorizaciones();
            } catch (error) {
                informar(error.message, "danger");
            }
        });
    }

    document.addEventListener(
        "DOMContentLoaded",
        iniciar
    );
})();