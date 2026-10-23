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
    configurarContadoresComentarios();

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

    // =====================================================
    // INASISTENCIA
    // =====================================================

    $("#inpInasistenciaFechaInicio, #inpInasistenciaFechaFin")
        .on("change", function () {

            calcularDias(
                "#inpInasistenciaFechaInicio",
                "#inpInasistenciaFechaFin",
                "#inpInasistenciaDias",
                "#lblDiasInasistencia"
            );
        });

    $("#inpInasistenciaFechaInicio")
        .on("change", function () {

            configurarLimiteFechaFin(
                "#inpInasistenciaFechaInicio",
                "#inpInasistenciaFechaFin"
            );

            calcularDias(
                "#inpInasistenciaFechaInicio",
                "#inpInasistenciaFechaFin",
                "#inpInasistenciaDias",
                "#lblDiasInasistencia"
            );
        });


    // =====================================================
    // INCAPACIDAD
    // =====================================================

    $("#inpIncapacidadFechaInicio, #inpIncapacidadFechaFin")
        .on("change", function () {

            calcularDias(
                "#inpIncapacidadFechaInicio",
                "#inpIncapacidadFechaFin",
                "#inpDiasIncapacidad",
                "#lblDiasIncapacidad"
            );
        });

    $("#inpIncapacidadFechaInicio")
        .on("change", function () {

            configurarLimiteFechaFin(
                "#inpIncapacidadFechaInicio",
                "#inpIncapacidadFechaFin"
            );

            calcularDias(
                "#inpIncapacidadFechaInicio",
                "#inpIncapacidadFechaFin",
                "#inpDiasIncapacidad",
                "#lblDiasIncapacidad"
            );
        });


    // =====================================================
    // REGISTRAR PERMISO
    // =====================================================

    $("#inpPermisoFechaInicio, #inpPermisoFechaFin, #inpPermisoHoraInicio, #inpPermisoHoraTermino")
        .on("change input", function () {

            actualizarResumenPermiso();
        });

    $("#inpPermisoFechaInicio")
        .on("change", function () {

            configurarLimiteFechaFin(
                "#inpPermisoFechaInicio",
                "#inpPermisoFechaFin"
            );

            actualizarResumenPermiso();
        });


    // =====================================================
    // SOLICITAR PERMISO
    // =====================================================

    $("#inpSolicitudPermisoFechaInicio, #inpSolicitudPermisoFechaFin, #inpSolicitudPermisoHoraInicio, #inpSolicitudPermisoHoraTermino")
        .on("change input", function () {

            actualizarResumenSolicitudPermiso();
        });

    $("#inpSolicitudPermisoFechaInicio")
        .on("change", function () {

            configurarLimiteFechaFin(
                "#inpSolicitudPermisoFechaInicio",
                "#inpSolicitudPermisoFechaFin"
            );

            actualizarResumenSolicitudPermiso();
        });


    // =====================================================
    // TIPO DE AUSENCIA - REGISTRAR PERMISO
    // =====================================================

    $("#selTipoAusenciaPermiso")
        .on("change", function () {

            alternarCamposPermiso(
                $(this).val(),
                false
            );

            actualizarResumenPermiso();
        });


    // =====================================================
    // TIPO DE AUSENCIA - SOLICITAR PERMISO
    // =====================================================

    $("#selTipoAusenciaSolicitud")
        .on("change", function () {

            alternarCamposPermiso(
                $(this).val(),
                true
            );

            actualizarResumenSolicitudPermiso();
        });


    // =====================================================
    // EDICIÓN
    // =====================================================

    $("#editTipoAusenciaId")
        .on("change", function () {

            alternarCamposEdicion(
                $(this).val()
            );
        });


    $("#editFechaInicio, #editFechaFin")
        .on("change", function () {

            configurarLimiteFechaFin(
                "#editFechaInicio",
                "#editFechaFin"
            );

            const fechaInicio =
                $("#editFechaInicio").val();

            const fechaFin =
                $("#editFechaFin").val();


            if (
                fechaInicio &&
                fechaFin
            ) {

                const dias =
                    calcularDiasHabiles(
                        fechaInicio,
                        fechaFin
                    );


                $("#editDias")
                    .val(
                        dias
                    );

            } else {

                $("#editDias")
                    .val("");
            }
        });
}


// =========================================================
// LÍMITE MÁXIMO DE RANGO DE FECHAS
// 40 DÍAS HÁBILES
// =========================================================

const MAXIMO_DIAS_HABILES_AUSENCIA = 40;


// =========================================================
// CALCULAR DÍAS HÁBILES
// NO CUENTA SÁBADOS NI DOMINGOS
// =========================================================

function calcularDiasHabiles(
    fechaInicio,
    fechaFin
) {

    if (
        !fechaInicio ||
        !fechaFin
    ) {
        return 0;
    }


    const inicio =
        new Date(
            fechaInicio +
            "T00:00:00"
        );

    const fin =
        new Date(
            fechaFin +
            "T00:00:00"
        );


    if (
        isNaN(inicio.getTime()) ||
        isNaN(fin.getTime()) ||
        fin < inicio
    ) {
        return 0;
    }


    let totalDiasHabiles = 0;

    const fechaActual =
        new Date(inicio);


    while (
        fechaActual <= fin
    ) {

        const diaSemana =
            fechaActual.getDay();

        // 0 = domingo
        // 6 = sábado
        if (
            diaSemana !== 0 &&
            diaSemana !== 6
        ) {
            totalDiasHabiles++;
        }


        fechaActual.setDate(
            fechaActual.getDate() + 1
        );
    }


    return totalDiasHabiles;
}


// =========================================================
// OBTENER FECHA MÁXIMA
// 40 DÍAS HÁBILES DESDE FECHA INICIO
// =========================================================

function obtenerFechaMaximaAusencia(
    fechaInicio
) {

    if (!fechaInicio) {
        return "";
    }


    const fecha =
        new Date(
            fechaInicio +
            "T00:00:00"
        );


    if (
        isNaN(
            fecha.getTime()
        )
    ) {
        return "";
    }


    let diasHabiles = 0;


    while (
        diasHabiles <
        MAXIMO_DIAS_HABILES_AUSENCIA
    ) {

        const diaSemana =
            fecha.getDay();


        if (
            diaSemana !== 0 &&
            diaSemana !== 6
        ) {
            diasHabiles++;
        }


        if (
            diasHabiles <
            MAXIMO_DIAS_HABILES_AUSENCIA
        ) {
            fecha.setDate(
                fecha.getDate() + 1
            );
        }
    }


    const anio =
        fecha.getFullYear();

    const mes =
        String(
            fecha.getMonth() + 1
        )
            .padStart(
                2,
                "0"
            );

    const dia =
        String(
            fecha.getDate()
        )
            .padStart(
                2,
                "0"
            );


    return (
        `${anio}-${mes}-${dia}`
    );
}


// =========================================================
// CONFIGURAR RANGO DE FECHA FIN
// =========================================================

function configurarLimiteFechaFin(
    selectorInicio,
    selectorFin
) {

    const fechaInicio =
        $(selectorInicio)
            .val();

    const campoFin =
        $(selectorFin);


    if (!fechaInicio) {

        campoFin
            .removeAttr("min")
            .removeAttr("max");

        return;
    }


    const fechaMaxima =
        obtenerFechaMaximaAusencia(
            fechaInicio
        );


    campoFin
        .attr(
            "min",
            fechaInicio
        )
        .attr(
            "max",
            fechaMaxima
        );


    const fechaFin =
        campoFin.val();


    if (
        fechaFin &&
        (
            fechaFin < fechaInicio ||
            fechaFin > fechaMaxima
        )
    ) {

        campoFin.val("");
    }
}


function initTableDias() {
    tableDias.bootstrapTable('destroy').bootstrapTable({
        height: 520,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100, "All"],
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
        height: 520,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100, "All"],
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

    const columnas = [
        {
            field: "empleado",
            title: "Empleado",
            align: "left"
        },
        {
            field: "categoria",
            title: "Categoría",
            align: "center"
        },
        {
            field: "tipo",
            title: "Tipo",
            align: "left"
        }
    ];


    // =====================================================
    // FECHA DE REGISTRO
    // Solamente Administrador y Administrador TH
    // =====================================================

    if (
        typeof puedeVerTrazabilidadAusencias !== "undefined" &&
        puedeVerTrazabilidadAusencias === true
    ) {

        columnas.push(
            {
                field: "fechaCreacion",
                title: "Registrada",
                align: "center",

                formatter: function (value) {
                    return value || "-";
                }
            }
        );
    }


    columnas.push(
        {
            field: "periodo",
            title: "Periodo",
            align: "center",

            formatter: function (
                value,
                row
            ) {

                if (
                    row.captura ===
                    "Horas"
                ) {

                    return (
                        `${row.horaInicio || ""}` +
                        `${row.horaTermino
                            ? " - " + row.horaTermino
                            : ""}`
                    );
                }


                return (
                    `${row.fechaInicio || ""}` +
                    `${row.fechaFin
                        ? " al " + row.fechaFin
                        : ""}`
                );
            }
        },
        {
            field: "duracion",
            title: "Duración",
            align: "center",

            formatter: function (
                value,
                row
            ) {

                if (
                    row.captura ===
                    "Horas"
                ) {

                    return (
                        row.horas ||
                        ""
                    );
                }


                return (
                    row.dias ||
                    ""
                );
            }
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
            formatter: accionesFormatter,
            events: accionesEvents
        }
    );


    tableAutorizar
        .bootstrapTable(
            "destroy"
        )
        .bootstrapTable(
            {
                pagination: true,

                pageSize: 10,

                pageList:
                    [
                        10,
                        25,
                        50,
                        100,
                        "All"
                    ],

                locale:
                    cultureName,

                columns:
                    columnas
            }
        );
}


function estadoFormatter(value) {

    if (
        value ===
        "Pendiente jefe directo"
    ) {
        return `<span class="badge bg-warning-subtle text-warning border border-warning-subtle">${value}</span>`;
    }


    if (
        value ===
        "Aprobado por jefe directo"
    ) {
        return `<span class="badge bg-secondary-subtle text-secondary border border-secondary-subtle">${value}</span>`;
    }


    if (
        value ===
        "Aprobado"
    ) {
        return `<span class="badge bg-success-subtle text-success border border-success-subtle">✔ ${value}</span>`;
    }


    if (
        (value || "")
            .includes(
                "Rechazado"
            )
    ) {
        return `<span class="badge bg-danger-subtle text-danger border border-danger-subtle">${value}</span>`;
    }


    return `<span class="badge bg-secondary">${value || ""}</span>`;
}


function accionesFormatter(value, row) {

    let html =
        `<div class="d-flex justify-content-center gap-2 flex-wrap">`;


    html +=
        `<a href="javascript:void(0)" class="accion-ver" title="Ver"><i class="bi bi-search text-primary"></i></a>`;


    if (row.puedeEditar) {

        html +=
            `<a href="javascript:void(0)" class="accion-editar" title="Editar"><i class="bi bi-pencil text-primary"></i></a>`;
    }


    if (row.puedeEliminar) {

        html +=
            `<a href="javascript:void(0)" class="accion-eliminar" title="Eliminar"><i class="bi bi-trash text-danger"></i></a>`;
    }


    if (row.puedeAprobarJefe) {

        html +=
            `<a href="javascript:void(0)" class="accion-aprobar-jefe" title="Aprobar jefe directo"><i class="bi bi-check-circle text-secondary"></i></a>`;

        html +=
            `<a href="javascript:void(0)" class="accion-rechazar-jefe" title="Rechazar jefe directo"><i class="bi bi-x-circle text-danger"></i></a>`;
    }


    if (row.puedeAprobarTH) {

        html +=
            `<a href="javascript:void(0)" class="accion-aprobar-th" title="Aprobar TH"><i class="bi bi-check-circle text-success"></i></a>`;

        html +=
            `<a href="javascript:void(0)" class="accion-rechazar-th" title="Rechazar TH"><i class="bi bi-x-circle text-danger"></i></a>`;
    }


    html +=
        `</div>`;


    return html;
}


window.accionesEvents = {

    'click .accion-ver': function (
        e,
        value,
        row
    ) {
        verDetalle(
            row.id
        );
    },


    'click .accion-editar': function (
        e,
        value,
        row
    ) {
        abrirEdicion(
            row.id
        );
    },


    'click .accion-eliminar': function (
        e,
        value,
        row
    ) {
        eliminarAusencia(
            row.id
        );
    },


    'click .accion-aprobar-jefe': function (
        e,
        value,
        row
    ) {
        aprobarJefeDirecto(
            row.id
        );
    },


    'click .accion-rechazar-jefe': function (
        e,
        value,
        row
    ) {
        rechazarJefeDirecto(
            row.id
        );
    },


    'click .accion-aprobar-th': function (
        e,
        value,
        row
    ) {
        aprobarTH(
            row.id
        );
    },


    'click .accion-rechazar-th': function (
        e,
        value,
        row
    ) {
        rechazarTH(
            row.id
        );
    }
};


function abrirModalInasistencia() {

    new bootstrap.Modal(
        document.getElementById(
            "modalInasistencia"
        )
    )
        .show();
}


function abrirModalIncapacidad() {

    new bootstrap.Modal(
        document.getElementById(
            "modalIncapacidad"
        )
    )
        .show();
}


function abrirModalPermiso() {

    new bootstrap.Modal(
        document.getElementById(
            "modalPermiso"
        )
    )
        .show();
}


function abrirModalSolicitarPermiso() {

    new bootstrap.Modal(
        document.getElementById(
            "modalSolicitarPermiso"
        )
    )
        .show();
}


function getToken(formId) {

    return $(
        `#${formId} input[name="__RequestVerificationToken"]`
    )
        .val();
}


function refrescarTablas() {

    tableDias
        .bootstrapTable(
            'refresh'
        );


    tableHoras
        .bootstrapTable(
            'refresh'
        );


    if (
        tableAutorizar &&
        tableAutorizar.length
    ) {

        tableAutorizar
            .bootstrapTable(
                'refresh'
            );
    }
}


function mostrarResultado(
    esError,
    mensaje,
    titulo = null
) {

    $("#modalResultadoTitulo")
        .text(
            titulo ||
            (
                esError
                    ? "Ocurrió un problema"
                    : "Proceso correcto"
            )
        );


    $("#modalResultadoMensaje")
        .text(
            mensaje
        );


    if (esError) {

        $("#modalResultadoIcono")
            .html(
                `<i class="bi bi-x-circle-fill text-danger"></i>`
            );

    } else {

        $("#modalResultadoIcono")
            .html(
                `<i class="bi bi-check-circle-fill text-success"></i>`
            );
    }


    new bootstrap.Modal(
        document.getElementById(
            "modalResultado"
        )
    )
        .show();
}


// =========================================================
// CALCULAR DÍAS
// =========================================================

function calcularDias(
    selectorInicio,
    selectorFin,
    selectorDias,
    selectorLabel
) {

    const inicio =
        $(selectorInicio).val();

    const fin =
        $(selectorFin).val();


    if (
        !inicio ||
        !fin
    ) {

        $(selectorDias)
            .val("");

        $(selectorLabel)
            .text("0");

        return;
    }


    const diasHabiles =
        calcularDiasHabiles(
            inicio,
            fin
        );


    $(selectorDias)
        .val(
            diasHabiles
        );


    $(selectorLabel)
        .text(
            diasHabiles
        );
}


function calcularSoloLabel(
    selectorInicio,
    selectorFin,
    selectorLabel
) {

    const inicio =
        $(selectorInicio).val();

    const fin =
        $(selectorFin).val();


    if (
        !inicio ||
        !fin
    ) {

        $(selectorLabel)
            .text("0");

        return;
    }


    const diasHabiles =
        calcularDiasHabiles(
            inicio,
            fin
        );


    $(selectorLabel)
        .text(
            diasHabiles
        );
}


// =========================================================
// ACTUALIZAR RESUMEN PERMISO
// =========================================================

function actualizarResumenPermiso() {

    const tipoId =
        $("#selTipoAusenciaPermiso").val();

    const resumen =
        $("#lblResumenPermiso");


    if (!tipoId) {

        resumen.text(
            "Estás registrando 0 días de permiso"
        );

        return;
    }


    const tipo =
        tiposAusenciaCache.find(
            x => x.id == tipoId
        );


    if (!tipo) {

        resumen.text(
            "Estás registrando 0 días de permiso"
        );

        return;
    }


    // =====================================================
    // PERMISOS MANEJADOS POR HORAS
    // =====================================================

    if (tipo.manejaHoras) {

        const horaInicio =
            $("#inpPermisoHoraInicio").val();

        const horaTermino =
            $("#inpPermisoHoraTermino").val();


        if (
            !horaInicio ||
            !horaTermino
        ) {

            resumen.text(
                "Selecciona la hora de inicio y término"
            );

            return;
        }


        const inicio =
            new Date(
                `2000-01-01T${horaInicio}:00`
            );

        const termino =
            new Date(
                `2000-01-01T${horaTermino}:00`
            );


        const diferenciaMs =
            termino.getTime() -
            inicio.getTime();


        if (
            diferenciaMs <= 0
        ) {

            resumen.text(
                "La hora término debe ser mayor que la hora inicio"
            );

            return;
        }


        const minutosTotales =
            Math.floor(
                diferenciaMs /
                (1000 * 60)
            );


        const horas =
            Math.floor(
                minutosTotales / 60
            );


        const minutos =
            minutosTotales % 60;


        let duracion = "";


        if (
            horas > 0
        ) {

            duracion +=
                `${horas} ${horas === 1 ? "hora" : "horas"}`;
        }


        if (
            minutos > 0
        ) {

            if (
                horas > 0
            ) {
                duracion += " ";
            }


            duracion +=
                `${minutos} ${minutos === 1 ? "minuto" : "minutos"}`;
        }


        if (!duracion) {

            duracion =
                "0 minutos";
        }


        const nombreTipo =
            $("#selTipoAusenciaPermiso option:selected")
                .text()
                .trim();


        if (
            nombreTipo
                .toLowerCase()
                .includes(
                    "llegada tardía"
                )
        ) {

            resumen.text(
                `Estás registrando una llegada tardía de ${duracion}`
            );

        } else {

            resumen.text(
                `Estás registrando un permiso de ${duracion}`
            );
        }


        return;
    }


    // =====================================================
    // PERMISOS MANEJADOS POR DÍAS
    // =====================================================

    const fechaInicio =
        $("#inpPermisoFechaInicio").val();

    const fechaFin =
        $("#inpPermisoFechaFin").val();


    if (
        !fechaInicio ||
        !fechaFin
    ) {

        resumen.text(
            "Estás registrando 0 días de permiso"
        );

        return;
    }


    const inicio =
        new Date(
            fechaInicio +
            "T00:00:00"
        );


    const fin =
        new Date(
            fechaFin +
            "T00:00:00"
        );


    if (
        fin < inicio
    ) {

        resumen.text(
            "Estás registrando 0 días de permiso"
        );

        return;
    }


    const dias =
        calcularDiasHabiles(
            fechaInicio,
            fechaFin
        );


    $("#inpPermisoDiasAporte")
        .val(
            dias
        );


    resumen.text(
        `Estás registrando ${dias} ${dias === 1 ? "día hábil" : "días hábiles"} de permiso`
    );
}


// =========================================================
// ACTUALIZAR RESUMEN SOLICITUD PERMISO
// =========================================================

function actualizarResumenSolicitudPermiso() {

    const tipoId =
        $("#selTipoAusenciaSolicitud").val();

    const resumen =
        $("#lblResumenSolicitudPermiso");


    if (!tipoId) {

        resumen.text(
            "Estás solicitando 0 días de permiso"
        );

        return;
    }


    const tipo =
        tiposAusenciaCache.find(
            x => x.id == tipoId
        );


    if (!tipo) {

        resumen.text(
            "Estás solicitando 0 días de permiso"
        );

        return;
    }


    // =====================================================
    // PERMISOS POR HORAS
    // =====================================================

    if (tipo.manejaHoras) {

        const horaInicio =
            $("#inpSolicitudPermisoHoraInicio").val();

        const horaTermino =
            $("#inpSolicitudPermisoHoraTermino").val();


        if (
            !horaInicio ||
            !horaTermino
        ) {

            resumen.text(
                "Selecciona la hora de inicio y término"
            );

            return;
        }


        const inicio =
            new Date(
                `2000-01-01T${horaInicio}:00`
            );


        const termino =
            new Date(
                `2000-01-01T${horaTermino}:00`
            );


        const diferenciaMs =
            termino.getTime() -
            inicio.getTime();


        if (
            diferenciaMs <= 0
        ) {

            resumen.text(
                "La hora término debe ser mayor que la hora inicio"
            );

            return;
        }


        const minutosTotales =
            Math.floor(
                diferenciaMs /
                (1000 * 60)
            );


        const horas =
            Math.floor(
                minutosTotales / 60
            );


        const minutos =
            minutosTotales % 60;


        let duracion = "";


        if (
            horas > 0
        ) {

            duracion +=
                `${horas} ${horas === 1 ? "hora" : "horas"}`;
        }


        if (
            minutos > 0
        ) {

            if (
                horas > 0
            ) {
                duracion += " ";
            }


            duracion +=
                `${minutos} ${minutos === 1 ? "minuto" : "minutos"}`;
        }


        if (!duracion) {

            duracion =
                "0 minutos";
        }


        const nombreTipo =
            $("#selTipoAusenciaSolicitud option:selected")
                .text()
                .trim();


        if (
            nombreTipo
                .toLowerCase()
                .includes(
                    "llegada tardía"
                )
        ) {

            resumen.text(
                `Estás solicitando una llegada tardía de ${duracion}`
            );

        } else {

            resumen.text(
                `Estás solicitando un permiso de ${duracion}`
            );
        }


        return;
    }


    // =====================================================
    // PERMISOS POR DÍAS
    // =====================================================

    const fechaInicio =
        $("#inpSolicitudPermisoFechaInicio").val();

    const fechaFin =
        $("#inpSolicitudPermisoFechaFin").val();


    if (
        !fechaInicio ||
        !fechaFin
    ) {

        resumen.text(
            "Estás solicitando 0 días de permiso"
        );

        return;
    }


    const inicio =
        new Date(
            fechaInicio +
            "T00:00:00"
        );


    const fin =
        new Date(
            fechaFin +
            "T00:00:00"
        );


    if (
        isNaN(inicio.getTime()) ||
        isNaN(fin.getTime()) ||
        fin < inicio
    ) {

        resumen.text(
            "Estás solicitando 0 días de permiso"
        );

        return;
    }


    const dias =
        calcularDiasHabiles(
            fechaInicio,
            fechaFin
        );


    resumen.text(
        `Estás solicitando ${dias} ${dias === 1 ? "día hábil" : "días hábiles"} de permiso`
    );
}


function cargarTiposAusencia() {

    $.get(
        "/ERP/Ausencias?handler=TiposAusencia",

        function (resp) {

            tiposAusenciaCache =
                resp || [];
        }
    );
}


function alternarCamposPermiso(
    tipoAusenciaId,
    esSolicitud
) {

    if (
        !tipoAusenciaId ||
        !tiposAusenciaCache.length
    ) {
        return;
    }


    const item =
        tiposAusenciaCache.find(
            x => x.id == tipoAusenciaId
        );


    if (!item) {
        return;
    }


    if (esSolicitud) {

        if (item.manejaHoras) {

            $(".campo-horas-solicitud")
                .show();

        } else {

            $(".campo-horas-solicitud")
                .hide();

            $("#inpSolicitudPermisoHoraInicio")
                .val("");

            $("#inpSolicitudPermisoHoraTermino")
                .val("");
        }

    } else {

        if (item.manejaHoras) {

            $(".campo-horas-permiso")
                .show();

            $(".campo-dias-permiso")
                .hide();

            $("#inpPermisoDiasAporte")
                .val("");

            $("#lblResumenPermiso")
                .text(
                    "Selecciona la hora de inicio y término"
                );

        } else {

            $(".campo-horas-permiso")
                .hide();

            $("#inpPermisoHoraInicio")
                .val("");

            $("#inpPermisoHoraTermino")
                .val("");

            $(".campo-dias-permiso")
                .show();
        }
    }
}


function alternarCamposEdicion(
    tipoAusenciaId
) {

    if (
        !tipoAusenciaId ||
        !tiposAusenciaCache.length
    ) {

        $(".edit-horas")
            .show();

        $(".edit-dias")
            .show();

        return;
    }


    const item =
        tiposAusenciaCache.find(
            x => x.id == tipoAusenciaId
        );


    if (!item) {
        return;
    }


    if (item.manejaHoras) {

        $(".edit-horas")
            .show();

        $(".edit-dias")
            .hide();

        $("#editDias")
            .val("");

    } else {

        $(".edit-horas")
            .hide();

        $("#editHoraInicio")
            .val("");

        $("#editHoraTermino")
            .val("");

        $(".edit-dias")
            .show();
    }
}


// =========================================================
// GUARDAR INCAPACIDAD
// =========================================================

function guardarIncapacidad() {

    if (
        !validarComentario(
            "#txtComentarioIncapacidad"
        )
    ) {
        return;
    }


    if (
        !validarJustificantes(
            "#inpDocumentosIncapacidad"
        )
    ) {
        return;
    }


    if (
        !validarRangoFechas(
            $("#inpIncapacidadFechaInicio").val(),
            $("#inpIncapacidadFechaFin").val()
        )
    ) {

        mostrarResultado(
            true,
            "La fecha de término debe ser igual o posterior a la fecha de inicio y el periodo no puede superar 40 días hábiles."
        );

        return;
    }


    const formData =
        new FormData();


    formData.append(
        "IncapacidadInput.FechaInicio",
        $("#inpIncapacidadFechaInicio").val()
    );


    formData.append(
        "IncapacidadInput.FechaFin",
        $("#inpIncapacidadFechaFin").val()
    );


    formData.append(
        "IncapacidadInput.TipoIncapacidadId",
        $("#selTipoIncapacidad").val()
    );


    formData.append(
        "IncapacidadInput.NumeroFolio",
        $("#inpNumeroFolioIncapacidad").val()
    );


    formData.append(
        "IncapacidadInput.Dias",
        $("#inpDiasIncapacidad").val()
    );


    formData.append(
        "IncapacidadInput.FechaAplicacion",
        $("#inpFechaAplicacionIncapacidad").val()
    );


    formData.append(
        "IncapacidadInput.Suplencia",
        $("#chkSuplenciaIncapacidad").is(":checked")
    );


    formData.append(
        "IncapacidadInput.Comentario",
        $("#txtComentarioIncapacidad").val()
    );


    const archivos =
        $("#inpDocumentosIncapacidad")[0].files;


    for (
        let i = 0;
        i < archivos.length;
        i++
    ) {

        formData.append(
            "IncapacidadInput.Documentos",
            archivos[i]
        );
    }


    $.ajax({

        url:
            "/ERP/Ausencias?handler=GuardarIncapacidad",

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getToken(
                    "formIncapacidad"
                )
        },

        data:
            formData,

        processData:
            false,

        contentType:
            false,


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    if (
                        window.IntranetDrafts &&
                        typeof window.IntranetDrafts.clearById ===
                        "function"
                    ) {

                        window.IntranetDrafts.clearById(
                            "formIncapacidad"
                        );
                    }


                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "modalIncapacidad"
                            )
                        )
                        ?.hide();


                    $("#formIncapacidad")[0]
                        .reset();


                    $("#inpDocumentosIncapacidad")
                        .val("");


                    $("#lblDiasIncapacidad")
                        .text("0");


                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            },


        error:
            function (xhr) {

                mostrarResultado(
                    true,
                    `Error ${xhr.status}: no fue posible guardar la incapacidad.`
                );
            }
    });
}


// =========================================================
// VALIDAR JUSTIFICANTES
// PDF / PNG / JPG / JPEG
// =========================================================

function validarJustificantes(
    selector
) {

    const input =
        document.querySelector(
            selector
        );


    if (
        !input ||
        !input.files ||
        input.files.length === 0
    ) {

        return true;
    }


    const extensionesPermitidas =
        [
            ".pdf",
            ".png",
            ".jpg",
            ".jpeg"
        ];


    const tiposPermitidos =
        [
            "application/pdf",
            "image/png",
            "image/jpeg"
        ];


    for (
        let i = 0;
        i < input.files.length;
        i++
    ) {

        const archivo =
            input.files[i];


        const nombre =
            (
                archivo.name ||
                ""
            )
                .toLowerCase();


        const extension =
            nombre.includes(".")
                ? nombre.substring(
                    nombre.lastIndexOf(".")
                )
                : "";


        const extensionValida =
            extensionesPermitidas.includes(
                extension
            );


        const mimeValido =
            !archivo.type ||
            tiposPermitidos.includes(
                archivo.type
            );


        if (
            !extensionValida ||
            !mimeValido
        ) {

            input.value =
                "";


            mostrarResultado(
                true,
                "No puedes adjuntar ese archivo. Solo se permiten archivos PDF, PNG o JPG."
            );


            return false;
        }
    }


    return true;
}


// =========================================================
// GUARDAR INASISTENCIA
// =========================================================

function guardarInasistencia() {

    if (
        !validarComentario(
            "#txtComentarioInasistencia"
        )
    ) {
        return;
    }


    if (
        !validarJustificantes(
            "#inpDocumentosInasistencia"
        )
    ) {
        return;
    }


    if (
        !validarRangoFechas(
            $("#inpInasistenciaFechaInicio").val(),
            $("#inpInasistenciaFechaFin").val()
        )
    ) {

        mostrarResultado(
            true,
            "La fecha de término debe ser igual o posterior a la fecha de inicio y el periodo no puede superar 40 días hábiles."
        );

        return;
    }


    const formData =
        new FormData();


    formData.append(
        "InasistenciaInput.FechaInicio",
        $("#inpInasistenciaFechaInicio").val()
    );


    formData.append(
        "InasistenciaInput.FechaFin",
        $("#inpInasistenciaFechaFin").val()
    );


    formData.append(
        "InasistenciaInput.Dias",
        $("#inpInasistenciaDias").val()
    );


    formData.append(
        "InasistenciaInput.FechaAplicacion",
        $("#inpInasistenciaFechaAplicacion").val()
    );


    formData.append(
        "InasistenciaInput.Suplencia",
        $("#chkSuplenciaInasistencia").is(":checked")
    );


    formData.append(
        "InasistenciaInput.Comentario",
        $("#txtComentarioInasistencia").val()
    );


    const archivos =
        $("#inpDocumentosInasistencia")[0].files;


    for (
        let i = 0;
        i < archivos.length;
        i++
    ) {

        formData.append(
            "InasistenciaInput.Documentos",
            archivos[i]
        );
    }


    $.ajax({

        url:
            "/ERP/Ausencias?handler=GuardarInasistencia",

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getToken(
                    "formInasistencia"
                )
        },

        data:
            formData,

        processData:
            false,

        contentType:
            false,


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    if (
                        window.IntranetDrafts &&
                        typeof window.IntranetDrafts.clearById ===
                        "function"
                    ) {

                        window.IntranetDrafts.clearById(
                            "formInasistencia"
                        );
                    }


                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "modalInasistencia"
                            )
                        )
                        ?.hide();


                    $("#formInasistencia")[0]
                        .reset();


                    $("#inpDocumentosInasistencia")
                        .val("");


                    $("#lblDiasInasistencia")
                        .text("0");


                    $("#contadorComentarioInasistencia")
                        .text("0");


                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            },


        error:
            function (xhr) {

                mostrarResultado(
                    true,
                    `Error ${xhr.status}: no fue posible guardar la inasistencia.`
                );
            }
    });
}


// =========================================================
// GUARDAR PERMISO
// =========================================================

function guardarPermiso() {

    if (
        !validarComentario(
            "#txtComentarioPermiso"
        )
    ) {
        return;
    }


    if (
        !validarJustificantes(
            "#inpDocumentosPermiso"
        )
    ) {
        return;
    }


    if (
        !validarRangoFechas(
            $("#inpPermisoFechaInicio").val(),
            $("#inpPermisoFechaFin").val()
        )
    ) {

        mostrarResultado(
            true,
            "La fecha de término debe ser igual o posterior a la fecha de inicio y el periodo no puede superar 40 días hábiles."
        );

        return;
    }


    const formData =
        new FormData();


    formData.append(
        "PermisoInput.TipoAusenciaId",
        $("#selTipoAusenciaPermiso").val()
    );


    formData.append(
        "PermisoInput.FechaInicio",
        $("#inpPermisoFechaInicio").val()
    );


    formData.append(
        "PermisoInput.FechaFin",
        $("#inpPermisoFechaFin").val()
    );


    formData.append(
        "PermisoInput.HoraInicio",
        $("#inpPermisoHoraInicio").val()
    );


    formData.append(
        "PermisoInput.HoraTermino",
        $("#inpPermisoHoraTermino").val()
    );


    formData.append(
        "PermisoInput.Dias",
        $("#inpPermisoDiasAporte").val()
    );


    formData.append(
        "PermisoInput.FechaAplicacion",
        $("#inpPermisoFechaAplicacion").val()
    );


    formData.append(
        "PermisoInput.Suplencia",
        $("#chkSuplenciaPermiso").is(":checked")
    );


    formData.append(
        "PermisoInput.Comentario",
        $("#txtComentarioPermiso").val()
    );


    const archivos =
        $("#inpDocumentosPermiso")[0].files;


    for (
        let i = 0;
        i < archivos.length;
        i++
    ) {

        formData.append(
            "PermisoInput.Documentos",
            archivos[i]
        );
    }


    $.ajax({

        url:
            "/ERP/Ausencias?handler=GuardarPermiso",

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getToken(
                    "formPermiso"
                )
        },

        data:
            formData,

        processData:
            false,

        contentType:
            false,


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    if (
                        window.IntranetDrafts &&
                        typeof window.IntranetDrafts.clearById ===
                        "function"
                    ) {

                        window.IntranetDrafts.clearById(
                            "formPermiso"
                        );
                    }


                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "modalPermiso"
                            )
                        )
                        ?.hide();


                    $("#formPermiso")[0]
                        .reset();


                    $("#inpDocumentosPermiso")
                        .val("");


                    $("#lblResumenPermiso")
                        .text(
                            "Estás registrando 0 días de permiso"
                        );


                    $(".campo-horas-permiso")
                        .show();


                    $(".campo-dias-permiso")
                        .show();


                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            },


        error:
            function (xhr) {

                mostrarResultado(
                    true,
                    `Error ${xhr.status}: no fue posible guardar el permiso.`
                );
            }
    });
}


// =========================================================
// SOLICITAR PERMISO
// =========================================================

function solicitarPermiso() {

    if (
        !validarComentario(
            "#txtComentarioSolicitudPermiso"
        )
    ) {
        return;
    }


    if (
        !validarJustificantes(
            "#inpDocumentosSolicitudPermiso"
        )
    ) {
        return;
    }


    if (
        !validarRangoFechas(
            $("#inpSolicitudPermisoFechaInicio").val(),
            $("#inpSolicitudPermisoFechaFin").val()
        )
    ) {

        mostrarResultado(
            true,
            "La fecha de término debe ser igual o posterior a la fecha de inicio y el periodo no puede superar 40 días hábiles."
        );

        return;
    }


    const formData =
        new FormData();


    formData.append(
        "SolicitudPermisoInput.TipoAusenciaId",
        $("#selTipoAusenciaSolicitud").val()
    );


    formData.append(
        "SolicitudPermisoInput.FechaInicio",
        $("#inpSolicitudPermisoFechaInicio").val()
    );


    formData.append(
        "SolicitudPermisoInput.FechaFin",
        $("#inpSolicitudPermisoFechaFin").val()
    );


    formData.append(
        "SolicitudPermisoInput.HoraInicio",
        $("#inpSolicitudPermisoHoraInicio").val()
    );


    formData.append(
        "SolicitudPermisoInput.HoraTermino",
        $("#inpSolicitudPermisoHoraTermino").val()
    );


    formData.append(
        "SolicitudPermisoInput.FechaAplicacion",
        $("#inpSolicitudPermisoFechaAplicacion").val()
    );


    formData.append(
        "SolicitudPermisoInput.Comentario",
        $("#txtComentarioSolicitudPermiso").val()
    );


    const inputArchivos =
        document.getElementById(
            "inpDocumentosSolicitudPermiso"
        );


    if (
        inputArchivos &&
        inputArchivos.files &&
        inputArchivos.files.length > 0
    ) {

        for (
            let i = 0;
            i < inputArchivos.files.length;
            i++
        ) {

            formData.append(
                "SolicitudPermisoInput.Documentos",
                inputArchivos.files[i]
            );
        }
    }


    $.ajax({

        url:
            "/ERP/Ausencias?handler=SolicitarPermiso",

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getToken(
                    "formSolicitarPermiso"
                )
        },

        data:
            formData,

        processData:
            false,

        contentType:
            false,


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    if (
                        window.IntranetDrafts &&
                        typeof window.IntranetDrafts.clearById ===
                        "function"
                    ) {

                        window.IntranetDrafts.clearById(
                            "formSolicitarPermiso"
                        );
                    }


                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "modalSolicitarPermiso"
                            )
                        )
                        ?.hide();


                    $("#formSolicitarPermiso")[0]
                        .reset();


                    $("#inpDocumentosSolicitudPermiso")
                        .val("");


                    $("#lblResumenSolicitudPermiso")
                        .text(
                            "Estás solicitando 0 días de permiso"
                        );


                    $(".campo-horas-solicitud")
                        .show();


                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            },


        error:
            function (xhr) {

                mostrarResultado(
                    true,
                    `Error ${xhr.status}: no fue posible solicitar el permiso.`
                );
            }
    });
}


// =========================================================
// VER DETALLE
// =========================================================

function verDetalle(id) {

    $.get(
        `/ERP/Ausencias?handler=Detalle&id=${id}`,

        function (resp) {

            if (
                resp.tieneError
            ) {

                mostrarResultado(
                    true,
                    resp.mensaje
                );

                return;
            }


            $("#detEmpleado")
                .text(
                    resp.empleado ||
                    ""
                );


            $("#detCategoria")
                .text(
                    resp.categoria ||
                    ""
                );


            $("#detTipo")
                .text(
                    resp.tipo ||
                    ""
                );


            $("#detEstado")
                .text(
                    resp.estado ||
                    ""
                );


            $("#detFechaInicio")
                .text(
                    resp.fechaInicio ||
                    ""
                );


            $("#detFechaFin")
                .text(
                    resp.fechaFin ||
                    ""
                );


            $("#detHoraInicio")
                .text(
                    resp.horaInicio ||
                    ""
                );


            $("#detHoraTermino")
                .text(
                    resp.horaTermino ||
                    ""
                );


            $("#detDias")
                .text(
                    resp.dias ??
                    ""
                );


            $("#detHoras")
                .text(
                    resp.horas ??
                    ""
                );


            $("#detFechaAplicacion")
                .text(
                    resp.fechaAplicacion ||
                    ""
                );


            $("#detSuplencia")
                .text(
                    resp.suplencia
                        ? "Sí"
                        : "No"
                );


            $("#detFolio")
                .text(
                    resp.numeroFolio ||
                    ""
                );


            $("#detUsuarioCreador")
                .text(
                    resp.usuarioCreador ||
                    ""
                );


            // =====================================================
            // TRAZABILIDAD
            // Solo Administrador y Administrador TH
            // =====================================================

            const trazabilidad =
                $("#detTrazabilidadAusencia");


            if (
                resp.puedeVerTrazabilidad ===
                true
            ) {

                $("#detFechaCreacion")
                    .text(
                        resp.fechaCreacion ||
                        "-"
                    );


                $("#detFechaRevisionJefe")
                    .text(
                        resp.fechaRevisionJefeDirecto ||
                        "Pendiente de revisión"
                    );


                $("#detEstadoRevisionJefe")
                    .text(
                        resp.estadoRevisionJefeDirecto ||
                        "Pendiente"
                    );


                trazabilidad
                    .removeClass(
                        "d-none"
                    );

            } else {

                $("#detFechaCreacion")
                    .text(
                        ""
                    );


                $("#detFechaRevisionJefe")
                    .text(
                        ""
                    );


                $("#detEstadoRevisionJefe")
                    .text(
                        ""
                    );


                trazabilidad
                    .addClass(
                        "d-none"
                    );
            }


            $("#detComentario")
                .text(
                    resp.comentario ||
                    ""
                );


            const contenedorDocs =
                $("#detDocumentos");


            contenedorDocs
                .empty();


            if (
                resp.documentos &&
                resp.documentos.length > 0
            ) {

                resp.documentos
                    .forEach(
                        doc => {

                            contenedorDocs.append(
                                `
                                <div class="mb-2">
                                    <a href="${doc.ruta}"
                                       target="_blank"
                                       rel="noopener"
                                       class="btn btn-sm btn-outline-primary">
                                        <i class="bi bi-file-earmark"></i>
                                        ${doc.nombre}
                                    </a>
                                </div>
                                `
                            );
                        }
                    );

            } else {

                contenedorDocs
                    .html(
                        `<span class="text-muted">Sin documentos adjuntos.</span>`
                    );
            }


            const contenedor =
                $("#detalleAccionesAprobacion");


            contenedor
                .empty();


            if (
                resp.puedeAprobarJefe
            ) {

                contenedor.append(
                    `
                    <button type="button"
                            class="btn btn-secondary"
                            onclick="aprobarJefeDirecto(${resp.id})">
                        Aprobar jefe directo
                    </button>

                    <button type="button"
                            class="btn btn-outline-danger"
                            onclick="rechazarJefeDirecto(${resp.id})">
                        Rechazar jefe directo
                    </button>
                    `
                );
            }


            if (
                resp.puedeAprobarTH
            ) {

                contenedor.append(
                    `
                    <button type="button"
                            class="btn btn-success"
                            onclick="aprobarTH(${resp.id})">
                        Aprobar TH
                    </button>

                    <button type="button"
                            class="btn btn-outline-danger"
                            onclick="rechazarTH(${resp.id})">
                        Rechazar TH
                    </button>
                    `
                );
            }


            new bootstrap.Modal(
                document.getElementById(
                    "modalDetalleAusencia"
                )
            )
                .show();
        }
    );
}


// =========================================================
// ABRIR EDICIÓN
// =========================================================

function abrirEdicion(id) {

    $.get(
        `/ERP/Ausencias?handler=Detalle&id=${id}`,

        function (resp) {

            if (
                resp.tieneError
            ) {

                mostrarResultado(
                    true,
                    resp.mensaje
                );

                return;
            }


            $("#editId")
                .val(
                    resp.id
                );


            $("#editTipoAusenciaId")
                .val(
                    resp.tipoAusenciaId ||
                    ""
                );


            $("#editTipoIncapacidadId")
                .val(
                    resp.tipoIncapacidadId ||
                    ""
                );


            $("#editFechaInicio")
                .val(
                    resp.fechaInicio ||
                    ""
                );


            $("#editFechaFin")
                .val(
                    resp.fechaFin ||
                    ""
                );


            $("#editHoraInicio")
                .val(
                    resp.horaInicio ||
                    ""
                );


            $("#editHoraTermino")
                .val(
                    resp.horaTermino ||
                    ""
                );


            $("#editDias")
                .val(
                    resp.dias ||
                    ""
                );


            $("#editFechaAplicacion")
                .val(
                    resp.fechaAplicacion ||
                    ""
                );


            $("#editNumeroFolio")
                .val(
                    resp.numeroFolio ||
                    ""
                );


            $("#editSuplencia")
                .prop(
                    "checked",
                    resp.suplencia ===
                    true
                );


            $("#editComentario")
                .val(
                    resp.comentario ||
                    ""
                );


            $("#editComentario")
                .trigger(
                    "input"
                );


            configurarLimiteFechaFin(
                "#editFechaInicio",
                "#editFechaFin"
            );


            alternarCamposEdicion(
                resp.tipoAusenciaId
            );


            new bootstrap.Modal(
                document.getElementById(
                    "modalEditarAusencia"
                )
            )
                .show();
        }
    );
}


// =========================================================
// GUARDAR EDICIÓN
// =========================================================

function guardarEdicionAusencia() {

    if (
        !validarRangoFechas(
            $("#editFechaInicio").val(),
            $("#editFechaFin").val()
        )
    ) {

        mostrarResultado(
            true,
            "La fecha de término debe ser igual o posterior a la fecha de inicio y el periodo no puede superar 40 días hábiles."
        );

        return;
    }


    $.ajax({

        url:
            "/ERP/Ausencias?handler=Editar",

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getToken(
                    "formEditarAusencia"
                )
        },

        data:
        {
            "EditarInput.Id":
                $("#editId").val(),

            "EditarInput.TipoAusenciaId":
                $("#editTipoAusenciaId").val(),

            "EditarInput.TipoIncapacidadId":
                $("#editTipoIncapacidadId").val(),

            "EditarInput.FechaInicio":
                $("#editFechaInicio").val(),

            "EditarInput.FechaFin":
                $("#editFechaFin").val(),

            "EditarInput.HoraInicio":
                $("#editHoraInicio").val(),

            "EditarInput.HoraTermino":
                $("#editHoraTermino").val(),

            "EditarInput.Dias":
                $("#editDias").val(),

            "EditarInput.FechaAplicacion":
                $("#editFechaAplicacion").val(),

            "EditarInput.NumeroFolio":
                $("#editNumeroFolio").val(),

            "EditarInput.Suplencia":
                $("#editSuplencia").is(
                    ":checked"
                ),

            "EditarInput.Comentario":
                $("#editComentario").val()
        },


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "modalEditarAusencia"
                            )
                        )
                        ?.hide();


                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            }
    });
}


// =========================================================
// ELIMINAR
// =========================================================

function eliminarAusencia(id) {

    if (
        !confirm(
            "¿Deseas eliminar este registro?"
        )
    ) {
        return;
    }


    $.ajax({

        url:
            `/ERP/Ausencias?handler=Eliminar&id=${id}`,

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getMainToken()
        },


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            },


        error:
            function (xhr) {

                mostrarResultado(
                    true,
                    `Error ${xhr.status}: no fue posible eliminar el registro.`
                );
            }
    });
}


// =========================================================
// APROBAR JEFE DIRECTO
// =========================================================

function aprobarJefeDirecto(id) {

    $.ajax({

        url:
            `/ERP/Ausencias?handler=AprobarJefeDirecto&id=${id}`,

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getMainToken()
        },


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "modalDetalleAusencia"
                            )
                        )
                        ?.hide();


                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            },


        error:
            function (xhr) {

                mostrarResultado(
                    true,
                    `Error ${xhr.status}: no fue posible aprobar el registro.`
                );
            }
    });
}


// =========================================================
// RECHAZAR JEFE DIRECTO
// =========================================================

function rechazarJefeDirecto(id) {

    $.ajax({

        url:
            `/ERP/Ausencias?handler=RechazarJefeDirecto&id=${id}`,

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getMainToken()
        },


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "modalDetalleAusencia"
                            )
                        )
                        ?.hide();


                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            },


        error:
            function (xhr) {

                mostrarResultado(
                    true,
                    `Error ${xhr.status}: no fue posible rechazar el registro.`
                );
            }
    });
}


// =========================================================
// APROBAR TH
// =========================================================

function aprobarTH(id) {

    $.ajax({

        url:
            `/ERP/Ausencias?handler=AprobarTH&id=${id}`,

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getMainToken()
        },


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "modalDetalleAusencia"
                            )
                        )
                        ?.hide();


                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            },


        error:
            function (xhr) {

                mostrarResultado(
                    true,
                    `Error ${xhr.status}: no fue posible aprobar el registro en TH.`
                );
            }
    });
}


function exportarDetalleAusencias() {

    window.location.href =
        "/ERP/Ausencias?handler=ExportarDetalleAusencias";
}


// =========================================================
// RECHAZAR TH
// =========================================================

function rechazarTH(id) {

    $.ajax({

        url:
            `/ERP/Ausencias?handler=RechazarTH&id=${id}`,

        type:
            "POST",

        headers:
        {
            "RequestVerificationToken":
                getMainToken()
        },


        success:
            function (resp) {

                if (
                    !resp.tieneError
                ) {

                    bootstrap.Modal
                        .getInstance(
                            document.getElementById(
                                "modalDetalleAusencia"
                            )
                        )
                        ?.hide();


                    refrescarTablas();
                }


                mostrarResultado(
                    resp.tieneError,
                    resp.mensaje
                );
            },


        error:
            function (xhr) {

                mostrarResultado(
                    true,
                    `Error ${xhr.status}: no fue posible rechazar el registro en TH.`
                );
            }
    });
}


function getMainToken() {

    return $(
        '#formAntiForgeryAusencias input[name="__RequestVerificationToken"]'
    )
        .val();
}


// =========================================================
// CONTADORES DE COMENTARIOS
// =========================================================

function configurarContadoresComentarios() {

    const LIMITE_COMENTARIO =
        1000;


    function longitudReal(texto) {

        if (!texto) {
            return 0;
        }


        return texto
            .replace(
                /\r\n/g,
                "\n"
            )
            .replace(
                /\r/g,
                "\n"
            )
            .replace(
                /\n/g,
                "\r\n"
            )
            .length;
    }


    function recortarAlLimite(
        texto,
        limite
    ) {

        let resultado =
            "";


        for (
            let i = 0;
            i < texto.length;
            i++
        ) {

            const siguiente =
                resultado +
                texto[i];


            if (
                longitudReal(
                    siguiente
                ) >
                limite
            ) {
                break;
            }


            resultado =
                siguiente;
        }


        return resultado;
    }


    $(document)
        .on(
            "input",
            ".comentario-ausencia",

            function () {

                const $textarea =
                    $(this);


                const $contenedor =
                    $textarea.closest(
                        ".comentario-ausencia-container"
                    );


                const $contador =
                    $contenedor.find(
                        ".contador-actual"
                    );


                const $mensaje =
                    $contenedor.find(
                        ".mensaje-limite-comentario"
                    );


                const $contadorCompleto =
                    $contenedor.find(
                        ".contador-comentario"
                    );


                let texto =
                    $textarea.val() ||
                    "";


                let cantidad =
                    longitudReal(
                        texto
                    );


                if (
                    cantidad >
                    LIMITE_COMENTARIO
                ) {

                    texto =
                        recortarAlLimite(
                            texto,
                            LIMITE_COMENTARIO
                        );


                    $textarea
                        .val(
                            texto
                        );


                    cantidad =
                        longitudReal(
                            texto
                        );


                    $mensaje
                        .removeClass(
                            "d-none"
                        )
                        .text(
                            "Has excedido el límite de 1000 caracteres. Por favor, haz más corto tu comentario."
                        );

                } else {

                    $mensaje
                        .addClass(
                            "d-none"
                        );
                }


                $contador
                    .text(
                        cantidad
                    );


                if (
                    cantidad >=
                    900
                ) {

                    $contadorCompleto
                        .removeClass(
                            "text-muted"
                        )
                        .addClass(
                            "text-danger fw-bold"
                        );

                } else {

                    $contadorCompleto
                        .removeClass(
                            "text-danger fw-bold"
                        )
                        .addClass(
                            "text-muted"
                        );
                }
            }
        );
}


// =========================================================
// VALIDAR COMENTARIO
// =========================================================

function validarComentario(
    selector
) {

    const comentario =
        $(selector).val() ||
        "";


    if (
        comentario.length >
        1000
    ) {

        mostrarResultado(
            true,
            "El comentario excede el límite de 1000 caracteres. Por favor, haz más corto tu comentario."
        );


        $(selector)
            .focus();


        return false;
    }


    return true;
}


// =========================================================
// BORRADOR AUTOMÁTICO - AUSENCIAS
// =========================================================

document.addEventListener(
    "erpsei:draft-restored",

    function (event) {

        const form =
            event.detail?.form;


        if (!form) {
            return;
        }


        // =========================================
        // INASISTENCIA
        // =========================================

        if (
            form.id ===
            "formInasistencia"
        ) {

            configurarLimiteFechaFin(
                "#inpInasistenciaFechaInicio",
                "#inpInasistenciaFechaFin"
            );


            calcularDias(
                "#inpInasistenciaFechaInicio",
                "#inpInasistenciaFechaFin",
                "#inpInasistenciaDias",
                "#lblDiasInasistencia"
            );


            actualizarContadorComentarioBorrador(
                "#txtComentarioInasistencia"
            );


            mostrarMensajeBorradorAusencias(
                "Inasistencia"
            );


            return;
        }


        // =========================================
        // INCAPACIDAD
        // =========================================

        if (
            form.id ===
            "formIncapacidad"
        ) {

            configurarLimiteFechaFin(
                "#inpIncapacidadFechaInicio",
                "#inpIncapacidadFechaFin"
            );


            calcularDias(
                "#inpIncapacidadFechaInicio",
                "#inpIncapacidadFechaFin",
                "#inpDiasIncapacidad",
                "#lblDiasIncapacidad"
            );


            actualizarContadorComentarioBorrador(
                "#txtComentarioIncapacidad"
            );


            mostrarMensajeBorradorAusencias(
                "Incapacidad"
            );


            return;
        }


        // =========================================
        // REGISTRAR PERMISO
        // =========================================

        if (
            form.id ===
            "formPermiso"
        ) {

            setTimeout(
                function () {

                    configurarLimiteFechaFin(
                        "#inpPermisoFechaInicio",
                        "#inpPermisoFechaFin"
                    );


                    alternarCamposPermiso(
                        $("#selTipoAusenciaPermiso")
                            .val(),
                        false
                    );


                    actualizarResumenPermiso();


                    actualizarContadorComentarioBorrador(
                        "#txtComentarioPermiso"
                    );

                },
                300
            );


            mostrarMensajeBorradorAusencias(
                "Permiso"
            );


            return;
        }


        // =========================================
        // SOLICITAR PERMISO
        // =========================================

        if (
            form.id ===
            "formSolicitarPermiso"
        ) {

            setTimeout(
                function () {

                    configurarLimiteFechaFin(
                        "#inpSolicitudPermisoFechaInicio",
                        "#inpSolicitudPermisoFechaFin"
                    );


                    alternarCamposPermiso(
                        $("#selTipoAusenciaSolicitud")
                            .val(),
                        true
                    );


                    actualizarResumenSolicitudPermiso();


                    actualizarContadorComentarioBorrador(
                        "#txtComentarioSolicitudPermiso"
                    );

                },
                300
            );


            mostrarMensajeBorradorAusencias(
                "Solicitud de permiso"
            );
        }
    }
);


// =========================================================
// MENSAJE DE BORRADOR RECUPERADO
// =========================================================

function mostrarMensajeBorradorAusencias(
    tipo
) {

    if (
        typeof showSuccess ===
        "function"
    ) {

        showSuccess(
            "Borrador recuperado",
            `Se restauró la información de ${tipo.toLowerCase()} que estabas capturando antes de que finalizara tu sesión.`
        );


        return;
    }


    console.log(
        `Borrador recuperado: ${tipo}`
    );
}


// =========================================================
// ACTUALIZAR CONTADOR DEL COMENTARIO RESTAURADO
// =========================================================

function actualizarContadorComentarioBorrador(
    selector
) {

    const textarea =
        document.querySelector(
            selector
        );


    if (!textarea) {
        return;
    }


    const contenedor =
        textarea.closest(
            ".comentario-ausencia-container"
        );


    if (!contenedor) {
        return;
    }


    const contador =
        contenedor.querySelector(
            ".contador-actual"
        );


    if (contador) {

        contador.textContent =
            textarea.value.length;
    }
}


// =========================================================
// VALIDAR RANGO DE FECHAS
// =========================================================

function validarRangoFechas(
    fechaInicio,
    fechaFin
) {

    if (
        !fechaInicio ||
        !fechaFin
    ) {
        return true;
    }


    const inicio =
        new Date(
            fechaInicio +
            "T00:00:00"
        );


    const fin =
        new Date(
            fechaFin +
            "T00:00:00"
        );


    if (
        isNaN(
            inicio.getTime()
        ) ||
        isNaN(
            fin.getTime()
        )
    ) {
        return false;
    }


    if (
        fin <
        inicio
    ) {
        return false;
    }


    const diasHabiles =
        calcularDiasHabiles(
            fechaInicio,
            fechaFin
        );


    return (
        diasHabiles <=
        MAXIMO_DIAS_HABILES_AUSENCIA
    );
}