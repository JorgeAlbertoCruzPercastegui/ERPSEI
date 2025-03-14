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
    let oCuentaNueva = {
        id: nuevoRegistro,
        cuenta: "",
        nombre: "",
        rfc: "",
        empresaDescripcion: '',
        empresaId: '',
        tipoId: 0,
        subtipoId: 0
    };

    return oCuentaNueva;
}

//Función para inicializar el cuadro de diálogo
function initCuentaContableDialog(action, row) {
    $("#saveValidationSummary").html("");

    dialogMode = action;

    $("#inpId").attr("disabled", true);
    switch (action) {
        case NUEVO:
        case EDITAR:
            if (action == NUEVO) {
                $("#dlgDetalleTitle").html(dlgNuevoTitle);
            }
            else {
                $("#dlgDetalleTitle").html(dlgEditarTitle);
            }

            document.querySelectorAll(".formButton").forEach(function (btn) { btn.classList.remove("disabled"); });
            document.querySelectorAll(".formInput, .formSelect").forEach(function (e) { e.removeAttribute("disabled"); });

            break;
        default:
            $("#dlgDetalleTitle").html(dlgVerTitle);

            document.querySelectorAll(".formButton").forEach(function (btn) { btn.classList.add("disabled"); });
            document.querySelectorAll(".formInput, .formSelect").forEach(function (e) { e.setAttribute("disabled", true); });
            break;
    }

    $("#inpId").val(row.id);
    $("#inpNombre").val(row.nombre);
    $("#inpCuenta").val(row.cuenta);
    $("#inpEmpresaId").val(row.empresaDescripcion);
    $("#inpEmpresaId").attr("idselected", row.empresaId);
    $("#selTipoId").val(row.tipoId);
    $("#selSubtipoId").val(row.subtipoId);

    if (action == NUEVO) {
        dlgCuentaContableModal.toggle();
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