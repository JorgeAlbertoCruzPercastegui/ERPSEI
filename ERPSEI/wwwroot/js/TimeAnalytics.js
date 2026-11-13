(() => {
    "use strict";

    // ============================================================
    // ELEMENTOS DEL DOM
    // ============================================================


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

    function obtenerFechaInicio() {

        return document.getElementById(
            "analyticsFechaInicio"
        );
    }

    function obtenerFechaFin() {

        return document.getElementById(
            "analyticsFechaFin"
        );
    }

    function obtenerArea() {

        return document.getElementById(
            "analyticsArea"
        );
    }

    function obtenerUsuario() {

        return document.getElementById(
            "analyticsUsuario"
        );
    }

    // ============================================================
    // GRÁFICAS
    // ============================================================

    let chartDistribucionTipo = null;
    let chartTendencia = null;

    // Cumplimiento por área se implementará cuando traslademos
    // la fórmula exacta del PBIX.
    let chartCumplimientoArea = null;

    let chartEvaluacionClaves = null;
    let chartEvaluacionPuestos = null;

    let chartPersonalClaves = null;
    let chartPersonalPuestos = null;

    let chartBackOfficeAreas = null;
    let chartBackOfficeDistribucion = null;
    let chartBackOfficeColaboradores = null;

    let chartOperativasAreas = null;
    let chartOperativasDistribucion = null;
    let chartOperativasColaboradores = null;

    let chartExtrasColaboradores = null;
    let chartExtrasClasificacion = null;
    let chartExtrasAreas = null;

    let catalogosEvaluacionCargados = false;
    let catalogosPersonalCargados = false;

    const coloresTime = {
        clientes: "#5B8FF9",
        backOffice: "#61DDAA",
        operativa: "#65789B",
        administrativa: "#F6BD16",
        sinClasificacion: "#B8C1CC"
    };

    const paletaTime = [
        "#5B8FF9",
        "#61DDAA",
        "#65789B",
        "#F6BD16",
        "#7262FD",
        "#78D3F8",
        "#9661BC",
        "#F6903D",
        "#008685",
        "#F08BB4",
        "#91CB74",
        "#D3ADF7"
    ];

    const tamanosPaginaTime = [
        10,
        15,
        20,
        25,
        50,
        100,
        200
    ];

    const paginadoresTime = {};

    let vistaActivaTime =
        "dashboard";

    let temporizadorFiltrosTime =
        null;

    let controladorConsultaTime =
        null;

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

    function crearPaginadorTime({
        id,
        registros,
        renderizar,
        tamanoInicial = 10
    }) {

        if (!Array.isArray(registros)) {
            registros = [];
        }

        let estado =
            paginadoresTime[id];

        if (!estado) {

            estado = {
                pagina: 1,
                tamano: tamanoInicial,
                registros: []
            };

            paginadoresTime[id] =
                estado;
        }

        estado.registros =
            registros;

        const totalPaginas =
            Math.max(
                1,
                Math.ceil(
                    registros.length /
                    estado.tamano
                )
            );

        if (estado.pagina >
            totalPaginas) {

            estado.pagina =
                totalPaginas;
        }

        const inicio =
            (estado.pagina - 1) *
            estado.tamano;

        const fin =
            Math.min(
                inicio +
                estado.tamano,
                registros.length
            );

        const paginaRegistros =
            registros.slice(
                inicio,
                fin
            );

        renderizar(
            paginaRegistros
        );

        pintarControlesPaginacionTime(
            id,
            registros.length,
            inicio,
            fin,
            totalPaginas,
            renderizar
        );
    }

    function pintarControlesPaginacionTime(
        id,
        total,
        inicio,
        fin,
        totalPaginas,
        renderizar
    ) {

        const estado =
            paginadoresTime[id];

        const contenedor =
            document.getElementById(
                `${id}Paginacion`
            );

        if (!contenedor) {
            return;
        }

        const opciones =
            tamanosPaginaTime
                .map(x =>
                    `
                <option value="${x}"
                    ${estado.tamano === x
                        ? "selected"
                        : ""}>
                    ${x}
                </option>
                `
                )
                .join("");

        contenedor.innerHTML =
            `
        <div class="time-table-pagination">

            <div class="time-table-pagination-left">

                <span>
                    Mostrar
                </span>

                <select class="form-select form-select-sm"
                        id="${id}Tamano">

                    ${opciones}

                </select>

                <span>
                    registros
                </span>

            </div>


            <div class="time-table-pagination-info">

                ${total === 0
                ? "0 registros"
                : `Mostrando ${inicio + 1}–${fin} de ${total}`
            }

            </div>


            <div class="time-table-pagination-actions">

                <button type="button"
                        class="btn btn-sm btn-outline-secondary"
                        id="${id}Anterior"
                        ${estado.pagina <= 1
                ? "disabled"
                : ""}>

                    <i class="bi bi-chevron-left"></i>

                </button>

                <span class="time-table-page">
                    ${estado.pagina} / ${totalPaginas}
                </span>

                <button type="button"
                        class="btn btn-sm btn-outline-secondary"
                        id="${id}Siguiente"
                        ${estado.pagina >= totalPaginas
                ? "disabled"
                : ""}>

                    <i class="bi bi-chevron-right"></i>

                </button>

            </div>

        </div>
        `;

        document
            .getElementById(
                `${id}Tamano`
            )
            .addEventListener(
                "change",
                event => {

                    estado.tamano =
                        Number(
                            event.target.value
                        );

                    estado.pagina =
                        1;

                    crearPaginadorTime({
                        id,
                        registros:
                            estado.registros,
                        renderizar,
                        tamanoInicial:
                            estado.tamano
                    });
                }
            );


        document
            .getElementById(
                `${id}Anterior`
            )
            ?.addEventListener(
                "click",
                () => {

                    if (estado.pagina <= 1) {
                        return;
                    }

                    estado.pagina--;

                    crearPaginadorTime({
                        id,
                        registros:
                            estado.registros,
                        renderizar,
                        tamanoInicial:
                            estado.tamano
                    });
                }
            );


        document
            .getElementById(
                `${id}Siguiente`
            )
            ?.addEventListener(
                "click",
                () => {

                    if (estado.pagina >=
                        totalPaginas) {
                        return;
                    }

                    estado.pagina++;

                    crearPaginadorTime({
                        id,
                        registros:
                            estado.registros,
                        renderizar,
                        tamanoInicial:
                            estado.tamano
                    });
                }
            );
    }

    function obtenerColorTipo(tipo) {

        const valor =
            (tipo || "")
                .trim()
                .toLowerCase();

        switch (valor) {

            case "clientes":
                return coloresTime.clientes;

            case "back office":
                return coloresTime.backOffice;

            case "operativa":
                return coloresTime.operativa;

            case "administrativa":
                return coloresTime.administrativa;

            default:
                return coloresTime.sinClasificacion;
        }
    }

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

        const inputInicio =
            document.getElementById(
                "analyticsFechaInicio"
            );

        const inputFin =
            document.getElementById(
                "analyticsFechaFin"
            );

        inputInicio.value =
            convertirFechaInput(
                inicio
            );

        inputFin.value =
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
                obtenerUsuario(),
                datos.usuarios,
                "Todos los colaboradores"
            );

            cargarSelect(
                obtenerArea(),
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

    async function cargarColaboradoresPorArea() {

        const selectArea =
            obtenerArea();

        const selectUsuario =
            obtenerUsuario();

        if (!selectUsuario) {
            return;
        }

        const parametros =
            new URLSearchParams();

        if (selectArea?.value) {

            parametros.set(
                "departamentoId",
                selectArea.value
            );
        }

        const textoAnterior =
            selectUsuario.options[
                selectUsuario.selectedIndex
            ]?.textContent || "";

        const valorAnterior =
            selectUsuario.value;

        try {

            selectUsuario.disabled =
                true;

            cargarSelect(
                selectUsuario,
                [],
                "Cargando colaboradores..."
            );

            const url =
                parametros.toString()
                    ? `?handler=Catalogos&${parametros.toString()}`
                    : "?handler=Catalogos";

            const respuesta =
                await fetch(
                    url,
                    {
                        method:
                            "GET",

                        headers: {
                            "Accept":
                                "application/json"
                        }
                    }
                );

            const datos =
                await respuesta.json();

            if (!respuesta.ok ||
                !datos.success) {

                throw new Error(
                    datos.message ||
                    "No fue posible obtener los colaboradores."
                );
            }

            cargarSelect(
                selectUsuario,
                datos.usuarios || [],
                "Todos los colaboradores"
            );

            // Si el colaborador anterior sigue perteneciendo
            // al área seleccionada, lo conservamos.
            if (valorAnterior) {

                const opcionExistente =
                    Array.from(
                        selectUsuario.options
                    )
                        .find(x =>
                            x.value === valorAnterior
                        );

                if (opcionExistente) {

                    selectUsuario.value =
                        valorAnterior;
                }
            }
        }
        catch (error) {

            console.error(
                "TIME Analytics - colaboradores por área:",
                error
            );

            cargarSelect(
                selectUsuario,
                [],
                "Todos los colaboradores"
            );
        }
        finally {

            selectUsuario.disabled =
                false;
        }
    }

    function cargarSelect(
        select,
        opciones,
        textoTodos
    ) {

        if (!select) {
            return;
        }

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

        obtenerFechaInicio()
            ?.addEventListener(
                "change",
                programarActualizacionFiltros
            );

        obtenerFechaFin()
            ?.addEventListener(
                "change",
                programarActualizacionFiltros
            );

        obtenerArea()
            ?.addEventListener(
                "change",
                async () => {

                    await cargarColaboradoresPorArea();

                    programarActualizacionFiltros();
                }
            );

        obtenerUsuario()
            ?.addEventListener(
                "change",
                programarActualizacionFiltros
            );

        document
            .getElementById(
                "tabDashboardCumplimiento"
            )
            ?.addEventListener(
                "click",
                mostrarDashboardCumplimiento
            );

        document
            .getElementById(
                "tabEvaluacionAreas"
            )
            ?.addEventListener(
                "click",
                mostrarEvaluacionAreas
            );

        document
            .getElementById(
                "tabAnalisisPersonal"
            )
            ?.addEventListener(
                "click",
                mostrarAnalisisPersonal
            );

        document
            .getElementById(
                "tabAlineacionBackOffice"
            )
            ?.addEventListener(
                "click",
                mostrarAlineacionBackOffice
            );

        document
            .getElementById(
                "tabAlineacionOperativas"
            )
            ?.addEventListener(
                "click",
                mostrarAlineacionOperativas
            );

        document
            .getElementById(
                "tabHorasExtras"
            )
            ?.addEventListener(
                "click",
                mostrarHorasExtras
            );

        document
            .getElementById(
                "evaluacionTipo"
            )
            ?.addEventListener(
                "change",
                programarActualizacionFiltros
            );

        document
            .getElementById(
                "evaluacionPuesto"
            )
            ?.addEventListener(
                "change",
                programarActualizacionFiltros
            );

        document
            .getElementById(
                "personalTipo"
            )
            ?.addEventListener(
                "change",
                programarActualizacionFiltros
            );

        document
            .getElementById(
                "personalPuesto"
            )
            ?.addEventListener(
                "change",
                programarActualizacionFiltros
            );
    }


    function programarActualizacionFiltros() {

        if (temporizadorFiltrosTime) {

            clearTimeout(
                temporizadorFiltrosTime
            );
        }

        temporizadorFiltrosTime =
            setTimeout(
                () => {

                    actualizarVistaActiva();

                },
                250
            );
    }


    async function actualizarVistaActiva() {

        if (!validarFechas()) {
            return;
        }

        reiniciarPaginadoresVistaActiva();

        if (vistaActivaTime ===
            "evaluacion") {

            await cargarEvaluacionAreas();

            return;
        }

        if (vistaActivaTime ===
            "personal") {

            await cargarAnalisisPersonal();

            return;
        }

        if (vistaActivaTime ===
            "backoffice") {

            await cargarAlineacionBackOffice();

            return;
        }

        if (vistaActivaTime ===
            "operativas") {

            await cargarAlineacionOperativas();

            return;
        }

        if (vistaActivaTime ===
            "extras") {

            await cargarHorasExtras();

            return;
        }

        await cargarDashboard();
    }


    function reiniciarPaginadoresVistaActiva() {

        if (vistaActivaTime ===
            "dashboard") {

            if (paginadoresTime.colaboradores) {

                paginadoresTime
                    .colaboradores
                    .pagina = 1;
            }

            return;
        }

        if (vistaActivaTime ===
            "evaluacion") {

            if (paginadoresTime
                .empresaColaborador) {

                paginadoresTime
                    .empresaColaborador
                    .pagina = 1;
            }

            return;
        }

        if (vistaActivaTime ===
            "personal") {

            if (paginadoresTime
                .personalColaboradores) {

                paginadoresTime
                    .personalColaboradores
                    .pagina = 1;
            }

            return;
        }

        if (vistaActivaTime ===
            "backoffice") {

            if (paginadoresTime
                .backOfficeHallazgos) {

                paginadoresTime
                    .backOfficeHallazgos
                    .pagina = 1;
            }

            return;
        }

        if (vistaActivaTime ===
            "operativas") {

            if (paginadoresTime
                .operativasResumen) {

                paginadoresTime
                    .operativasResumen
                    .pagina = 1;
            }

            return;
        }

        if (paginadoresTime
            .extrasDetalle) {

            paginadoresTime
                .extrasDetalle
                .pagina = 1;
        }
    }

    async function mostrarDashboardCumplimiento() {

        vistaActivaTime =
            "dashboard";

        cambiarVistaAnalytics(
            "vistaDashboardCumplimiento",
            "tabDashboardCumplimiento"
        );

        await cargarDashboard();
    }


    async function mostrarEvaluacionAreas() {

        vistaActivaTime =
            "evaluacion";

        cambiarVistaAnalytics(
            "vistaEvaluacionAreas",
            "tabEvaluacionAreas"
        );

        await cargarEvaluacionAreas();
    }


    async function mostrarAnalisisPersonal() {

        vistaActivaTime =
            "personal";

        cambiarVistaAnalytics(
            "vistaAnalisisPersonal",
            "tabAnalisisPersonal"
        );

        await cargarAnalisisPersonal();
    }


    async function cargarAnalisisPersonal() {

        if (!validarFechas()) {
            return;
        }

        actualizarEstado(
            "Actualizando Análisis de Personal...",
            "loading"
        );

        activarCargaVista(
            "personal",
            true
        );

        if (controladorConsultaTime) {

            controladorConsultaTime
                .abort();
        }

        controladorConsultaTime =
            new AbortController();

        try {

            const parametros =
                new URLSearchParams();

            const inputFechaInicio =
                obtenerFechaInicio();

            const inputFechaFin =
                obtenerFechaFin();

            const selectArea =
                obtenerArea();

            const selectUsuario =
                obtenerUsuario();

            const tipo =
                document.getElementById(
                    "personalTipo"
                );

            const puesto =
                document.getElementById(
                    "personalPuesto"
                );

            if (inputFechaInicio?.value) {

                parametros.set(
                    "fechaInicio",
                    inputFechaInicio.value
                );
            }

            if (inputFechaFin?.value) {

                parametros.set(
                    "fechaFin",
                    inputFechaFin.value
                );
            }

            if (selectUsuario?.value) {

                parametros.set(
                    "usuarioId",
                    selectUsuario.value
                );
            }

            if (selectArea?.value) {

                parametros.set(
                    "departamentoId",
                    selectArea.value
                );
            }

            if (tipo?.value) {

                parametros.set(
                    "tipo",
                    tipo.value
                );
            }

            if (puesto?.value) {

                parametros.set(
                    "puesto",
                    puesto.value
                );
            }

            const respuesta =
                await fetch(
                    `?handler=AnalisisPersonal&${parametros.toString()}`,
                    {
                        method:
                            "GET",

                        headers: {
                            "Accept":
                                "application/json"
                        },

                        signal:
                            controladorConsultaTime
                                .signal
                    }
                );

            const datos =
                await respuesta.json();

            if (!respuesta.ok ||
                !datos.success) {

                throw new Error(
                    datos.message ||
                    "No fue posible obtener el Análisis de Personal."
                );
            }

            pintarAnalisisPersonal(
                datos.personal
            );

            actualizarEstado(
                crearTextoActualizacion(
                    datos.personal
                        .fechaActualizacion
                ),
                "success"
            );
        }
        catch (error) {

            if (error.name ===
                "AbortError") {

                return;
            }

            console.error(
                "TIME Analytics - Análisis de Personal:",
                error
            );

            actualizarEstado(
                "Error al consultar Análisis de Personal",
                "error"
            );
        }
        finally {

            activarCargaVista(
                "personal",
                false
            );
        }
    }


    function pintarAnalisisPersonal(datos) {

        if (!datos) {
            return;
        }

        document
            .getElementById(
                "personalHoras"
            )
            .textContent =
            `${formatoNumero.format(
                datos.horas || 0
            )} h`;

        document
            .getElementById(
                "personalPersonas"
            )
            .textContent =
            formatoNumero.format(
                datos.personas || 0
            );

        cargarCatalogosPersonal(
            datos
        );

        pintarPersonalClaves(
            datos.claves || []
        );

        pintarPersonalPuestos(
            datos.distribucionPuestos || []
        );

        pintarPersonalColaboradores(
            datos.colaboradoresEmpresa || []
        );
    }


    function cargarCatalogosPersonal(datos) {

        if (catalogosPersonalCargados) {
            return;
        }

        const selectTipo =
            document.getElementById(
                "personalTipo"
            );

        const selectPuesto =
            document.getElementById(
                "personalPuesto"
            );

        (datos.tipos || []).forEach(tipo => {

            const opcion =
                document.createElement(
                    "option"
                );

            opcion.value =
                tipo;

            opcion.textContent =
                tipo;

            selectTipo.appendChild(
                opcion
            );
        });

        (datos.puestos || []).forEach(puesto => {

            const opcion =
                document.createElement(
                    "option"
                );

            opcion.value =
                puesto;

            opcion.textContent =
                puesto;

            selectPuesto.appendChild(
                opcion
            );
        });

        catalogosPersonalCargados =
            true;
    }


    function pintarPersonalClaves(datos) {

        if (chartPersonalClaves) {
            chartPersonalClaves.destroy();
            chartPersonalClaves = null;
        }

        const canvas =
            document.getElementById(
                "chartPersonalClaves"
            );

        if (!canvas ||
            typeof Chart === "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        const alturaMinima =
            Math.max(
                360,
                datos.length * 46
            );

        canvas.style.height =
            `${alturaMinima}px`;

        chartPersonalClaves =
            new Chart(
                canvas,
                {
                    type:
                        "bar",

                    data: {

                        labels:
                            datos.map(x =>
                                x.descripcion
                            ),

                        datasets: [
                            {
                                label:
                                    "Horas",

                                data:
                                    datos.map(x =>
                                        x.horas
                                    ),

                                backgroundColor:
                                    datos.map(x =>
                                        obtenerColorTipo(
                                            x.tipo
                                        )
                                    ),

                                borderRadius:
                                    5,

                                borderSkipped:
                                    false
                            }
                        ]
                    },

                    options: {

                        indexAxis:
                            "y",

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        layout: {

                            padding: {
                                left:
                                    12,

                                right:
                                    22,

                                top:
                                    10,

                                bottom:
                                    10
                            }
                        },

                        plugins: {

                            legend: {
                                display:
                                    false
                            },

                            tooltip: {

                                callbacks: {

                                    title:
                                        context =>
                                            datos[
                                                context[0]
                                                    .dataIndex
                                            ]
                                                .descripcion,

                                    label:
                                        context =>
                                            `Horas: ${formatoNumero.format(
                                                context.parsed.x || 0
                                            )} h`,

                                    afterLabel:
                                        context =>
                                            `Tipo: ${datos[
                                                context.dataIndex
                                            ].tipo}`
                                }
                            }
                        },

                        scales: {

                            x: {

                                beginAtZero:
                                    true,

                                grid: {
                                    color:
                                        "rgba(33, 22, 107, .07)"
                                }
                            },

                            y: {

                                afterFit:
                                    scale => {

                                        scale.width =
                                            Math.min(
                                                Math.max(
                                                    scale.width,
                                                    260
                                                ),
                                                420
                                            );
                                    },

                                grid: {
                                    display:
                                        false
                                },

                                ticks: {

                                    autoSkip:
                                        false,

                                    font: {
                                        size:
                                            11
                                    },

                                    padding:
                                        10,

                                    callback:
                                        function (value) {

                                            return this
                                                .getLabelForValue(
                                                    value
                                                );
                                        }
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarPersonalPuestos(datos) {

        if (chartPersonalPuestos) {
            chartPersonalPuestos.destroy();
            chartPersonalPuestos = null;
        }

        const canvas =
            document.getElementById(
                "chartPersonalPuestos"
            );

        if (!canvas ||
            typeof Chart === "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        canvas.style.height =
            "360px";

        chartPersonalPuestos =
            new Chart(
                canvas,
                {
                    type:
                        "doughnut",

                    data: {

                        labels:
                            datos.map(x =>
                                x.puesto
                            ),

                        datasets: [
                            {
                                data:
                                    datos.map(x =>
                                        x.horas
                                    ),

                                backgroundColor:
                                    datos.map(
                                        (_, index) =>
                                            paletaTime[
                                            index %
                                            paletaTime.length
                                            ]
                                    ),

                                borderWidth:
                                    2,

                                borderColor:
                                    "#ffffff",

                                hoverOffset:
                                    8
                            }
                        ]
                    },

                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        cutout:
                            "65%",

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true,

                                    padding:
                                        14,

                                    boxWidth:
                                        10,

                                    font: {
                                        size:
                                            11
                                    }
                                }
                            },

                            tooltip: {

                                callbacks: {

                                    title:
                                        context =>
                                            context[0]
                                                .label,

                                    label:
                                        context =>
                                            `${context.label}: ${formatoNumero.format(
                                                context.parsed || 0
                                            )} h`
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarPersonalColaboradores(
        datos
    ) {

        if (paginadoresTime
            .personalColaboradores) {

            paginadoresTime
                .personalColaboradores
                .pagina = 1;
        }

        crearPaginadorTime({
            id:
                "personalColaboradores",

            registros:
                datos || [],

            renderizar:
                renderizarPersonalColaboradores,

            tamanoInicial:
                10
        });
    }


    function renderizarPersonalColaboradores(
        datos
    ) {

        const tbody =
            document.getElementById(
                "tablaPersonalColaboradores"
            );

        tbody.innerHTML =
            "";

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            tbody.innerHTML =
                `
                <tr>
                    <td colspan="3"
                        class="text-center text-muted py-4">

                        No hay información para los filtros seleccionados.

                    </td>
                </tr>
                `;

            return;
        }

        const fragmento =
            document.createDocumentFragment();

        datos.forEach(
            registro => {

                const fila =
                    document.createElement(
                        "tr"
                    );

                fila.innerHTML =
                    `
                    <td>
                        ${escaparHtml(
                        registro.nombre || ""
                    )}
                    </td>

                    <td>
                        ${escaparHtml(
                        registro.grupoEmpresa ||
                        "Sin Grupo / Empresa"
                    )}
                    </td>

                    <td class="text-end fw-semibold">
                        ${formatoNumero.format(
                        registro.horas || 0
                    )} h
                    </td>
                    `;

                fragmento.appendChild(
                    fila
                );
            }
        );

        tbody.appendChild(
            fragmento
        );
    }


    function cambiarVistaAnalytics(
        idVistaActiva,
        idTabActivo
    ) {

        const vistas = [
            "vistaDashboardCumplimiento",
            "vistaEvaluacionAreas",
            "vistaAnalisisPersonal",
            "vistaAlineacionBackOffice",
            "vistaAlineacionOperativas",
            "vistaHorasExtras"
        ];

        const tabs = [
            "tabDashboardCumplimiento",
            "tabEvaluacionAreas",
            "tabAnalisisPersonal",
            "tabAlineacionBackOffice",
            "tabAlineacionOperativas",
            "tabHorasExtras"
        ];

        vistas.forEach(id => {

            document
                .getElementById(id)
                ?.classList
                .toggle(
                    "d-none",
                    id !== idVistaActiva
                );
        });

        tabs.forEach(id => {

            document
                .getElementById(id)
                ?.classList
                .toggle(
                    "active",
                    id === idTabActivo
                );
        });
    }


    async function mostrarAlineacionBackOffice() {

        vistaActivaTime =
            "backoffice";

        cambiarVistaAnalytics(
            "vistaAlineacionBackOffice",
            "tabAlineacionBackOffice"
        );

        await cargarAlineacionBackOffice();
    }


    async function cargarAlineacionBackOffice() {

        if (!validarFechas()) {
            return;
        }

        actualizarEstado(
            "Actualizando Alineación Back Office...",
            "loading"
        );

        activarCargaVista(
            "backoffice",
            true
        );

        if (controladorConsultaTime) {

            controladorConsultaTime
                .abort();
        }

        controladorConsultaTime =
            new AbortController();

        try {

            const parametros =
                new URLSearchParams();

            const inputFechaInicio =
                obtenerFechaInicio();

            const inputFechaFin =
                obtenerFechaFin();

            const selectArea =
                obtenerArea();

            const selectUsuario =
                obtenerUsuario();

            if (inputFechaInicio?.value) {

                parametros.set(
                    "fechaInicio",
                    inputFechaInicio.value
                );
            }

            if (inputFechaFin?.value) {

                parametros.set(
                    "fechaFin",
                    inputFechaFin.value
                );
            }

            if (selectUsuario?.value) {

                parametros.set(
                    "usuarioId",
                    selectUsuario.value
                );
            }

            if (selectArea?.value) {

                parametros.set(
                    "departamentoId",
                    selectArea.value
                );
            }

            const respuesta =
                await fetch(
                    `?handler=AlineacionBackOffice&${parametros.toString()}`,
                    {
                        method:
                            "GET",

                        headers: {
                            "Accept":
                                "application/json"
                        },

                        signal:
                            controladorConsultaTime
                                .signal
                    }
                );

            const tipoContenido =
                respuesta.headers
                    .get("content-type") || "";

            if (!tipoContenido
                .includes(
                    "application/json"
                )) {

                throw new Error(
                    "El servidor no devolvió JSON para Alineación Back Office."
                );
            }

            const datos =
                await respuesta.json();

            if (!respuesta.ok ||
                !datos.success) {

                throw new Error(
                    datos.message ||
                    "No fue posible obtener Alineación Back Office."
                );
            }

            pintarAlineacionBackOffice(
                datos.backOffice
            );

            actualizarEstado(
                crearTextoActualizacion(
                    datos.backOffice
                        .fechaActualizacion
                ),
                "success"
            );
        }
        catch (error) {

            if (error.name ===
                "AbortError") {

                return;
            }

            console.error(
                "TIME Analytics - Alineación Back Office:",
                error
            );

            actualizarEstado(
                "Error al consultar Alineación Back Office",
                "error"
            );
        }
        finally {

            activarCargaVista(
                "backoffice",
                false
            );
        }
    }


    function pintarAlineacionBackOffice(
        datos
    ) {

        if (!datos) {
            return;
        }

        document
            .getElementById(
                "backOfficeEvaluacion"
            )
            .textContent =
            `${formatoNumero.format(
                (datos.evaluacionBackOffice || 0) *
                100
            )}%`;

        document
            .getElementById(
                "backOfficeHoras"
            )
            .textContent =
            `${formatoNumero.format(
                datos.horasBackOffice || 0
            )} h`;

        document
            .getElementById(
                "backOfficeHorasTotales"
            )
            .textContent =
            `${formatoNumero.format(
                datos.horasTotales || 0
            )} h`;

        pintarBackOfficeAreas(
            datos.areas || []
        );

        pintarBackOfficeDistribucion(
            datos.distribucion || []
        );

        pintarBackOfficeColaboradores(
            datos.colaboradores || []
        );

        pintarBackOfficeHallazgos(
            datos.hallazgos || []
        );
    }


    function obtenerColorBackOffice(
        clasificacion
    ) {

        const valor =
            (clasificacion || "")
                .trim()
                .toLowerCase();

        if (valor ===
            "horas back office") {

            return "#76F3EF";
        }

        if (valor ===
            "administrativa") {

            return "#FFF689";
        }

        return "#7D8CA3";
    }


    function pintarBackOfficeAreas(
        datos
    ) {

        if (chartBackOfficeAreas) {

            chartBackOfficeAreas
                .destroy();

            chartBackOfficeAreas =
                null;
        }

        const canvas =
            document.getElementById(
                "chartBackOfficeAreas"
            );

        if (!canvas ||
            typeof Chart ===
            "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        const altura =
            Math.max(
                360,
                datos.length * 30
            );

        canvas.style.height =
            `${altura}px`;

        chartBackOfficeAreas =
            new Chart(
                canvas,
                {
                    type:
                        "bar",

                    data: {

                        labels:
                            datos.map(x =>
                                x.etiqueta
                            ),

                        datasets: [
                            {
                                label:
                                    "Horas Back Office",

                                data:
                                    datos.map(x =>
                                        x.horasBackOffice
                                    ),

                                backgroundColor:
                                    "#76F3EF",

                                borderRadius:
                                    4
                            },
                            {
                                label:
                                    "Administrativa",

                                data:
                                    datos.map(x =>
                                        x.horasAdministrativas
                                    ),

                                backgroundColor:
                                    "#FFF689",

                                borderRadius:
                                    4
                            },
                            {
                                label:
                                    "Clientes",

                                data:
                                    datos.map(x =>
                                        x.horasClientes
                                    ),

                                backgroundColor:
                                    "#7D8CA3",

                                borderRadius:
                                    4
                            }
                        ]
                    },

                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        scales: {

                            x: {
                                stacked:
                                    true,

                                ticks: {
                                    autoSkip:
                                        false,

                                    maxRotation:
                                        45,

                                    minRotation:
                                        0
                                }
                            },

                            y: {
                                stacked:
                                    true,

                                beginAtZero:
                                    true
                            }
                        },

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true
                                }
                            },

                            tooltip: {

                                callbacks: {

                                    label:
                                        context =>
                                            `${context.dataset.label}: ${formatoNumero.format(
                                                context.parsed.y || 0
                                            )} h`
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarBackOfficeDistribucion(
        datos
    ) {

        if (chartBackOfficeDistribucion) {

            chartBackOfficeDistribucion
                .destroy();

            chartBackOfficeDistribucion =
                null;
        }

        const canvas =
            document.getElementById(
                "chartBackOfficeDistribucion"
            );

        if (!canvas ||
            typeof Chart ===
            "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        chartBackOfficeDistribucion =
            new Chart(
                canvas,
                {
                    type:
                        "doughnut",

                    data: {

                        labels:
                            datos.map(x =>
                                x.clasificacion
                            ),

                        datasets: [
                            {
                                data:
                                    datos.map(x =>
                                        x.horas
                                    ),

                                backgroundColor:
                                    datos.map(x =>
                                        obtenerColorBackOffice(
                                            x.clasificacion
                                        )
                                    ),

                                borderColor:
                                    "#ffffff",

                                borderWidth:
                                    2,

                                hoverOffset:
                                    8
                            }
                        ]
                    },

                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        cutout:
                            "64%",

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true,

                                    padding:
                                        14
                                }
                            },

                            tooltip: {

                                callbacks: {

                                    label:
                                        context =>
                                            `${context.label}: ${formatoNumero.format(
                                                context.parsed || 0
                                            )} h`
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarBackOfficeColaboradores(
        datos
    ) {

        if (chartBackOfficeColaboradores) {

            chartBackOfficeColaboradores
                .destroy();

            chartBackOfficeColaboradores =
                null;
        }

        const canvas =
            document.getElementById(
                "chartBackOfficeColaboradores"
            );

        if (!canvas ||
            typeof Chart ===
            "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        const altura =
            Math.max(
                360,
                datos.length * 34
            );

        canvas.style.height =
            `${altura}px`;

        chartBackOfficeColaboradores =
            new Chart(
                canvas,
                {
                    type:
                        "bar",

                    data: {

                        labels:
                            datos.map(x =>
                                x.etiqueta
                            ),

                        datasets: [
                            {
                                label:
                                    "Horas Back Office",

                                data:
                                    datos.map(x =>
                                        x.horasBackOffice
                                    ),

                                backgroundColor:
                                    "#76F3EF"
                            },
                            {
                                label:
                                    "Administrativa",

                                data:
                                    datos.map(x =>
                                        x.horasAdministrativas
                                    ),

                                backgroundColor:
                                    "#FFF689"
                            },
                            {
                                label:
                                    "Clientes",

                                data:
                                    datos.map(x =>
                                        x.horasClientes
                                    ),

                                backgroundColor:
                                    "#7D8CA3"
                            }
                        ]
                    },

                    options: {

                        indexAxis:
                            "y",

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        scales: {

                            x: {
                                stacked:
                                    true,

                                beginAtZero:
                                    true
                            },

                            y: {
                                stacked:
                                    true,

                                afterFit:
                                    scale => {

                                        scale.width =
                                            Math.min(
                                                Math.max(
                                                    scale.width,
                                                    220
                                                ),
                                                420
                                            );
                                    },

                                ticks: {
                                    autoSkip:
                                        false,

                                    font: {
                                        size:
                                            11
                                    }
                                }
                            }
                        },

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true
                                }
                            },

                            tooltip: {

                                callbacks: {

                                    label:
                                        context =>
                                            `${context.dataset.label}: ${formatoNumero.format(
                                                context.parsed.x || 0
                                            )} h`
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarBackOfficeHallazgos(
        datos
    ) {

        if (paginadoresTime
            .backOfficeHallazgos) {

            paginadoresTime
                .backOfficeHallazgos
                .pagina = 1;
        }

        crearPaginadorTime({
            id:
                "backOfficeHallazgos",

            registros:
                datos || [],

            renderizar:
                renderizarBackOfficeHallazgos,

            tamanoInicial:
                10
        });
    }


    function renderizarBackOfficeHallazgos(
        datos
    ) {

        const tbody =
            document.getElementById(
                "tablaBackOfficeHallazgos"
            );

        if (!tbody) {
            return;
        }

        tbody.innerHTML =
            "";

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            tbody.innerHTML =
                `
                <tr>
                    <td colspan="5"
                        class="text-center text-muted py-4">

                        No hay información para los filtros seleccionados.

                    </td>
                </tr>
                `;

            return;
        }

        const fragmento =
            document.createDocumentFragment();

        datos.forEach(
            registro => {

                const fila =
                    document.createElement(
                        "tr"
                    );

                fila.innerHTML =
                    `
                    <td>
                        ${escaparHtml(
                        registro.area || ""
                    )}
                    </td>

                    <td class="text-end">
                        ${formatoNumero.format(
                        registro.horasTotales || 0
                    )} h
                    </td>

                    <td class="text-end fw-semibold">
                        ${formatoNumero.format(
                        registro.horasBackOffice || 0
                    )} h
                    </td>

                    <td class="text-end fw-semibold">
                        ${formatoNumero.format(
                        (registro.evaluacion || 0) *
                        100
                    )}%
                    </td>

                    <td>
                        <span class="time-backoffice-level ${escaparHtml(
                        registro.claseCss || ""
                    )}">
                            ${escaparHtml(
                        registro.nivel || ""
                    )}
                        </span>
                    </td>
                    `;

                fragmento.appendChild(
                    fila
                );
            }
        );

        tbody.appendChild(
            fragmento
        );
    }


    async function mostrarAlineacionOperativas() {

        vistaActivaTime =
            "operativas";

        cambiarVistaAnalytics(
            "vistaAlineacionOperativas",
            "tabAlineacionOperativas"
        );

        await cargarAlineacionOperativas();
    }


    async function cargarAlineacionOperativas() {

        if (!validarFechas()) {
            return;
        }

        actualizarEstado(
            "Actualizando Alineación Operativas...",
            "loading"
        );

        activarCargaVista(
            "operativas",
            true
        );

        if (controladorConsultaTime) {
            controladorConsultaTime.abort();
        }

        controladorConsultaTime =
            new AbortController();

        try {

            const parametros =
                new URLSearchParams();

            const inputFechaInicio =
                obtenerFechaInicio();

            const inputFechaFin =
                obtenerFechaFin();

            const selectArea =
                obtenerArea();

            const selectUsuario =
                obtenerUsuario();

            if (inputFechaInicio?.value) {
                parametros.set(
                    "fechaInicio",
                    inputFechaInicio.value
                );
            }

            if (inputFechaFin?.value) {
                parametros.set(
                    "fechaFin",
                    inputFechaFin.value
                );
            }

            if (selectUsuario?.value) {
                parametros.set(
                    "usuarioId",
                    selectUsuario.value
                );
            }

            if (selectArea?.value) {
                parametros.set(
                    "departamentoId",
                    selectArea.value
                );
            }

            const respuesta =
                await fetch(
                    `?handler=AlineacionOperativas&${parametros.toString()}`,
                    {
                        method:
                            "GET",

                        headers: {
                            "Accept":
                                "application/json"
                        },

                        signal:
                            controladorConsultaTime.signal
                    }
                );

            const datos =
                await respuesta.json();

            if (!respuesta.ok ||
                !datos.success) {

                throw new Error(
                    datos.message ||
                    "No fue posible obtener Alineación Operativas."
                );
            }

            pintarAlineacionOperativas(
                datos.operativas
            );

            actualizarEstado(
                crearTextoActualizacion(
                    datos.operativas
                        .fechaActualizacion
                ),
                "success"
            );
        }
        catch (error) {

            if (error.name ===
                "AbortError") {

                return;
            }

            console.error(
                "TIME Analytics - Alineación Operativas:",
                error
            );

            actualizarEstado(
                "Error al consultar Alineación Operativas",
                "error"
            );
        }
        finally {

            activarCargaVista(
                "operativas",
                false
            );
        }
    }


    function pintarAlineacionOperativas(
        datos
    ) {

        if (!datos) {
            return;
        }

        document
            .getElementById(
                "operativasEvaluacion"
            )
            .textContent =
            `${formatoNumero.format(
                (datos.evaluacionOperativasClientes || 0) *
                100
            )}%`;

        document
            .getElementById(
                "operativasHoras"
            )
            .textContent =
            `${formatoNumero.format(
                datos.horasOperativasClientes || 0
            )} h`;

        document
            .getElementById(
                "operativasHorasTotales"
            )
            .textContent =
            `${formatoNumero.format(
                datos.horasTotales || 0
            )} h`;

        pintarOperativasAreas(
            datos.areas || []
        );

        pintarOperativasDistribucion(
            datos.distribucion || []
        );

        pintarOperativasColaboradores(
            datos.colaboradores || []
        );

        pintarOperativasResumen(
            datos.resumen || []
        );
    }


    function obtenerColorOperativas(
        clasificacion
    ) {

        const valor =
            (clasificacion || "")
                .trim()
                .toLowerCase();

        if (valor === "operativa") {
            return "#DB5375";
        }

        if (valor === "clientes") {
            return "#7D8CA3";
        }

        if (valor === "administrativa") {
            return "#FFF689";
        }

        if (valor === "back office") {
            return "#76F3EF";
        }

        return "#B8C1CC";
    }


    function pintarOperativasAreas(
        datos
    ) {

        if (chartOperativasAreas) {
            chartOperativasAreas.destroy();
            chartOperativasAreas = null;
        }

        const canvas =
            document.getElementById(
                "chartOperativasAreas"
            );

        if (!canvas ||
            typeof Chart ===
            "undefined" ||
            !Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        canvas.style.height =
            `${Math.max(
                360,
                datos.length * 30
            )}px`;

        chartOperativasAreas =
            new Chart(
                canvas,
                {
                    type:
                        "bar",

                    data: {

                        labels:
                            datos.map(x =>
                                x.etiqueta
                            ),

                        datasets: [
                            {
                                label:
                                    "Operativas + Clientes",

                                data:
                                    datos.map(x =>
                                        x.horasClientes
                                    ),

                                backgroundColor:
                                    "#DB5375"
                            },
                            {
                                label:
                                    "Administrativa",

                                data:
                                    datos.map(x =>
                                        x.horasAdministrativas
                                    ),

                                backgroundColor:
                                    "#FFF689"
                            }
                        ]
                    },

                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        scales: {

                            x: {
                                stacked:
                                    true,

                                ticks: {
                                    autoSkip:
                                        false,

                                    maxRotation:
                                        45
                                }
                            },

                            y: {
                                stacked:
                                    true,

                                beginAtZero:
                                    true
                            }
                        },

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarOperativasDistribucion(
        datos
    ) {

        if (chartOperativasDistribucion) {
            chartOperativasDistribucion.destroy();
            chartOperativasDistribucion = null;
        }

        const canvas =
            document.getElementById(
                "chartOperativasDistribucion"
            );

        if (!canvas ||
            typeof Chart ===
            "undefined" ||
            !Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        chartOperativasDistribucion =
            new Chart(
                canvas,
                {
                    type:
                        "doughnut",

                    data: {

                        labels:
                            datos.map(x =>
                                x.clasificacion
                            ),

                        datasets: [
                            {
                                data:
                                    datos.map(x =>
                                        x.horas
                                    ),

                                backgroundColor:
                                    datos.map(x =>
                                        obtenerColorOperativas(
                                            x.clasificacion
                                        )
                                    ),

                                borderColor:
                                    "#ffffff",

                                borderWidth:
                                    2
                            }
                        ]
                    },

                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        cutout:
                            "64%",

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarOperativasColaboradores(
        datos
    ) {

        if (chartOperativasColaboradores) {
            chartOperativasColaboradores.destroy();
            chartOperativasColaboradores = null;
        }

        const canvas =
            document.getElementById(
                "chartOperativasColaboradores"
            );

        if (!canvas ||
            typeof Chart ===
            "undefined" ||
            !Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        canvas.style.height =
            `${Math.max(
                360,
                datos.length * 34
            )}px`;

        chartOperativasColaboradores =
            new Chart(
                canvas,
                {
                    type:
                        "bar",

                    data: {

                        labels:
                            datos.map(x =>
                                x.etiqueta
                            ),

                        datasets: [
                            {
                                label:
                                    "Operativas + Clientes",

                                data:
                                    datos.map(x =>
                                        x.horasClientes
                                    ),

                                backgroundColor:
                                    "#DB5375"
                            },
                            {
                                label:
                                    "Administrativa",

                                data:
                                    datos.map(x =>
                                        x.horasAdministrativas
                                    ),

                                backgroundColor:
                                    "#FFF689"
                            }
                        ]
                    },

                    options: {

                        indexAxis:
                            "y",

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        scales: {

                            x: {
                                stacked:
                                    true,

                                beginAtZero:
                                    true
                            },

                            y: {
                                stacked:
                                    true,

                                afterFit:
                                    scale => {

                                        scale.width =
                                            Math.min(
                                                Math.max(
                                                    scale.width,
                                                    220
                                                ),
                                                420
                                            );
                                    },

                                ticks: {
                                    autoSkip:
                                        false
                                }
                            }
                        },

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarOperativasResumen(
        datos
    ) {

        if (paginadoresTime
            .operativasResumen) {

            paginadoresTime
                .operativasResumen
                .pagina = 1;
        }

        crearPaginadorTime({
            id:
                "operativasResumen",

            registros:
                datos || [],

            renderizar:
                renderizarOperativasResumen,

            tamanoInicial:
                10
        });
    }


    function renderizarOperativasResumen(
        datos
    ) {

        const tbody =
            document.getElementById(
                "tablaOperativasResumen"
            );

        if (!tbody) {
            return;
        }

        tbody.innerHTML = "";

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            tbody.innerHTML =
                `
                <tr>
                    <td colspan="4"
                        class="text-center text-muted py-4">
                        No hay información para los filtros seleccionados.
                    </td>
                </tr>
                `;

            return;
        }

        const fragmento =
            document.createDocumentFragment();

        datos.forEach(
            registro => {

                const fila =
                    document.createElement(
                        "tr"
                    );

                fila.innerHTML =
                    `
                    <td>${escaparHtml(
                        registro.area || ""
                    )}</td>

                    <td>${escaparHtml(
                        registro.tipo || ""
                    )}</td>

                    <td class="text-end fw-semibold">
                        ${formatoNumero.format(
                        registro.horas || 0
                    )} h
                    </td>

                    <td class="text-end">
                        ${formatoNumero.format(
                        (registro.participacion || 0) *
                        100
                    )}%
                    </td>
                    `;

                fragmento.appendChild(
                    fila
                );
            }
        );

        tbody.appendChild(
            fragmento
        );
    }


    async function mostrarHorasExtras() {

        vistaActivaTime =
            "extras";

        cambiarVistaAnalytics(
            "vistaHorasExtras",
            "tabHorasExtras"
        );

        await cargarHorasExtras();
    }


    async function cargarHorasExtras() {

        if (!validarFechas()) {
            return;
        }

        actualizarEstado(
            "Actualizando Horas Extras...",
            "loading"
        );

        activarCargaVista(
            "extras",
            true
        );

        if (controladorConsultaTime) {
            controladorConsultaTime.abort();
        }

        controladorConsultaTime =
            new AbortController();

        try {

            const parametros =
                new URLSearchParams();

            const inputFechaInicio =
                obtenerFechaInicio();

            const inputFechaFin =
                obtenerFechaFin();

            const selectArea =
                obtenerArea();

            const selectUsuario =
                obtenerUsuario();

            if (inputFechaInicio?.value) {
                parametros.set(
                    "fechaInicio",
                    inputFechaInicio.value
                );
            }

            if (inputFechaFin?.value) {
                parametros.set(
                    "fechaFin",
                    inputFechaFin.value
                );
            }

            if (selectUsuario?.value) {
                parametros.set(
                    "usuarioId",
                    selectUsuario.value
                );
            }

            if (selectArea?.value) {
                parametros.set(
                    "departamentoId",
                    selectArea.value
                );
            }

            const respuesta =
                await fetch(
                    `?handler=HorasExtras&${parametros.toString()}`,
                    {
                        method:
                            "GET",

                        headers: {
                            "Accept":
                                "application/json"
                        },

                        signal:
                            controladorConsultaTime.signal
                    }
                );

            const datos =
                await respuesta.json();

            if (!respuesta.ok ||
                !datos.success) {

                throw new Error(
                    datos.message ||
                    "No fue posible obtener Horas Extras."
                );
            }

            pintarHorasExtras(
                datos.extras
            );

            actualizarEstado(
                crearTextoActualizacion(
                    datos.extras
                        .fechaActualizacion
                ),
                "success"
            );
        }
        catch (error) {

            if (error.name ===
                "AbortError") {

                return;
            }

            console.error(
                "TIME Analytics - Horas Extras:",
                error
            );

            actualizarEstado(
                "Error al consultar Horas Extras",
                "error"
            );
        }
        finally {

            activarCargaVista(
                "extras",
                false
            );
        }
    }


    function pintarHorasExtras(
        datos
    ) {

        if (!datos) {
            return;
        }

        document
            .getElementById(
                "extrasHoras"
            )
            .textContent =
            `${formatoNumero.format(
                datos.horas || 0
            )} h`;

        document
            .getElementById(
                "extrasHorasExtras"
            )
            .textContent =
            `${formatoNumero.format(
                datos.horasExtras || 0
            )} h`;

        document
            .getElementById(
                "extrasHorasTotales"
            )
            .textContent =
            `${formatoNumero.format(
                datos.horasTotales || 0
            )} h`;

        document
            .getElementById(
                "extrasHorasFaltantes"
            )
            .textContent =
            `${formatoNumero.format(
                datos.horasFaltantes || 0
            )} h`;

        document
            .getElementById(
                "extrasPromedioPersona"
            )
            .textContent =
            `${formatoNumero.format(
                datos.promedioPersona || 0
            )} h`;

        pintarExtrasColaboradores(
            datos.colaboradores || []
        );

        pintarExtrasClasificacion(
            datos.clasificaciones || []
        );

        pintarExtrasAreas(
            datos.areasTipo || []
        );

        pintarExtrasDetalle(
            datos.colaboradores || []
        );
    }


    function pintarExtrasColaboradores(
        datos
    ) {

        if (chartExtrasColaboradores) {

            chartExtrasColaboradores.destroy();

            chartExtrasColaboradores =
                null;
        }

        const canvas =
            document.getElementById(
                "chartExtrasColaboradores"
            );

        if (!canvas ||
            typeof Chart === "undefined" ||
            !Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        const registrosOrdenados =
            [...datos]
                .sort(
                    (a, b) =>
                        Number(
                            b.horas ||
                            0
                        ) -
                        Number(
                            a.horas ||
                            0
                        )
                )
                .slice(
                    0,
                    20
                );

        canvas.style.height =
            "520px";

        chartExtrasColaboradores =
            new Chart(
                canvas,
                {
                    type:
                        "bar",

                    data: {

                        labels:
                            registrosOrdenados
                                .map(x =>
                                    x.nombre
                                ),

                        datasets: [
                            {
                                label:
                                    "Horas registradas",

                                data:
                                    registrosOrdenados
                                        .map(x =>
                                            Number(
                                                x.horas ||
                                                0
                                            )
                                        ),

                                backgroundColor:
                                    registrosOrdenados
                                        .map(x => {

                                            const horas =
                                                Number(
                                                    x.horas ||
                                                    0
                                                );

                                            if (
                                                horas >
                                                176
                                            ) {

                                                return "#DB5375";
                                            }

                                            if (
                                                Math.abs(
                                                    horas -
                                                    176
                                                ) <
                                                0.0001
                                            ) {

                                                return "#76F3EF";
                                            }

                                            return "#7D8CA3";
                                        }),

                                borderRadius:
                                    6,

                                borderSkipped:
                                    false,

                                barThickness:
                                    18,

                                maxBarThickness:
                                    22
                            }
                        ]
                    },

                    options: {

                        indexAxis:
                            "y",

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        layout: {

                            padding: {

                                left:
                                    10,

                                right:
                                    20,

                                top:
                                    10,

                                bottom:
                                    10
                            }
                        },

                        interaction: {

                            mode:
                                "nearest",

                            intersect:
                                false
                        },

                        plugins: {

                            legend: {

                                display:
                                    false
                            },

                            tooltip: {

                                callbacks: {

                                    title:
                                        context =>
                                            context[0]
                                                ?.label ||
                                            "",

                                    label:
                                        context => {

                                            const registro =
                                                registrosOrdenados[
                                                context.dataIndex
                                                ];

                                            if (!registro) {

                                                return "";
                                            }

                                            return [
                                                `Horas: ${formatoNumero.format(
                                                    Number(
                                                        registro.horas ||
                                                        0
                                                    )
                                                )} h`,

                                                `Extras: ${formatoNumero.format(
                                                    Number(
                                                        registro.horasExtras ||
                                                        0
                                                    )
                                                )} h`,

                                                `Faltantes: ${formatoNumero.format(
                                                    Number(
                                                        registro.horasFaltantes ||
                                                        0
                                                    )
                                                )} h`,

                                                `Estado: ${registro.clasificacion ||
                                                ""
                                                }`
                                            ];
                                        }
                                }
                            }
                        },

                        scales: {

                            x: {

                                beginAtZero:
                                    true,

                                grid: {

                                    color:
                                        "rgba(33, 22, 107, .07)"
                                },

                                title: {

                                    display:
                                        true,

                                    text:
                                        "Horas"
                                }
                            },

                            y: {

                                grid: {

                                    display:
                                        false
                                },

                                afterFit:
                                    scale => {

                                        scale.width =
                                            250;
                                    },

                                ticks: {

                                    autoSkip:
                                        false,

                                    padding:
                                        8,

                                    font: {

                                        size:
                                            11
                                    }
                                }
                            }
                        }
                    }
                }
            );
    }

    function pintarExtrasClasificacion(
        datos
    ) {

        if (chartExtrasClasificacion) {
            chartExtrasClasificacion.destroy();
            chartExtrasClasificacion = null;
        }

        const canvas =
            document.getElementById(
                "chartExtrasClasificacion"
            );

        if (!canvas ||
            typeof Chart ===
            "undefined" ||
            !Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        chartExtrasClasificacion =
            new Chart(
                canvas,
                {
                    type:
                        "doughnut",

                    data: {

                        labels:
                            datos.map(x =>
                                x.clasificacion
                            ),

                        datasets: [
                            {
                                data:
                                    datos.map(x =>
                                        x.personas
                                    ),

                                backgroundColor:
                                    datos.map(x =>
                                        x.clasificacion ===
                                            "Con horas extras"
                                            ? "#DB5375"
                                            : x.clasificacion ===
                                                "Cumplimiento"
                                                ? "#76F3EF"
                                                : "#7D8CA3"
                                    ),

                                borderColor:
                                    "#ffffff",

                                borderWidth:
                                    2
                            }
                        ]
                    },

                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        cutout:
                            "64%",

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarExtrasAreas(
        datos
    ) {

        if (chartExtrasAreas) {
            chartExtrasAreas.destroy();
            chartExtrasAreas = null;
        }

        const canvas =
            document.getElementById(
                "chartExtrasAreas"
            );

        if (!canvas ||
            typeof Chart ===
            "undefined" ||
            !Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        const areas =
            [...new Set(
                datos.map(x =>
                    x.area
                )
            )];

        const tipos =
            [...new Set(
                datos.map(x =>
                    x.tipo
                )
            )];

        canvas.style.height =
            `${Math.max(
                360,
                areas.length * 34
            )}px`;

        const datasets =
            tipos.map(
                (tipo, index) => ({
                    label:
                        tipo,

                    data:
                        areas.map(area => {

                            return datos
                                .filter(x =>
                                    x.area === area &&
                                    x.tipo === tipo
                                )
                                .reduce(
                                    (suma, x) =>
                                        suma +
                                        (x.horasExtras || 0),
                                    0
                                );
                        }),

                    backgroundColor:
                        obtenerColorTipo(
                            tipo
                        )
                })
            );

        chartExtrasAreas =
            new Chart(
                canvas,
                {
                    type:
                        "bar",

                    data: {

                        labels:
                            areas,

                        datasets:
                            datasets
                    },

                    options: {

                        indexAxis:
                            "y",

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        scales: {

                            x: {
                                stacked:
                                    true,

                                beginAtZero:
                                    true
                            },

                            y: {
                                stacked:
                                    true,

                                afterFit:
                                    scale => {

                                        scale.width =
                                            Math.min(
                                                Math.max(
                                                    scale.width,
                                                    180
                                                ),
                                                340
                                            );
                                    }
                            }
                        },

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true
                                }
                            }
                        }
                    }
                }
            );
    }


    function pintarExtrasDetalle(
        datos
    ) {

        if (paginadoresTime
            .extrasDetalle) {

            paginadoresTime
                .extrasDetalle
                .pagina = 1;
        }

        const registrosOrdenados =
            [...(datos || [])]
                .sort(
                    (a, b) =>
                        (a.nombre || "")
                            .localeCompare(
                                b.nombre || "",
                                "es",
                                {
                                    sensitivity:
                                        "base"
                                }
                            )
                );

        crearPaginadorTime({
            id:
                "extrasDetalle",

            registros:
                registrosOrdenados,

            renderizar:
                renderizarExtrasDetalle,

            tamanoInicial:
                10
        });
    }


    function renderizarExtrasDetalle(
        datos
    ) {

        const tbody =
            document.getElementById(
                "tablaExtrasDetalle"
            );

        if (!tbody) {
            return;
        }

        tbody.innerHTML = "";

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            tbody.innerHTML =
                `
                <tr>
                    <td colspan="6"
                        class="text-center text-muted py-4">
                        No hay información para los filtros seleccionados.
                    </td>
                </tr>
                `;

            return;
        }

        const fragmento =
            document.createDocumentFragment();

        datos.forEach(
            registro => {

                const fila =
                    document.createElement(
                        "tr"
                    );

                fila.innerHTML =
                    `
                    <td>${escaparHtml(
                        registro.nombre || ""
                    )}</td>

                    <td>${escaparHtml(
                        registro.area || ""
                    )}</td>

                    <td class="text-end">
                        ${formatoNumero.format(
                        registro.horas || 0
                    )} h
                    </td>

                    <td class="text-end fw-semibold">
                        ${formatoNumero.format(
                        registro.horasExtras || 0
                    )} h
                    </td>

                    <td class="text-end">
                        ${formatoNumero.format(
                        registro.horasFaltantes || 0
                    )} h
                    </td>

                    <td>
                        <span class="time-extras-badge ${escaparHtml(
                        registro.claseCss || ""
                    )}">
                            ${escaparHtml(
                        registro.clasificacion || ""
                    )}
                        </span>
                    </td>
                    `;

                fragmento.appendChild(
                    fila
                );
            }
        );

        tbody.appendChild(
            fragmento
        );
    }

    async function cargarEvaluacionAreas() {

        if (!validarFechas()) {
            return;
        }

        actualizarEstado(
            "Actualizando Evaluación de Áreas...",
            "loading"
        );

        activarCargaVista(
            "evaluacion",
            true
        );

        if (controladorConsultaTime) {

            controladorConsultaTime
                .abort();
        }

        controladorConsultaTime =
            new AbortController();

        try {

            const parametros =
                new URLSearchParams();

            const inputFechaInicio =
                obtenerFechaInicio();

            const inputFechaFin =
                obtenerFechaFin();

            const selectArea =
                obtenerArea();

            const selectUsuario =
                obtenerUsuario();

            if (inputFechaInicio?.value) {
                parametros.set(
                    "fechaInicio",
                    inputFechaInicio.value
                );
            }

            if (inputFechaFin?.value) {
                parametros.set(
                    "fechaFin",
                    inputFechaFin.value
                );
            }

            if (selectUsuario?.value) {
                parametros.set(
                    "usuarioId",
                    selectUsuario.value
                );
            }

            if (selectArea?.value) {
                parametros.set(
                    "departamentoId",
                    selectArea.value
                );
            }

            const tipo =
                document.getElementById(
                    "evaluacionTipo"
                );

            const puesto =
                document.getElementById(
                    "evaluacionPuesto"
                );

            if (tipo.value) {
                parametros.set(
                    "tipo",
                    tipo.value
                );
            }

            if (puesto.value) {
                parametros.set(
                    "puesto",
                    puesto.value
                );
            }

            const respuesta =
                await fetch(
                    `?handler=EvaluacionAreas&${parametros.toString()}`,
                    {
                        method:
                            "GET",

                        headers: {
                            "Accept":
                                "application/json"
                        },

                        signal:
                            controladorConsultaTime
                                .signal
                    }
                );

            const datos =
                await respuesta.json();

            if (!respuesta.ok ||
                !datos.success) {

                throw new Error(
                    datos.message ||
                    "No fue posible obtener Evaluación de Áreas."
                );
            }

            pintarEvaluacionAreas(
                datos.evaluacion
            );

            actualizarEstado(
                crearTextoActualizacion(
                    datos.evaluacion
                        .fechaActualizacion
                ),
                "success"
            );
        }
        catch (error) {

            if (error.name ===
                "AbortError") {

                return;
            }

            console.error(
                "TIME Analytics - Evaluación de Áreas:",
                error
            );

            actualizarEstado(
                "Error al consultar Evaluación de Áreas",
                "error"
            );
        }
        finally {

            activarCargaVista(
                "evaluacion",
                false
            );
        }
    }

    function pintarEvaluacionAreas(datos) {

        if (!datos) {
            return;
        }

        document
            .getElementById("evaluacionHoras")
            .textContent =
            `${formatoNumero.format(
                datos.horas || 0
            )} h`;

        document
            .getElementById("evaluacionPersonas")
            .textContent =
            formatoNumero.format(
                datos.personas || 0
            );

        cargarCatalogosEvaluacion(
            datos
        );

        pintarEvaluacionClaves(
            datos.claves || []
        );

        pintarEvaluacionPuestos(
            datos.distribucionPuestos || []
        );

        pintarEvaluacionEmpresa(
            datos.empresaColaborador || []
        );
    }

    function cargarCatalogosEvaluacion(datos) {

        if (catalogosEvaluacionCargados) {
            return;
        }

        const selectTipo =
            document.getElementById(
                "evaluacionTipo"
            );

        const selectPuesto =
            document.getElementById(
                "evaluacionPuesto"
            );

        (datos.tipos || []).forEach(tipo => {

            const opcion =
                document.createElement(
                    "option"
                );

            opcion.value =
                tipo;

            opcion.textContent =
                tipo;

            selectTipo.appendChild(
                opcion
            );
        });

        (datos.puestos || []).forEach(puesto => {

            const opcion =
                document.createElement(
                    "option"
                );

            opcion.value =
                puesto;

            opcion.textContent =
                puesto;

            selectPuesto.appendChild(
                opcion
            );
        });

        catalogosEvaluacionCargados =
            true;
    }

    function pintarEvaluacionClaves(datos) {

        if (chartEvaluacionClaves) {
            chartEvaluacionClaves.destroy();
            chartEvaluacionClaves = null;
        }

        const canvas =
            document.getElementById(
                "chartEvaluacionClaves"
            );

        if (!canvas ||
            typeof Chart === "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        const alturaMinima =
            Math.max(
                360,
                datos.length * 46
            );

        canvas.style.height =
            `${alturaMinima}px`;

        chartEvaluacionClaves =
            new Chart(
                canvas,
                {
                    type: "bar",

                    data: {

                        labels:
                            datos.map(x =>
                                x.descripcion
                            ),

                        datasets: [
                            {
                                label:
                                    "Horas",

                                data:
                                    datos.map(x =>
                                        x.horas
                                    ),

                                backgroundColor:
                                    datos.map(x =>
                                        obtenerColorTipo(
                                            x.tipo
                                        )
                                    ),

                                borderRadius:
                                    5,

                                borderSkipped:
                                    false
                            }
                        ]
                    },

                    options: {

                        indexAxis:
                            "y",

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        layout: {

                            padding: {
                                left: 12,
                                right: 22,
                                top: 10,
                                bottom: 10
                            }
                        },

                        plugins: {

                            legend: {
                                display:
                                    false
                            },

                            tooltip: {

                                callbacks: {

                                    title:
                                        context =>
                                            datos[
                                                context[0]
                                                    .dataIndex
                                            ]
                                                .descripcion,

                                    label:
                                        context =>
                                            `Horas: ${formatoNumero.format(
                                                context.parsed.x || 0
                                            )} h`,

                                    afterLabel:
                                        context =>
                                            `Tipo: ${datos[
                                                context.dataIndex
                                            ].tipo}`
                                }
                            }
                        },

                        scales: {

                            x: {

                                beginAtZero:
                                    true,

                                grid: {
                                    color:
                                        "rgba(33, 22, 107, .07)"
                                },

                                ticks: {

                                    callback:
                                        value =>
                                            formatoNumero.format(
                                                value
                                            )
                                }
                            },

                            y: {

                                afterFit:
                                    scale => {
                                        scale.width =
                                            Math.min(
                                                Math.max(
                                                    scale.width,
                                                    260
                                                ),
                                                420
                                            );
                                    },

                                grid: {
                                    display:
                                        false
                                },

                                ticks: {

                                    autoSkip:
                                        false,

                                    mirror:
                                        false,

                                    crossAlign:
                                        "far",

                                    font: {
                                        size:
                                            11
                                    },

                                    padding:
                                        10,

                                    callback:
                                        function (value) {

                                            return this
                                                .getLabelForValue(
                                                    value
                                                );
                                        }
                                }
                            }
                        }
                    }
                }
            );
    }

    function pintarEvaluacionPuestos(datos) {

        if (chartEvaluacionPuestos) {
            chartEvaluacionPuestos.destroy();
            chartEvaluacionPuestos = null;
        }

        const canvas =
            document.getElementById(
                "chartEvaluacionPuestos"
            );

        if (!canvas ||
            typeof Chart === "undefined") {

            return;
        }

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            return;
        }

        canvas.style.height =
            "360px";

        chartEvaluacionPuestos =
            new Chart(
                canvas,
                {
                    type: "doughnut",

                    data: {

                        labels:
                            datos.map(x =>
                                x.puesto
                            ),

                        datasets: [
                            {
                                data:
                                    datos.map(x =>
                                        x.horas
                                    ),

                                backgroundColor:
                                    datos.map(
                                        (_, index) =>
                                            paletaTime[
                                            index %
                                            paletaTime.length
                                            ]
                                    ),

                                borderWidth:
                                    2,

                                borderColor:
                                    "#ffffff",

                                hoverOffset:
                                    8
                            }
                        ]
                    },

                    options: {

                        responsive:
                            true,

                        maintainAspectRatio:
                            false,

                        cutout:
                            "65%",

                        plugins: {

                            legend: {
                                position:
                                    "bottom",

                                labels: {
                                    usePointStyle:
                                        true,

                                    padding:
                                        14,

                                    boxWidth:
                                        10,

                                    font: {
                                        size:
                                            11
                                    }
                                }
                            },

                            tooltip: {

                                callbacks: {

                                    title:
                                        context =>
                                            context[0]
                                                .label,

                                    label:
                                        context =>
                                            `${context.label}: ${formatoNumero.format(
                                                context.parsed || 0
                                            )} h`
                                }
                            }
                        }
                    }
                }
            );
    }

    function pintarEvaluacionEmpresa(
        datos
    ) {

        if (paginadoresTime.empresaColaborador) {

            paginadoresTime
                .empresaColaborador
                .pagina = 1;
        }

        crearPaginadorTime({
            id:
                "empresaColaborador",

            registros:
                datos || [],

            renderizar:
                renderizarEvaluacionEmpresa,

            tamanoInicial:
                10
        });
    }


    function renderizarEvaluacionEmpresa(
        datos
    ) {

        const tbody =
            document.getElementById(
                "tablaEvaluacionEmpresa"
            );

        tbody.innerHTML =
            "";

        if (!Array.isArray(datos) ||
            datos.length === 0) {

            tbody.innerHTML =
                `
            <tr>
                <td colspan="3"
                    class="text-center text-muted py-4">

                    No hay información para los filtros seleccionados.

                </td>
            </tr>
            `;

            return;
        }

        const fragmento =
            document.createDocumentFragment();

        datos.forEach(
            registro => {

                const fila =
                    document.createElement(
                        "tr"
                    );

                fila.innerHTML =
                    `
                <td>
                    ${escaparHtml(
                        registro.grupoEmpresa || ""
                    )}
                </td>

                <td>
                    ${escaparHtml(
                        registro.nombre || ""
                    )}
                </td>

                <td class="text-end fw-semibold">
                    ${formatoNumero.format(
                        registro.horas || 0
                    )} h
                </td>
                `;

                fragmento.appendChild(
                    fila
                );
            }
        );

        tbody.appendChild(
            fragmento
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

        activarCargaVista(
            "dashboard",
            true
        );

        if (controladorConsultaTime) {

            controladorConsultaTime
                .abort();
        }

        controladorConsultaTime =
            new AbortController();

        try {

            const parametros =
                new URLSearchParams();

            const inputFechaInicio =
                obtenerFechaInicio();

            const inputFechaFin =
                obtenerFechaFin();

            const selectArea =
                obtenerArea();

            const selectUsuario =
                obtenerUsuario();

            if (inputFechaInicio?.value) {
                parametros.set(
                    "fechaInicio",
                    inputFechaInicio.value
                );
            }

            if (inputFechaFin?.value) {
                parametros.set(
                    "fechaFin",
                    inputFechaFin.value
                );
            }

            if (selectUsuario?.value) {
                parametros.set(
                    "usuarioId",
                    selectUsuario.value
                );
            }

            if (selectArea?.value) {
                parametros.set(
                    "departamentoId",
                    selectArea.value
                );
            }

            const url =
                `?handler=Dashboard&${parametros.toString()}`;

            const respuesta =
                await fetch(
                    url,
                    {
                        method:
                            "GET",

                        headers: {
                            "Accept":
                                "application/json"
                        },

                        signal:
                            controladorConsultaTime
                                .signal
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

            if (error.name ===
                "AbortError") {

                return;
            }

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

            activarCargaVista(
                "dashboard",
                false
            );
        }
    }

    // ============================================================
    // VALIDACIÓN DE FILTROS
    // ============================================================

    function validarFechas() {

        const inputInicio =
            obtenerFechaInicio();

        const inputFin =
            obtenerFechaFin();

        if (!inputInicio ||
            !inputFin) {

            return false;
        }

        if (!inputInicio.value ||
            !inputFin.value) {

            actualizarEstado(
                "Selecciona un periodo",
                "warning"
            );

            return false;
        }

        if (inputInicio.value >
            inputFin.value) {

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

        const inputInicio =
            obtenerFechaInicio();

        const inputFin =
            obtenerFechaFin();

        const selectArea =
            obtenerArea();

        const selectUsuario =
            obtenerUsuario();

        if (inputInicio) {
            inputInicio.disabled =
                bloquear;
        }

        if (inputFin) {
            inputFin.disabled =
                bloquear;
        }

        if (selectArea) {
            selectArea.disabled =
                bloquear;
        }

        if (selectUsuario) {
            selectUsuario.disabled =
                bloquear;
        }
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

        document.getElementById("kpiHorasRegistradas").textContent =
            `${formatoNumero.format(
                kpis.horasRegistradas || 0
            )} h`;

        document.getElementById("kpiHorasFaltantes").textContent =
            `${formatoNumero.format(
                kpis.horasFaltantes || 0
            )} h`;

        document.getElementById("kpiCumplimiento").textContent =
            `${formatoNumero.format(
                (kpis.cumplimiento || 0) * 100
            )}%`;

        document.getElementById("kpiColaboradores").textContent =
            formatoNumero.format(
                kpis.colaboradores || 0
            );

        document.getElementById("kpiAreas").textContent =
            formatoNumero.format(
                kpis.areas || 0
            );
    }

    function limpiarKpis() {

        document.getElementById("kpiHorasRegistradas").textContent =
            "—";

        document.getElementById("kpiHorasFaltantes").textContent =
            "—";

        document.getElementById("kpiCumplimiento").textContent =
            "—";

        document.getElementById("kpiColaboradores").textContent =
            "—";

        document.getElementById("kpiAreas").textContent =
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
                                data: valores,

                                backgroundColor:
                                    datos.map(x =>
                                        obtenerColorTipo(
                                            x.tipo
                                        )
                                    ),

                                borderWidth: 2,

                                borderColor: "#ffffff",

                                hoverOffset: 8
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
                                        16,

                                    boxWidth:
                                        10,

                                    font: {
                                        size:
                                            11
                                    }
                                }
                            },

                            tooltip: {

                                callbacks: {

                                    title:
                                        context =>
                                            context[0]
                                                .label,

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

        const alturaMinima =
            Math.max(
                360,
                datos.length * 34
            );

        canvas.style.height =
            `${alturaMinima}px`;

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

                                backgroundColor:
                                    "#78D3F8",

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

                                backgroundColor:
                                    "#F08BB4",

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

                                backgroundColor:
                                    "#F6BD78",

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

                        layout: {

                            padding: {
                                left: 10,
                                right: 18,
                                top: 8,
                                bottom: 8
                            }
                        },

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

                                    title:
                                        context =>
                                            context[0]
                                                .label,

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
                                    true,

                                afterFit:
                                    scale => {
                                        scale.width =
                                            Math.min(
                                                Math.max(
                                                    scale.width,
                                                    180
                                                ),
                                                320
                                            );
                                    },

                                grid: {
                                    display:
                                        false
                                },

                                ticks: {

                                    autoSkip:
                                        false,

                                    font: {
                                        size:
                                            11
                                    },

                                    padding:
                                        8,

                                    callback:
                                        function (value) {

                                            return this
                                                .getLabelForValue(
                                                    value
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
    // TABLA COLABORADORES
    // ============================================================

    function pintarTablaColaboradores(
        registros
    ) {

        if (paginadoresTime.colaboradores) {

            paginadoresTime
                .colaboradores
                .pagina = 1;
        }

        crearPaginadorTime({
            id:
                "colaboradores",

            registros:
                registros || [],

            renderizar:
                renderizarTablaColaboradores,

            tamanoInicial:
                10
        });
    }


    function renderizarTablaColaboradores(
        registros
    ) {

        const tablaUsuarios =
            document.getElementById(
                "analyticsTablaUsuarios"
            );

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
                        (registro.cumplimiento || 0) *
                        100
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
                        registro.area ||
                        "Sin área"
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

    function activarCargaVista(
        vista,
        cargando
    ) {

        let idVista =
            "vistaDashboardCumplimiento";

        if (vista ===
            "evaluacion") {

            idVista =
                "vistaEvaluacionAreas";
        }

        if (vista ===
            "personal") {

            idVista =
                "vistaAnalisisPersonal";
        }

        if (vista ===
            "backoffice") {

            idVista =
                "vistaAlineacionBackOffice";
        }

        if (vista ===
            "operativas") {

            idVista =
                "vistaAlineacionOperativas";
        }

        if (vista ===
            "extras") {

            idVista =
                "vistaHorasExtras";
        }

        const elemento =
            document.getElementById(
                idVista
            );

        if (!elemento) {
            return;
        }

        elemento.classList.toggle(
            "time-analytics-refreshing",
            cargando
        );
    }


    // ============================================================
    // ESTADO
    // ============================================================

    function actualizarEstado(
        texto,
        tipo
    ) {

        const textoEstado =
            document.querySelector(
                ".time-analytics-status strong"
            );

        const puntoEstado =
            document.querySelector(
                ".time-analytics-status-dot"
            );

        if (textoEstado) {
            textoEstado.textContent =
                texto;
        }

        if (!puntoEstado) {
            return;
        }

        switch (tipo) {

            case "success":
                puntoEstado.style.background =
                    "#22c55e";
                break;

            case "error":
                puntoEstado.style.background =
                    "#dc3545";
                break;

            case "warning":
                puntoEstado.style.background =
                    "#d4ad45";
                break;

            default:
                puntoEstado.style.background =
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