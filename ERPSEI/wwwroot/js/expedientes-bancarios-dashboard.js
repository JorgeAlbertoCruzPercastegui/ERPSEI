document.addEventListener(
    "DOMContentLoaded",
    function () {
        document.body.classList.add(
            "module-main-theme",
            "expedientes-bancarios-page"
        );

        inicializarDashboard();
    }
);

let graficaAvanceEmpresas = null;
let graficaDescargasUsuarios = null;
let graficaBancos = null;
let graficaActividadDiaria = null;
let graficaActividadDocumentalUsuarios = null;
let graficaActividadEmpresasUsuarios = null;

// =====================================================
// INICIALIZACIÓN
// =====================================================
function inicializarDashboard() {
    establecerPeriodoInicial();

    document
        .getElementById("btnConsultarDashboard")
        ?.addEventListener(
            "click",
            cargarDashboard
    );

    document
        .getElementById(
            "btnExportarBitacora"
        )
        ?.addEventListener(
            "click",
            exportarBitacora
        );

    document
        .getElementById("dashboardFechaInicio")
        ?.addEventListener(
            "change",
            validarPeriodoSeleccionado
        );

    document
        .getElementById("dashboardFechaFin")
        ?.addEventListener(
            "change",
            validarPeriodoSeleccionado
        );

    cargarDashboard();
}

// =====================================================
// PERIODO INICIAL
// Primer día del mes actual hasta hoy
// =====================================================
function establecerPeriodoInicial() {
    const fechaActual = new Date();

    const primerDiaMes = new Date(
        fechaActual.getFullYear(),
        fechaActual.getMonth(),
        1
    );

    establecerValorFecha(
        "dashboardFechaInicio",
        primerDiaMes
    );

    establecerValorFecha(
        "dashboardFechaFin",
        fechaActual
    );
}

function establecerValorFecha(
    id,
    fecha
) {
    const elemento =
        document.getElementById(id);

    if (!elemento || !(fecha instanceof Date)) {
        return;
    }

    const anio =
        fecha.getFullYear();

    const mes =
        String(
            fecha.getMonth() + 1
        ).padStart(2, "0");

    const dia =
        String(
            fecha.getDate()
        ).padStart(2, "0");

    elemento.value =
        `${anio}-${mes}-${dia}`;
}

// =====================================================
// VALIDAR PERIODO
// =====================================================
function validarPeriodoSeleccionado() {
    const fechaInicio =
        document.getElementById(
            "dashboardFechaInicio"
        )?.value;

    const fechaFin =
        document.getElementById(
            "dashboardFechaFin"
        )?.value;

    ocultarErrorDashboard();

    if (!fechaInicio || !fechaFin) {
        return true;
    }

    if (fechaFin < fechaInicio) {
        mostrarErrorDashboard(
            "La fecha final no puede ser anterior a la fecha inicial."
        );

        return false;
    }

    const inicio =
        crearFechaLocal(fechaInicio);

    const fin =
        crearFechaLocal(fechaFin);

    if (!inicio || !fin) {
        mostrarErrorDashboard(
            "El periodo seleccionado no contiene fechas válidas."
        );

        return false;
    }

    const diferenciaMilisegundos =
        fin.getTime() -
        inicio.getTime();

    const diferenciaDias =
        Math.floor(
            diferenciaMilisegundos /
            (1000 * 60 * 60 * 24)
        );

    if (diferenciaDias > 1826) {
        mostrarErrorDashboard(
            "El periodo seleccionado no puede superar cinco años."
        );

        return false;
    }

    return true;
}

function crearFechaLocal(valor) {
    if (
        typeof valor !== "string" ||
        !valor
    ) {
        return null;
    }

    const partes = valor
        .split("-")
        .map(Number);

    if (
        partes.length !== 3 ||
        partes.some(numero =>
            Number.isNaN(numero)
        )
    ) {
        return null;
    }

    const [anio, mes, dia] =
        partes;

    const fecha = new Date(
        anio,
        mes - 1,
        dia
    );

    if (
        fecha.getFullYear() !== anio ||
        fecha.getMonth() !== mes - 1 ||
        fecha.getDate() !== dia
    ) {
        return null;
    }

    return fecha;
}

// =====================================================
// CARGAR DASHBOARD
// =====================================================
async function cargarDashboard() {
    const fechaInicio =
        document.getElementById(
            "dashboardFechaInicio"
        )?.value;

    const fechaFin =
        document.getElementById(
            "dashboardFechaFin"
        )?.value;

    if (!fechaInicio) {
        mostrarErrorDashboard(
            "Selecciona la fecha inicial."
        );

        return;
    }

    if (!fechaFin) {
        mostrarErrorDashboard(
            "Selecciona la fecha final."
        );

        return;
    }

    if (!validarPeriodoSeleccionado()) {
        return;
    }

    mostrarCargaDashboard();

    try {
        const parametros =
            new URLSearchParams({
                handler: "Datos",
                fechaInicio,
                fechaFin
            });

        const response = await fetch(
            `${window.location.pathname}` +
            `?${parametros.toString()}`,
            {
                credentials: "same-origin",
                headers: {
                    Accept: "application/json"
                }
            }
        );

        let resultado = null;

        try {
            resultado =
                await response.json();
        } catch {
            throw new Error(
                "El servidor devolvió una respuesta no válida."
            );
        }

        if (
            !response.ok ||
            !resultado?.success
        ) {
            throw new Error(
                resultado?.message ??
                "No fue posible consultar el dashboard."
            );
        }

        actualizarIndicadores(
            resultado.resumen
        );

        renderizarAvanceEmpresas(
            resultado.graficas
                ?.avanceEmpresas ?? []
        );

        renderizarDescargasUsuarios(
            resultado.graficas
                ?.descargasUsuarios ?? []
        );

        renderizarBancos(
            resultado.graficas
                ?.documentosPorBanco ?? []
        );

        renderizarActividadDiaria(
            resultado.graficas
                ?.actividadDiaria ?? [],
            resultado.periodo
                ?.agrupacion ?? "Diaria"
        );

        renderizarActividadDocumentalUsuarios(
            resultado.graficas
                ?.actividadDocumentalUsuarios ?? []
        );

        renderizarActividadEmpresasUsuarios(
            resultado.graficas
                ?.actividadEmpresasUsuarios ?? []
        );

        ocultarCargaDashboard();
        ocultarErrorDashboard();

    } catch (error) {
        console.error(
            "Error al consultar dashboard:",
            error
        );

        mostrarErrorDashboard(
            error?.message ??
            "Ocurrió un error al consultar el dashboard."
        );
    }
}

// =====================================================
// EXPORTAR BITÁCORA
// =====================================================
function exportarBitacora() {
    const fechaInicio =
        document.getElementById(
            "dashboardFechaInicio"
        )?.value;

    const fechaFin =
        document.getElementById(
            "dashboardFechaFin"
        )?.value;

    ocultarErrorDashboard();

    if (!fechaInicio) {
        mostrarErrorDashboard(
            "Selecciona la fecha inicial."
        );

        document
            .getElementById(
                "dashboardFechaInicio"
            )
            ?.focus();

        return;
    }

    if (!fechaFin) {
        mostrarErrorDashboard(
            "Selecciona la fecha final."
        );

        document
            .getElementById(
                "dashboardFechaFin"
            )
            ?.focus();

        return;
    }

    if (!validarPeriodoSeleccionado()) {
        return;
    }

    const boton =
        document.getElementById(
            "btnExportarBitacora"
        );

    if (!boton) {
        return;
    }

    const htmlOriginal =
        boton.innerHTML;

    boton.disabled = true;

    boton.innerHTML =
        '<i class="fa-solid fa-spinner ' +
        'fa-spin me-1"></i>' +
        " Generando...";

    try {
        const parametros =
            new URLSearchParams({
                handler:
                    "ExportarBitacora",

                fechaInicio:
                    fechaInicio,

                fechaFin:
                    fechaFin
            });

        const url =
            `${window.location.pathname}` +
            `?${parametros.toString()}`;

        /*
         * Utilizamos un enlace temporal porque el handler
         * devuelve directamente un archivo Excel.
         */
        const enlace =
            document.createElement("a");

        enlace.href = url;
        enlace.style.display = "none";

        document.body.appendChild(
            enlace
        );

        enlace.click();
        enlace.remove();
    } catch (error) {
        console.error(
            "Error al exportar la bitácora:",
            error
        );

        mostrarErrorDashboard(
            "No fue posible generar la bitácora."
        );
    } finally {
        setTimeout(
            function () {
                boton.disabled = false;
                boton.innerHTML =
                    htmlOriginal;
            },
            800
        );
    }
}

// =====================================================
// INDICADORES
// =====================================================
function actualizarIndicadores(resumen) {
    establecerTexto(
        "kpiDocumentosDescargados",
        resumen?.documentosDescargados ?? 0
    );

    establecerTexto(
        "kpiVisualizaciones",
        resumen?.visualizaciones ?? 0
    );

    establecerTexto(
        "kpiEmpresasNuevas",
        resumen?.empresasNuevas ?? 0
    );

    establecerTexto(
        "kpiDocumentosCargados",
        resumen?.documentosCargados ?? 0
    );
}

// =====================================================
// GRÁFICA: AVANCE DOCUMENTAL
// =====================================================
function renderizarAvanceEmpresas(datos) {
    destruirGrafica(
        graficaAvanceEmpresas
    );

    graficaAvanceEmpresas = null;

    const vacio =
        document.getElementById(
            "vacioAvanceEmpresas"
        );

    const canvas =
        document.getElementById(
            "graficaAvanceEmpresas"
        );

    if (
        !canvas ||
        !Array.isArray(datos) ||
        datos.length === 0
    ) {
        canvas?.classList.add(
            "d-none"
        );

        vacio?.classList.remove(
            "d-none"
        );

        return;
    }

    canvas.classList.remove(
        "d-none"
    );

    vacio?.classList.add(
        "d-none"
    );

    graficaAvanceEmpresas =
        new Chart(
            canvas,
            {
                type: "bar",

                data: {
                    labels: datos.map(
                        item =>
                            item.empresa
                    ),

                    datasets: [
                        {
                            label:
                                "Avance documental",

                            data: datos.map(
                                item =>
                                    item.porcentaje
                            ),

                            borderWidth: 1,
                            borderRadius: 7
                        }
                    ]
                },

                options: {
                    indexAxis: "y",
                    responsive: true,
                    maintainAspectRatio: false,

                    scales: {
                        x: {
                            beginAtZero: true,
                            max: 100,

                            ticks: {
                                callback:
                                    function (
                                        valor
                                    ) {
                                        return `${valor} %`;
                                    }
                            }
                        }
                    },

                    plugins: {
                        legend: {
                            display: false
                        },

                        tooltip: {
                            callbacks: {
                                label:
                                    function (
                                        contexto
                                    ) {
                                        const registro =
                                            datos[
                                            contexto
                                                .dataIndex
                                            ];

                                        return [
                                            `Avance: ${registro.porcentaje} %`,
                                            `Cargados: ${registro.cargados}`,
                                            `Requeridos: ${registro.requeridos}`
                                        ];
                                    }
                            }
                        }
                    }
                }
            }
        );
}

// =====================================================
// GRÁFICA: DESCARGAS POR USUARIO
// =====================================================
function renderizarDescargasUsuarios(datos) {
    destruirGrafica(
        graficaDescargasUsuarios
    );

    graficaDescargasUsuarios = null;

    const canvas =
        document.getElementById(
            "graficaDescargasUsuarios"
        );

    const vacio =
        document.getElementById(
            "vacioDescargasUsuarios"
        );

    if (
        !canvas ||
        !Array.isArray(datos) ||
        datos.length === 0
    ) {
        canvas?.classList.add(
            "d-none"
        );

        vacio?.classList.remove(
            "d-none"
        );

        return;
    }

    canvas.classList.remove(
        "d-none"
    );

    vacio?.classList.add(
        "d-none"
    );

    graficaDescargasUsuarios =
        new Chart(
            canvas,
            {
                type: "bar",

                data: {
                    labels: datos.map(
                        item =>
                            item.usuario
                    ),

                    datasets: [
                        {
                            label:
                                "Descargas",

                            data: datos.map(
                                item =>
                                    item.total
                            ),

                            borderWidth: 1,
                            borderRadius: 7
                        }
                    ]
                },

                options: {
                    indexAxis: "y",
                    responsive: true,
                    maintainAspectRatio: false,

                    plugins: {
                        legend: {
                            display: false
                        }
                    },

                    scales: {
                        x: {
                            beginAtZero: true,

                            ticks: {
                                precision: 0
                            }
                        }
                    }
                }
            }
        );
}

// =====================================================
// GRÁFICA: DOCUMENTOS POR BANCO
// =====================================================
function renderizarBancos(datos) {
    destruirGrafica(
        graficaBancos
    );

    graficaBancos = null;

    const canvas =
        document.getElementById(
            "graficaBancos"
        );

    const vacio =
        document.getElementById(
            "vacioBancos"
        );

    if (
        !canvas ||
        !Array.isArray(datos) ||
        datos.length === 0
    ) {
        canvas?.classList.add(
            "d-none"
        );

        vacio?.classList.remove(
            "d-none"
        );

        return;
    }

    canvas.classList.remove(
        "d-none"
    );

    vacio?.classList.add(
        "d-none"
    );

    graficaBancos =
        new Chart(
            canvas,
            {
                type: "bar",

                data: {
                    labels: datos.map(
                        item =>
                            item.banco
                    ),

                    datasets: [
                        {
                            label:
                                "Documentos utilizados",

                            data: datos.map(
                                item =>
                                    item.total
                            ),

                            borderWidth: 1,
                            borderRadius: 7
                        }
                    ]
                },

                options: {
                    responsive: true,
                    maintainAspectRatio: false,

                    plugins: {
                        legend: {
                            display: false
                        }
                    },

                    scales: {
                        y: {
                            beginAtZero: true,

                            ticks: {
                                precision: 0
                            }
                        }
                    }
                }
            }
        );
}

// =====================================================
// GRÁFICA: ACTIVIDAD DOCUMENTAL
// =====================================================
function renderizarActividadDiaria(
    datos,
    agrupacion
) {
    destruirGrafica(
        graficaActividadDiaria
    );

    graficaActividadDiaria = null;

    const canvas =
        document.getElementById(
            "graficaActividadDiaria"
        );

    const vacio =
        document.getElementById(
            "vacioActividadDiaria"
        );

    const contieneActividad =
        Array.isArray(datos) &&
        datos.some(
            item =>
                Number(
                    item.documentosCargados
                ) > 0
        );

    if (
        !canvas ||
        !Array.isArray(datos) ||
        datos.length === 0 ||
        !contieneActividad
    ) {
        canvas?.classList.add(
            "d-none"
        );

        vacio?.classList.remove(
            "d-none"
        );

        return;
    }

    canvas.classList.remove(
        "d-none"
    );

    vacio?.classList.add(
        "d-none"
    );

    const tituloEje =
        agrupacion === "Mensual"
            ? "Mes"
            : "Día";

    graficaActividadDiaria =
        new Chart(
            canvas,
            {
                type: "line",

                data: {
                    labels: datos.map(
                        item =>
                            item.etiqueta
                    ),

                    datasets: [
                        {
                            label:
                                "Documentos cargados",

                            data: datos.map(
                                item =>
                                    item.documentosCargados
                            ),

                            borderWidth: 2,
                            tension: 0.25,
                            fill: false,
                            pointRadius: 3,
                            pointHoverRadius: 5
                        }
                    ]
                },

                options: {
                    responsive: true,
                    maintainAspectRatio: false,

                    scales: {
                        y: {
                            beginAtZero: true,

                            ticks: {
                                precision: 0
                            },

                            title: {
                                display: true,
                                text:
                                    "Documentos cargados"
                            }
                        },

                        x: {
                            title: {
                                display: true,
                                text: tituloEje
                            }
                        }
                    },

                    plugins: {
                        legend: {
                            display: false
                        },

                        tooltip: {
                            callbacks: {
                                label:
                                    function (
                                        contexto
                                    ) {
                                        const total =
                                            contexto
                                                .parsed
                                                .y ?? 0;

                                        return (
                                            `Documentos cargados: ${total}`
                                        );
                                    }
                            }
                        }
                    }
                }
            }
        );
}

// =====================================================
// GRÁFICA: ACTIVIDAD DOCUMENTAL POR USUARIO
// =====================================================
function renderizarActividadDocumentalUsuarios(datos) {
    destruirGrafica(
        graficaActividadDocumentalUsuarios
    );

    graficaActividadDocumentalUsuarios = null;

    const canvas =
        document.getElementById(
            "graficaActividadDocumentalUsuarios"
        );

    const vacio =
        document.getElementById(
            "vacioActividadDocumentalUsuarios"
        );

    const contieneActividad =
        Array.isArray(datos) &&
        datos.some(item =>
            Number(item.creados ?? 0) > 0 ||
            Number(item.reemplazados ?? 0) > 0 ||
            Number(item.visualizados ?? 0) > 0 ||
            Number(item.descargados ?? 0) > 0 ||
            Number(item.eliminados ?? 0) > 0
        );

    if (
        !canvas ||
        !Array.isArray(datos) ||
        datos.length === 0 ||
        !contieneActividad
    ) {
        canvas?.classList.add(
            "d-none"
        );

        vacio?.classList.remove(
            "d-none"
        );

        return;
    }

    canvas.classList.remove(
        "d-none"
    );

    vacio?.classList.add(
        "d-none"
    );

    const datasetsDocumentales = [
        {
            label: "Cargados",

            data: datos.map(
                item =>
                    Number(
                        item.creados ?? 0
                    )
            ),

            backgroundColor:
                "#22C55E",

            borderColor:
                "#22C55E",

            borderWidth: 1,
            borderRadius: 4
        },
        {
            label: "Reemplazados",

            data: datos.map(
                item =>
                    Number(
                        item.reemplazados ?? 0
                    )
            ),

            backgroundColor:
                "#F59E0B",

            borderColor:
                "#F59E0B",

            borderWidth: 1,
            borderRadius: 4
        },
        {
            label: "Visualizados",

            data: datos.map(
                item =>
                    Number(
                        item.visualizados ?? 0
                    )
            ),

            backgroundColor:
                "#3B82F6",

            borderColor:
                "#3B82F6",

            borderWidth: 1,
            borderRadius: 4
        },
        {
            label: "Descargados",

            data: datos.map(
                item =>
                    Number(
                        item.descargados ?? 0
                    )
            ),

            backgroundColor:
                "#8B5CF6",

            borderColor:
                "#8B5CF6",

            borderWidth: 1,
            borderRadius: 4
        },
        {
            label: "Eliminados",

            data: datos.map(
                item =>
                    Number(
                        item.eliminados ?? 0
                    )
            ),

            backgroundColor:
                "#EF4444",

            borderColor:
                "#EF4444",

            borderWidth: 1,
            borderRadius: 4
        }
    ];

    graficaActividadDocumentalUsuarios =
        new Chart(
            canvas,
            {
                type: "bar",

                data: {
                    labels: datos.map(
                        item =>
                            item.usuario
                    ),

                    datasets:
                        datasetsDocumentales
                },

                options: {
                    indexAxis: "y",
                    responsive: true,
                    maintainAspectRatio: false,

                    interaction: {
                        mode: "nearest",
                        intersect: true
                    },

                    scales: {
                        x: {
                            beginAtZero: true,
                            stacked: true,

                            ticks: {
                                precision: 0
                            },

                            title: {
                                display: true,
                                text:
                                    "Total de movimientos"
                            }
                        },

                        y: {
                            stacked: true
                        }
                    },

                    plugins: {
                        legend: {
                            display: true,
                            position: "bottom",

                            labels: {
                                usePointStyle: true,
                                boxWidth: 10,
                                padding: 16
                            }
                        },

                        tooltip: {
                            filter: function (contexto) {
                                return Number(
                                    contexto.raw ?? 0
                                ) > 0;
                            },

                            callbacks: {
                                afterBody:
                                    function (
                                        elementos
                                    ) {
                                        if (
                                            !elementos ||
                                            elementos.length === 0
                                        ) {
                                            return "";
                                        }

                                        const indice =
                                            elementos[0]
                                                .dataIndex;

                                        const registro =
                                            datos[indice];

                                        const total =
                                            Number(
                                                registro.creados ?? 0
                                            ) +
                                            Number(
                                                registro.reemplazados ?? 0
                                            ) +
                                            Number(
                                                registro.visualizados ?? 0
                                            ) +
                                            Number(
                                                registro.descargados ?? 0
                                            ) +
                                            Number(
                                                registro.eliminados ?? 0
                                            );

                                        return (
                                            `Total: ${total}`
                                        );
                                    }
                            }
                        }
                    }
                }
            }
        );
}

// =====================================================
// GRÁFICA: ACTIVIDAD DE EMPRESAS POR USUARIO
// =====================================================
function renderizarActividadEmpresasUsuarios(datos) {
    destruirGrafica(
        graficaActividadEmpresasUsuarios
    );

    graficaActividadEmpresasUsuarios = null;

    const canvas =
        document.getElementById(
            "graficaActividadEmpresasUsuarios"
        );

    const vacio =
        document.getElementById(
            "vacioActividadEmpresasUsuarios"
        );

    const contieneActividad =
        Array.isArray(datos) &&
        datos.some(item =>
            Number(item.consultas ?? 0) > 0 ||
            Number(item.creaciones ?? 0) > 0 ||
            Number(item.ediciones ?? 0) > 0 ||
            Number(item.cambiosEstatus ?? 0) > 0 ||
            Number(item.eliminaciones ?? 0) > 0
        );

    if (
        !canvas ||
        !Array.isArray(datos) ||
        datos.length === 0 ||
        !contieneActividad
    ) {
        canvas?.classList.add(
            "d-none"
        );

        vacio?.classList.remove(
            "d-none"
        );

        return;
    }

    canvas.classList.remove(
        "d-none"
    );

    vacio?.classList.add(
        "d-none"
    );

    const datasetsEmpresas = [
        {
            label: "Consultas",

            data: datos.map(
                item =>
                    Number(
                        item.consultas ?? 0
                    )
            ),

            backgroundColor:
                "#3B82F6",

            borderColor:
                "#3B82F6",

            borderWidth: 1,
            borderRadius: 4
        },
        {
            label: "Creaciones",

            data: datos.map(
                item =>
                    Number(
                        item.creaciones ?? 0
                    )
            ),

            backgroundColor:
                "#10B981",

            borderColor:
                "#10B981",

            borderWidth: 1,
            borderRadius: 4
        },
        {
            label: "Ediciones",

            data: datos.map(
                item =>
                    Number(
                        item.ediciones ?? 0
                    )
            ),

            backgroundColor:
                "#F59E0B",

            borderColor:
                "#F59E0B",

            borderWidth: 1,
            borderRadius: 4
        },
        {
            label:
                "Cambios de estatus",

            data: datos.map(
                item =>
                    Number(
                        item.cambiosEstatus ?? 0
                    )
            ),

            backgroundColor:
                "#A855F7",

            borderColor:
                "#A855F7",

            borderWidth: 1,
            borderRadius: 4
        },
        {
            label: "Eliminaciones",

            data: datos.map(
                item =>
                    Number(
                        item.eliminaciones ?? 0
                    )
            ),

            backgroundColor:
                "#EF4444",

            borderColor:
                "#EF4444",

            borderWidth: 1,
            borderRadius: 4
        }
    ];

    graficaActividadEmpresasUsuarios =
        new Chart(
            canvas,
            {
                type: "bar",

                data: {
                    labels: datos.map(
                        item =>
                            item.usuario
                    ),

                    datasets:
                        datasetsEmpresas
                },

                options: {
                    indexAxis: "y",
                    responsive: true,
                    maintainAspectRatio: false,

                    interaction: {
                        mode: "nearest",
                        intersect: true
                    },

                    scales: {
                        x: {
                            beginAtZero: true,
                            stacked: true,

                            ticks: {
                                precision: 0
                            },

                            title: {
                                display: true,
                                text:
                                    "Total de movimientos"
                            }
                        },

                        y: {
                            stacked: true
                        }
                    },

                    plugins: {
                        legend: {
                            display: true,
                            position: "bottom",

                            labels: {
                                usePointStyle: true,
                                boxWidth: 10,
                                padding: 16
                            }
                        },

                        tooltip: {
                            filter: function (contexto) {
                                return Number(
                                    contexto.raw ?? 0
                                ) > 0;
                            },

                            callbacks: {
                                afterBody:
                                    function (
                                        elementos
                                    ) {
                                        if (
                                            !elementos ||
                                            elementos.length === 0
                                        ) {
                                            return "";
                                        }

                                        const indice =
                                            elementos[0]
                                                .dataIndex;

                                        const registro =
                                            datos[indice];

                                        const total =
                                            Number(
                                                registro.consultas ?? 0
                                            ) +
                                            Number(
                                                registro.creaciones ?? 0
                                            ) +
                                            Number(
                                                registro.ediciones ?? 0
                                            ) +
                                            Number(
                                                registro.cambiosEstatus ?? 0
                                            ) +
                                            Number(
                                                registro.eliminaciones ?? 0
                                            );

                                        return (
                                            `Total: ${total}`
                                        );
                                    }
                            }
                        }
                    }
                }
            }
        );
}

// =====================================================
// DESTRUIR GRÁFICAS
// =====================================================
function destruirGrafica(grafica) {
    if (
        grafica &&
        typeof grafica.destroy ===
        "function"
    ) {
        grafica.destroy();
    }
}

// =====================================================
// ESTADOS DEL DASHBOARD
// =====================================================
function mostrarCargaDashboard() {
    document
        .getElementById(
            "dashboardCargando"
        )
        ?.classList.remove(
            "d-none"
        );

    document
        .getElementById(
            "dashboardError"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "btnConsultarDashboard"
        )
        ?.setAttribute(
            "disabled",
            "disabled"
    );

    document
        .getElementById(
            "btnExportarBitacora"
        )
        ?.setAttribute(
            "disabled",
            "disabled"
        );
}

function ocultarCargaDashboard() {
    document
        .getElementById(
            "dashboardCargando"
        )
        ?.classList.add(
            "d-none"
        );

    document
        .getElementById(
            "btnConsultarDashboard"
        )
        ?.removeAttribute(
            "disabled"
    );

    document
        .getElementById(
            "btnExportarBitacora"
        )
        ?.removeAttribute(
            "disabled"
        );
}

function mostrarErrorDashboard(mensaje) {
    ocultarCargaDashboard();

    establecerTexto(
        "dashboardErrorMensaje",
        mensaje
    );

    document
        .getElementById(
            "dashboardError"
        )
        ?.classList.remove(
            "d-none"
        );
}

function ocultarErrorDashboard() {
    document
        .getElementById(
            "dashboardError"
        )
        ?.classList.add(
            "d-none"
        );
}

// =====================================================
// UTILIDADES
// =====================================================
function establecerTexto(
    id,
    valor
) {
    const elemento =
        document.getElementById(id);

    if (elemento) {
        elemento.textContent =
            valor?.toString() ?? "";
    }
}