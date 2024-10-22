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

    autoCompletar("#inpFiltroEmpresaRFC");
    autoCompletar("#inpFiltroEmisor");
    autoCompletar("#inpFiltroReceptor");

    if (window.tipoId == "1") {
        $("#inpFiltroEmisor").parent().parent().hide();
        $("#inpFiltroReceptor").parent().parent().show();
        table.bootstrapTable('hideColumn', 'emisor');
        table.bootstrapTable('showColumn', 'receptor');
    }
    else if (window.tipoId == "2") {
        $("#inpFiltroReceptor").parent().parent().hide();
        $("#inpFiltroEmisor").parent().parent().show();
        table.bootstrapTable('showColumn', 'emisor');
        table.bootstrapTable('hideColumn', 'receptor');
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

});

////////////////////////////////
//Funcionalidad Tabla
////////////////////////////////
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

            if (oParams.ids != null) {
                oParams.ids = [];
                selections = null;
                if (buttonAcciones) { buttonAcciones.prop('disabled', true); }
                table.bootstrapTable('uncheckAll');
            }

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

        // save your data, here just save the current page
        selections = getIdSelections()
    });
}

//Función para mostrar una prefactura como PDF
function onShowPDFClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");
    //onShowCFDIs(TIPO_EXPORTADO_PDF);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
function onShowXMLClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");
    //onShowCFDIs(TIPO_EXPORTADO_XML);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
function onShowExcelClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");
    //onShowCFDIs(TIPO_EXPORTADO_EXCEL);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
function onShowPolizaIngresoClick() {
    onShowCFDIs(TIPO_EXPORTADO_POLIZA_INGRESO);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
function onShowPolizaEgresoClick() {
    showInfo("En desarrollo", "Esta funcionalidad se encuentra en desarrollo. Seguimos trabajando para tenerla disponible cuanto antes.");
    //onShowCFDIs(TIPO_EXPORTADO_POLIZA_EGRESO);

    //if (safeL.length >= 1) { window.open(`/FileViewer?safeL=${encodeURIComponent(safeL)}`, "_blank"); }
}
function onShowCFDIs(tipoExportado) {
    let ids = [];
    let descTipo = "";
    switch (tipoExportado) {
        case TIPO_EXPORTADO_POLIZA_INGRESO:
            descTipo = "Ingreso";
            break;
        case TIPO_EXPORTADO_POLIZA_EGRESO:
            descTipo = "Egreso"
            break;
        default:
            break;
    }

    let selections = table.bootstrapTable('getSelections')||[];
    ids = $.map(selections, function (row) {
        if (row.tipoComprobante == descTipo) { return row.id }
    })||[];

    if (ids.length <= 0) {
        showError(dlgExportTitle, NoItemSelectedMessage);
        return;
    }
    else if (ids.length < selections.length) {
        showInfo(dlgExportTitle, MixedItemsMessage, function () {
            let oParams = { ids: ids, tipoExportado: tipoExportado };

            ajaxExportCFDIS(oParams)
        });
    }
    else {
        let oParams = { ids: ids, tipoExportado: tipoExportado };

        ajaxExportCFDIS(oParams)
    }

}
////////////////////////////////

////////////////////////////////
//Funcionalidad Filtrar
////////////////////////////////
//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    //Ejecuta la validación de los campos
    $("#filtros").validate();

    //Determina los errores. Si la forma no es válida, entonces finaliza.
    if (!$("#filtros").valid()) { return; }

    let oParams = {
        EmpresaRFC: ($("#inpFiltroEmpresaRFC").data("rfc") || "") == "" ? null : $("#inpFiltroEmpresaRFC").data("rfc"),
        Anio: $("#selFiltroAnio").val(),
        Mes: $("#selFiltroMes").val() == 0 ? null : $("#selFiltroMes").val(),
        EstatusId: $("#selFiltroEstatus").val() == 0 ? null : parseInt($("#selFiltroEstatus").val()),
        TipoId: $("#selFiltroTipo").val() == 0 ? null : parseInt($("#selFiltroTipo").val()),
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