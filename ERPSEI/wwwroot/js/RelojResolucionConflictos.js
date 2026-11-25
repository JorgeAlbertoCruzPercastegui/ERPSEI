
(() => {
    "use strict";

    const $ = id => document.getElementById(id);

    const estado = {
        empleadoId: null,
        fecha: null,
        grupos: [],
        movimientos: [],
        cargando: false,
        guardando: false
    };

    function escapar(valor) {
        return String(valor ?? "")
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#39;");
    }

    function endpoint(handler) {
        const url = new URL(window.location.href);
        url.searchParams.set("handler", handler);
        return url;
    }

    function hoyMexico() {
        const partes = new Intl.DateTimeFormat("en-US", {
            timeZone: "America/Mexico_City",
            year: "numeric",
            month: "2-digit",
            day: "2-digit"
        }).formatToParts(new Date());

        const obtener = tipo =>
            partes.find(x => x.type === tipo)?.value || "";

        return `${obtener("year")}-${obtener("month")}-${obtener("day")}`;
    }

    function mensaje(texto, tipo = "info") {
        const elemento = $("rcResMensaje");
        elemento.className = `alert alert-${tipo} small`;
        elemento.textContent = texto;
    }

    function errorModal(texto = "") {
        const el = $("rcResModalError");
        el.textContent = texto;
        el.classList.toggle("d-none", !texto);
    }

    function encontrarMovimiento(id) {
        return estado.movimientos.find(x =>
            x.idMovimiento === id);
    }

    function formatoMovimiento(id) {
        const m = encontrarMovimiento(id);

        if (!m) return id;

        const hora = String(
            m.fechaHoraMexico || ""
        ).replace("T", " ").slice(0, 19);

        return `${id} · ${hora} · ${m.origen || m.fuente}`;
    }

    async function recibirJson(respuesta) {
        const texto = await respuesta.text();

        let json;

        try {
            json = JSON.parse(texto);
        } catch {
            throw new Error(
                `El servidor devolvió HTML o una respuesta inválida ` +
                `(HTTP ${respuesta.status}). Revisa Network.`
            );
        }

        if (!respuesta.ok || !json.success) {
            throw new Error(
                json.message ||
                `Operación rechazada (HTTP ${respuesta.status}).`
            );
        }

        return json;
    }

    function dibujar() {
        const contenedor = $("rcResGrupos");

        if (!estado.grupos.length) {
            contenedor.innerHTML = `
                <div class="alert alert-light small mb-0">
                    No existen duplicidades confirmadas ni
                    resoluciones previas para el empleado y fecha.
                    Las checadas sin clasificar deben revisarse
                    primero en Revisión biométrica.
                </div>`;

            return;
        }

        contenedor.innerHTML = estado.grupos.map(
            (grupo, indice) => {

                const puedeResolver =
                    grupo.candidatos.length >= 2;

                const activa =
                    grupo.estadoResolucion === "Aplicada" ||
                    grupo.estadoResolucion === "Desactualizada";

                const opciones = grupo.candidatos.map(id => `
                    <li class="rc-res-candidato">
                        <code>${escapar(id)}</code>
                        <span>${escapar(formatoMovimiento(id))}</span>
                        ${id === grupo.seleccionado
                        ? '<span class="badge text-bg-info">Seleccionado</span>'
                        : ""}
                    </li>
                `).join("");

                return `
                    <div class="rc-res-grupo mb-3">

                        <div class="d-flex justify-content-between
                                    align-items-center flex-wrap gap-2 mb-2">
                            <strong>
                                ${escapar(grupo.tipoMovimiento)}
                            </strong>

                            <span class="badge text-bg-secondary">
                                ${escapar(grupo.estadoResolucion)}
                            </span>
                        </div>

                        <div class="small text-muted mb-2">
                            ${grupo.candidatos.length}
                            marcas confirmadas
                        </div>

                        <ul class="rc-res-lista">
                            ${opciones ||
                    '<li class="small text-muted">Sin candidatos confirmados.</li>'}
                        </ul>

                        <div class="d-flex gap-2 flex-wrap">
                            <button type="button"
                                    class="btn btn-sm btn-outline-primary"
                                    data-res-accion="resolver"
                                    data-res-indice="${indice}"
                                    ${puedeResolver ? "" : "disabled"}>
                                <i class="bi bi-check2-square me-1"></i>
                                ${activa ? "Cambiar decisión" : "Resolver"}
                            </button>

                            ${activa ? `
                                <button type="button"
                                        class="btn btn-sm btn-outline-danger"
                                        data-res-accion="revocar"
                                        data-res-indice="${indice}">
                                    <i class="bi bi-arrow-counterclockwise me-1"></i>
                                    Revocar resolución
                                </button>` : ""}
                        </div>

                    </div>
                `;
            }
        ).join("");
    }

    async function consultar() {
        if (estado.cargando) return;

        const empleadoId = Number(
            $("rcResEmpleado").value);

        const fecha = $("rcResFecha").value;

        if (!Number.isInteger(empleadoId) ||
            empleadoId <= 0 ||
            !/^\d{4}-\d{2}-\d{2}$/.test(fecha)) {
            mensaje("Indica un empleado y fecha válidos.", "warning");
            return;
        }

        estado.cargando = true;
        $("rcResConsultar").disabled = true;
        mensaje("Consultando conflictos y resoluciones...");

        try {
            const url = endpoint("ResolucionesConflictos");
            url.searchParams.set("empleadoId", empleadoId);
            url.searchParams.set("fecha", fecha);

            const respuesta = await fetch(url, {
                credentials: "same-origin",
                headers: { "Accept": "application/json" }
            });

            const json = await recibirJson(respuesta);

            estado.empleadoId = empleadoId;
            estado.fecha = fecha;
            estado.grupos = json.grupos || [];
            estado.movimientos = json.movimientos || [];

            dibujar();

            mensaje(
                `${estado.grupos.length} grupos de conciliación consultados.`,
                "success"
            );
        } catch (error) {
            estado.grupos = [];
            estado.movimientos = [];
            estado.empleadoId = null;
            estado.fecha = null;

            dibujar();
            mensaje(error.message, "danger");
        } finally {
            estado.cargando = false;
            $("rcResConsultar").disabled = false;
        }
    }

    function abrirModal(indice, accion) {
        const grupo = estado.grupos[indice];
        if (!grupo || estado.guardando) return;

        $("rcResForm").reset();

        $("rcResTipo").value = grupo.tipoMovimiento;
        $("rcResFirma").value = grupo.firma;
        $("rcResAccion").value = accion;

        errorModal();

        const revocar = accion === "revocar";

        $("rcResModalTitulo").textContent = revocar
            ? "Revocar resolución administrativa"
            : "Resolver conflicto de asistencia";

        $("rcResDescripcion").textContent =
            `Movimiento: ${grupo.tipoMovimiento}. ` +
            (revocar
                ? "La decisión dejará de aplicarse."
                : "Selecciona una de las marcas confirmadas.");

        $("rcResSeleccionContenedor").hidden = revocar;
        $("rcResSeleccion").required = !revocar;

        $("rcResSeleccion").innerHTML =
            grupo.candidatos.map(id => `
                <option value="${escapar(id)}">
                    ${escapar(formatoMovimiento(id))}
                </option>
            `).join("");

        if (grupo.seleccionado &&
            grupo.candidatos.includes(grupo.seleccionado)) {
            $("rcResSeleccion").value = grupo.seleccionado;
        }

        $("rcResGuardar").className = revocar
            ? "btn btn-danger"
            : "btn btn-primary";

        $("rcResGuardar").textContent = revocar
            ? "Confirmar revocación"
            : "Guardar decisión";

        bootstrap.Modal
            .getOrCreateInstance($("rcResModal"))
            .show();
    }

    async function guardar(event) {
        event.preventDefault();

        if (estado.guardando) return;

        if (!$("rcResForm").reportValidity())
            return;

        const motivo = $("rcResMotivo").value.trim();

        if (motivo.length < 10 || motivo.length > 1000) {
            errorModal(
                "La justificación debe contener entre 10 y 1000 caracteres."
            );
            return;
        }

        const token = document.querySelector(
            'input[name="__RequestVerificationToken"]'
        )?.value;

        if (!token) {
            errorModal(
                "No se encontró el token antifalsificación."
            );
            return;
        }

        const revocar =
            $("rcResAccion").value === "revocar";

        const handler = revocar
            ? "RevocarResolucionConflicto"
            : "ResolverConflicto";

        const datos = new URLSearchParams({
            empleadoId: estado.empleadoId,
            fecha: estado.fecha,
            tipoMovimiento: $("rcResTipo").value,
            motivo
        });

        if (!revocar) {
            datos.set(
                "movimientoSeleccionado",
                $("rcResSeleccion").value
            );

            datos.set(
                "firmaEsperada",
                $("rcResFirma").value
            );
        }

        estado.guardando = true;
        $("rcResGuardar").disabled = true;
        errorModal();

        try {
            const respuesta = await fetch(endpoint(handler), {
                method: "POST",
                credentials: "same-origin",
                headers: {
                    "Content-Type":
                        "application/x-www-form-urlencoded; charset=UTF-8",
                    "RequestVerificationToken": token,
                    "Accept": "application/json"
                },
                body: datos.toString()
            });

            const json = await recibirJson(respuesta);

            bootstrap.Modal
                .getInstance($("rcResModal"))
                ?.hide();

            await consultar();

            mensaje(
                json.message ||
                "Resolución registrada correctamente.",
                "success"
            );

            // Actualiza las otras secciones si sus filtros
            // ya corresponden al empleado consultado.
            if (
                $("rcConEmpleado")?.value === String(estado.empleadoId) &&
                $("rcConFecha")?.value === estado.fecha
            ) {
                $("rcConConsultar")?.click();
            }

            $("rcAdConsultar")?.click();

        } catch (error) {
            errorModal(
                error.message || "No fue posible guardar la resolución."
            );
        } finally {
            estado.guardando = false;
            $("rcResGuardar").disabled = false;
        }
    }

    function iniciar() {
        if (!$("rcResPanel")) return;

        $("rcResFecha").value = hoyMexico();

        $("rcResConsultar").addEventListener(
            "click", consultar);

        $("rcResForm").addEventListener(
            "submit", guardar);

        $("rcResGrupos").addEventListener(
            "click",
            event => {
                const boton = event.target.closest(
                    "[data-res-accion]");

                if (!boton) return;

                abrirModal(
                    Number(boton.dataset.resIndice),
                    boton.dataset.resAccion
                );
            }
        );

        for (const id of ["rcResEmpleado", "rcResFecha"]) {
            $(id).addEventListener("change", () => {
                estado.grupos = [];
                estado.movimientos = [];
                estado.empleadoId = null;
                estado.fecha = null;

                dibujar();
                mensaje("Los filtros cambiaron. Consulta nuevamente.");
            });
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", iniciar);
    } else {
        iniciar();
    }
})();
