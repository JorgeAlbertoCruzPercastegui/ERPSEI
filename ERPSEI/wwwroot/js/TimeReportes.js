
/* =====================================================
   ERPSEI - TIME REPORTES
   CATÁLOGOS TIME + REPORTES SQL SERVER
   ===================================================== */

$(document).ready(function () {

    const $modal = $("#modalTimeReportes");
    const $mensaje = $("#timeReporteMensaje");
    const $resultados = $("#timeReporteResultados");
    const $tabla = $("#timeReporteTabla");

    let catalogosCargados = false;
    let cargandoCatalogos = false;
    let cargandoReporte = false;

    let paginaActual = 1;
    let totalPaginas = 0;
    let filtrosAplicados = null;

    // =================================================
    // MENSAJES
    // =================================================

    function mostrarMensaje(texto, tipo = "info") {
        $mensaje
            .removeClass("alert-info alert-danger alert-success")
            .addClass("alert-" + tipo)
            .text(texto);
    }

    function limpiarResultados() {
        try {
            $tabla.bootstrapTable("destroy");
        } catch (_) {
            // Tabla aún no inicializada.
        }

        $tabla.empty();
        $resultados.addClass("d-none");

        $("#timeReportePaginacion").remove();

        $("#btnTimeExportarExcel").prop("disabled", true);
        $("#btnTimeExportarPdf").prop("disabled", true);

        paginaActual = 1;
        totalPaginas = 0;
        filtrosAplicados = null;
    }

    // =================================================
    // FILTROS
    // =================================================

    function obtenerFiltros() {
        return {
            tipo: $("#timeReporteTipo").val(),
            fechaInicio: $("#timeReporteFechaInicio").val(),
            fechaFin: $("#timeReporteFechaFin").val(),
            usuarioId: $("#timeReporteUsuario").val(),
            proyectoId: $("#timeReporteProyecto").val(),
            departamentoId: $("#timeReporteDepartamento").val()
        };
    }

    function llenarSelector(selector, registros, etiqueta) {

        const $select = $(selector);
        const valorActual = $select.val();

        $select.empty();

        $select.append(
            $("<option>", {
                value: "",
                text: etiqueta
            })
        );

        registros.forEach(function (item) {
            $select.append(
                $("<option>", {
                    value: String(item.id),
                    text: item.nombre
                })
            );
        });

        if (
            valorActual &&
            $select.find("option").filter(function () {
                return this.value === String(valorActual);
            }).length > 0
        ) {
            $select.val(valorActual);
        }
    }

    // =================================================
    // CATÁLOGOS REALES DESDE TIME API
    // =================================================

    async function cargarCatalogos() {

        if (catalogosCargados || cargandoCatalogos) {
            return;
        }

        cargandoCatalogos = true;
        $("#btnTimeVistaPrevia").prop("disabled", true);

        mostrarMensaje("Cargando catálogos de TIME...");

        try {
            const response = await fetch(
                window.location.pathname + "?handler=CatalogosReportes",
                {
                    method: "GET",
                    credentials: "same-origin",
                    headers: { "Accept": "application/json" }
                }
            );

            if (!response.ok) {
                throw new Error("Error HTTP " + response.status);
            }

            const data = await response.json();

            if (
                !data.success ||
                !Array.isArray(data.usuarios) ||
                !Array.isArray(data.proyectos) ||
                !Array.isArray(data.departamentos)
            ) {
                throw new Error("Respuesta de catálogos inválida.");
            }

            llenarSelector(
                "#timeReporteUsuario",
                data.usuarios,
                "Todos los usuarios"
            );

            llenarSelector(
                "#timeReporteProyecto",
                data.proyectos,
                "Todos los proyectos"
            );

            llenarSelector(
                "#timeReporteDepartamento",
                data.departamentos,
                "Todos los departamentos"
            );

            catalogosCargados = true;

            mostrarMensaje(
                "Catálogos cargados. Puedes generar reportes " +
                "sobre el histórico sincronizado.",
                "success"
            );

        } catch (error) {
            mostrarMensaje(
                "No fue posible cargar los catálogos: " + error.message,
                "danger"
            );

        } finally {
            cargandoCatalogos = false;

            $("#btnTimeVistaPrevia").prop(
                "disabled",
                !catalogosCargados
            );
        }
    }

    // =================================================
    // PAGINACIÓN DEL REPORTE
    // =================================================

    function actualizarPaginacion() {

        let $contenedor = $("#timeReportePaginacion");

        if (!$contenedor.length) {

            $contenedor = $(`
                <div id="timeReportePaginacion"
                     class="d-flex justify-content-between
                            align-items-center flex-wrap gap-2 mt-3">

                    <button type="button"
                            id="btnReporteAnterior"
                            class="btn btn-outline-secondary">
                        Anterior
                    </button>

                    <span id="timeReportePaginaTexto"></span>

                    <button type="button"
                            id="btnReporteSiguiente"
                            class="btn btn-primary">
                        Siguiente
                    </button>
                </div>
            `);

            $resultados.append($contenedor);
        }

        $("#timeReportePaginaTexto").text(
            "Página " + paginaActual +
            " de " + Math.max(totalPaginas, 1)
        );

        $("#btnReporteAnterior").prop(
            "disabled",
            cargandoReporte || paginaActual <= 1
        );

        $("#btnReporteSiguiente").prop(
            "disabled",
            cargandoReporte || paginaActual >= totalPaginas
        );
    }

    // =================================================
    // SEGURIDAD Y FORMATO
    // =================================================

    function textoSeguro(valor) {
        if (valor === null || valor === undefined) {
            return "-";
        }

        return $("<div>").text(String(valor)).html();
    }

    function fechaCorta(valor) {
        if (!valor) return "-";

        const fecha = String(valor).substring(0, 10);
        const match = fecha.match(
            /^(\d{4})-(\d{2})-(\d{2})$/
        );

        return match
            ? `${match[3]}/${match[2]}/${match[1]}`
            : textoSeguro(valor);
    }

    function renderizarTabla(registros) {

        $tabla.bootstrapTable("destroy");

        $tabla.bootstrapTable({
            columns: [
                {
                    field: "nombre",
                    title: "Nombre",
                    formatter: textoSeguro
                },
                {
                    field: "area",
                    title: "Área",
                    formatter: textoSeguro
                },
                {
                    field: "puesto",
                    title: "Puesto",
                    formatter: textoSeguro
                },
                {
                    field: "nivelCliente",
                    title: "Nivel de Cliente",
                    formatter: textoSeguro
                },
                {
                    field: "grupoEmpresa",
                    title: "Grupo / Empresa",
                    formatter: textoSeguro
                },
                {
                    field: "internaExterna",
                    title: "Interna/externa",
                    formatter: textoSeguro
                },
                {
                    field: "tipo",
                    title: "Tipo",
                    formatter: textoSeguro
                },
                {
                    field: "fecha",
                    title: "Fecha",
                    formatter: fechaCorta
                },
                {
                    field: "actividad",
                    title: "Actividad",
                    formatter: textoSeguro
                },
                {
                    field: "descripcion",
                    title: "Descripción",
                    formatter: textoSeguro
                },
                {
                    field: "horas",
                    title: "Horas",
                    formatter: textoSeguro
                }
            ],
            data: registros,
            search: false,
            pagination: false,
            classes: "table table-striped table-bordered table-sm"
        });

        $resultados.removeClass("d-none");
    }

    // =================================================
    // CONSULTAR REPORTE SQL
    // =================================================

    async function consultarReporte(pagina = 1, nuevosFiltros = null) {

        if (cargandoReporte) return;

        const filtros = nuevosFiltros || filtrosAplicados;

        if (!filtros) return;

        cargandoReporte = true;

        $("#btnTimeVistaPrevia").prop("disabled", true);
        $("#btnReporteAnterior, #btnReporteSiguiente")
            .prop("disabled", true);

        mostrarMensaje("Consultando histórico en SQL Server...");

        try {
            const parametros = new URLSearchParams({
                handler: "ReporteHorasSql",
                tipo: filtros.tipo,
                pagina: String(pagina),
                tamanoPagina: "25"
            });

            [
                "fechaInicio",
                "fechaFin",
                "usuarioId",
                "proyectoId",
                "departamentoId"
            ].forEach(function (campo) {
                if (filtros[campo]) {
                    parametros.set(campo, filtros[campo]);
                }
            });

            const response = await fetch(
                window.location.pathname + "?" + parametros.toString(),
                {
                    method: "GET",
                    credentials: "same-origin",
                    headers: { "Accept": "application/json" }
                }
            );

            const data = await response.json();

            if (!response.ok || !data.success) {
                throw new Error(
                    data.message || "No fue posible consultar el reporte."
                );
            }

            const reporte = data.reporte;

            if (!reporte || !Array.isArray(reporte.registros)) {
                throw new Error("Respuesta SQL inválida.");
            }

            // Confirmar los filtros y la página solo
            // después de recibir una respuesta válida.
            filtrosAplicados = { ...filtros };
            paginaActual = reporte.pagina;
            totalPaginas = reporte.totalPaginas;

            renderizarTabla(reporte.registros);

            mostrarMensaje(
                "Consulta completada: " +
                Number(reporte.totalRegistros).toLocaleString("es-MX") +
                " registros coinciden con los filtros. " +
                "Mostrando " + reporte.registros.length +
                " registros en esta página.",
                "success"
            );

            actualizarPaginacion();

        } catch (error) {
            mostrarMensaje(
                "Error al consultar el reporte: " + error.message,
                "danger"
            );

        } finally {
            cargandoReporte = false;

            $("#btnTimeVistaPrevia").prop(
                "disabled",
                !catalogosCargados
            );

            if ($("#timeReportePaginacion").length) {
                actualizarPaginacion();
            }
        }
    }

    // =================================================
    // EVENTOS DEL MODAL
    // =================================================

    $modal.on("shown.bs.modal", async function () {

        if (!catalogosCargados) {
            await cargarCatalogos();
        }
    });

    $("#btnTimeVistaPrevia").on("click", async function () {

        const filtros = obtenerFiltros();

        if (!catalogosCargados) {
            mostrarMensaje(
                "Primero deben cargarse los catálogos.",
                "danger"
            );
            return;
        }

        if (filtros.tipo !== "horas") {
            mostrarMensaje(
                "Por ahora está disponible el reporte " +
                "Horas y actividades.",
                "info"
            );
            return;
        }

        if (
            filtros.fechaInicio &&
            filtros.fechaFin &&
            filtros.fechaInicio > filtros.fechaFin
        ) {
            mostrarMensaje(
                "La fecha inicial no puede superar la fecha final.",
                "danger"
            );
            return;
        }

        await consultarReporte(1, filtros);
    });

    $(document).on("click", "#btnReporteAnterior", function () {
        if (!cargandoReporte && paginaActual > 1) {
            consultarReporte(paginaActual - 1);
        }
    });

    $(document).on("click", "#btnReporteSiguiente", function () {
        if (!cargandoReporte && paginaActual < totalPaginas) {
            consultarReporte(paginaActual + 1);
        }
    });

    $modal.find("input, select").on("change", function () {
        if (cargandoReporte) return;

        limpiarResultados();

        mostrarMensaje(
            "Los filtros cambiaron. Selecciona Vista previa " +
            "para consultar el nuevo resultado."
        );
    });

    // Exportaciones pendientes de implementación.
    $("#btnTimeExportarExcel, #btnTimeExportarPdf")
        .prop("disabled", true);

});
