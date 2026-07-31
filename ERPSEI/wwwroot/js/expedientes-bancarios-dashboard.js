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