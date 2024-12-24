var table;
var buttonRemove;
var selections = [];
var dlg = null;
var dlgModal = null;

const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
const maxFileSizeInBytes = 5242880; //5mb = (5 * 1024) * 1024;
const oneMegabyteSizeInBytes = 1048576; // 1mb = (1 * 1024) * 1024
const postOptions = {
    headers: {
        "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
    },
    type: 'POST'
};
const getOptions = {
    headers: postOptions.headers,
    type: 'GET'
};
const putOptions = {
    headers: postOptions.headers,
    type: 'PUT'
};

document.addEventListener("DOMContentLoaded", function () {
    table = $("#table");
    dlg = document.getElementById('dlgPoliza');
    dlgModal = new bootstrap.Modal(dlg, null);
    //Función para limpiar el cuadro de diálogo cuando es cerrado
    dlg.addEventListener('hidden.bs.modal', function (event) {
        onCerrarClick();
    });

    initTable();
    onObtenerRegistrosClick();
});

//Función para procesar la respuesta del servidor al consultar datos
function responseHandler(res) {
    if (typeof res === "string" && res.length >= 1) {
        res = JSON.parse(res);
    }

    $.each(res, function (i, row) {
        row.state = $.inArray(row.Id, selections) !== -1;
    });

    return res;
}


//Función para añadir botones a la cinta de botones de la tabla
function additionalButtons() {
    return {
        btnImport: {
            text: btnImportarText,
            icon: 'bi-upload',
            event: function () { },
            attributes: {
                "title": btnImportarTitle,
                "data-bs-toggle": "modal",
                "data-bs-target": "#dlgImportarExcel"
            }
        }
    }
}

function operateFormatter(value, row, index) {
    let icons = [];

    // Icono Ver
    icons.push(`<li><a class="dropdown-item see" href="#" title="${btnVerTitle}"><i class="bi bi-search"></i> ${btnVerTitle}</a></li>`);

    // Icono Exportar a Excel
    icons.push(`<li><a class="dropdown-item export" href="#" title="Exportar a Excel"><i class="bi bi-file-earmark-excel"></i> Exportar a Excel</a></li>`);

    return `<div class="dropdown">
              <button class="btn" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                <i class="bi bi-three-dots-vertical success"></i>
              </button>
              <ul class="dropdown-menu">${icons.join("")}</ul>
            </div>`;
}

window.operateEvents = {
    'click .see': function (e, value, row, index) {
        initPolizaDialog(VER, row); // Lógica para "Ver"
    },
    'click .export': function (e, value, row, index) {
        exportarExcel(row.Id); // Llama a la función para exportar a Excel
    }
};

       
function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        height: 550,
        locale: cultureName,
        exportDataType: 'all',
        exportTypes: ['excel'],
        toolbar: '#toolbar',
        showColumns: true,
        columns: [
            {
                title: colIdHeader,
                field: "Id",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFechaCreacionHeader,
                field: "FechaHoraCreacion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colFechaModificacionHeader,
                field: "FechaHoraModificacion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colUsuarioCreadorHeader,
                field: "UsuarioCreador",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colUsuarioModificadorHeader,
                field: "UsuarioModificador",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colPrintNumberHeader,
                field: "NumeroImpresion",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: colAccionesHeader,
                field: "operate",
                align: 'center',
                width: "100px",
                clickToSelect: false,
                events: window.operateEvents,
                formatter: operateFormatter
            }
        ]
    });
}


//Función para filtrar los datos de la tabla.
function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");
    let inpId = document.getElementById("inpFiltroId");
    let inpUsuarioCreador = document.getElementById("inpFiltroUsuarioCreador");
    let inpUsuarioModificador = document.getElementById("inpFiltroUsuarioModificador");
    let inpFechaInicio = document.getElementById("inpFiltroFechaInicio");
    let inpFechaFin = document.getElementById("inpFiltroFechaFin");

    let oParams = {
        Id: inpId.value ? parseInt(inpId.value) || null : null,
        FechaCreacion: inpFechaInicio.value || null,
        FechaModificacion: inpFechaFin.value || null,
        UsuarioCreador: inpUsuarioCreador.value || null,
        UsuarioModificador: inpUsuarioModificador.value || null
    };

    doAjax(
        "/Reportes/AdministradorPolizas/FiltrarPolizas",
        oParams,
        function (resp) {
            if (resp.tieneError) {
                if (Array.isArray(resp.errores) && resp.errores.length > 0) {
                    let summary = resp.errores.map(error => `<li>${error}</li>`).join("");
                    saveValidationSummary.innerHTML = `<ul>${summary}</ul>`;
                }
                showError(btnBuscar.innerHTML, resp.mensaje);
                return;
            }

            table.bootstrapTable('load', responseHandler(resp.datos));
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );

    // Resetea el valor de los filtros después de la solicitud.
    document.querySelectorAll("#filtros .form-control").forEach(function (e) { e.value = ""; });
    document.querySelectorAll("#filtros .form-select").forEach(function (e) { e.value = 0; });
}

function onObtenerRegistrosClick() {
    doAjax(
        "/Reportes/AdministradorPolizas/Polizas",
        null,
        function (resp) {
            console.log("Respuesta recibida:", resp);

            if (resp.tieneError) {
                showError("Error", resp.mensaje);
                return;
            }

            // Extraer el valor de resp.datos
            let datos = resp.datos && resp.datos.value ? resp.datos.value : [];

            if (typeof datos === "string") {
                datos = JSON.parse(datos);
            }

            // Asegúrate de que la tabla está inicializada antes de cargar los datos
            if (!table.data('bootstrap.table')) {
                table.bootstrapTable();
            }

            table.bootstrapTable('load', responseHandler(datos));
        },
        function (error) {
            showError("Error", error);
        },
        getOptions
    );
}

//Funcionalidad Diálogo
function initPolizaDialog(action, row) {
    let polizaIdField = document.getElementById("inpPolizaId");
    let polizaUsuarioCreadorField = document.getElementById("inpPolizaUsuarioCreador");
    let polizaUsuarioModificadorField = document.getElementById("inpPolizaUsuarioModificador");
    let polizaFechaCreacionField = document.getElementById("inpPolizaFechaCreacion");
    let polizaFechaModificacionField = document.getElementById("inpPolizaFechaModificacion");
    let btnGuardar = document.getElementById("dlgPolizaBtnGuardar");
    let dlgTitle = document.getElementById("dlgPolizaTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    polizaIdField.setAttribute("disabled", true);
    polizaUsuarioCreadorField.setAttribute("disabled", true);
    polizaUsuarioModificadorField.setAttribute("disabled", true);
    polizaFechaCreacionField.setAttribute("disabled", true);
    polizaFechaModificacionField.setAttribute("disabled", true);

    switch (action) {
        case NUEVO:
            dlgTitle.innerHTML = dlgNuevoTitle;

            polizaUsuarioCreadorField.removeAttribute("disabled");
            polizaUsuarioModificadorField.removeAttribute("disabled");
            polizaFechaCreacionField.removeAttribute("disabled");
            polizaFechaModificacionField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;
        case EDITAR:
            dlgTitle.innerHTML = dlgEditarTitle;

            polizaUsuarioCreadorField.removeAttribute("disabled");
            polizaUsuarioModificadorField.removeAttribute("disabled");
            polizaFechaCreacionField.removeAttribute("disabled");
            polizaFechaModificacionField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;
        default:
            dlgTitle.innerHTML = dlgVerTitle;

            polizaUsuarioCreadorField.removeAttribute("disabled");
            polizaUsuarioModificadorField.removeAttribute("disabled");
            polizaFechaCreacionField.removeAttribute("disabled");
            polizaFechaModificacionField.removeAttribute("disabled");

            polizaUsuarioCreadorField.setAttribute("disabled", true);
            polizaUsuarioModificadorField.setAttribute("disabled", true);
            polizaFechaCreacionField.setAttribute("disabled", true);
            polizaFechaModificacionField.setAttribute("disabled", true);
            btnGuardar.setAttribute("disabled", true);
            break;
    }

    polizaIdField.value = row.Id;
    polizaUsuarioCreadorField.value = row.UsuarioCreador;
    polizaUsuarioModificadorField.value = row.UsuarioModificador;
    polizaFechaCreacionField.value = row.FechaHoraCreacion;
    polizaFechaModificacionField.value = row.FechaHoraModificacion;

    dlgModal.toggle();
}
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


