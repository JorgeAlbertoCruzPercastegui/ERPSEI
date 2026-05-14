var table;
var tableSolicitudesAutorizar;
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
    tableSolicitudesAutorizar = $("#tableSolicitudesAutorizar");
    buttonRemove = $("#remove");

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

    if (tableSolicitudesAutorizar.length) {
        initTableSolicitudesAutorizar();
    }

    obtenerDiasDisponibles();
    cargarResumenVacaciones();
    cargarVacacionesAcumuladas();
    cargarVacacionesTomadas();
    cargarAvisoVacacionesPorVencer();

    // Eventos para calcular días
    const inpFechaInicio = document.getElementById("inpFechaInicio");
    const inpFechaFin = document.getElementById("inpFechaFin");

    if (inpFechaInicio) inpFechaInicio.addEventListener("change", calcularDiasSolicitados);
    if (inpFechaFin) inpFechaFin.addEventListener("change", calcularDiasSolicitados);

    // Evento para vacaciones anticipadas
    $("#chkVacacionesAnticipadas").on("change", function () {
        toggleVacacionesAnticipadasAviso();
    });

    // Abrir detalle desde correo
    const params = new URLSearchParams(window.location.search);
    const solicitudId = params.get("solicitudId");
    const accionCorreo = params.get("accionCorreo");

    if (solicitudId) {
        setTimeout(() => {
            verDetalleVacacion(parseInt(solicitudId));

            if (accionCorreo === "aprobarJefe") {
                console.log("Abrir desde correo para aprobación jefe");
            }

            if (accionCorreo === "rechazarJefe") {
                console.log("Abrir desde correo para rechazo jefe");
            }
        }, 700);
    }
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

function estadoFormatter(value) {
    if (value === "Pendiente jefe directo") {
        return `<span class="badge bg-warning-subtle text-warning border border-warning-subtle">${value}</span>`;
    }
    if (value === "Pendiente TH") {
        return `<span class="badge bg-secondary-subtle text-secondary border border-secondary-subtle">${value}</span>`;
    }
    if (value === "Aprobado") {
        return `<span class="badge bg-success-subtle text-success border border-success-subtle">✔ ${value}</span>`;
    }
    if ((value || "").includes("Rechazado")) {
        return `<span class="badge bg-danger-subtle text-danger border border-danger-subtle">${value}</span>`;
    }
    return `<span class="badge bg-secondary">${value || ""}</span>`;
}

function accionesFormatter(value, row) {
    let html = `<div class="d-flex justify-content-center gap-2 flex-wrap">`;

    html += `<a href="javascript:void(0)" class="accion-ver" title="Ver"><i class="bi bi-search text-primary"></i></a>`;

    if (row.puedeEditar) {
        html += `<a href="javascript:void(0)" class="accion-editar" title="Editar"><i class="bi bi-pencil text-primary"></i></a>`;
    }

    if (row.puedeAprobarJefe) {
        html += `<a href="javascript:void(0)" class="accion-aprobar-jefe" title="Aprobar jefe directo"><i class="bi bi-check-circle text-secondary"></i></a>`;
        html += `<a href="javascript:void(0)" class="accion-rechazar-jefe" title="Rechazar jefe directo"><i class="bi bi-x-circle text-danger"></i></a>`;
    }

    if (row.puedeAprobarTH) {
        html += `<a href="javascript:void(0)" class="accion-aprobar-th" title="Aprobar TH"><i class="bi bi-check-circle text-success"></i></a>`;
        html += `<a href="javascript:void(0)" class="accion-rechazar-th" title="Rechazar TH"><i class="bi bi-x-circle text-danger"></i></a>`;
    }

    html += `</div>`;
    return html;
}
window.accionesEventsVacaciones = {
    'click .accion-ver': function (e, value, row) {
        verDetalleVacacion(row.id);
    },
    'click .accion-editar': function (e, value, row) {
        abrirEditarVacacion(row);
    },
    'click .accion-aprobar-jefe': function (e, value, row) {
        aprobarJefeDirectoVacacion(row.id);
    },
    'click .accion-rechazar-jefe': function (e, value, row) {
        rechazarJefeDirectoVacacion(row.id);
    },
    'click .accion-aprobar-th': function (e, value, row) {
        aprobarTHVacacion(row.id);
    },
    'click .accion-rechazar-th': function (e, value, row) {
        rechazarTHVacacion(row.id);
    }
};

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
                sortable: true
            },
            {
                title: "Fecha Solicitud",
                field: "fechaSolicitud",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Fecha Inicio",
                field: "fechaInicio",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Fecha Fin",
                field: "fechaFin",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Días Solicitados",
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
                sortable: true,
                formatter: estadoFormatter
            },
            {
                title: "Autorizador",
                field: "autorizador",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Comentario Empleado",
                field: "comentarioEmpleado",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Acciones",
                field: "acciones",
                align: "center",
                clickToSelect: false,
                events: window.accionesEventsVacaciones,
                formatter: accionesFormatter
            }
        ]
    });
}

function initTableSolicitudesAutorizar() {
    tableSolicitudesAutorizar.bootstrapTable('destroy').bootstrapTable({
        height: 350,
        locale: cultureName,
        columns: [
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
                sortable: true
            },
            {
                title: "Fecha Solicitud",
                field: "fechaSolicitud",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Fecha Inicio",
                field: "fechaInicio",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Fecha Fin",
                field: "fechaFin",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Días Solicitados",
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
                sortable: true,
                formatter: estadoFormatter
            },
            {
                title: "Autorizador",
                field: "autorizador",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Comentario Empleado",
                field: "comentarioEmpleado",
                align: "center",
                valign: "middle",
                sortable: true
            },
            {
                title: "Acciones",
                field: "acciones",
                align: "center",
                clickToSelect: false,
                events: window.accionesEventsVacaciones,
                formatter: accionesFormatter
            }
        ]
    });
}

function refrescarTablasVacaciones() {
    if (table && table.length) {
        table.bootstrapTable('refresh');
    }

    if (tableSolicitudesAutorizar && tableSolicitudesAutorizar.length) {
        tableSolicitudesAutorizar.bootstrapTable('refresh');
    }
}

function aprobarJefeDirectoVacacion(idSolicitud) {
    $.ajax({
        url: `/ERP/Vacaciones?handler=AprobarJefeDirecto&idSolicitud=${idSolicitud}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.tieneError) {
                showError("Vacaciones", resp.mensaje);
                return;
            }

            showSuccess("Vacaciones", resp.mensaje);
            refrescarTablasVacaciones();
            cargarVacacionesTomadas();
            cargarResumenVacaciones();
            cargarAvisoVacacionesPorVencer();
        },
        error: function () {
            showError("Vacaciones", "No se pudo aprobar la solicitud.");
        }
    });
}

function abrirEditarVacacion(row) {
    const modal = new bootstrap.Modal(document.getElementById("dlgEditarVacacion"));

    $("#editValidationSummary").html("");
    $("#editSolicitudId").val(row.id || "");

    if (row.fechaInicio && row.fechaInicio.includes("/")) {
        const [dia, mes, anio] = row.fechaInicio.split("/");
        $("#editFechaInicio").val(`${anio}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}`);
    } else {
        $("#editFechaInicio").val("");
    }

    if (row.fechaFin && row.fechaFin.includes("/")) {
        const [dia, mes, anio] = row.fechaFin.split("/");
        $("#editFechaFin").val(`${anio}-${mes.padStart(2, "0")}-${dia.padStart(2, "0")}`);
    } else {
        $("#editFechaFin").val("");
    }

    $("#editComentarioEmpleado").val(row.comentarioEmpleado || "");
    calcularDiasEditarVacacion();

    modal.show();
}

function calcularDiasEditarVacacion() {
    const inicio = new Date($("#editFechaInicio").val());
    const fin = new Date($("#editFechaFin").val());

    if (isNaN(inicio) || isNaN(fin) || fin < inicio) {
        $("#editDiasSolicitadosTexto").text("0");
        return;
    }

    let totalDias = 0;
    let fecha = new Date(inicio);

    while (fecha <= fin) {
        const dia = fecha.getDay();
        if (dia !== 0 && dia !== 6) {
            totalDias++;
        }
        fecha.setDate(fecha.getDate() + 1);
    }

    $("#editDiasSolicitadosTexto").text(totalDias);
}

function guardarEdicionVacacion() {
    $.ajax({
        url: "/ERP/Vacaciones?handler=EditarSolicitud",
        type: "POST",
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        data: {
            "InputEditarSolicitud.Id": $("#editSolicitudId").val(),
            "InputEditarSolicitud.FechaInicio": $("#editFechaInicio").val(),
            "InputEditarSolicitud.FechaFin": $("#editFechaFin").val(),
            "InputEditarSolicitud.ComentarioEmpleado": $("#editComentarioEmpleado").val()
        },
        success: function (resp) {
            if (resp.tieneError) {
                showError("Editar Vacación", resp.mensaje);
                return;
            }

            bootstrap.Modal.getInstance(document.getElementById("dlgEditarVacacion"))?.hide();
            showSuccess("Editar Vacación", resp.mensaje);
            refrescarTablasVacaciones();
            cargarVacacionesTomadas();
            cargarResumenVacaciones();
            cargarAvisoVacacionesPorVencer();
        },
        error: function () {
            showError("Editar Vacación", "No se pudo guardar la edición.");
        }
    });
}

function verDetalleVacacion(id) {
    $.get(`/ERP/Vacaciones?handler=DetalleVacacion&id=${id}`, function (resp) {
        if (resp.tieneError) {
            showError("Vacaciones", resp.mensaje);
            return;
        }

        $("#detVacEmpleado").text(resp.empleado || "");
        $("#detVacEstado").text(resp.estado || "");
        $("#detVacFechaSolicitud").text(resp.fechaSolicitud || "");
        $("#detVacAutorizador").text(resp.autorizador || "");
        $("#detVacFechaInicio").text(resp.fechaInicio || "");
        $("#detVacFechaFin").text(resp.fechaFin || "");
        $("#detVacDias").text(resp.diasSolicitados || "");
        $("#detVacComentario").text(resp.comentario || "");

        const contenedor = $("#detalleAccionesVacacion");
        contenedor.empty();

        if (resp.puedeAprobarJefe) {
            contenedor.append(`
                <button type="button" class="btn btn-secondary" onclick="aprobarJefeDirectoVacacion(${resp.id})">Aprobar jefe directo</button>
                <button type="button" class="btn btn-outline-danger" onclick="rechazarJefeDirectoVacacion(${resp.id})">Rechazar jefe directo</button>
            `);
        }

        if (resp.puedeAprobarTH) {
            contenedor.append(`
                <button type="button" class="btn btn-success" onclick="aprobarTHVacacion(${resp.id})">Aprobar TH</button>
                <button type="button" class="btn btn-outline-danger" onclick="rechazarTHVacacion(${resp.id})">Rechazar TH</button>
            `);
        }

        new bootstrap.Modal(document.getElementById("modalDetalleVacacion")).show();
    });
}

function rechazarJefeDirectoVacacion(idSolicitud) {
    $.ajax({
        url: `/ERP/Vacaciones?handler=RechazarJefeDirecto&idSolicitud=${idSolicitud}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.tieneError) {
                showError("Vacaciones", resp.mensaje);
                return;
            }

            showSuccess("Vacaciones", resp.mensaje);
            refrescarTablasVacaciones();
            cargarVacacionesTomadas();
            cargarResumenVacaciones();
            cargarAvisoVacacionesPorVencer();
        },
        error: function () {
            showError("Vacaciones", "No se pudo rechazar la solicitud.");
        }
    });
}

function aprobarTHVacacion(idSolicitud) {
    $.ajax({
        url: `/ERP/Vacaciones?handler=AprobarTH&idSolicitud=${idSolicitud}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.tieneError) {
                showError("Vacaciones", resp.mensaje);
                return;
            }

            showSuccess("Vacaciones", resp.mensaje);
            refrescarTablasVacaciones();
            cargarVacacionesTomadas();
            cargarResumenVacaciones();
            cargarAvisoVacacionesPorVencer();
        },
        error: function () {
            showError("Vacaciones", "No se pudo aprobar la solicitud en TH.");
        }
    });
}

function rechazarTHVacacion(idSolicitud) {
    $.ajax({
        url: `/ERP/Vacaciones?handler=RechazarTH&idSolicitud=${idSolicitud}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
        },
        success: function (resp) {
            if (resp.tieneError) {
                showError("Vacaciones", resp.mensaje);
                return;
            }

            showSuccess("Vacaciones", resp.mensaje);
            refrescarTablasVacaciones();
            cargarVacacionesTomadas();
            cargarResumenVacaciones();
            cargarAvisoVacacionesPorVencer();
        },
        error: function () {
            showError("Vacaciones", "No se pudo rechazar la solicitud en TH.");
        }
    });
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

    // ✅ Limpiar vacaciones anticipadas
    $("#chkVacacionesAnticipadas").prop("checked", false);
    $("#rowAvisoVacacionesAnticipadas").addClass("d-none");

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

        if (totalDias > diasDisponiblesActuales && !$("#chkVacacionesAnticipadas").is(":checked")) {
            output.innerHTML = `<span class="text-danger">${totalDias} días (excede saldo disponible de ${diasDisponiblesActuales.toFixed(1)} días)</span>`;
        } else {
            if ($("#chkVacacionesAnticipadas").is(":checked")) {
                output.innerHTML = `<span class="text-warning fw-bold">${totalDias} días (vacaciones anticipadas)</span>`;
            } else {
                output.innerText = `${totalDias}`;
            }
        }

        /*if (totalDias > diasDisponiblesActuales) {
            output.innerHTML = `<span class="text-danger">${totalDias} días (excede saldo disponible de ${diasDisponiblesActuales.toFixed(1)} días)</span>`;
        } else {
            output.innerText = `${totalDias}`;
        }*/

        // Actualizar "Tienes X días disponibles"
        lblDisponibles.innerText = restante.toFixed(1);

        // Actualizar "Total Saldo"
        tdSaldo.innerText = `${restante.toFixed(1)} días`;
    } else {
        output.innerText = "0";
        lblDisponibles.innerText = diasDisponiblesActuales.toFixed(1);
        tdSaldo.innerText = `${diasDisponiblesActuales.toFixed(1)} días`;
    }
}

function toggleVacacionesAnticipadasAviso() {
    const checked = $("#chkVacacionesAnticipadas").is(":checked");

    if (checked) {
        $("#rowAvisoVacacionesAnticipadas").removeClass("d-none");
    } else {
        $("#rowAvisoVacacionesAnticipadas").addClass("d-none");
    }
}

async function obtenerDiasDisponibles() {
    try {
        const response = await fetch("/ERP/Vacaciones?handler=ObtenerDiasDisponibles");
        const dias = await response.json();

        diasDisponiblesActuales = dias;
        document.getElementById("lblDiasDisponibles").innerText = dias.toFixed(1);

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
            //document.getElementById("tdVencidas").innerText = `0.0 días`;
            document.getElementById("tdVencidas").innerText = `${(resp.vencidas || 0).toFixed(1)} días`;
            document.getElementById("tdFuturas").innerText = `${(resp.futuras || 0).toFixed(1)} días`;
            //document.getElementById("tdFuturas").innerText = `0.0 días`;
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
            EmpleadoId: parseInt(empleadoId),
            EsVacacionAnticipada: $("#chkVacacionesAnticipadas").is(":checked")
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
            
            $("#chkVacacionesAnticipadas").prop("checked", false);
            $("#rowAvisoVacacionesAnticipadas").addClass("d-none");

            // Refrescar tabla principal
            document.querySelector("[name='refresh']").click();

            // Mostrar mensaje de éxito
            showSuccess(dlgTitle.innerHTML, resp.mensaje);

            // Esperar un poco para que el backend se actualice antes de recargar el resumen
            setTimeout(() => {
                obtenerDiasDisponibles();       // Actualiza lblDiasDisponibles
                cargarResumenVacaciones();      // Actualiza la tabla y saldo total
                cargarVacacionesTomadas();         // ✅ Actualiza tabla de vacaciones tomadas
                cargarAvisoVacacionesPorVencer();
            }, 300); // puede ajustarse a 500ms si necesario
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
}

async function cargarPoliticaVacaciones(tipoVacacion = "Legales") {
    try {
        const response = await fetch(`/ERP/Vacaciones?handler=PoliticaVacaciones&tipoVacacion=${encodeURIComponent(tipoVacacion)}`);
        const data = await response.json();

        const tbody = document.getElementById("tbodyPoliticasVacaciones");
        const lblNombrePolitica = document.getElementById("lblNombrePolitica");

        if (!tbody || !lblNombrePolitica) return;

        tbody.innerHTML = "";

        if (data.error) {
            lblNombrePolitica.innerText = "Sin política";
            tbody.innerHTML = `<tr><td colspan="4" class="text-center text-danger">${data.mensaje}</td></tr>`;
            return;
        }

        lblNombrePolitica.innerText = data.nombre;

        if (!data.detalles || data.detalles.length === 0) {
            tbody.innerHTML = `<tr><td colspan="4" class="text-center text-muted">No hay renglones configurados para esta política.</td></tr>`;
            return;
        }

        data.detalles.forEach(item => {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${parseFloat(item.aniosAntiguedad).toFixed(1)}</td>
                <td>${parseFloat(item.diasVacaciones).toFixed(1).replace('.0', '')}</td>
                <td>${parseFloat(item.primaVacacional).toFixed(2)}</td>
                <td>${parseFloat(item.diasAguinaldo).toFixed(1).replace('.0', '')}</td>
            `;
            tbody.appendChild(row);
        });
    } catch (error) {
        console.error("Error al cargar política de vacaciones:", error);

        const tbody = document.getElementById("tbodyPoliticasVacaciones");
        const lblNombrePolitica = document.getElementById("lblNombrePolitica");

        if (tbody) {
            tbody.innerHTML = `<tr><td colspan="4" class="text-center text-danger">Error al cargar la política.</td></tr>`;
        }

        if (lblNombrePolitica) {
            lblNombrePolitica.innerText = "Error";
        }
    }
}

document.addEventListener("DOMContentLoaded", function () {
    const selTipoPolitica = document.getElementById("selTipoPolitica");
    const modalVerPoliticas = document.getElementById("modalVerPoliticas");

    if (modalVerPoliticas) {
        modalVerPoliticas.addEventListener("shown.bs.modal", function () {
            const tipo = selTipoPolitica ? selTipoPolitica.value : "Legales";
            cargarPoliticaVacaciones(tipo);
        });
    }

    if (selTipoPolitica) {
        selTipoPolitica.addEventListener("change", function () {
            cargarPoliticaVacaciones(this.value);
        });
    }
});

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

/*function onAutorizarVacacionesClick() {
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
}*/

async function cargarAsignacionVacacionesActual() {
    try {
        const response = await fetch("/ERP/Vacaciones?handler=ObtenerAsignacionVacaciones");
        const data = await response.json();

        if (data.error) {
            showError("Asignación", data.mensaje);
            return;
        }

        const sel = document.getElementById("selTipoAsignacionVacaciones");

        if (sel) {
            sel.value = data.tipoAsignacion || "LegalesProporcionales";
        }
    } catch (error) {
        console.error("Error al cargar asignación de vacaciones:", error);
    }
}

function guardarAsignacionVacaciones() {
    const sel = document.getElementById("selTipoAsignacionVacaciones");
    const tipoAsignacion = sel ? sel.value : "LegalesProporcionales";

    doAjax(
        "/ERP/Vacaciones?handler=GuardarAsignacionVacaciones",
        { tipoAsignacion: tipoAsignacion },
        function (resp) {
            if (resp.tieneError) {
                showError("Asignación de vacaciones", resp.mensaje);
                return;
            }

            const modalElement = document.getElementById("modalAsignacionVacaciones");
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            if (modalInstance) {
                modalInstance.hide();
            }

            cargarResumenVacaciones();
            obtenerDiasDisponibles();
            cargarVacacionesAcumuladas();
            cargarAvisoVacacionesPorVencer();

            showSuccess("Asignación de vacaciones", resp.mensaje);
        },
        function (error) {
            showError("Error", error);
        },
        postOptions
    );
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

/*function enviarAccionSolicitud(estado) {
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
}*/

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

document.addEventListener("DOMContentLoaded", function () {
    const selTipoPolitica = document.getElementById("selTipoPolitica");

    if (selTipoPolitica) {
        selTipoPolitica.addEventListener("change", function () {
            const tipo = this.value;
            const titulo = document.querySelector("#modalVerPoliticas h5.mb-3.text-primary");

            if (titulo) {
                titulo.textContent = tipo === "Anuales" ? "Política Anual" : "Legal 2023";
            }
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const editFechaInicio = document.getElementById("editFechaInicio");
    const editFechaFin = document.getElementById("editFechaFin");

    if (editFechaInicio) editFechaInicio.addEventListener("change", calcularDiasEditarVacacion);
    if (editFechaFin) editFechaFin.addEventListener("change", calcularDiasEditarVacacion);
});

function exportarDetalleVacaciones() {
    window.location.href = "/ERP/Vacaciones?handler=ExportarDetalleVacaciones";
}

function exportarHistorialVacacionesUsuarios() {
    window.location.href = "/ERP/Vacaciones?handler=ExportarHistorialVacacionesUsuarios";
}

function cargarAvisoVacacionesPorVencer() {
    $.get("/ERP/Vacaciones?handler=AvisoVacacionesPorVencer", function (resp) {

        if (!resp || !resp.mostrar) {
            $("#rowAvisoVacacionesPorVencer").addClass("d-none");
            return;
        }

        $("#lblDiasPorVencer").text(resp.dias);
        $("#lblFechaVencimientoVacaciones").text(resp.fechaVencimiento);

        const mensaje = resp.mensaje ||
            `Tienes ${resp.dias} día(s) de vacaciones próximos a vencer. Fecha límite: ${resp.fechaVencimiento}.`;

        $("#rowAvisoVacacionesPorVencer .alert").html(`
            <i class="bi bi-exclamation-triangle-fill me-2"></i>
            ${mensaje}
        `);

        $("#rowAvisoVacacionesPorVencer").removeClass("d-none");
    });
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

document.getElementById("inpFechaInicio").addEventListener("change", calcularDiasSolicitados);
document.getElementById("inpFechaFin").addEventListener("change", calcularDiasSolicitados);