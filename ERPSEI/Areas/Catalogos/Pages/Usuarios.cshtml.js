var table;
var selections = [];
var dlgDetalleModal = null;

const EDITAR = 1;
const VER = 2;
const postOptions = { headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() } }

//Función para inicializar el módulo.
document.addEventListener("DOMContentLoaded", function (event) {
    table = $("#table");

    dlgDetalleModal = new bootstrap.Modal(document.getElementById('dlgDetalle'), null);

    initTable();

    onBuscarClick();
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
    if (typeof res == "string" && res.length >= 1) { res = JSON.parse(res); }
    $.each(res, function (i, row) { row.state = $.inArray(row.id, selections) !== -1 });
    return res
}
//Función para dar formato a los iconos de operación de los registros
function operateFormatter(value, row, index) {
    let icons = [];

    //Icono Ver
    icons.push(`<li><a class="dropdown-item see" href="#" title="${btnVerTitle}"><i class="bi bi-search"></i> ${btnVerTitle}</a></li>`);
    //Icono Editar
    icons.push(`<li><a class="dropdown-item edit" href="#" title="${btnEditarTitle}"><i class="bi bi-pencil-fill"></i> ${btnEditarTitle}</a></li>`);

    return `<div class="dropdown">
              <button class="btn" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-three-dots-vertical success"></i>
              </button>
              <ul class="dropdown-menu">${icons.join("")}</ul>
            </div>`;

    return icons.join('');
}
//Eventos de los iconos de operación
window.operateEvents = {
    'click .see': function (e, value, row, index) {
        initDetallesDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initDetallesDialog(EDITAR, row);
    }
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
                title: colNombreUsuarioHeader,
                field: "nombreUsuario",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colNombreEmpleadoHeader,
                field: "nombreEmpleado",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colNombreRolHeader,
                field: "rol",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colAccionesHeader,
                field: "operate",
                align: "center",
                width: "100px",
                clickToSelect: false,
                events: window.operateEvents,
                formatter: operateFormatter
            }
        ]
    })
    table.on('check.bs.table uncheck.bs.table ' +
        'check-all.bs.table uncheck-all.bs.table',
        function () {
                // save your data, here just save the current page
                selections = getIdSelections()
                // push or splice the selections if you want to save all data selections
        }
    );
    table.on('all.bs.table', function (e, name, args) {
        console.log(name, args)
    });
}
////////////////////////////////

/////////////////////////////////
//Funcionalidad Filtrar
/////////////////////////////////
//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    let oParams = {};

    doAjax(
        "/Catalogos/Usuarios/Filtrar",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length >= 1) {
                    let summary = ``;
                    resp.errores.forEach(function (error) {
                        summary += `<li>${error}</li>`;
                    });
                    summaryContainer.innerHTML += `<ul>${summary}</ul>`;
                }
                showError(btnBuscarTitle, resp.mensaje);
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

////////////////////////////////
//Funcionalidad Diálogo
function initDetallesDialog(action, row) {

    let inpUsuarioId =
        document.getElementById("inpUsuarioId");

    let inpNombreUsuario =
        document.getElementById("inpUsuarioNombre");

    let inpNombreEmpleado =
        document.getElementById("inpEmpleadoNombre");

    let dlgTitle =
        document.getElementById("dlgDetalleTitle");

    let summaryContainer =
        document.getElementById("saveValidationSummary");

    let rolesValidation =
        document.getElementById("rolesUsuarioValidation");

    let rolesChecks =
        document.querySelectorAll(".rol-usuario-check");


    summaryContainer.innerHTML = "";

    if (rolesValidation) {
        rolesValidation.innerHTML = "";
    }


    // =====================================================
    // LIMPIAR ROLES
    // =====================================================

    rolesChecks.forEach(function (check) {

        check.checked = false;
    });


    // =====================================================
    // MODO
    // =====================================================

    switch (action) {

        case EDITAR:

            dlgTitle.innerHTML =
                dlgEditarTitle;

            rolesChecks.forEach(
                function (check) {

                    check.disabled = false;
                }
            );

            document
                .getElementById(
                    "dlgDetallesBtnGuardar"
                )
                .removeAttribute(
                    "disabled"
                );

            break;


        default:

            dlgTitle.innerHTML =
                dlgVerTitle;

            rolesChecks.forEach(
                function (check) {

                    check.disabled = true;
                }
            );

            document
                .getElementById(
                    "dlgDetallesBtnGuardar"
                )
                .setAttribute(
                    "disabled",
                    true
                );

            break;
    }


    // =====================================================
    // DATOS DEL USUARIO
    // =====================================================

    inpUsuarioId.value =
        row.id;

    inpNombreUsuario.value =
        row.nombreUsuario;

    inpNombreEmpleado.value =
        row.nombreEmpleado;


    // =====================================================
    // MARCAR ROLES ACTUALES
    // =====================================================

    let rolesUsuario =
        Array.isArray(row.rolIds)
            ? row.rolIds
            : [];


    rolesChecks.forEach(
        function (check) {

            check.checked =
                rolesUsuario.includes(
                    check.value
                );
        }
    );


    // =====================================================
    // MOSTRAR MODAL
    // =====================================================

    dlgDetalleModal.show();
}

//Función para el cierre del cuadro de diálogo
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
    $(".text-danger").children().remove()
}

//Función para el guardado de información
function onGuardarClick() {

    let idField =
        document.getElementById(
            "inpUsuarioId"
        );

    let dlgTitle =
        document.getElementById(
            "dlgDetalleTitle"
        );

    let summaryContainer =
        document.getElementById(
            "saveValidationSummary"
        );

    let rolesValidation =
        document.getElementById(
            "rolesUsuarioValidation"
        );


    summaryContainer.innerHTML = "";

    if (rolesValidation) {
        rolesValidation.innerHTML = "";
    }


    // =====================================================
    // OBTENER ROLES SELECCIONADOS
    // =====================================================

    let rolesSeleccionados =
        Array.from(
            document.querySelectorAll(
                ".rol-usuario-check:checked"
            )
        )
            .map(function (check) {

                return check.value;
            });


    // =====================================================
    // VALIDAR
    // =====================================================

    if (rolesSeleccionados.length === 0) {

        if (rolesValidation) {

            rolesValidation.innerHTML =
                "Debe seleccionar al menos un rol.";
        }

        return;
    }


    // =====================================================
    // PARÁMETROS
    // =====================================================

    let oParams = {
        id: idField.value,
        rolIds: rolesSeleccionados
    };


    // =====================================================
    // GUARDAR
    // =====================================================

    doAjax(
        "/Catalogos/Usuarios/Save",
        oParams,

        function (resp) {

            if (resp.tieneError) {

                if (
                    Array.isArray(
                        resp.errores
                    ) &&
                    resp.errores.length >= 1
                ) {

                    let summary = ``;

                    resp.errores.forEach(
                        function (error) {

                            summary +=
                                `<li>${error}</li>`;
                        }
                    );

                    summaryContainer.innerHTML =
                        `<ul>${summary}</ul>`;
                }

                showError(
                    dlgTitle.innerHTML,
                    resp.mensaje
                );

                return;
            }


            onCerrarClick();

            dlgDetalleModal.hide();

            onBuscarClick();

            showSuccess(
                dlgTitle.innerHTML,
                resp.mensaje
            );
        },

        function (error) {

            showError(
                "Error",
                error
            );
        },

        postOptions
    );
}
////////////////////////////////