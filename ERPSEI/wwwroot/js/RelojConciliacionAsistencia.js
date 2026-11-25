
(() => {
    "use strict";

    const $ = id => document.getElementById(id);

    const estado = {
        empleadoId: null,
        fecha: null,
        cargando: false,
        version: 0
    };

    function escapar(valor) {
        return String(valor ?? "")
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#39;");
    }

    function fechaHoyMexico() {
        const partes = new Intl.DateTimeFormat(
            "en-US", {
            timeZone: "America/Mexico_City",
            year: "numeric",
            month: "2-digit",
            day: "2-digit"
        }
        ).formatToParts(new Date());

        const valor = tipo =>
            partes.find(x => x.type === tipo)?.value || "";

        return `${valor("year")}-${valor("month")}-${valor("day")}`;
    }

    function mensaje(texto, tipo = "info") {
        const el = $("rcConMensaje");
        el.className = `alert alert-${tipo} small`;
        el.textContent = texto;
    }

    function etiquetaEstado(valor) {
        const clases = {
            Confirmada: "text-bg-success",
            Pendiente: "text-bg-warning",
            Revocada: "text-bg-secondary",
            NoValidada: "text-bg-danger"
        };

        return `
            <span class="badge ${clases[valor] || "text-bg-secondary"}">
                ${escapar(valor)}
            </span>`;
    }

    function limpiar() {
        for (const id of [
            "rcConTotal",
            "rcConConfirmados",
            "rcConPendientes",
            "rcConRevocados",
            "rcConConflictos"
        ]) {
            $(id).textContent = "0";
        }

        $("rcConEstado").textContent = "Sin evaluación";
        $("rcConAbrirRevision").disabled = true;

        $("rcConTabla").innerHTML = `
            <tr>
                <td colspan="6"
                    class="text-center text-muted py-4">
                    Sin información consultada.
                </td>
            </tr>`;

        $("rcConConflictosDetalle").replaceChildren();

        estado.empleadoId = null;
        estado.fecha = null;
    }

    function renderizar(resultado) {
        $("rcConTotal").textContent = resultado.total;
        $("rcConConfirmados").textContent = resultado.confirmados;
        $("rcConPendientes").textContent = resultado.pendientes;
        $("rcConRevocados").textContent = resultado.revocados;
        $("rcConConflictos").textContent = resultado.conflictos;

        $("rcConEstado").textContent =
            `Conciliación: ${resultado.estadoConciliacion} · ` +
            `Evaluación: ${resultado.estadoEvaluacion}`;

        const conBioTime = resultado.movimientos.some(
            x => x.fuente === "BIOTIME"
        );

        $("rcConAbrirRevision").disabled = !conBioTime;

        if (!resultado.movimientos.length) {
            $("rcConTabla").innerHTML = `
                <tr>
                    <td colspan="6"
                        class="text-center text-muted py-4">
                        No existen movimientos visibles para esta fecha.
                    </td>
                </tr>`;
        } else {
            $("rcConTabla").innerHTML =
                resultado.movimientos.map(x => {
                    const hora = String(
                        x.fechaHoraMexico || ""
                    ).replace("T", " ").slice(0, 19);

                    return `
                        <tr>
                            <td class="text-nowrap">
                                ${escapar(hora)}
                            </td>

                            <td>
                                ${escapar(x.fuente)}
                            </td>

                            <td>
                                ${escapar(x.origen)}
                            </td>

                            <td>
                                ${escapar(x.tipoMovimiento)}
                            </td>

                            <td>
                                ${etiquetaEstado(x.estado)}
                            </td>

                            <td>
                                ${x.requiereRevision
                            ? '<span class="text-warning-emphasis">Revisar</span>'
                            : '<span class="text-success">Sin conflicto individual</span>'}
                            </td>
                        </tr>`;
                }).join("");
        }

        const contenedor = $("rcConConflictosDetalle");
        contenedor.replaceChildren();

        if (resultado.detalleConflictos.length) {
            const titulo = document.createElement("h6");
            titulo.textContent = "Conflictos detectados";
            contenedor.append(titulo);

            for (const conflicto of resultado.detalleConflictos) {
                const alerta = document.createElement("div");
                alerta.className = "alert alert-warning small mb-2";

                const encabezado = document.createElement("strong");
                encabezado.textContent = conflicto.codigo;

                const descripcion = document.createElement("p");
                descripcion.className = "mb-1 mt-1";
                descripcion.textContent = conflicto.descripcion;

                const referencias = document.createElement("small");
                referencias.textContent =
                    "Movimientos: " +
                    conflicto.movimientos.join(", ");

                alerta.append(
                    encabezado,
                    descripcion,
                    referencias
                );

                contenedor.append(alerta);
            }
        } else {
            const sinConflictos = document.createElement("div");
            sinConflictos.className = "alert alert-light small";
            sinConflictos.textContent =
                "No se encontraron contradicciones entre los " +
                "movimientos confirmados. Esto no equivale a " +
                "una aprobación laboral.";
            contenedor.append(sinConflictos);
        }
    }

    async function consultar() {
        if (estado.cargando) return;

        const empleadoId = Number(
            $("rcConEmpleado").value
        );

        const fecha = $("rcConFecha").value;

        if (!Number.isInteger(empleadoId) ||
            empleadoId <= 0 ||
            !/^\d{4}-\d{2}-\d{2}$/.test(fecha)) {
            mensaje(
                "Indica un empleado y una fecha válidos.",
                "warning"
            );
            return;
        }

        if (fecha > fechaHoyMexico()) {
            mensaje(
                "No se permiten fechas futuras.",
                "warning"
            );
            return;
        }

        estado.cargando = true;
        $("rcConConsultar").disabled = true;

        const version = ++estado.version;

        mensaje("Conciliando movimientos...");

        try {
            const url = new URL(window.location.href);

            url.searchParams.set(
                "handler",
                "ConciliacionAsistencia"
            );

            url.searchParams.set(
                "empleadoId",
                empleadoId
            );

            url.searchParams.set(
                "fecha",
                fecha
            );

            const respuesta = await fetch(url.toString(), {
                credentials: "same-origin",
                headers: {
                    Accept: "application/json"
                }
            });

            const json = await respuesta.json();

            if (!respuesta.ok || !json.success) {
                throw new Error(
                    json.message ||
                    `Error HTTP ${respuesta.status}`
                );
            }

            if (version !== estado.version)
                return;

            estado.empleadoId = empleadoId;
            estado.fecha = fecha;

            renderizar(json.resultado);

            mensaje(
                "Conciliación consultada. Revisa las " +
                "clasificaciones pendientes y los conflictos.",
                "success"
            );
        }
        catch (error) {
            limpiar();

            mensaje(
                error.message ||
                "No fue posible conciliar la asistencia.",
                "danger"
            );
        }
        finally {
            estado.cargando = false;
            $("rcConConsultar").disabled = false;
        }
    }

    function abrirRevision() {
        if (!estado.empleadoId || !estado.fecha)
            return;

        const panel = $("rcRevisionPanel");
        const empleado = $("rcRevEmpleado");
        const fecha = $("rcRevFecha");
        const boton = $("rcRevConsultar");

        if (!panel || !empleado || !fecha || !boton) {
            mensaje(
                "No se encontró el módulo de revisión biométrica.",
                "warning"
            );
            return;
        }

        empleado.value = String(estado.empleadoId);
        fecha.value = estado.fecha;

        // Reutilizamos la consulta y los modales
        // de clasificación ya existentes.
        boton.click();

        panel.scrollIntoView({
            behavior: "smooth",
            block: "start"
        });
    }

    function iniciar() {
        if (!$("rcConPanel"))
            return;

        $("rcConFecha").value = fechaHoyMexico();

        $("rcConConsultar").addEventListener(
            "click",
            consultar
        );

        $("rcConAbrirRevision").addEventListener(
            "click",
            abrirRevision
        );

        for (const id of [
            "rcConEmpleado",
            "rcConFecha"
        ]) {
            $(id).addEventListener("change", () => {
                estado.version++;
                limpiar();
                mensaje(
                    "Los filtros cambiaron. Consulta nuevamente."
                );
            });
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener(
            "DOMContentLoaded",
            iniciar
        );
    } else {
        iniciar();
    }
})();
