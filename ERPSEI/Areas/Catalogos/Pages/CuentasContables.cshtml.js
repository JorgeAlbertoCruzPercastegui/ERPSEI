var table;
var selections = [];
var dlgCuentaContable = null;
var dlgCuentaContableModal = null;
var rutaModulo = "/Catalogos/CuentasContables";

var dialogMode = null;

const NUEVO = 0;
const EDITAR = 1;
const VER = 2;

const postOptions = { headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() } }

document.addEventListener('DOMContentLoaded', function () {
    table = $("#table");
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

    initTable();

    dlgCuentaContable = document.getElementById('dlgCuentaContable');
    //Función para limpiar el cuadro de diálogo cuando es cerrado
    dlgCuentaContable.addEventListener('hidden.bs.modal', function (event) { onCerrarClick(); });

    dlgCuentaContableModal = new bootstrap.Modal(dlgCuentaContable, null);
});

////////////////////////////////
//Funcionalidad Filtrar
////////////////////////////////
//Función para convertir una cadena JSON a un objeto JSON
function responseHandler(res) {
    if (typeof res == "string" && res.length >= 1) {
        res = JSON.parse(res);
    }

    return res
}
//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    let oParams = {
        EmpresaRFC: ($("#inpFiltroEmpresaRFC").data("rfc") || "") == "" ? null : $("#inpFiltroEmpresaRFC").data("rfc"),
        ClienteRFC: ($("#inpFiltroClienteRFC").data("rfc") || "") == "" ? null : $("#inpFiltroClienteRFC").data("rfc"),
        ProveedorRFC: ($("#inpFiltroProveedorRFC").data("rfc") || "") == "" ? null : $("#inpFiltroProveedorRFC").data("rfc"),
        TipoId: $("#selFiltroTipo").val() == 0 ? null : $("#selFiltroTipo").val(),
        SubtipoId: $("#selFiltroSubtipo").val() == 0 ? null : $("#selFiltroSubtipo").val()
    };

    doAjax(
        `${rutaModulo}/Filtrar`,
        oParams,
        function (resp) {
            if (resp.tieneError) {
                showError($("#btnBuscar").text(), resp.mensaje);
                return;
            }

            //Se convierte la cadena JSON a objeto JSON
            resp.datos = responseHandler(resp.datos);

            table.bootstrapTable('load', responseHandler(resp.datos));
        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
////////////////////////////////

////////////////////////////////
//Funcionalidad Tabla de Resultados
////////////////////////////////
//Función para inicializar la tabla
function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        height: 550,
        locale: cultureName,
        columns: [
            {
                title: 'Hello',
                field: "serie",
                align: "center",
                valign: "middle",
                sortable: true
            }
        ]
    });
}
//Función para agregar cuentas contables
function onAgregarClick() {
    let oCuentaNueva = createNewCuenta();
    initCuentaContableDialog(NUEVO, oCuentaNueva);
    dlgCuentaContableModal.toggle();
}
////////////////////////////////

////////////////////////////////
//Funcionalidad Diálogo CFDI
////////////////////////////////
//Función para crear un nuevo objeto CFDI
function createNewCuenta() {
    let oCFDINuevo = {
        id: nuevoRegistro,
        fecha: strCurDate,
        fechaJS: strCurDate,
        tipoComprobanteId: 0,
        serie: "",
        folio: "",
        usoCFDIId: 0,
        formaPagoId: 0,
        metodoPagoId: 0,
        monedaId: 0,
        tipoCambio: "",
        exportacionId: 0,
        numeroOperacion: "",
        emisorId: 0,
        emisor: "",
        receptorId: 0,
        receptor: "",
        conceptos: []
    };

    return oCFDINuevo;
}

//Función para inicializar el cuadro de diálogo
function initCuentaContableDialog(action, row) {
    let tabGenerales = document.getElementById("tabGenerales");

    let idField = document.getElementById("inpCFDIId");

    let fechaField = document.getElementById("inpFecha"),
        tipoComprobanteField = document.getElementById("selTipoComprobante");

    let serieField = document.getElementById("inpSerie"),
        folioField = document.getElementById("inpFolio"),
        usoField = document.getElementById("selUsoCFDI");

    let formaField = document.getElementById("selFormaPago"),
        metodoField = document.getElementById("selMetodoPago"),
        monedaField = document.getElementById("selMoneda"),
        tipoCambioField = document.getElementById("inpTipoCambio");

    let exportacionField = document.getElementById("selExportacion"),
        numeroOperacionField = document.getElementById("inpNumeroOperacion");

    let emisorField = document.getElementById("inpEmisor"),
        btnInfoEmisor = document.getElementById("btnInfoEmisor"),
        receptorField = document.getElementById("inpReceptor"),
        btnInfoReceptor = document.getElementById("btnInfoReceptor");

    let btnLimpiar = document.getElementById("btnLimpiar"),
        btnGuardar = document.getElementById("dlgCFDIBtnGuardar");

    let dlgTitle = document.getElementById("dlgCFDITitle"),
        summaryContainer = document.getElementById("saveValidationSummary");

    summaryContainer.innerHTML = "";

    dialogMode = action;

    idField.setAttribute("disabled", true);
    switch (action) {
        case NUEVO:
        case EDITAR:
            if (action == NUEVO) {
                dlgTitle.innerHTML = dlgNuevoTitle;
            }
            else {
                dlgTitle.innerHTML = dlgEditarTitle;
            }

            btnLimpiar.hidden = false;
            btnGuardar.hidden = false;

            document.querySelectorAll(".formButton").forEach(function (btn) { btn.classList.remove("disabled"); });
            document.querySelectorAll(".formInput, .formSelect").forEach(function (e) { e.removeAttribute("disabled"); });

            break;
        default:
            dlgTitle.innerHTML = dlgVerTitle;

            btnLimpiar.hidden = true;
            btnGuardar.hidden = true;

            document.querySelectorAll(".formButton").forEach(function (btn) { btn.classList.add("disabled"); });
            document.querySelectorAll(".formInput, .formSelect").forEach(function (e) { e.setAttribute("disabled", true); });
            break;
    }

    idField.value = row.id;

    fechaField.value = row.fechaJS;
    tipoComprobanteField.value = row.tipoComprobanteId;

    serieField.value = row.serie;
    folioField.value = row.folio;
    usoField.value = row.usoCFDIId;

    formaField.value = row.formaPagoId;
    metodoField.value = row.metodoPagoId;
    monedaField.value = row.monedaId;
    tipoCambioField.value = row.tipoCambio;
    if (action != VER) { tipoCambioField.removeAttribute("disabled"); }

    exportacionField.value = row.exportacionId;
    numeroOperacionField.value = row.numeroOperacion;

    emisorField.setAttribute("idselected", row.emisorId);
    emisorField.value = row.emisor;
    btnInfoEmisor.setAttribute("hidden", true);
    receptorField.setAttribute("idselected", row.receptorId);
    receptorField.value = row.receptor;
    btnInfoReceptor.setAttribute("hidden", true);

    tabGenerales.click();

    if (action == NUEVO || (row.hasDatosAdicionales || false)) {
        establecerDatosAdicionales(row);
        dlgCFDIModal.toggle();
        return;
    }
}

//Función para limpiar los campos del cuadro de diálogo al cerrar.
function onCerrarClick() {
    //Removes validation from input-fields
    $('.input-validation-error').addClass('input-validation-valid');
    $('.input-validation-error').removeClass('input-validation-error');
    //Removes validation message after input-fields
    $('.field-validation-error').addClass('field-validation-valid');
    $('.field-validation-error').removeClass('field-validation-error');
    //Removes validation summary 
    $('.validation-summary-errors').addClass('validation-summary-valid');
    $('.validation-summary-errors').removeClass('validation-summary-errors');
    //Removes danger text from fields
    $(".text-danger").children().remove();
    //Removes is-valid and is-invalid class
    $(".is-valid").removeClass("is-valid");
    $(".is-invalid").removeClass("is-invalid");
}

//Función para el cierre del cuadro de diálogo
function onCerrarDialogoClick() {
    if (dialogMode == VER) {
        onCerrarClick();
        dlgCuentaContableModal.toggle();
    }
    else {
        askConfirmation(dlgConfirmActionTitle, dlgConfirmActionQuestion, function () {
            onCerrarClick();
            dlgCuentaContableModal.toggle();
        });
    }
}

//Función para el guardado de información del empleado
function onGuardarClick() {
    //Ejecuta la validación de los campos
    $("#theForm").validate();

    //Determina los errores. Si la forma no es válida, entonces finaliza.
    if (!$("#theForm").valid()) { return; }

    $("#saveValidationSummary").html("");

    let oParams = {
        id: idField.value == nuevoRegistro ? 0 : idField.value
    }

    doAjax(
        `${rutaModulo}/Guardar`,
        oParams,
        function (resp) {
            if (resp.tieneError) {
                showError($("#dlgCFDITitle"), resp.mensaje);
                return;
            }

            dlgCuentaContableModal.toggle();

            onBuscarClick();

            showSuccess($("#dlgCFDITitle"), resp.mensaje);
        }, function (error) {
            showError("Error", error);
        },
        postOptions
    );
}
/////////////////////////////////////////////