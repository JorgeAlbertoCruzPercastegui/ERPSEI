var table;
var buttonRemove;
var selections = [];
var dlg = null;
var dlgModal = null;

const NUEVO = 0;
const EDITAR = 1;
const VER = 2;
let diasDisponiblesActuales = 0;

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

    obtenerDiasDisponibles();

    cargarResumenVacaciones();
    
    cargarVacacionesAcumuladas();

    cargarVacacionesTomadas()
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
        rowStyle: function (row, index) {
            if (row.estado === "Aprobado" || row.estado === "Rechazado") {
                return { classes: 'table-secondary' };
            }
            return {};
        },
        columns: [
            {
                field: "state",
                checkbox: true,
                align: "center",
                valign: "middle",
                checkboxDisabled: function (row, index) {
                    return row.estado === "Aprobado" || row.estado === "Rechazado";
                }
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
                title: "ComentarioEmpleado",
                field: "comentarioEmpleado",
                align: "center",
                valign: "middle",
                sortable: true
            }/*,
            {
                title: colAccionesHeader,
                field: "operate",
                align: 'center',
                width: "100px",
                clickToSelect: false,
                events: window.operateEvents,
                formatter: operateFormatter
            }*/
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

    // ✅ Limpiar campos
    fechaInicioField.value = "";
    fechaFinField.value = "";
    comentarioField.value = "";
    diasSolicitadosTexto.innerText = "0";
    summaryContainer.innerHTML = "";

    // ✅ Obtener resumen actualizado (acumuladas, tomadas, saldo, disponibles)
    diasDisponiblesActuales = 0;
    cargarResumenVacaciones(); // esto también actualiza lblDiasDisponibles
    obtenerDiasDisponibles();

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
        cargarResumenVacaciones();
    }

    // Asociar evento al cambiar fechas
    fechaInicioField.addEventListener("change", calcularDiasSolicitados);
    fechaFinField.addEventListener("change", calcularDiasSolicitados);

    dlgModal.toggle(); // Mostrar el modal
}

/*function calcularDiasSolicitados() {
    const inicioInput = document.getElementById("inpFechaInicio").value;
    const finInput = document.getElementById("inpFechaFin").value;
    const output = document.getElementById("diasSolicitadosTexto");

    const inicio = new Date(inicioInput);
    const fin = new Date(finInput);

    if (!isNaN(inicio) && !isNaN(fin) && fin >= inicio) {
        let diasLaborales = 0;
        let temp = new Date(inicio);

        while (temp <= fin) {
            const dia = temp.getDay();
            if (dia !== 0 && dia !== 6) {
                diasLaborales++;
            }
            temp.setDate(temp.getDate() + 1);
        }

        output.innerText = diasLaborales;
    } else {
        output.innerText = "0";
    }
}*/

function calcularDiasSolicitados() {
    const inicio = new Date(document.getElementById("inpFechaInicio").value);
    const fin = new Date(document.getElementById("inpFechaFin").value);
    const output = document.getElementById("diasSolicitadosTexto");
    const tdSaldo = document.getElementById("tdSaldoTotal");
    const lblDisponibles = document.getElementById("lblDiasDisponibles");

    if (!isNaN(inicio) && !isNaN(fin) && fin >= inicio) {
        let totalDias = 0;
        let fecha = new Date(inicio);

        while (fecha <= fin) {
            const dia = fecha.getDay();
            if (dia !== 0 && dia !== 6) {
                totalDias++;
            }
            fecha.setDate(fecha.getDate() + 1);
        }

        const restante = Math.max(diasDisponiblesActuales - totalDias, 0);

        if (totalDias > diasDisponiblesActuales) {
            output.innerHTML = `<span class="text-danger">${totalDias} días (excede saldo disponible de ${diasDisponiblesActuales.toFixed(1)} días)</span>`;
        } else {
            output.innerText = `${totalDias}`;
        }

        // ✅ Actualizar "Tienes X días disponibles"
        lblDisponibles.innerText = restante.toFixed(1);

        // ✅ Actualizar "Total Saldo"
        tdSaldo.innerText = `${restante.toFixed(1)} días`;
    } else {
        output.innerText = "0";
        lblDisponibles.innerText = diasDisponiblesActuales.toFixed(1);
        tdSaldo.innerText = `${diasDisponiblesActuales.toFixed(1)} días`;
    }
}

async function obtenerDiasDisponibles() {
    try {
        const response = await fetch("/ERP/Vacaciones?handler=ObtenerDiasDisponibles");
        const dias = await response.json();

        diasDisponiblesActuales = dias; // ← Guarda en la variable global
        document.getElementById("lblDiasDisponibles").innerText = dias.toFixed(1);

        // También actualizar resumen si deseas
        document.getElementById("tdAcumuladas").innerText = `${dias.toFixed(1)} días`;
        document.getElementById("tdSaldoTotal").innerText = `${dias.toFixed(1)} días`;
    } catch (error) {
        console.error("Error al obtener días disponibles:", error);
    }
}


function cargarResumenVacaciones() {
    doAjax(
        "/ERP/Vacaciones?handler=ResumenVacaciones",
        null,
        function (resp) {
            if (resp.error) {
                showError("Resumen", resp.error);
                return;
            }

            document.getElementById("tdAcumuladas").innerText = `${resp.acumuladas.toFixed(1)} días`;
            document.getElementById("tdTomadas").innerText = `${resp.tomadas.toFixed(1)} días`;
            document.getElementById("tdVencidas").innerText = `0.0 días`;
            document.getElementById("tdFuturas").innerText = `0.0 días`;
            document.getElementById("tdSaldoTotal").innerText = `${resp.saldo.toFixed(1)} días`;

            document.getElementById("lblDiasDisponibles").innerText = `${resp.saldo.toFixed(1)}`;

            diasDisponiblesActuales = resp.saldo; // 👈 este valor será base
        },
        function (error) {
            console.error("Error al cargar resumen:", error);
        },
        { type: "GET" }
    );
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

function onGuardarClick() {
    $("#theFormS").validate();
    let valid = $("#theFormS").valid();
    if (!valid) { return; }

    let btnClose = document.getElementById("dlgSolicitudVacacionesBtnCancelar");

    let fechaInicio = document.getElementById("inpFechaInicio").value;
    let fechaFin = document.getElementById("inpFechaFin").value;
    let comentario = document.getElementById("inpComentarioEmpleado").value;
    let empleadoId = document.getElementById("inpEmpleadoId").value;

    let dlgTitle = document.getElementById("dlgSolicitudVacacionesTitle");
    let summaryContainer = document.getElementById("saveValidationSummary");
    summaryContainer.innerHTML = "";

    let oParams = {
        InputSolicitud: {
            FechaInicio: fechaInicio,
            FechaFin: fechaFin,
            ComentarioEmpleado: comentario,
            EmpleadoId: parseInt(empleadoId)
        }
    };

    doAjax(
        "/ERP/Vacaciones/GuardarSolicitud",
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
                showError(dlgTitle.innerHTML, resp.mensaje);
                return;
            }

            // Cerrar modal
            let modalElement = document.getElementById('dlgVacacionesSolicitud');
            let modalInstance = bootstrap.Modal.getInstance(modalElement);
            if (modalInstance) modalInstance.hide();

            // Limpiar campos después de guardar
            document.getElementById("inpFechaInicio").value = "";
            document.getElementById("inpFechaFin").value = "";
            document.getElementById("inpComentarioEmpleado").value = "";
            document.getElementById("diasSolicitadosTexto").innerText = "0";

            // Refrescar tabla principal
            document.querySelector("[name='refresh']").click();

            // Mostrar mensaje de éxito
            showSuccess(dlgTitle.innerHTML, resp.mensaje);

            // Esperar un poco para que el backend se actualice antes de recargar el resumen
            setTimeout(() => {
                obtenerDiasDisponibles();       // Actualiza lblDiasDisponibles
                cargarResumenVacaciones();      // Actualiza la tabla y saldo total
                cargarVacacionesTomadas();         // ✅ Actualiza tabla de vacaciones tomadas
            }, 300); // puede ajustarse a 500ms si necesario
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}

function cargarVacacionesAcumuladas() {
    fetch("/ERP/Vacaciones/VacacionesAcumuladas")
        .then(resp => resp.json())
        .then(data => {
            const tbody = document.getElementById("tbodyVacAcumuladas");
            tbody.innerHTML = "";

            if (data.error) {
                tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger">${data.error}</td></tr>`;
                return;
            }

            if (data.length === 0) {
                tbody.innerHTML = `<tr><td colspan="6" class="text-center text-muted">No hay vacaciones acumuladas</td></tr>`;
                return;
            }

            data.forEach(item => {
                const row = document.createElement("tr");
                row.innerHTML = `
                    <td>${item.fecha ? new Date(item.fecha).toLocaleDateString() : ""}</td>
                    <td>${item.numeroDias.toFixed(1)} días</td>
                    <td>${item.tipo}</td>
                    <td>${item.vencimiento ? new Date(item.vencimiento).toLocaleDateString() : ""}</td>
                    <td>${item.periodo || ""}</td>
                    <td>-</td>
                `;
                tbody.appendChild(row);
            });
        })
        .catch(err => {
            console.error("Error al cargar vacaciones acumuladas:", err);
            document.getElementById("tbodyVacAcumuladas").innerHTML = `<tr><td colspan="6" class="text-center text-danger">Error al cargar los datos</td></tr>`;
        });
}

function cargarVacacionesTomadas() {
    fetch("/ERP/Vacaciones?handler=VacacionesTomadas")
        .then(resp => resp.json())
        .then(data => {
            const tbody = document.getElementById("tbodyVacacionesTomadas");
            tbody.innerHTML = "";

            if (data.error) {
                tbody.innerHTML = `<tr><td colspan="6" class="text-center text-danger">${data.error}</td></tr>`;
                return;
            }

            if (data.length === 0) {
                tbody.innerHTML = `<tr><td colspan="6" class="text-center text-muted">No hay vacaciones tomadas.</td></tr>`;
                return;
            }

            data.forEach(vac => {
                const row = document.createElement("tr");
                row.innerHTML = `
                    <td>${vac.inicio}</td>
                    <td>${vac.fin}</td>
                    <td>${vac.dias} días</td>
                    <td>${vac.tipo}</td>
                    <td>${vac.estado}</td>
                    <td>-</td>
                `;
                tbody.appendChild(row);
            });
        })
        .catch(err => {
            console.error("Error al cargar vacaciones tomadas:", err);
            document.getElementById("tbodyVacacionesTomadas").innerHTML =
                `<tr><td colspan="6" class="text-center text-danger">Error al cargar los datos</td></tr>`;
        });
}

/*function onAutorizarVacacionesClick() {
    var selectedRow = $("input[type='checkbox']:checked").closest("tr");

    if (selectedRow.length === 0) {
        var modal = new bootstrap.Modal(document.getElementById("modalAlerta"));
        modal.show();
        return;
    }

    // 🔍 Corregido: extraer el ID desde la primera celda de la fila
    var solicitudId = selectedRow.find("td").eq(1).text().trim(); // columna 1 si el checkbox está en la 0
    $("#modalSolicitudId").val(solicitudId);

    var modal = new bootstrap.Modal(document.getElementById("modalAutorizar"));
    modal.show();
}*/
function onAutorizarVacacionesClick() {
    var selectedRow = $("input[type='checkbox']:checked").closest("tr");

    if (selectedRow.length === 0) {
        var modal = new bootstrap.Modal(document.getElementById("modalAlerta"));
        modal.show();
        return;
    }

    var estado = selectedRow.find("td").eq(7).text().trim(); // ← CORREGIDO A 7
    if (estado !== "Pendiente") {
        showError("Acción no permitida", "No puedes modificar una solicitud que ya ha sido procesada.");
        selectedRow.find("input[type='checkbox']").prop("checked", false);
        return;
    }

    var solicitudId = selectedRow.find("td").eq(1).text().trim(); // ID sigue en la columna 1
    $("#modalSolicitudId").val(solicitudId);

    var modal = new bootstrap.Modal(document.getElementById("modalAutorizar"));
    modal.show();
}

$(document).ready(function () {
    $('#tableVacaciones tbody tr').each(function () {
        var estado = $(this).find("td").eq(7).text().trim(); // ← TAMBIÉN CORREGIDO A 7
        if (estado === "Aprobado" || estado === "Rechazado") {
            $(this).find('input[type="checkbox"]').prop("disabled", true);
            $(this).addClass('table-secondary'); // Color visual gris
        }
    });
});

function enviarAccionSolicitud(estado) {
    var id = $("#modalSolicitudId").val();
    var autorizar = estado === "Aprobado";

    doAjax(
        "/ERP/Vacaciones?handler=AutorizarSolicitud",
        { idSolicitud: parseInt(id), autorizar: autorizar },
        function (resp) {
            $("#modalAutorizar").modal("hide");
            mostrarConfirmacion(
                autorizar
                    ? "La solicitud fue autorizada exitosamente."
                    : "La solicitud fue rechazada correctamente."
            );
        },
        function (error) {
            console.error("Error al procesar:", error);
            showError("Error", "No se pudo procesar la solicitud.");
        },
        postOptions
    );
}

function mostrarConfirmacion(mensaje) {
    $("#modalConfirmacion").remove();

    var modalHtml = `
        <div class="modal fade" id="modalConfirmacion" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header bg-success text-white">
                        <h5 class="modal-title">Confirmación</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <p>${mensaje}</p>
                    </div>
                    <div class="modal-footer">
                        <button class="btn btn-primary" data-bs-dismiss="modal" onclick="location.reload()">Aceptar</button>
                    </div>
                </div>
            </div>
        </div>`;

    $("body").append(modalHtml);
    var confirmModal = new bootstrap.Modal(document.getElementById("modalConfirmacion"));
    confirmModal.show();
}

$(document).ready(function () {
    // Al finalizar la carga de datos en la tabla
    $('#tableVacaciones').on('post-body.bs.table', function () {
        $('#tableVacaciones tbody tr').each(function () {
            var estado = $(this).find("td").eq(7).text().trim(); // ← columna "Estatus"
            if (estado === "Aprobado" || estado === "Rechazado") {
                $(this).find('input[type="checkbox"]').prop("disabled", true);
                $(this).addClass('table-secondary');
            }
        });
    });
});


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

document.getElementById("inpFechaInicio").addEventListener("change", calcularDiasSolicitados);
document.getElementById("inpFechaFin").addEventListener("change", calcularDiasSolicitados);
