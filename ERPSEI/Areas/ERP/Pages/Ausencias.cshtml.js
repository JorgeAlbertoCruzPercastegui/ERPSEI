var tableDias;
var tableHoras;

document.addEventListener("DOMContentLoaded", function () {
    tableDias = $("#tableAusenciasDias");
    tableHoras = $("#tableAusenciasHoras");

    initTableDias();
    initTableHoras();
});

function initTableDias() {
    tableDias.bootstrapTable('destroy').bootstrapTable({
        height: 220,
        locale: cultureName,
        columns: [
            {
                field: "tipo",
                title: "Tipo",
                align: "left"
            },
            {
                field: "fechaInicio",
                title: "Fecha inicio",
                align: "center"
            },
            {
                field: "fechaFin",
                title: "Fecha término",
                align: "center"
            },
            {
                field: "dias",
                title: "Días",
                align: "center"
            },
            {
                field: "estado",
                title: "Estado",
                align: "center"
            },
            {
                field: "acciones",
                title: "Acciones",
                align: "center",
                formatter: accionesFormatter
            }
        ]
    });
}

function initTableHoras() {
    tableHoras.bootstrapTable('destroy').bootstrapTable({
        height: 260,
        locale: cultureName,
        columns: [
            {
                field: "tipo",
                title: "Tipo",
                align: "left"
            },
            {
                field: "fechaInicio",
                title: "Fecha inicio",
                align: "center"
            },
            {
                field: "horaInicio",
                title: "Hora inicio",
                align: "center"
            },
            {
                field: "horaTermino",
                title: "Hora término",
                align: "center"
            },
            {
                field: "horas",
                title: "Horas",
                align: "center"
            },
            {
                field: "estado",
                title: "Estado",
                align: "center",
                formatter: estadoFormatter
            },
            {
                field: "acciones",
                title: "Acciones",
                align: "center",
                formatter: accionesFormatter
            }
        ]
    });
}

function estadoFormatter(value) {
    if (value === "Aprobado") {
        return `<span class="badge bg-success-subtle text-success border border-success-subtle">✔ ${value}</span>`;
    }
    if (value === "Pendiente") {
        return `<span class="badge bg-warning-subtle text-warning border border-warning-subtle">${value}</span>`;
    }
    return `<span class="badge bg-secondary">${value || ""}</span>`;
}

function accionesFormatter() {
    return `
        <div class="d-flex justify-content-center gap-2">
            <a href="#" title="Ver"><i class="bi bi-search text-primary"></i></a>
            <a href="#" title="Editar"><i class="bi bi-pencil text-primary"></i></a>
            <a href="#" title="Eliminar"><i class="bi bi-trash text-danger"></i></a>
        </div>
    `;
}

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

function onBuscarAusenciasClick() {
    tableDias.bootstrapTable('refresh');
    tableHoras.bootstrapTable('refresh');
}

function guardarInasistencia() {
    alert("Aquí conectarás el guardado de inasistencia.");
}

function guardarIncapacidad() {
    alert("Aquí conectarás el guardado de incapacidad.");
}

function guardarPermiso() {
    alert("Aquí conectarás el guardado de permiso.");
}

function solicitarPermiso() {
    alert("Aquí conectarás la solicitud de permiso.");
}