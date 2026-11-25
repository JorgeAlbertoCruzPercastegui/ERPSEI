
(() => {
    "use strict";

    // INTRANET - Dashboard consolidado de asistencia
    // Todos los indicadores laborales son preliminares.

    const $ = id => document.getElementById(id);

    const estado = {
        pagina: 1,
        totalPaginas: 0,
        registros: [],
        filtros: null,
        cargando: false,
        version: 0,
        controlador: null
    };

    function endpoint(handler) {
        const url = new URL(window.location.href);
        url.searchParams.set("handler", handler);
        return url;
    }

    function escapar(valor) {
        return String(valor ?? "")
            .replaceAll("&", "&amp;")
            .replaceAll("<", "&lt;")
            .replaceAll(">", "&gt;")
            .replaceAll('"', "&quot;")
            .replaceAll("'", "&#39;");
    }

    function fechaMexicoHoy() {
        const partes = new Intl.DateTimeFormat("en-US", {
            timeZone: "America/Mexico_City",
            year: "numeric",
            month: "2-digit",
            day: "2-digit"
        }).formatToParts(new Date());

        const parte = tipo =>
            partes.find(x => x.type === tipo)?.value || "";

        return `${parte("year")}-${parte("month")}-${parte("day")}`;
    }

    function sumarDias(fecha, dias) {
        const d = new Date(`${fecha}T12:00:00Z`);
        d.setUTCDate(d.getUTCDate() + dias);
        return d.toISOString().slice(0, 10);
    }

    function fechaTexto(valor) {
        return String(valor || "").slice(0, 10);
    }

    function horaTexto(valor) {
        return valor || "—";
    }

    function siNo(valor) {
        if (valor === true) return "Sí";
        if (valor === false) return "No";
        return "Sin determinar";
    }

    function numero(valor) {
        return valor === null || valor === undefined
            ? "No evaluable"
            : String(valor);
    }

    function informar(texto, tipo = "info") {
        const el = $("rcAdMensaje");
        if (!el) return;

        el.className = `alert alert-${tipo} small`;
        el.textContent = texto;
    }

    function filtrosActuales() {
        return {
            fechaInicio: $("rcAdInicio").value,
            fechaFin: $("rcAdFin").value,
            empleadoId: $("rcAdEmpleado").value.trim(),
            buscar: $("rcAdBuscar").value.trim()
        };
    }

    function validarFiltros(f) {
        if (!/^\d{4}-\d{2}-\d{2}$/.test(f.fechaInicio) ||
            !/^\d{4}-\d{2}-\d{2}$/.test(f.fechaFin)) {
            return "Selecciona ambas fechas.";
        }

        if (f.fechaFin < f.fechaInicio) {
            return "La fecha final debe ser posterior a la inicial.";
        }

        if (f.fechaFin > fechaMexicoHoy()) {
            return "No se permiten fechas futuras.";
        }

        if (f.fechaFin > sumarDias(f.fechaInicio, 6)) {
            return "Selecciona un máximo de siete días.";
        }

        if (f.empleadoId &&
            (!/^\d+$/.test(f.empleadoId) ||
                !Number.isSafeInteger(Number(f.empleadoId)) ||
                Number(f.empleadoId) <= 0)) {
            return "El ID del empleado no es válido.";
        }

        if (f.buscar.length > 100) {
            return "La búsqueda permite máximo 100 caracteres.";
        }

        return null;
    }

    function urlConFiltros(handler, f, pagina) {
        const url = endpoint(handler);

        url.searchParams.set("fechaInicio", f.fechaInicio);
        url.searchParams.set("fechaFin", f.fechaFin);
        url.searchParams.set("pagina", String(pagina));

        if (f.empleadoId) {
            url.searchParams.set("empleadoId", f.empleadoId);
        } else {
            url.searchParams.delete("empleadoId");
        }

        if (f.buscar) {
            url.searchParams.set("buscar", f.buscar);
        } else {
            url.searchParams.delete("buscar");
        }

        return url;
    }

    function badge(texto, clase = "text-bg-secondary") {
        return `<span class="badge ${clase}" style="color: #111827 !important; font-weight: 600;">${escapar(texto)}</span>`;
    }


    function estadoBadge(valor) {
        const textos = {
            JornadaIncompleta: "Incompleta",
            CompletaParaRevision: "Completa preliminar",
            ConflictoMovimientos: "Conflicto",
            SecuenciaInvalida: "Secuencia inválida",
            SinConfiguracion: "Sin horario",
            NoLaborable: "No laborable",
            SinMovimientos: "Sin movimientos"
        };

        const clase =
            valor === "CompletaParaRevision"
                ? "text-bg-info"
                : valor === "ConflictoMovimientos" ||
                    valor === "SecuenciaInvalida"
                    ? "text-bg-danger"
                    : "text-bg-warning";

        return badge(
            textos[valor] || valor || "Sin evaluar",
            clase
        );
    }

    function integridadBadge(valor) {
        const textos = {
            Completa: "Completa",
            Incompleta: "Incompleta",
            SinMovimientos: "Sin confirmadas",
            SecuenciaInconsistente: "Inconsistente",
            RequiereConciliacion: "Conciliación",
            NoEvaluable: "No evaluable"
        };

        const clase =
            valor === "Completa"
                ? "text-bg-info"
                : valor === "RequiereConciliacion" ||
                    valor === "SecuenciaInconsistente"
                    ? "text-bg-danger"
                    : "text-bg-secondary";

        return badge(
            textos[valor] || valor || "No evaluable",
            clase
        );
    }

    function cumplimientoBadge(valor) {
        const textos = {
            SinDesviacionesDetectadas: "Sin desviaciones detectadas",
            DesviacionesPotenciales: "Desviaciones potenciales",
            NoConcluyente: "No concluyente",
            NoEvaluable: "No evaluable",
            RequiereRevisionHorario: "Revisar horario"
        };

        const clase =
            valor === "SinDesviacionesDetectadas"
                ? "text-bg-info"
                : valor === "DesviacionesPotenciales"
                    ? "text-bg-warning"
                    : "text-bg-secondary";

        return badge(
            textos[valor] || valor || "No evaluable",
            clase
        );
    }

    function renderizar() {
        const tbody = $("rcAdTabla");
        if (!tbody) return;

        $("rcAdExportar").disabled =
            estado.cargando ||
            !estado.filtros ||
            estado.registros.length === 0;

        $("rcAdAnterior").disabled =
            estado.cargando || estado.pagina <= 1;

        $("rcAdSiguiente").disabled =
            estado.cargando ||
            estado.totalPaginas === 0 ||
            estado.pagina >= estado.totalPaginas;

        $("rcAdPagina").textContent =
            `${estado.totalPaginas === 0 ? 0 : estado.pagina}` +
            ` / ${estado.totalPaginas}`;

        if (!estado.registros.length) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="15"
                        class="text-center text-muted py-4">
                        No hay jornadas con actividad
                        para los filtros seleccionados.
                    </td>
                </tr>`;
            return;
        }

        tbody.innerHTML = estado.registros.map((r, i) => `
            <tr>
                <td class="text-nowrap">
                    ${escapar(fechaTexto(r.fecha))}
                </td>
                <td>
                    <strong>${escapar(r.colaborador)}</strong>
                    <div class="small text-muted">
                        ID ${escapar(r.empleadoId)}
                    </div>
                </td>
                <td>${escapar(r.horario)}</td>
                <td>${estadoBadge(r.estado)}</td>
                <td>${integridadBadge(r.integridadChecadas)}</td>
                <td>${cumplimientoBadge(r.cumplimientoHorario)}</td>
                <td>${escapar(horaTexto(r.entrada))}</td>
                <td>${escapar(horaTexto(r.salidaComer))}</td>
                <td>${escapar(horaTexto(r.regresoComer))}</td>
                <td>${escapar(horaTexto(r.salida))}</td>
                <td>${escapar(r.movimientosBioTime)}</td>
                <td>${escapar(r.movimientosIntranet)}</td>
                <td>${escapar(r.pendientes)}</td>
                <td>
                    ${r.homeOfficeAutorizado
                ? badge("HO autorizado", "text-bg-info")
                : ""}
                    ${r.tieneAusencias
                ? badge("Solicitud de ausencia")
                : ""}
                    ${!r.homeOfficeAutorizado && !r.tieneAusencias
                ? "—" : ""}
                </td>
                <td>
                    <button type="button"
                            class="btn btn-sm btn-outline-primary"
                            data-rc-ad-detalle="${i}">
                        <i class="bi bi-eye me-1"></i>
                        Ver
                    </button>
                </td>
            </tr>
        `).join("");
    }

    function actualizarIndicadores(r) {
        $("rcAdTotal").textContent = r.totalJornadas ?? 0;
        $("rcAdCompletas").textContent =
            r.jornadasCompletasPagina ?? 0;
        $("rcAdRevision").textContent =
            r.jornadasRevisionPagina ?? 0;
        $("rcAdPendientes").textContent =
            r.movimientosPendientesPagina ?? 0;

        const inicio = (r.pagina - 1) * r.tamano + 1;
        const fin = Math.min(
            r.pagina * r.tamano,
            r.totalJornadas
        );

        $("rcAdInfoPagina").textContent =
            r.totalJornadas > 0
                ? `Mostrando ${inicio}–${fin} de ${r.totalJornadas} jornadas`
                : "0 jornadas";
    }

    function limpiarResultados() {
        estado.version++;
        estado.controlador?.abort();
        estado.controlador = null;
        estado.cargando = false;
        estado.registros = [];
        estado.totalPaginas = 0;
        estado.pagina = 1;
        estado.filtros = null;

        $("rcAdConsultar").disabled = false;
        $("rcAdTotal").textContent = "0";
        $("rcAdCompletas").textContent = "0";
        $("rcAdRevision").textContent = "0";
        $("rcAdPendientes").textContent = "0";
        $("rcAdInfoPagina").textContent = "Sin resultados";

        renderizar();
    }

    async function consultar(pagina = 1) {
        if (estado.cargando) return;

        const filtros = filtrosActuales();
        const error = validarFiltros(filtros);

        if (error) {
            informar(error, "warning");
            return;
        }

        estado.cargando = true;
        estado.controlador = new AbortController();
        $("rcAdConsultar").disabled = true;

        const version = ++estado.version;

        informar("Consultando asistencia consolidada...");
        renderizar();

        try {
            const url = urlConFiltros(
                "AsistenciaDashboard",
                filtros,
                pagina
            );

            const respuesta = await fetch(url, {
                credentials: "same-origin",
                signal: estado.controlador.signal,
                headers: {
                    "Accept": "application/json"
                }
            });

            const json = await respuesta.json();

            if (version !== estado.version) return;

            if (!respuesta.ok || !json.success) {
                throw new Error(
                    json.message ||
                    `Error HTTP ${respuesta.status}`
                );
            }

            const resultado = json.resultado;

            estado.registros = Array.isArray(resultado.registros)
                ? resultado.registros
                : [];

            estado.pagina = resultado.pagina;
            estado.totalPaginas = resultado.totalPaginas;
            estado.filtros = { ...filtros };

            actualizarIndicadores(resultado);
            renderizar();

            informar(
                "Asistencia consultada correctamente. " +
                "Los resultados requieren revisión de TH.",
                "success"
            );
        } catch (error) {
            if (version !== estado.version ||
                error.name === "AbortError") {
                return;
            }

            limpiarResultados();

            informar(
                error.message ||
                "No fue posible consultar la asistencia.",
                "danger"
            );
        } finally {
            if (version === estado.version) {
                estado.cargando = false;
                estado.controlador = null;
                $("rcAdConsultar").disabled = false;
                renderizar();
            }
        }
    }

    function mostrarDetalle(indice) {
        const r = estado.registros[indice];
        if (!r) return;

        $("rcAdModalTitulo").textContent =
            `Asistencia · ${r.colaborador}`;

        const campo = (titulo, valor) => `
            <div class="col-md-6">
                <div class="rc-ad-detail-item">
                    <div class="small text-muted">
                        ${escapar(titulo)}
                    </div>
                    <strong>${escapar(valor ?? "—")}</strong>
                </div>
            </div>`;

        $("rcAdModalContenido").innerHTML = `
            <h6 class="mb-3">Evaluación técnica</h6>
            <div class="row g-3 mb-4">
                ${campo("Empleado", r.empleadoId)}
                ${campo("Fecha", fechaTexto(r.fecha))}
                ${campo("Horario", r.horario)}
                ${campo("Estado técnico", r.estado)}
                ${campo("Integridad de checadas",
            r.integridadChecadas)}
                ${campo("Cumplimiento preliminar",
                r.cumplimientoHorario)}
            </div>

            <h6 class="mb-3">Movimientos confirmados</h6>
            <div class="row g-3 mb-4">
                ${campo("Entrada", r.entrada)}
                ${campo("Salida a comer", r.salidaComer)}
                ${campo("Regreso de comer", r.regresoComer)}
                ${campo("Salida final", r.salida)}
                ${campo("BioTime", r.movimientosBioTime)}
                ${campo("INTRANET", r.movimientosIntranet)}
                ${campo("Confirmados", r.confirmados)}
                ${campo("Pendientes", r.pendientes)}
            </div>

            <h6 class="mb-3">Indicadores preliminares TH</h6>
            <div class="row g-3 mb-4">
                ${campo("Entrada puntual",
                    siNo(r.entradaPuntual))}
                ${campo("Regreso de comida",
                        siNo(r.regresoComidaPuntual))}
                ${campo("Salida en horario",
                            siNo(r.salidaEnHorario))}
                ${campo("Minutos efectivos registrados",
                                numero(r.minutosEfectivos))}
                ${campo("Minutos de retardo potencial",
                                    numero(r.minutosRetardoPotencial))}
                ${campo("Minutos de salida anticipada potencial",
                                        numero(r.minutosSalidaAnticipadaPotencial))}
                ${campo("Home Office autorizado",
                                            siNo(r.homeOfficeAutorizado))}
                ${campo("Existen solicitudes de ausencia",
                                                siNo(r.tieneAusencias))}
                ${campo("Requiere revisión TH",
                                                    siNo(r.requiereRevision))}
            </div>

            <div class="alert alert-warning small">
                <strong>Observaciones para Talento Humano</strong>
                <p class="mb-0 mt-2">
                    ${escapar(
                                                        r.observaciones ||
                                                        "Sin observaciones adicionales."
                                                    )}
                </p>
            </div>

            <div class="small text-muted">
                Resultado informativo. Las desviaciones potenciales
                no constituyen faltas ni sanciones laborales.
                Las solicitudes y autorizaciones deben revisarse
                conforme a las reglas aprobadas por TH.
            </div>
        `;

        bootstrap.Modal
            .getOrCreateInstance($("rcAdModal"))
            .show();
    }

    function exportar() {
        if (estado.cargando ||
            !estado.filtros ||
            !estado.registros.length) {
            return;
        }

        const error = validarFiltros(estado.filtros);

        if (error) {
            informar(error, "warning");
            return;
        }

        const url = urlConFiltros(
            "ExportarAsistenciaDashboard",
            estado.filtros,
            estado.pagina
        );

        window.location.assign(url.toString());
    }

    function iniciar() {
        if (!$("rcAsistenciaDashboard")) return;

        const hoy = fechaMexicoHoy();
        $("rcAdInicio").value = hoy;
        $("rcAdFin").value = hoy;

        $("rcAdConsultar").addEventListener(
            "click",
            () => consultar(1)
        );

        $("rcAdAnterior").addEventListener(
            "click",
            () => consultar(estado.pagina - 1)
        );

        $("rcAdSiguiente").addEventListener(
            "click",
            () => consultar(estado.pagina + 1)
        );

        $("rcAdExportar").addEventListener(
            "click",
            exportar
        );

        $("rcAdTabla").addEventListener("click", event => {
            const boton = event.target.closest(
                "[data-rc-ad-detalle]"
            );

            if (!boton) return;

            mostrarDetalle(
                Number(boton.dataset.rcAdDetalle)
            );
        });

        for (const id of [
            "rcAdInicio",
            "rcAdFin",
            "rcAdEmpleado",
            "rcAdBuscar"
        ]) {
            const evento = id === "rcAdBuscar"
                ? "input" : "change";

            $(id).addEventListener(evento, () => {
                limpiarResultados();

                informar(
                    "Los filtros cambiaron. Consulta nuevamente.",
                    "info"
                );
            });
        }

        renderizar();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", iniciar);
    } else {
        iniciar();
    }
})();
