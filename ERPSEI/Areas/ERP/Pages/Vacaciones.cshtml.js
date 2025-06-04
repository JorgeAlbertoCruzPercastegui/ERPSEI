var table;
var buttonRemove;
var selections = [];
var dlg = null;
var dlgModal = null;

const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
const postOptions = { headers: { "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val() } }

document.addEventListener("DOMContentLoaded", function (event) {
    table = $("#table");
    buttonRemove = $("#remove");
    //buttonExportAll = $("#exportAll");
    dlg = document.getElementById('dlgVacaciones');

    if (dlg) {
        dlgModal = new bootstrap.Modal(dlg, null);

        dlg.addEventListener('hidden.bs.modal', function (event) {
            onCerrarClick();
        });
    } else {
        console.error("No se encontró el modal con id #dlgVacaciones");
    }

    initTable();
});

function getIdSelections() {
    return $.map(table.bootstrapTable('getSelections'), function (row) {
        return row.id
    })
}
function responseHandler(res) {
    $.each(res, function (i, row) {
        row.state = $.inArray(row.id, selections) !== -1
    })
    return res
}

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
}
window.operateEvents = {
    'click .see': function (e, value, row, index) {
        initSolicitudVacacionesDialog(VER, row);
    },
    'click .edit': function (e, value, row, index) {
        initSolicitudVacacionesDialog(EDITAR, row);
        //table.bootstrapTable('remove', {
        //    field: 'id',
        //    values: [row.id]
        //})
    }
}

function onAgregarClick() {
    initSolicitudVacacionesDialog(NUEVO, { id: "Nuevo", nombre: "" });
}
function initTable() {
    table.bootstrapTable('destroy').bootstrapTable({
        height: 550,
        locale: cultureName,
        exportDataType: 'all',
        exportTypes: ['excel'],
        columns: [
            {
                field: "state",
                checkbox: true,
                align: "center",
                valign: "middle"
            },
            {
                title: "Id",
                field: "id",
                align: "center",
                valign: "middle",
                sortable: true,
                width: "80px"
            },
            {
                title: "Empleado",
                field: "empleado",
                align: "center",
                valign: "middle",
                sortable: true,
                width: "80px"
            },
            {
                title: "FechaSolicitud",
                field: "fechaSolicitud",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "FechaInicio",
                field: "fechaInicio",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "FechaFin",
                field: "fechaFin",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "DiasSolicitados",
                field: "diasSolicitados",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Estatus",
                field: "estado",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Autorizador",
                field: "autorizador",
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
    })
    table.on('check.bs.table uncheck.bs.table ' +
        'check-all.bs.table uncheck-all.bs.table',
        function () {
            buttonRemove.prop('disabled', !table.bootstrapTable('getSelections').length)
            buttonExportAll.prop('disabled', !table.bootstrapTable('getSelections').length)

            // save your data, here just save the current page
            selections = getIdSelections()
            // push or splice the selections if you want to save all data selections
        })
    table.on('all.bs.table', function (e, name, args) {
        console.log(name, args)
    })
    buttonRemove.click(function () {
        askConfirmation(dlgDeleteTitle, dlgDeleteQuestion, function () {
            let oParams = { ids: selections };

            doAjax(
                "/ERP/Vacaciones/DeleteVacaciones",
                oParams,
                function (resp) {
                    if (resp.tieneError) {
                        showError(dlgDeleteTitle, resp.mensaje);
                        return;
                    }

                    table.bootstrapTable('remove', {
                        field: 'id',
                        values: selections
                    })
                    selections = [];
                    buttonRemove.prop('disabled', true);
                    buttonExportAll.prop('disabled', true);

                    let e = document.querySelector("[name='refresh']");
                    e.click();

                    showSuccess(dlgDeleteTitle, resp.mensaje);
                }, function (error) {
                    showError(dlgDeleteTitle, error);
                },
                postOptions
            );

        });
    })
}

function initSolicitudVacacionesDialog(action, row) {
    let fechaInicioField = document.getElementById("inpFechaInicio");
    let fechaFinField = document.getElementById("inpFechaFin");
    let comentarioField = document.getElementById("inpComentarioEmpleado");
    let empleadoIdField = document.getElementById("inpEmpleadoId");

    let btnGuardar = document.getElementById("dlgSolicitudVacacionesBtnGuardar");
    let dlgTitle = document.getElementById("dlgSolicitudVacacionesTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    let diasSolicitadosTexto = document.getElementById("diasSolicitadosTexto");

    summaryContainer.innerHTML = "";
    diasSolicitadosTexto.innerText = "0";

    // Limpiar campos
    fechaInicioField.value = "";
    fechaFinField.value = "";
    comentarioField.value = "";

    switch (action) {
        case NUEVO:
            dlgTitle.innerHTML = "Nueva Solicitud de Vacaciones";
            fechaInicioField.removeAttribute("disabled");
            fechaFinField.removeAttribute("disabled");
            comentarioField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;

        case EDITAR:
            dlgTitle.innerHTML = "Editar Solicitud de Vacaciones";
            fechaInicioField.removeAttribute("disabled");
            fechaFinField.removeAttribute("disabled");
            comentarioField.removeAttribute("disabled");
            btnGuardar.removeAttribute("disabled");
            break;

        default:
            dlgTitle.innerHTML = "Ver Solicitud de Vacaciones";
            fechaInicioField.setAttribute("disabled", true);
            fechaFinField.setAttribute("disabled", true);
            comentarioField.setAttribute("disabled", true);
            btnGuardar.setAttribute("disabled", true);
            break;
    }

    // Asignar valores si existen
    if (row) {
        if (row.fechaInicio) {
            try {
                if (row.fechaInicio.includes("/")) {
                    const [dia, mes, anio] = row.fechaInicio.split("/");
                    fechaInicioField.value = `${anio}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}`;
                } else {
                    const fecha = new Date(row.fechaInicio);
                    fechaInicioField.value = fecha.toISOString().split("T")[0];
                }
            } catch { fechaInicioField.value = ""; }
        }

        if (row.fechaFin) {
            try {
                if (row.fechaFin.includes("/")) {
                    const [dia, mes, anio] = row.fechaFin.split("/");
                    fechaFinField.value = `${anio}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}`;
                } else {
                    const fecha = new Date(row.fechaFin);
                    fechaFinField.value = fecha.toISOString().split("T")[0];
                }
            } catch { fechaFinField.value = ""; }
        }

        comentarioField.value = row.comentario || "";
        empleadoIdField.value = row.empleadoId || "0";

        // Calcular días solicitados si hay fechas válidas
        calcularDiasSolicitados();
    }

    // Asociar evento al cambiar fechas
    fechaInicioField.addEventListener("change", calcularDiasSolicitados);
    fechaFinField.addEventListener("change", calcularDiasSolicitados);

    dlgModal.toggle(); // Mostrar el modal
}

function calcularDiasSolicitados() {
    const inicio = new Date(document.getElementById("inpFechaInicio").value);
    const fin = new Date(document.getElementById("inpFechaFin").value);
    const output = document.getElementById("diasSolicitadosTexto");

    if (!isNaN(inicio) && !isNaN(fin) && fin >= inicio) {
        const diff = Math.ceil((fin - inicio) / (1000 * 60 * 60 * 24)) + 1;
        output.innerText = diff;
    } else {
        output.innerText = "0";
    }
}



function onBuscarClick() {
    let btnBuscar = document.getElementById("btnBuscar");

    let selEmpleado = document.getElementById("inpFiltroEmpleado");
    let selAutorizador = document.getElementById("inpFiltroAutorizador");
    let selEstado = document.getElementById("inpFiltroEstado");
    let inpFechaInicio = document.getElementById("inpFiltroFechaInicio");
    let inpFechaFin = document.getElementById("inpFiltroFechaFin");

    let oParams = {
        empleado: selEmpleado.value.trim() || null,
        autorizador: selAutorizador.value.trim() || null,
        estado: selEstado.value === "" ? null : parseInt(selEstado.value),
        fechaInicioDesde: inpFechaInicio.value || null,
        fechaFinHasta: inpFechaFin.value || null
    };

    doAjax(
        "/ERP/Vacaciones/FiltrarVacaciones",
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

    //limpia los filtros tras buscar
    document.querySelectorAll("#filtros .form-control, #filtros .form-select").forEach(function (e) { e.value = ""; });
}

$(document).ready(function () {
    autoCompletar("#inpFiltroEmpleado"); // si usas autocompletado para empleados
    autoCompletar("#inpFiltroAutorizador");
});
