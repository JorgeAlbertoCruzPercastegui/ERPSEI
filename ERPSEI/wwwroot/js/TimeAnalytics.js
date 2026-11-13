(() => {
    "use strict";

    // ============================================================
    // ELEMENTOS DEL DOM
    // ============================================================

    const fechaInicio =
        document.getElementById("analyticsFechaInicio");

    const fechaFin =
        document.getElementById("analyticsFechaFin");

    const area =
        document.getElementById("analyticsArea");

    const usuario =
        document.getElementById("analyticsUsuario");

    const kpiHorasRegistradas =
        document.getElementById("kpiHorasRegistradas");

    const kpiHorasFaltantes =
        document.getElementById("kpiHorasFaltantes");

    const kpiCumplimiento =
        document.getElementById("kpiCumplimiento");

    const kpiColaboradores =
        document.getElementById("kpiColaboradores");

    const kpiAreas =
        document.getElementById("kpiAreas");

    const tablaUsuarios =
        document.getElementById("analyticsTablaUsuarios");

    const estadoTexto =
        document.querySelector(
            ".time-analytics-status strong"
        );

    const estadoPunto =
        document.querySelector(
            ".time-analytics-status-dot"
        );

    // ============================================================
    // GRÁFICAS
    // ============================================================

    let chartDistribucionTipo = null;
    let chartTendencia = null;

    // Cumplimiento por área se implementará cuando traslademos
    // la fórmula exacta del PBIX.
    let chartCumplimientoArea = null;

    // ============================================================
    // FORMATEADORES
    // ============================================================

    const formatoNumero =
        new Intl.NumberFormat(
            "es-MX",
            {
                minimumFractionDigits: 0,
                maximumFractionDigits: 2
            }
        );

    const formatoFecha =
        new Intl.DateTimeFormat(
            "es-MX",
            {
                day: "2-digit",
                month: "short"
            }
        );

    // ============================================================
    // INICIALIZAR
    // ============================================================

    async function inicializar() {

        establecerFechasIniciales();

        await cargarCatalogos();

        configurarEventos();

        await cargarDashboard();
    }

    // ============================================================
    // FECHAS INICIALES
    //
    // Por defecto:
    // primer día del mes actual
    // hasta hoy.
    // ============================================================

    function establecerFechasIniciales() {

        const hoy =
            new Date();

        const inicio =
            new Date(
                hoy.getFullYear(),
                hoy.getMonth(),
                1
            );

        fechaInicio.value =
            convertirFechaInput(
                inicio
            );

        fechaFin.value =
            convertirFechaInput(
                hoy
            );
    }

    function convertirFechaInput(fecha) {

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

        return `${anio}-${mes}-${dia}`;
    }

    // ============================================================
    // CATÁLOGOS
    // ============================================================

    async function cargarCatalogos() {

        actualizarEstado(
            "Cargando filtros...",
            "loading"
        );

        try {

            const respuesta =
                await fetch(
                    "?handler=Catalogos",
                    {
                        method: "GET",
                        headers: {
                            "Accept": "application/json"
                        }
                    }
                );

            if (!respuesta.ok) {
                throw new Error(
                    "No fue posible obtener los filtros."
                );
            }

            const datos =
                await respuesta.json();

            if (!datos.success) {
                throw new Error(
                    datos.message ||
                    "No fue posible obtener los filtros."
                );
            }

            cargarSelect(
                usuario,
                datos.usuarios,
                "Todos los colaboradores"
            );

            cargarSelect(
                area,
                datos.areas,
                "Todas las áreas"
            );
        }
        catch (error) {

            console.error(
                "TIME Analytics - Catálogos:",
                error
            );

            actualizarEstado(
                "Error al cargar filtros",
                "error"
            );
        }
    }

    function cargarSelect(
        select,
        opciones,
        textoTodos
    ) {

        select.innerHTML = "";

        const opcionTodos =
            document.createElement(
                "option"
            );

        opcionTodos.value = "";
        opcionTodos.textContent =
            textoTodos;

        select.appendChild(
            opcionTodos
        );

        if (!Array.isArray(opciones)) {
            return;
        }

        opciones.forEach(opcion => {

            const elemento =
                document.createElement(
                    "option"
                );

            elemento.value =
                opcion.id;

            elemento.textContent =
                opcion.nombre;

            select.appendChild(
                elemento
            );
        });
    }

    // ============================================================
    // EVENTOS
    // ============================================================

    function configurarEventos() {

        fechaInicio.addEventListener(
            "change",
            cargarDashboard
        );

        fechaFin.addEventListener(
            "change",
            cargarDashboard
        );

        area.addEventListener(
            "change",
            cargarDashboard
        );

        usuario.addEventListener(
            "change",
            cargarDashboard
        );
    }

    // ============================================================
    // DASHBOARD
    // ============================================================

    async function cargarDashboard() {

        if (!validarFechas()) {
            return;
        }

        bloquearFiltros(
            true
        );

        actualizarEstado(
            "Actualizando información...",
            "loading"
        );

        try {

            const parametros =
                new URLSearchParams();

            if (fechaInicio.value) {
                parametros.set(
                    "fechaInicio",
                    fechaInicio.value
                );
            }

            if (fechaFin.value) {
                parametros.set(
                    "fechaFin",
                    fechaFin.value
                );
            }

            if (usuario.value) {
                parametros.set(
                    "usuarioId",
                    usuario.value
                );
            }

            if (area.value) {
                parametros.set(
                    "departamentoId",
                    area.value
                );
            }

            const url =
                `?handler=Dashboard&${parametros.toString()}`;

            const respuesta =
                await fetch(
                    url,
                    {
                        method: "GET",
                        headers: {
                            "Accept": "application/json"
                        }
                    }
                );

            const datos =
                await respuesta.json();

            if (!respuesta.ok ||
                !datos.success) {

                throw new Error(
                    datos.message ||
                    "No fue posible consultar Analítica TIME."
                );
            }

            pintarDashboard(
                datos.dashboard
            );

            actualizarEstado(
                crearTextoActualizacion(
                    datos.dashboard
                        .fechaActualizacion
                ),
                "success"
            );
        }
        catch (error) {

            console.error(
                "TIME Analytics - Dashboard:",
                error
            );

            limpiarDashboard();

            actualizarEstado(
                "Error al consultar información",
                "error"
            );
        }
        finally {

            bloquearFiltros(
                false
            );
        }
    }

    // ============================================================
    // VALIDACIÓN DE FILTROS
    // ============================================================

    function validarFechas() {

        if (!fechaInicio.value ||
            !fechaFin.value) {

            actualizarEstado(
                "Selecciona un periodo",
                "warning"
            );

            return false;
        }

        if (fechaInicio.value >
            fechaFin.value) {

            actualizarEstado(
                "La fecha inicial supera la fecha final",
                "error"
            );

            return false;
        }

        return true;
    }

    function bloquearFiltros(
        bloquear
    ) {

        fechaInicio.disabled =
            bloquear;

        fechaFin.disabled =
            bloquear;

        area.disabled =
            bloquear;

        usuario.disabled =
            bloquear;
    }

    // ============================================================
    // PINTAR DASHBOARD
    // ============================================================

    function pintarDashboard(
        dashboard
    ) {

        if (!dashboard) {
            limpiarDashboard();
            return;
        }

        pintarKpis(
            dashboard.kpis
        );

        pintarDistribucionTipo(
            dashboard.distribucionTipo || []
        );

        pintarTendencia(
            dashboard.tendencia || []
        );

        pintarTablaColaboradores(
            dashboard.colaboradores || []
        );

        pintarCumplimientoArea(
            dashboard.areas || []
        );
    }

    // ============================================================
    // KPIs
    // ============================================================

    function pintarKpis(kpis) {

        if (!kpis) {
            limpiarKpis();
            return;
        }

        kpiHorasRegistradas.textContent =
            `${formatoNumero.format(
                kpis.horasRegistradas || 0
            )} h`;

        kpiHorasFaltantes.textContent =
            `${formatoNumero.format(
                kpis.horasFaltantes || 0
            )} h`;

        kpiCumplimiento.textContent =
            `${formatoNumero.format(
                (kpis.cumplimiento || 0) * 100
            )}%`;

        kpiColaboradores.textContent =
            formatoNumero.format(
                kpis.colaboradores || 0
            );

        kpiAreas.textContent =
            formatoNumero.format(
                kpis.areas || 0
            );
    }

    function limpiarKpis() {

        kpiHorasRegistradas.textContent =
            "—";

        kpiHorasFaltantes.textContent =
            "—";

        kpiCumplimiento.textContent =
            "—";

        kpiColaboradores.textContent =
            "—";

        kpiAreas.textContent =
            "—";
    }

    // ============================================================
    // DISTRIBUCIÓN POR TIPO
    // ============================================================

    function pintarDistribucionTipo(
        datos
    ) {

        if (chartDistribucionTipo) {

            chartDistribucionTipo.destroy();

            chartDistribucionTipo =
                null;
        }

        const canvas =
            document.getElementById(
                "chartDistribucionTipo"
            );

        if (!canvas ||
            typeof Chart === "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        const etiquetas =
            datos.map(x =>
                x.tipo
            );

        const valores =
            datos.map(x =>
                x.horas
            );

        chartDistribucionTipo =
            new Chart(
                canvas,
                {
                    type: "doughnut",

                    data: {
                        labels:
                            etiquetas,

                        datasets: [
                            {
                                data:
                                    valores,

                                borderWidth:
                                    2,

                                borderColor:
                                    "#ffffff"
                            }
                        ]
                    },

                    options: {
                        responsive: true,

                        maintainAspectRatio:
                            false,

                        cutout:
                            "68%",

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true,

                                    padding:
                                        18
                                }
                            },

                            tooltip: {

                                callbacks: {

                                    label:
                                        context => {

                                            const valor =
                                                context
                                                    .parsed ||
                                                0;

                                            return (
                                                `${context.label}: ` +
                                                `${formatoNumero.format(valor)} h`
                                            );
                                        }
                                }
                            }
                        }
                    }
                }
            );
    }

    // ============================================================
    // TENDENCIA DE HORAS
    // ============================================================

    function pintarTendencia(
        datos
    ) {

        if (chartTendencia) {

            chartTendencia.destroy();

            chartTendencia =
                null;
        }

        const canvas =
            document.getElementById(
                "chartTendencia"
            );

        if (!canvas ||
            typeof Chart === "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        const etiquetas =
            datos.map(x =>
                formatoFecha.format(
                    convertirFechaJson(
                        x.fecha
                    )
                )
            );

        const valores =
            datos.map(x =>
                x.horas
            );

        chartTendencia =
            new Chart(
                canvas,
                {
                    type:
                        "line",

                    data: {

                        labels:
                            etiquetas,

                        datasets: [
                            {
                                label:
                                    "Horas registradas",

                                data:
                                    valores,

                                borderWidth:
                                    3,

                                tension:
                                    0.28,

                                pointRadius:
                                    3,

                                pointHoverRadius:
                                    5,

                                fill:
                                    false
                            }
                        ]
                    },

                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        interaction: {
                            intersect:
                                false,

                            mode:
                                "index"
                        },

                        plugins: {

                            legend: {
                                display:
                                    false
                            },

                            tooltip: {

                                callbacks: {

                                    label:
                                        context =>
                                            `${formatoNumero.format(
                                                context.parsed.y || 0
                                            )} h`
                                }
                            }
                        },

                        scales: {

                            y: {
                                beginAtZero:
                                    true,

                                ticks: {

                                    callback:
                                        value =>
                                            `${formatoNumero.format(
                                                value
                                            )} h`
                                }
                            }
                        }
                    }
                }
            );
    }

    // ============================================================
    // CUMPLIMIENTO POR ÁREA
    // ============================================================

    function pintarCumplimientoArea(datos) {

        if (chartCumplimientoArea) {

            chartCumplimientoArea.destroy();

            chartCumplimientoArea = null;
        }

        const canvas =
            document.getElementById(
                "chartCumplimientoArea"
            );

        if (!canvas ||
            typeof Chart === "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        const etiquetas =
            datos.map(x =>
                x.area
            );

        chartCumplimientoArea =
            new Chart(
                canvas,
                {
                    type: "bar",

                    data: {

                        labels:
                            etiquetas,

                        datasets: [

                            {
                                label:
                                    "Back Office",

                                data:
                                    datos.map(x =>
                                        x.horasBackOffice
                                    ),

                                stack:
                                    "cumplimiento"
                            },

                            {
                                label:
                                    "Operativas + Clientes",

                                data:
                                    datos.map(x =>
                                        x.horasOperativasClientes
                                    ),

                                stack:
                                    "cumplimiento"
                            },

                            {
                                label:
                                    "Horas Faltantes",

                                data:
                                    datos.map(x =>
                                        x.horasFaltantes
                                    ),

                                stack:
                                    "cumplimiento"
                            }
                        ]
                    },

                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        indexAxis:
                            "y",

                        interaction: {
                            intersect:
                                false,

                            mode:
                                "index"
                        },

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true,

                                    padding:
                                        18
                                }
                            },

                            tooltip: {

                                callbacks: {

                                    label:
                                        context => {

                                            return (
                                                `${context.dataset.label}: ` +
                                                `${formatoNumero.format(
                                                    context.parsed.x || 0
                                                )} h`
                                            );
                                        }
                                }
                            }
                        },

                        scales: {

                            x: {
                                stacked:
                                    true,

                                beginAtZero:
                                    true
                            },

                            y: {
                                stacked:
                                    true
                            }
                        }
                    }
                }
            );
    }

    // ============================================================
    // TABLA COLABORADORES
    // ============================================================

    function pintarTablaColaboradores(
        registros
    ) {

        tablaUsuarios.innerHTML =
            "";

        if (!Array.isArray(registros) ||
            registros.length === 0) {

            tablaUsuarios.innerHTML =
                `
                <tr>
                    <td colspan="5"
                        class="text-center text-muted py-4">
                        No se encontraron registros
                        para el periodo seleccionado.
                    </td>
                </tr>
                `;

            return;
        }

        const fragmento =
            document.createDocumentFragment();

        registros.forEach(
            registro => {

                const fila =
                    document.createElement(
                        "tr"
                    );

                const cumplimiento =
                    `${formatoNumero.format(
                        (registro.cumplimiento || 0) * 100
                    )}%`;

                fila.innerHTML =
                    `
                    <td>
                        ${escaparHtml(
                                        registro.nombre || ""
                                    )}
                    </td>

                    <td>
                        ${escaparHtml(
                                        registro.area || "Sin área"
                                    )}
                    </td>

                    <td class="text-end fw-semibold">
                        ${formatoNumero.format(
                                        registro.horas || 0
                                    )}
                    </td>

                    <td class="text-end">
                        ${cumplimiento}
                    </td>

                    <td>
                        <span class="time-tendencia-badge"
                              style="
                                  background:${registro.colorHoras};
                                  color:#ffffff;
                              ">
                            ${obtenerEstadoHoras(
                                        registro.horas || 0
                                    )}
                        </span>
                    </td>
                    `;

                fragmento.appendChild(
                    fila
                );
            }
        );

        tablaUsuarios.appendChild(
            fragmento
        );
    }

    function construirTendencia(
        cumplimiento
    ) {

        if (cumplimiento >= 90) {
            return (
                '<span class="badge bg-success">' +
                'Óptimo' +
                '</span>'
            );
        }

        if (cumplimiento >= 70) {
            return (
                '<span class="badge bg-warning text-dark">' +
                'En seguimiento' +
                '</span>'
            );
        }

        return (
            '<span class="badge bg-danger">' +
            'Bajo' +
            '</span>'
        );
    }

    function obtenerEstadoHoras(horas) {

        if (horas > 176) {
            return "Excedidas";
        }

        if (Math.abs(
            horas - 176
        ) < 0.0001) {

            return "Cumplimiento";
        }

        return "Por completar";
    }

    // ============================================================
    // ESTADO
    // ============================================================

    function actualizarEstado(
        texto,
        tipo
    ) {

        if (estadoTexto) {
            estadoTexto.textContent =
                texto;
        }

        if (!estadoPunto) {
            return;
        }

        switch (tipo) {

            case "success":
                estadoPunto.style.background =
                    "#22c55e";
                break;

            case "error":
                estadoPunto.style.background =
                    "#dc3545";
                break;

            case "warning":
                estadoPunto.style.background =
                    "#d4ad45";
                break;

            default:
                estadoPunto.style.background =
                    "#c7c3d7";
                break;
        }
    }

    function crearTextoActualizacion(
        fecha
    ) {

        if (!fecha) {
            return "Información actualizada";
        }

        const valor =
            new Date(fecha);

        if (Number.isNaN(
            valor.getTime()
        )) {

            return "Información actualizada";
        }

        return (
            "Actualizado " +
            valor.toLocaleTimeString(
                "es-MX",
                {
                    hour:
                        "2-digit",

                    minute:
                        "2-digit"
                }
            )
        );
    }

    // ============================================================
    // LIMPIAR
    // ============================================================

    function limpiarDashboard() {

        limpiarKpis();

        tablaUsuarios.innerHTML =
            `
            <tr>
                <td colspan="5"
                    class="text-center text-muted py-4">
                    No fue posible cargar
                    la información.
                </td>
            </tr>
            `;

        if (chartDistribucionTipo) {

            chartDistribucionTipo.destroy();

            chartDistribucionTipo =
                null;
        }

        if (chartTendencia) {

            chartTendencia.destroy();

            chartTendencia =
                null;
        }

        if (chartCumplimientoArea) {

            chartCumplimientoArea.destroy();

            chartCumplimientoArea =
                null;
        }
    }

    // ============================================================
    // UTILIDADES
    // ============================================================

    function convertirFechaJson(
        valor
    ) {

        if (!valor) {
            return new Date();
        }

        const partes =
            valor
                .substring(0, 10)
                .split("-");

        if (partes.length !== 3) {
            return new Date(valor);
        }

        return new Date(
            Number(partes[0]),
            Number(partes[1]) - 1,
            Number(partes[2])
        );
    }

    function escaparHtml(
        texto
    ) {

        const elemento =
            document.createElement(
                "div"
            );

        elemento.textContent =
            texto ?? "";

        return elemento.innerHTML;
    }

    // ============================================================
    // EJECUCIÓN
    // ============================================================

    inicializar();

})();