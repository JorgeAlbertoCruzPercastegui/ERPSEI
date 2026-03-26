var tableDias;
var tableHoras;
var tableAutorizar;
var tiposAusenciaCache = [];

document.addEventListener("DOMContentLoaded", function () {
    tableDias = $("#tableAusenciasDias");
    tableHoras = $("#tableAusenciasHoras");
    tableAutorizar = $("#tableAusenciasAutorizar");

    initTableDias();
    initTableHoras();

    if (tableAutorizar.length) {
        initTableAutorizar();
    }

    cargarTiposAusencia();
    configurarEventos();

    $(".campo-horas-permiso").show();
    $(".campo-dias-permiso").show();
    $(".campo-horas-solicitud").show();

    // 🔥 NUEVO BLOQUE (AGREGAR AQUÍ)
    const params = new URLSearchParams(window.location.search);
    const ausenciaId = params.get("ausenciaId");
    const accionCorreo = params.get("accionCorreo");

    if (ausenciaId) {
        setTimeout(() => {
            verDetalle(ausenciaId);

            if (accionCorreo === "aprobarJefe") {
                console.log("Abrir detalle para aprobar como jefe");
            }

            if (accionCorreo === "rechazarJefe") {
                console.log("Abrir detalle para rechazar como jefe");
            }
        }, 700);
    }
});

function configurarEventos() {
    $("#inpInasistenciaFechaInicio, #inpInasistenciaFechaFin").on("change", function () {
        calcularDias("#inpInasistenciaFechaInicio", "#inpInasistenciaFechaFin", "#inpInasistenciaDias", "#lblDiasInasistencia");
    });

    $("#inpIncapacidadFechaInicio, #inpIncapacidadFechaFin").on("change", function () {
        calcularDias("#inpIncapacidadFechaInicio", "#inpIncapacidadFechaFin", "#inpDiasIncapacidad", "#lblDiasIncapacidad");
    });

    $("#inpPermisoFechaInicio, #inpPermisoFechaFin").on("change", function () {
        calcularDias("#inpPermisoFechaInicio", "#inpPermisoFechaFin", "#inpPermisoDiasAporte", "#lblDiasPermiso");
    });

    $("#inpSolicitudPermisoFechaInicio, #inpSolicitudPermisoFechaFin").on("change", function () {
        calcularSoloLabel("#inpSolicitudPermisoFechaInicio", "#inpSolicitudPermisoFechaFin", "#lblDiasSolicitudPermiso");
    });

    $("#selTipoAusenciaPermiso").on("change", function () {
        alternarCamposPermiso($(this).val(), false);
    });

    $("#selTipoAusenciaSolicitud").on("change", function () {
        alternarCamposPermiso($(this).val(), true);
    });

    $("#editTipoAusenciaId").on("change", function () {
        alternarCamposEdicion($(this).val());
    });
}

function initTableDias() {
    tableDias.bootstrapTable('destroy').bootstrapTable({
        height: 220,
        locale: cultureName,
        columns: [
            { field: "tipo", title: "Tipo", align: "left" },
            { field: "fechaInicio", title: "Fecha inicio", align: "center" },
            { field: "fechaFin", title: "Fecha término", align: "center" },
            { field: "dias", title: "Días", align: "center" },
            { field: "estado", title: "Estado", align: "center", formatter: estadoFormatter },
            { field: "acciones", title: "Acciones", align: "center", formatter: accionesFormatter, events: accionesEvents }
        ]
    });
}

function initTableHoras() {
    tableHoras.bootstrapTable('destroy').bootstrapTable({
        height: 260,
        locale: cultureName,
        columns: [
            { field: "tipo", title: "Tipo", align: "left" },
            { field: "fechaInicio", title: "Fecha inicio", align: "center" },
            { field: "horaInicio", title: "Hora inicio", align: "center" },
            { field: "horaTermino", title: "Hora término", align: "center" },
            { field: "horas", title: "Horas", align: "center" },
            { field: "estado", title: "Estado", align: "center", formatter: estadoFormatter },
            { field: "acciones", title: "Acciones", align: "center", formatter: accionesFormatter, events: accionesEvents }
        ]
    });
}

function initTableAutorizar() {
    tableAutorizar.bootstrapTable('destroy').bootstrapTable({
        height: 260,
        locale: cultureName,
        columns: [
            { field: "empleado", title: "Empleado", align: "left" },
            { field: "categoria", title: "Categoría", align: "center" },
            { field: "tipo", title: "Tipo", align: "left" },
            {
                field: "periodo",
                title: "Periodo",
                align: "center",
                formatter: function (value, row) {
                    if (row.captura === "Horas") {
                        return `${row.horaInicio || ""} ${row.horaTermino ? " - " + row.horaTermino : ""}`;
                    }
                    return `${row.fechaInicio || ""} ${row.fechaFin ? " al " + row.fechaFin : ""}`;
                }
            },
            {
                field: "duracion",
                title: "Duración",
                align: "center",
                formatter: function (value, row) {
                    if (row.captura === "Horas") {
                        return row.horas || "";
                    }
                    return row.dias || "";
                }
            },
            { field: "estado", title: "Estado", align: "center", formatter: estadoFormatter },
            { field: "acciones", title: "Acciones", align: "center", formatter: accionesFormatter, events: accionesEvents }
        ]
    });
}

function estadoFormatter(value) {
    if (value === "Pendiente jefe directo") {
        return `<span class="badge bg-warning-subtle text-warning border border-warning-subtle">${value}</span>`;
    }
    if (value === "Aprobado por jefe directo") {
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

    if (row.puedeEliminar) {
        html += `<a href="javascript:void(0)" class="accion-eliminar" title="Eliminar"><i class="bi bi-trash text-danger"></i></a>`;
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

window.accionesEvents = {
    'click .accion-ver': function (e, value, row) {
        verDetalle(row.id);
    },
    'click .accion-editar': function (e, value, row) {
        abrirEdicion(row.id);
    },
    'click .accion-eliminar': function (e, value, row) {
        eliminarAusencia(row.id);
    },
    'click .accion-aprobar-jefe': function (e, value, row) {
        aprobarJefeDirecto(row.id);
    },
    'click .accion-rechazar-jefe': function (e, value, row) {
        rechazarJefeDirecto(row.id);
    },
    'click .accion-aprobar-th': function (e, value, row) {
        aprobarTH(row.id);
    },
    'click .accion-rechazar-th': function (e, value, row) {
        rechazarTH(row.id);
    }
};

function abrirModalInasistencia() {
    new bootstrap.Modal(document.getElementById("modalInasistencia")).show();
}

function abrirModalIncapacidad() {
    new bootstrap.Modal(document.getElementById("modalIncapacidad")).show();
}

function abrirModalPermiso() {
    new bootstrap.Modal(document.getElementById("modalPermiso")).show();
}

function abrirModalSolicitarPermiso() {
    new bootstrap.Modal(document.getElementById("modalSolicitarPermiso")).show();
}

function getToken(formId) {
    return $(`#${formId} input[name="__RequestVerificationToken"]`).val();
}

function refrescarTablas() {
    tableDias.bootstrapTable('refresh');
    tableHoras.bootstrapTable('refresh');

    if (tableAutorizar && tableAutorizar.length) {
        tableAutorizar.bootstrapTable('refresh');
    }
}

function mostrarResultado(esError, mensaje, titulo = null) {
    $("#modalResultadoTitulo").text(titulo || (esError ? "Ocurrió un problema" : "Proceso correcto"));
    $("#modalResultadoMensaje").text(mensaje);

    if (esError) {
        $("#modalResultadoIcono").html(`<i class="bi bi-x-circle-fill text-danger"></i>`);
    } else {
        $("#modalResultadoIcono").html(`<i class="bi bi-check-circle-fill text-success"></i>`);
    }

    new bootstrap.Modal(document.getElementById("modalResultado")).show();
}

function calcularDias(selectorInicio, selectorFin, selectorDias, selectorLabel) {
    const inicio = $(selectorInicio).val();
    const fin = $(selectorFin).val();

    if (!inicio || !fin) {
        $(selectorDias).val("");
        $(selectorLabel).text("0");
        return;
    }

    const fechaInicio = new Date(inicio + "T00:00:00");
    const fechaFin = new Date(fin + "T00:00:00");

    if (fechaFin < fechaInicio) {
        $(selectorDias).val("");
        $(selectorLabel).text("0");
        return;
    }

    const diferencia = Math.floor((fechaFin - fechaInicio) / (1000 * 60 * 60 * 24)) + 1;
    $(selectorDias).val(diferencia);
    $(selectorLabel).text(diferencia);
}

function calcularSoloLabel(selectorInicio, selectorFin, selectorLabel) {
    const inicio = $(selectorInicio).val();
    const fin = $(selectorFin).val();

    if (!inicio || !fin) {
        $(selectorLabel).text("0");
        return;
    }

    const fechaInicio = new Date(inicio + "T00:00:00");
    const fechaFin = new Date(fin + "T00:00:00");

    if (fechaFin < fechaInicio) {
        $(selectorLabel).text("0");
        return;
    }

    const diferencia = Math.floor((fechaFin - fechaInicio) / (1000 * 60 * 60 * 24)) + 1;
    $(selectorLabel).text(diferencia);
}

function cargarTiposAusencia() {
    $.get("/ERP/Ausencias?handler=TiposAusencia", function (resp) {
        tiposAusenciaCache = resp || [];
    });
}

function alternarCamposPermiso(tipoAusenciaId, esSolicitud) {
    if (!tipoAusenciaId || !tiposAusenciaCache.length) return;

    const item = tiposAusenciaCache.find(x => x.id == tipoAusenciaId);
    if (!item) return;

    if (esSolicitud) {
        if (item.manejaHoras) {
            $(".campo-horas-solicitud").show();
        } else {
            $(".campo-horas-solicitud").hide();
            $("#inpSolicitudPermisoHoraInicio").val("");
            $("#inpSolicitudPermisoHoraTermino").val("");
        }
    } else {
        if (item.manejaHoras) {
            $(".campo-horas-permiso").show();
            $(".campo-dias-permiso").hide();
            $("#inpPermisoDiasAporte").val("");
            $("#lblDiasPermiso").text("0");
        } else {
            $(".campo-horas-permiso").hide();
            $("#inpPermisoHoraInicio").val("");
            $("#inpPermisoHoraTermino").val("");
            $(".campo-dias-permiso").show();
        }
    }
}

function alternarCamposEdicion(tipoAusenciaId) {
    if (!tipoAusenciaId || !tiposAusenciaCache.length) {
        $(".edit-horas").show();
        $(".edit-dias").show();
        return;
    }

    const item = tiposAusenciaCache.find(x => x.id == tipoAusenciaId);
    if (!item) return;

    if (item.manejaHoras) {
        $(".edit-horas").show();
        $(".edit-dias").hide();
        $("#editDias").val("");
    } else {
        $(".edit-horas").hide();
        $("#editHoraInicio").val("");
        $("#editHoraTermino").val("");
        $(".edit-dias").show();
    }
}

function guardarInasistencia() {
    $.ajax({
        url: "/ERP/Ausencias?handler=GuardarInasistencia",
        type: "POST",
        headers: { "RequestVerificationToken": getToken("formInasistencia") },
        data: {
            "InasistenciaInput.FechaInicio": $("#inpInasistenciaFechaInicio").val(),
            "InasistenciaInput.FechaFin": $("#inpInasistenciaFechaFin").val(),
            "InasistenciaInput.Dias": $("#inpInasistenciaDias").val(),
            "InasistenciaInput.FechaAplicacion": $("#inpInasistenciaFechaAplicacion").val(),
            "InasistenciaInput.Suplencia": $("#chkSuplenciaInasistencia").is(":checked"),
            "InasistenciaInput.Comentario": $("#txtComentarioInasistencia").val()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                bootstrap.Modal.getInstance(document.getElementById("modalInasistencia"))?.hide();
                $("#formInasistencia")[0].reset();
                $("#lblDiasInasistencia").text("0");
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        }
    });
}

function guardarIncapacidad() {
    $.ajax({
        url: "/ERP/Ausencias?handler=GuardarIncapacidad",
        type: "POST",
        headers: { "RequestVerificationToken": getToken("formIncapacidad") },
        data: {
            "IncapacidadInput.FechaInicio": $("#inpIncapacidadFechaInicio").val(),
            "IncapacidadInput.FechaFin": $("#inpIncapacidadFechaFin").val(),
            "IncapacidadInput.TipoIncapacidadId": $("#selTipoIncapacidad").val(),
            "IncapacidadInput.NumeroFolio": $("#inpNumeroFolioIncapacidad").val(),
            "IncapacidadInput.Dias": $("#inpDiasIncapacidad").val(),
            "IncapacidadInput.FechaAplicacion": $("#inpFechaAplicacionIncapacidad").val(),
            "IncapacidadInput.Suplencia": $("#chkSuplenciaIncapacidad").is(":checked"),
            "IncapacidadInput.Comentario": $("#txtComentarioIncapacidad").val()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                bootstrap.Modal.getInstance(document.getElementById("modalIncapacidad"))?.hide();
                $("#formIncapacidad")[0].reset();
                $("#lblDiasIncapacidad").text("0");
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        }
    });
}

function guardarPermiso() {
    $.ajax({
        url: "/ERP/Ausencias?handler=GuardarPermiso",
        type: "POST",
        headers: { "RequestVerificationToken": getToken("formPermiso") },
        data: {
            "PermisoInput.TipoAusenciaId": $("#selTipoAusenciaPermiso").val(),
            "PermisoInput.FechaInicio": $("#inpPermisoFechaInicio").val(),
            "PermisoInput.FechaFin": $("#inpPermisoFechaFin").val(),
            "PermisoInput.HoraInicio": $("#inpPermisoHoraInicio").val(),
            "PermisoInput.HoraTermino": $("#inpPermisoHoraTermino").val(),
            "PermisoInput.Dias": $("#inpPermisoDiasAporte").val(),
            "PermisoInput.FechaAplicacion": $("#inpPermisoFechaAplicacion").val(),
            "PermisoInput.Suplencia": $("#chkSuplenciaPermiso").is(":checked"),
            "PermisoInput.Comentario": $("#txtComentarioPermiso").val()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                bootstrap.Modal.getInstance(document.getElementById("modalPermiso"))?.hide();
                $("#formPermiso")[0].reset();
                $("#lblDiasPermiso").text("0");
                $(".campo-horas-permiso").show();
                $(".campo-dias-permiso").show();
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        }
    });
}

function solicitarPermiso() {
    $.ajax({
        url: "/ERP/Ausencias?handler=SolicitarPermiso",
        type: "POST",
        headers: { "RequestVerificationToken": getToken("formSolicitarPermiso") },
        data: {
            "SolicitudPermisoInput.TipoAusenciaId": $("#selTipoAusenciaSolicitud").val(),
            "SolicitudPermisoInput.FechaInicio": $("#inpSolicitudPermisoFechaInicio").val(),
            "SolicitudPermisoInput.FechaFin": $("#inpSolicitudPermisoFechaFin").val(),
            "SolicitudPermisoInput.HoraInicio": $("#inpSolicitudPermisoHoraInicio").val(),
            "SolicitudPermisoInput.HoraTermino": $("#inpSolicitudPermisoHoraTermino").val(),
            "SolicitudPermisoInput.FechaAplicacion": $("#inpSolicitudPermisoFechaAplicacion").val(),
            "SolicitudPermisoInput.Comentario": $("#txtComentarioSolicitudPermiso").val()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                bootstrap.Modal.getInstance(document.getElementById("modalSolicitarPermiso"))?.hide();
                $("#formSolicitarPermiso")[0].reset();
                $("#lblDiasSolicitudPermiso").text("0");
                $(".campo-horas-solicitud").show();
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        }
    });
}

function verDetalle(id) {
    $.get(`/ERP/Ausencias?handler=Detalle&id=${id}`, function (resp) {
        if (resp.tieneError) {
            mostrarResultado(true, resp.mensaje);
            return;
        }

        $("#detEmpleado").text(resp.empleado || "");
        $("#detCategoria").text(resp.categoria || "");
        $("#detTipo").text(resp.tipo || "");
        $("#detEstado").text(resp.estado || "");
        $("#detFechaInicio").text(resp.fechaInicio || "");
        $("#detFechaFin").text(resp.fechaFin || "");
        $("#detHoraInicio").text(resp.horaInicio || "");
        $("#detHoraTermino").text(resp.horaTermino || "");
        $("#detDias").text(resp.dias ?? "");
        $("#detHoras").text(resp.horas ?? "");
        $("#detFechaAplicacion").text(resp.fechaAplicacion || "");
        $("#detSuplencia").text(resp.suplencia ? "Sí" : "No");
        $("#detFolio").text(resp.numeroFolio || "");
        $("#detUsuarioCreador").text(resp.usuarioCreador || "");
        $("#detComentario").text(resp.comentario || "");

        const contenedor = $("#detalleAccionesAprobacion");
        contenedor.empty();

        if (resp.puedeAprobarJefe) {
            contenedor.append(`
                <button type="button" class="btn btn-secondary" onclick="aprobarJefeDirecto(${resp.id})">Aprobar jefe directo</button>
                <button type="button" class="btn btn-outline-danger" onclick="rechazarJefeDirecto(${resp.id})">Rechazar jefe directo</button>
            `);
        }

        if (resp.puedeAprobarTH) {
            contenedor.append(`
                <button type="button" class="btn btn-success" onclick="aprobarTH(${resp.id})">Aprobar TH</button>
                <button type="button" class="btn btn-outline-danger" onclick="rechazarTH(${resp.id})">Rechazar TH</button>
            `);
        }

        new bootstrap.Modal(document.getElementById("modalDetalleAusencia")).show();
    });
}

function abrirEdicion(id) {
    $.get(`/ERP/Ausencias?handler=Detalle&id=${id}`, function (resp) {
        if (resp.tieneError) {
            mostrarResultado(true, resp.mensaje);
            return;
        }

        $("#editId").val(resp.id);
        $("#editTipoAusenciaId").val(resp.tipoAusenciaId || "");
        $("#editTipoIncapacidadId").val(resp.tipoIncapacidadId || "");
        $("#editFechaInicio").val(resp.fechaInicio || "");
        $("#editFechaFin").val(resp.fechaFin || "");
        $("#editHoraInicio").val(resp.horaInicio || "");
        $("#editHoraTermino").val(resp.horaTermino || "");
        $("#editDias").val(resp.dias || "");
        $("#editFechaAplicacion").val(resp.fechaAplicacion || "");
        $("#editNumeroFolio").val(resp.numeroFolio || "");
        $("#editSuplencia").prop("checked", resp.suplencia === true);
        $("#editComentario").val(resp.comentario || "");

        alternarCamposEdicion(resp.tipoAusenciaId);

        new bootstrap.Modal(document.getElementById("modalEditarAusencia")).show();
    });
}

function guardarEdicionAusencia() {
    $.ajax({
        url: "/ERP/Ausencias?handler=Editar",
        type: "POST",
        headers: { "RequestVerificationToken": getToken("formEditarAusencia") },
        data: {
            "EditarInput.Id": $("#editId").val(),
            "EditarInput.TipoAusenciaId": $("#editTipoAusenciaId").val(),
            "EditarInput.TipoIncapacidadId": $("#editTipoIncapacidadId").val(),
            "EditarInput.FechaInicio": $("#editFechaInicio").val(),
            "EditarInput.FechaFin": $("#editFechaFin").val(),
            "EditarInput.HoraInicio": $("#editHoraInicio").val(),
            "EditarInput.HoraTermino": $("#editHoraTermino").val(),
            "EditarInput.Dias": $("#editDias").val(),
            "EditarInput.FechaAplicacion": $("#editFechaAplicacion").val(),
            "EditarInput.NumeroFolio": $("#editNumeroFolio").val(),
            "EditarInput.Suplencia": $("#editSuplencia").is(":checked"),
            "EditarInput.Comentario": $("#editComentario").val()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                bootstrap.Modal.getInstance(document.getElementById("modalEditarAusencia"))?.hide();
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        }
    });
}

function eliminarAusencia(id) {
    if (!confirm("¿Deseas eliminar este registro?")) return;

    $.ajax({
        url: `/ERP/Ausencias?handler=Eliminar&id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": getMainToken()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        },
        error: function (xhr) {
            mostrarResultado(true, `Error ${xhr.status}: no fue posible eliminar el registro.`);
        }
    });
}

function aprobarJefeDirecto(id) {
    $.ajax({
        url: `/ERP/Ausencias?handler=AprobarJefeDirecto&id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": getMainToken()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                bootstrap.Modal.getInstance(document.getElementById("modalDetalleAusencia"))?.hide();
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        },
        error: function (xhr) {
            mostrarResultado(true, `Error ${xhr.status}: no fue posible aprobar el registro.`);
        }
    });
}

function rechazarJefeDirecto(id) {
    $.ajax({
        url: `/ERP/Ausencias?handler=RechazarJefeDirecto&id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": getMainToken()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                bootstrap.Modal.getInstance(document.getElementById("modalDetalleAusencia"))?.hide();
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        },
        error: function (xhr) {
            mostrarResultado(true, `Error ${xhr.status}: no fue posible rechazar el registro.`);
        }
    });
}

function aprobarTH(id) {
    $.ajax({
        url: `/ERP/Ausencias?handler=AprobarTH&id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": getMainToken()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                bootstrap.Modal.getInstance(document.getElementById("modalDetalleAusencia"))?.hide();
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        },
        error: function (xhr) {
            mostrarResultado(true, `Error ${xhr.status}: no fue posible aprobar el registro en TH.`);
        }
    });
}

function exportarDetalleAusencias() {
    window.location.href = "/ERP/Ausencias?handler=ExportarDetalleAusencias";
}

function rechazarTH(id) {
    $.ajax({
        url: `/ERP/Ausencias?handler=RechazarTH&id=${id}`,
        type: "POST",
        headers: {
            "RequestVerificationToken": getMainToken()
        },
        success: function (resp) {
            if (!resp.tieneError) {
                bootstrap.Modal.getInstance(document.getElementById("modalDetalleAusencia"))?.hide();
                refrescarTablas();
            }
            mostrarResultado(resp.tieneError, resp.mensaje);
        },
        error: function (xhr) {
            mostrarResultado(true, `Error ${xhr.status}: no fue posible rechazar el registro en TH.`);
        }
    });
}

function getMainToken() {
    return $('#formAntiForgeryAusencias input[name="__RequestVerificationToken"]').val();
}