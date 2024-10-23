var table;
var buttonAcciones;
var buttonExport;
var buttonCancel;
var tableProdServ;
var selections = [];
var dlgProdServ = null;
var dlgCFDI = null;
var dlgCFDIModal = null;
const ESTATUS_SOLICITADA = 1;
const ESTATUS_AUTORIZADA = 2;
const ESTATUS_FINALIZADA = 3;

const TIPO_EXPORTADO_PDF = 0
const TIPO_EXPORTADO_XML = 1
const TIPO_EXPORTADO_EXCEL = 2
const TIPO_EXPORTADO_POLIZA_INGRESO = 3
const TIPO_EXPORTADO_POLIZA_EGRESO = 4

var numFormatter = null;
var dialogMode = null;
const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
const postOptions = { headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() } }

document.addEventListener('DOMContentLoaded', function () {
    numFormatter = new Intl.NumberFormat(cultureName);

    table = $("#table");
    buttonAcciones = $("#btnAcciones");
    buttonExport = $("#btnExportar");
    buttonCancel = $("#btnCancelar");

    initTable();

    autoCompletar("#inpFiltroEmpresaRFC", {
        change: function (element, item) {
            clearTable();
            if (!item) { $('#inpFiltroEmpresaRFC').data('rfc', null); }
        }
    });
    autoCompletar("#inpFiltroEmisor", {
        change: function (element, item) {
            clearTable();
            if (!item) { $('#inpFiltroEmisor').data('rfc', null); }
        }
    });
    autoCompletar("#inpFiltroReceptor", {
        change: function (element, item) {
            clearTable();
            if (!item) { $('#inpFiltroReceptor').data('rfc', null); }
        }
    });

    if (window.tipoId == "1") {
        //Para los comprobantes emitidos, no se muestra el filtro de emisor pero si el de receptor
        $("#inpFiltroEmisor").parent().parent().hide();
        $("#inpFiltroReceptor").parent().parent().show();
        table.bootstrapTable('hideColumn', 'emisor');
        table.bootstrapTable('showColumn', 'receptor');

        //El tipo de comprobante seleccionado será el de I - Ingreso
        $("#selFiltroTipoComprobante").val("I");
    }
    else if (window.tipoId == "2") {
        //Para los comprobantes recibidos, no se muestra el filtro de receptor pero si el de emisor
        $("#inpFiltroReceptor").parent().parent().hide();
        $("#inpFiltroEmisor").parent().parent().show();
        table.bootstrapTable('showColumn', 'emisor');
        table.bootstrapTable('hideColumn', 'receptor');

        //El tipo de comprobante seleccionado será el de E - Egreso
        $("#selFiltroTipoComprobante").val("E");
    }

    $("#inpFiltroEmisor").val("").attr("idselected", "");
    $("#inpFiltroReceptor").val("").attr("idselected", "");

    jQuery.validator.setDefaults({
        highlight: function (element, errorClass, validClass) {
            $(element).addClass("is-invalid").removeClass("is-valid");
        },
        unhighlight: function (element, errorClass, validClass) {
            if ($(element).hasClass("is-invalid")) {
                $(element).addClass("is-valid").removeClass("is-invalid");
            }
        }
    });

    onTipoComprobanteChanged();

});

////////////////////////////////
//Funcionalidad Tabla
////////////////////////////////
//Función para establecer el estilo de los rows individualmente
function rowStyle(row, index) {
    //Se verifica el estatus del row
    if (row.contabilizado == 1) {
        return {
            classes: "bd-callout bd-callout-success border-5 border-top-0 border-bottom-0"
        };
    }

    return {};
}
//Función para obtener los identificadores de los registros seleccionados
function getIdSelections() {
    return $.map(table.bootstrapTable('getSelections'), function (row) {
        return row.id
    })
}
//Función para procesar la respuesta del servidor al consultar datos
function responseHandler(res) {
    if (typeof res == "string" && res.length >= 1) {
        res = JSON.parse(res);
    }
    $.each(res, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1
    });

    return res
}

//Función para dar formato de moneda a los campos numéricos.
function currencyFormatter(value, row, index) {
    return `$ ${numFormatter.format(value)}`;
}
//Función para dar formato a la validez de un comprobante
function validFormatter(value, row, index) {
    if ((row.cancelado || 0) == 1) {
        return `<i title="${tooltipCancelado}" class="bi bi-x-circle-fill text-danger"></i>`;
    }
    else if ((row.valido || 0) == 1) {
        return `<i title="${tooltipValido}" class="bi bi-check-circle-fill text-success"></i>`;
    }
    else {
        return `<i title="${tooltipSinValidar}" class="bi bi-question-circle-fill text-primary"></i>`;
    }
}

//Función para cancelar cfdis
function onCancelarClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");

    return;

    let oParams = {};

    let ids = getIdSelections();
    if ((ids || "").length <= 0) { showError(btnCancelarTitle, NoItemSelectedMessage); }

    doAjax(
        "/ERP/AdministradorDeComprobantes/CancelarComprobante",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                showError(btnCancelarTitle, resp.mensaje);
                return;
            }

            if (ids != null) {
                ids = [];
                selections = null;
                if (buttonExport) { buttonExport.prop('disabled', true); }
                table.bootstrapTable('uncheckAll');
            }

            showSuccess(btnCancelarTitle, resp.mensaje);
        }, function (error) {
            showError(btnCancelarTitle, error);
        },
        postOptions
    );
}

//Función para exportar cfdis
function ajaxExportCFDIS(oParams) {
    doAjax(
        `/ERP/AdministradorDeComprobantes/ExportCFDIS`,
        oParams,
        function (resp) {
            if (resp.tieneError) {
                showError(dlgExportTitle, resp.mensaje);
                return;
            }

            clearTable();

            let fileLink = document.getElementById("downloadFileLink");
            fileLink.setAttribute("href", `/ERP/AdministradorDeComprobantes/DownloadExcel?nombreArchivo=${resp.datos}`)
            fileLink.click();

            showSuccess(dlgExportTitle, resp.mensaje);
        }, function (error) {
            showError(dlgExportTitle, error);
        },
        postOptions
    );
}
//Función para inicializar la tabla
function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        height: 550,
        locale: cultureName,
        columns: [
            {
                field: "state",
                checkbox: true,
                align: "center",
                valign: "middle"
            },
            {
                align: "center",
                valign: "middle",
                formatter: validFormatter,
                sortable: true,
                width: "30px"
            },
            {
                title: colSerieHeader,
                field: "serie",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFolioHeader,
                field: "folio",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFechaHeader,
                field: "fecha",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colUUIDHeader,
                field: "uuid",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colEmisorHeader,
                field: "emisor",
                align: "center",
                valign: "middle",
                visible: false,
                sortable: true
            },
            {
                title: colReceptorHeader,
                field: "receptor",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFormaPagoHeader,
                field: "formaPago",
                align: "center",
                valign: "middle",
                visible: false,
                sortable: true
            },
            {
                title: colMonedaHeader,
                field: "moneda",
                align: "center",
                valign: "middle",
                visible: false,
                sortable: true
            },
            {
                title: colTipoCambio,
                field: "tipoCambio",
                align: "center",
                valign: "middle",
                visible: false,
                sortable: true
            },
            {
                title: colTipoComprobante,
                field: "tipoComprobante",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colMetodoPagoHeader,
                field: "metodoPago",
                align: "center",
                valign: "middle",
                visible: false,
                sortable: true
            },
            {
                title: colUsoCFDIHeader,
                field: "usoCFDI",
                align: "center",
                valign: "middle",
                visible: false,
                sortable: true
            },
            {
                title: colSubtotalHeader,
                field: "subtotal",
                align: "center",
                valign: "middle",
                sortable: true,
                visible: false,
                formatter: currencyFormatter
            },
            {
                title: colDescuentoHeader,
                field: "descuento",
                align: "center",
                valign: "middle",
                sortable: true,
                visible: false,
                formatter: currencyFormatter
            },
            {
                title: colTotalHeader,
                field: "total",
                align: "center",
                valign: "middle",
                sortable: true,
                formatter: currencyFormatter
            }
        ]
    })
    table.on('check.bs.table uncheck.bs.table check-all.bs.table uncheck-all.bs.table', function () {
        if (buttonAcciones) { buttonAcciones.prop('disabled', !table.bootstrapTable('getSelections').length) }
        selections = getIdSelections();
        let selectedRows = table.bootstrapTable('getSelections') || [];

        //Obtiene todos los comprobantes que no se han contabilizado
        unaccounted = $.map(selectedRows, function (row) { if (row.contabilizado == 0) { return row.id } }) || [];
        if (unaccounted.length <= 0) {
            $(".dropdown-item.polizaIngreso").parent().hide();
            $(".dropdown-item.polizaEgreso").parent().hide();
        }
        else {
            onTipoComprobanteChanged(false);
        }
    });
}

//Función para mostrar comprobantes como PDF
function onShowPDFClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");
    //onShowCFDIs(TIPO_EXPORTADO_PDF);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
//Función para mostrar comprobantes como XML
function onShowXMLClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");
    //onShowCFDIs(TIPO_EXPORTADO_XML);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
//Función para mostrar comprobantes como Excel
function onShowExcelClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");
    //onShowCFDIs(TIPO_EXPORTADO_EXCEL);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
//Función para mostrar comprobantes como Póliza de Ingresos
function onShowPolizaIngresoClick() {
    onShowCFDIs(TIPO_EXPORTADO_POLIZA_INGRESO);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
//Función para mostrar comprobantes como Póliza de Egresos
function onShowPolizaEgresoClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");
    //onShowCFDIs(TIPO_EXPORTADO_POLIZA_EGRESO);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
//Función para mostrar comprobantes en diferentes formatos
function onShowCFDIs(tipoExportado) {
    switch (tipoExportado) {
        case TIPO_EXPORTADO_PDF:
            break;
        case TIPO_EXPORTADO_XML:
            break;
        case TIPO_EXPORTADO_EXCEL:
            break;
        case TIPO_EXPORTADO_POLIZA_INGRESO:
        case TIPO_EXPORTADO_POLIZA_EGRESO:
            let unaccounted = [];
            let selections = table.bootstrapTable('getSelections') || [];

            //Obtiene todos los comprobantes que no se han contabilizado
            unaccounted = $.map(selections, function (row) { if (row.contabilizado == 0) { return row.id } }) || [];

            if (unaccounted.length <= 0) {
                //Si no hay elementos sin contabilizar seleccionados, se notifica error al usuario.
                showError(dlgExportTitle, NoItemSelectedMessage);
                return;
            }
            else if (unaccounted.length < selections.length) {
                //Si la cantidad de elementos no contabilizados es menor a la cantidad de elementos seleccionados, notifica al usuario que solo se realizará la póliza con los elementos sin contabilizar y los contabilizados se ignorarán.
                showInfo(dlgExportTitle, MixedItemsMessage, function () {
                    ajaxExportCFDIS({ ids: unaccounted, tipoExportado: tipoExportado })
                });
            }
            else {
                //En cualquier otro caso, manda a elaborar la póliza directamente.
                ajaxExportCFDIS({ ids: unaccounted, tipoExportado: tipoExportado })
            }
            break;
        default:
    }
}

//Función para validar comprobantes
function onValidarClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");
    return;

    let ids = getIdSelections();
    let oParams = { ids: ids };
    doAjax(
        `/ERP/AdministradorDeComprobantes/ValidarCFDIS`,
        oParams,
        function (resp) {
            if (resp.tieneError) {
                showError(dlgExportTitle, resp.mensaje);
                return;
            }

            resp.datos.foreach(function (row) { table.bootstrapTable('updateByUniqueId', { id: row.id, row: row }); });

            showSuccess(dlgExportTitle, resp.mensaje);
        }, function (error) {
            showError(dlgExportTitle, error);
        },
        postOptions
    );
}
////////////////////////////////

////////////////////////////////
//Funcionalidad Filtrar
////////////////////////////////
//Función para limpiar la tabla de resultados
function clearTable() {
    table.bootstrapTable('load', []);
    table.bootstrapTable('uncheckAll');
    if (buttonAcciones) { buttonAcciones.prop('disabled', true) }
}
//Función para mostrar/ocultar las opciones de exportado de pólizas dependiendo el tipo de comprobante seleccionado
function onTipoComprobanteChanged(clear = true) {
    switch ($("#selFiltroTipoComprobante").val()) {
        case "I":
            $(".dropdown-item.polizaIngreso").parent().show();
            $(".dropdown-item.polizaEgreso").parent().hide();
            break;
        case "E":
            $(".dropdown-item.polizaIngreso").parent().hide();
            $(".dropdown-item.polizaEgreso").parent().show();
            break;
        case "T":
        case "N":
        case "P":
        default:
            $(".dropdown-item.polizaIngreso").parent().hide();
            $(".dropdown-item.polizaEgreso").parent().hide();
    }

    if (clear) { clearTable(); }
}
//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    //Ejecuta la validación de los campos
    $("#filtros").validate();

    //Determina los errores. Si la forma no es válida, entonces finaliza.
    if (!$("#filtros").valid()) { return; }

    clearTable();

    let oParams = {
        EmpresaRFC: ($("#inpFiltroEmpresaRFC").data("rfc") || "") == "" ? null : $("#inpFiltroEmpresaRFC").data("rfc"),
        Anio: $("#selFiltroAnio").val(),
        Mes: $("#selFiltroMes").val() == 0 ? null : $("#selFiltroMes").val(),
        EstatusId: $("#selFiltroEstatus").val() == 0 ? null : parseInt($("#selFiltroEstatus").val()),
        EstatusContableId: $("#selFiltroEstatusContable").val() == 0 ? null : parseInt($("#selFiltroEstatusContable").val()),
        TipoId: window.tipoId,
        TipoComprobanteClave: $("#selFiltroTipoComprobante").val() == 0 ? null : $("#selFiltroTipoComprobante").val(),
        FormaPagoClave: $("#selFiltroFormaPago").val() == 0 ? null : $("#selFiltroFormaPago").val(),
        MetodoPagoClave: $("#selFiltroMetodoPago").val() == 0 ? null : $("#selFiltroMetodoPago").val(),
        UsoCFDIClave: $("#selFiltroUsoCFDI").val() == 0 ? null : $("#selFiltroUsoCFDI").val(),
        EmisorRFC: ($("#inpFiltroEmisor").data("rfc") || "") == "" ? null : $("#inpFiltroEmisor").data("rfc"), 
        ReceptorRFC: ($("#inpFiltroReceptor").data("rfc") || "") == "" ? null : $("#inpFiltroReceptor").data("rfc")
    };

    doAjax(
        "/ERP/AdministradorDeComprobantes/Filtrar",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                let summary = ``;
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                }
                showError($("#btnBuscar").text(), resp.mensaje + " " + summary);
                return;
            }

            table.bootstrapTable('load', responseHandler(resp.datos));
        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
////////////////////////////////