document.addEventListener(
    "DOMContentLoaded",
    function () {

        // =========================================================
        // ELEMENTOS
        // =========================================================

        const botonesCarpeta =
            Array.from(
                document.querySelectorAll(
                    ".adq-folder-card"
                )
            );


        const botonesArea =
            Array.from(
                document.querySelectorAll(
                    ".adq-area-card"
                )
            );


        const filas =
            Array.from(
                document.querySelectorAll(
                    ".adq-dashboard-order-row"
                )
            );


        const buscar =
            document.getElementById(
                "buscarOrdenDashboardAdq"
            );


        const limpiarBusqueda =
            document.getElementById(
                "limpiarBusquedaDashboardAdq"
            );


        const totalOrdenes =
            document.getElementById(
                "totalOrdenesDashboardAdq"
            );


        const vacio =
            document.getElementById(
                "sinOrdenesDashboardAdq"
            );


        const tabla =
            document.getElementById(
                "tablaOrdenesDashboardAdq"
            );


        const titulo =
            document.getElementById(
                "tituloCarpetaDashboardAdq"
            );


        const descripcion =
            document.getElementById(
                "descripcionCarpetaDashboardAdq"
            );


        const paginacion =
            document.getElementById(
                "paginacionOrdenesDashboardAdq"
            );


        const paginacionLista =
            document.getElementById(
                "dashboardPaginacionLista"
            );


        const paginaInicio =
            document.getElementById(
                "dashboardPaginaInicio"
            );


        const paginaFin =
            document.getElementById(
                "dashboardPaginaFin"
            );


        const paginaTotal =
            document.getElementById(
                "dashboardPaginaTotal"
            );


        // =========================================================
        // ESTADO
        // =========================================================

        let carpetaActual =
            "solicitudes";


        let areaActual =
            0;


        let paginaActual =
            1;


        const registrosPorPagina =
            10;


        // =========================================================
        // CONFIGURACIÓN DE CARPETAS
        // =========================================================

        const configuracionCarpetas =
        {
            solicitudes:
            {
                titulo:
                    "Solicitudes realizadas",

                descripcion:
                    "Consulta todas las órdenes registradas dentro del módulo de Adquisiciones."
            },

            proceso:
            {
                titulo:
                    "Aceptadas / En proceso",

                descripcion:
                    "Órdenes que actualmente continúan dentro del flujo operativo de adquisición."
            },

            rechazadas:
            {
                titulo:
                    "Solicitudes rechazadas",

                descripcion:
                    "Órdenes que fueron rechazadas durante el proceso de autorización."
            },

            canceladas:
            {
                titulo:
                    "Solicitudes canceladas",

                descripcion:
                    "Órdenes cuyo proceso fue cancelado."
            },

            pagos:
            {
                titulo:
                    "Solicitudes de pago",

                descripcion:
                    "Órdenes que cuentan con una Solicitud de Pago oficial generada."
            }
        };


        // =========================================================
        // VALIDAR CARPETA
        // =========================================================

        function perteneceACarpeta(
            fila
        ) {

            const estatusId =
                Number(
                    fila.dataset.estatusId
                    ??
                    0
                );


            switch (
            carpetaActual
            ) {

                case "proceso":

                    return (
                        fila.dataset.proceso ===
                        "true"
                    );


                case "rechazadas":

                    return (
                        estatusId ===
                        6
                    );


                case "canceladas":

                    return (
                        estatusId ===
                        7
                    );


                case "pagos":

                    return (
                        fila.dataset.pago ===
                        "true"
                    );


                default:

                    return true;
            }
        }


        // =========================================================
        // VALIDAR ÁREA
        // =========================================================

        function perteneceAArea(
            fila
        ) {

            if (
                areaActual ===
                0
            ) {
                return true;
            }


            return (
                Number(
                    fila.dataset.areaId
                    ??
                    0
                ) ===
                areaActual
            );
        }


        // =========================================================
        // OBTENER FILAS FILTRADAS
        // =========================================================

        function obtenerFilasFiltradas() {

            const termino =
                (
                    buscar?.value
                    ??
                    ""
                )
                    .trim()
                    .toLowerCase();


            return filas.filter(
                function (
                    fila
                ) {

                    if (
                        !perteneceACarpeta(
                            fila
                        )
                    ) {
                        return false;
                    }


                    if (
                        !perteneceAArea(
                            fila
                        )
                    ) {
                        return false;
                    }


                    const texto =
                        [
                            fila.dataset.folio,
                            fila.dataset.titulo,
                            fila.dataset.area,
                            fila.dataset.estatus
                        ]
                            .join(
                                " "
                            )
                            .toLowerCase();


                    return (
                        !termino
                        ||
                        texto.includes(
                            termino
                        )
                    );
                }
            );
        }


        // =========================================================
        // PAGINADOR
        // =========================================================

        function crearBotonPagina(
            texto,
            pagina,
            activo,
            deshabilitado
        ) {

            const li =
                document.createElement(
                    "li"
                );


            li.className =
                "page-item";


            if (
                activo
            ) {
                li.classList.add(
                    "active"
                );
            }


            if (
                deshabilitado
            ) {
                li.classList.add(
                    "disabled"
                );
            }


            const boton =
                document.createElement(
                    "button"
                );


            boton.type =
                "button";


            boton.className =
                "page-link";


            boton.textContent =
                texto;


            boton.disabled =
                deshabilitado;


            boton.addEventListener(
                "click",
                function () {

                    if (
                        deshabilitado
                    ) {
                        return;
                    }


                    paginaActual =
                        pagina;


                    renderizar();
                }
            );


            li.appendChild(
                boton
            );


            return li;
        }


        function renderizarPaginador(
            totalRegistros
        ) {

            if (
                !paginacionLista
            ) {
                return;
            }


            paginacionLista.innerHTML =
                "";


            const totalPaginas =
                Math.max(
                    1,
                    Math.ceil(
                        totalRegistros
                        /
                        registrosPorPagina
                    )
                );


            if (
                paginaActual >
                totalPaginas
            ) {
                paginaActual =
                    totalPaginas;
            }


            paginacionLista.appendChild(
                crearBotonPagina(
                    "Anterior",
                    Math.max(
                        1,
                        paginaActual - 1
                    ),
                    false,
                    paginaActual === 1
                )
            );


            for (
                let pagina = 1;
                pagina <= totalPaginas;
                pagina++
            ) {

                paginacionLista.appendChild(
                    crearBotonPagina(
                        String(
                            pagina
                        ),
                        pagina,
                        pagina === paginaActual,
                        false
                    )
                );
            }


            paginacionLista.appendChild(
                crearBotonPagina(
                    "Siguiente",
                    Math.min(
                        totalPaginas,
                        paginaActual + 1
                    ),
                    false,
                    paginaActual === totalPaginas
                )
            );


            paginacion
                ?.classList
                .toggle(
                    "d-none",
                    totalRegistros === 0
                );
        }


        // =========================================================
        // RENDERIZAR
        // =========================================================

        function renderizar() {

            const filtradas =
                obtenerFilasFiltradas();


            const total =
                filtradas.length;


            const totalPaginas =
                Math.max(
                    1,
                    Math.ceil(
                        total
                        /
                        registrosPorPagina
                    )
                );


            if (
                paginaActual >
                totalPaginas
            ) {
                paginaActual =
                    totalPaginas;
            }


            const indiceInicial =
                (
                    paginaActual - 1
                )
                *
                registrosPorPagina;


            const indiceFinal =
                Math.min(
                    indiceInicial
                    +
                    registrosPorPagina,
                    total
                );


            filas.forEach(
                function (
                    fila
                ) {

                    fila.classList.add(
                        "d-none"
                    );
                }
            );


            filtradas
                .slice(
                    indiceInicial,
                    indiceFinal
                )
                .forEach(
                    function (
                        fila
                    ) {

                        fila.classList.remove(
                            "d-none"
                        );
                    }
                );


            if (
                totalOrdenes
            ) {
                totalOrdenes.textContent =
                    String(
                        total
                    );
            }


            if (
                paginaInicio
            ) {

                paginaInicio.textContent =
                    total === 0
                        ? "0"
                        : String(
                            indiceInicial + 1
                        );
            }


            if (
                paginaFin
            ) {

                paginaFin.textContent =
                    String(
                        indiceFinal
                    );
            }


            if (
                paginaTotal
            ) {

                paginaTotal.textContent =
                    String(
                        total
                    );
            }


            tabla
                ?.classList
                .toggle(
                    "d-none",
                    total === 0
                );


            vacio
                ?.classList
                .toggle(
                    "d-none",
                    total > 0
                );


            renderizarPaginador(
                total
            );
        }


        // =========================================================
        // SELECCIONAR CARPETA
        // =========================================================

        botonesCarpeta.forEach(
            function (
                boton
            ) {

                boton.addEventListener(
                    "click",
                    function () {

                        carpetaActual =
                            boton.dataset.folder
                            ??
                            "solicitudes";


                        paginaActual =
                            1;


                        botonesCarpeta.forEach(
                            function (
                                item
                            ) {

                                item.classList.remove(
                                    "active"
                                );
                            }
                        );


                        boton.classList.add(
                            "active"
                        );


                        const configuracion =
                            configuracionCarpetas[
                            carpetaActual
                            ];


                        if (
                            configuracion
                        ) {

                            if (
                                titulo
                            ) {
                                titulo.textContent =
                                    configuracion.titulo;
                            }


                            if (
                                descripcion
                            ) {
                                descripcion.textContent =
                                    configuracion.descripcion;
                            }
                        }


                        renderizar();
                    }
                );
            }
        );


        // =========================================================
        // SELECCIONAR ÁREA
        // =========================================================

        botonesArea.forEach(
            function (
                boton
            ) {

                boton.addEventListener(
                    "click",
                    function () {

                        areaActual =
                            Number(
                                boton.dataset.areaId
                                ??
                                0
                            );


                        paginaActual =
                            1;


                        botonesArea.forEach(
                            function (
                                item
                            ) {

                                item.classList.remove(
                                    "active"
                                );
                            }
                        );


                        boton.classList.add(
                            "active"
                        );


                        renderizar();
                    }
                );
            }
        );


        // =========================================================
        // BUSCADOR
        // =========================================================

        buscar
            ?.addEventListener(
                "input",
                function () {

                    paginaActual =
                        1;

                    renderizar();
                }
            );


        limpiarBusqueda
            ?.addEventListener(
                "click",
                function () {

                    if (
                        buscar
                    ) {

                        buscar.value =
                            "";

                        buscar.focus();
                    }


                    paginaActual =
                        1;


                    renderizar();
                }
            );


        // =========================================================
        // INICIO
        // =========================================================

        document
            .querySelector(
                '.adq-folder-card[data-folder="solicitudes"]'
            )
            ?.classList
            .add(
                "active"
            );


        document
            .querySelector(
                '.adq-area-card[data-area-id="0"]'
            )
            ?.classList
            .add(
                "active"
            );


        renderizar();
    

    // =========================================================
// DIRECTORIO DE PROVEEDORES
// =========================================================

const filasProveedores =
    Array.from(
        document.querySelectorAll(
            ".adq-dashboard-supplier-row"
        )
    );


const buscarProveedor =
    document.getElementById(
        "buscarProveedorDashboardAdq"
    );


const limpiarProveedor =
    document.getElementById(
        "limpiarBusquedaProveedorDashboardAdq"
    );


const tablaProveedores =
    document.getElementById(
        "tablaProveedoresDashboardAdq"
    );


const totalProveedores =
    document.getElementById(
        "totalProveedoresDashboardAdq"
    );


const sinProveedores =
    document.getElementById(
        "sinProveedoresDashboardAdq"
    );


const paginacionProveedores =
    document.getElementById(
        "paginacionProveedoresDashboardAdq"
    );


const listaPaginacionProveedores =
    document.getElementById(
        "proveedoresPaginacionLista"
    );


const proveedoresInicio =
    document.getElementById(
        "proveedoresPaginaInicio"
    );


const proveedoresFin =
    document.getElementById(
        "proveedoresPaginaFin"
    );


const proveedoresTotal =
    document.getElementById(
        "proveedoresPaginaTotal"
    );


let paginaProveedorActual =
    1;


const proveedoresPorPagina =
    10;


// =========================================================
// FILTRAR PROVEEDORES
// =========================================================

function obtenerProveedoresFiltrados() {

    const termino =
        (
            buscarProveedor?.value
            ??
            ""
        )
            .trim()
            .toLowerCase();


    if (
        !termino
    ) {
        return filasProveedores;
    }


    return filasProveedores.filter(
        function (
            fila
        ) {

            const texto =
                [
                    fila.dataset.nombre,
                    fila.dataset.rfc,
                    fila.dataset.contacto,
                    fila.dataset.email,
                    fila.dataset.telefono
                ]
                    .join(
                        " "
                    )
                    .toLowerCase();


            return texto.includes(
                termino
            );
        }
    );
}


// =========================================================
// CREAR BOTÓN PAGINACIÓN PROVEEDOR
// =========================================================

function crearBotonPaginaProveedor(
    texto,
    pagina,
    activo,
    deshabilitado
) {

    const li =
        document.createElement(
            "li"
        );


    li.className =
        "page-item";


    if (
        activo
    ) {
        li.classList.add(
            "active"
        );
    }


    if (
        deshabilitado
    ) {
        li.classList.add(
            "disabled"
        );
    }


    const boton =
        document.createElement(
            "button"
        );


    boton.type =
        "button";


    boton.className =
        "page-link";


    boton.textContent =
        texto;


    boton.disabled =
        deshabilitado;


    boton.addEventListener(
        "click",
        function () {

            if (
                deshabilitado
            ) {
                return;
            }


            paginaProveedorActual =
                pagina;


            renderizarProveedores();
        }
    );


    li.appendChild(
        boton
    );


    return li;
}


// =========================================================
// PAGINADOR PROVEEDORES
// =========================================================

function renderizarPaginadorProveedores(
    totalRegistros
) {

    if (
        !listaPaginacionProveedores
    ) {
        return;
    }


    listaPaginacionProveedores.innerHTML =
        "";


    const totalPaginas =
        Math.max(
            1,
            Math.ceil(
                totalRegistros
                /
                proveedoresPorPagina
            )
        );


    if (
        paginaProveedorActual >
        totalPaginas
    ) {
        paginaProveedorActual =
            totalPaginas;
    }


    listaPaginacionProveedores.appendChild(
        crearBotonPaginaProveedor(
            "Anterior",
            Math.max(
                1,
                paginaProveedorActual - 1
            ),
            false,
            paginaProveedorActual === 1
        )
    );


    for (
        let pagina = 1;
        pagina <= totalPaginas;
        pagina++
    ) {

        listaPaginacionProveedores.appendChild(
            crearBotonPaginaProveedor(
                String(
                    pagina
                ),
                pagina,
                pagina === paginaProveedorActual,
                false
            )
        );
    }


    listaPaginacionProveedores.appendChild(
        crearBotonPaginaProveedor(
            "Siguiente",
            Math.min(
                totalPaginas,
                paginaProveedorActual + 1
            ),
            false,
            paginaProveedorActual === totalPaginas
        )
    );


    paginacionProveedores
        ?.classList
        .toggle(
            "d-none",
            totalRegistros === 0
        );
}


// =========================================================
// RENDERIZAR PROVEEDORES
// =========================================================

function renderizarProveedores() {

    const filtrados =
        obtenerProveedoresFiltrados();


    const total =
        filtrados.length;


    const totalPaginas =
        Math.max(
            1,
            Math.ceil(
                total
                /
                proveedoresPorPagina
            )
        );


    if (
        paginaProveedorActual >
        totalPaginas
    ) {
        paginaProveedorActual =
            totalPaginas;
    }


    const indiceInicial =
        (
            paginaProveedorActual - 1
        )
        *
        proveedoresPorPagina;


    const indiceFinal =
        Math.min(
            indiceInicial
            +
            proveedoresPorPagina,
            total
        );


    filasProveedores.forEach(
        function (
            fila
        ) {

            fila.classList.add(
                "d-none"
            );
        }
    );


    filtrados
        .slice(
            indiceInicial,
            indiceFinal
        )
        .forEach(
            function (
                fila
            ) {

                fila.classList.remove(
                    "d-none"
                );
            }
        );


    if (
        totalProveedores
    ) {
        totalProveedores.textContent =
            String(
                total
            );
    }


    if (
        proveedoresInicio
    ) {
        proveedoresInicio.textContent =
            total === 0
                ? "0"
                : String(
                    indiceInicial + 1
                );
    }


    if (
        proveedoresFin
    ) {
        proveedoresFin.textContent =
            String(
                indiceFinal
            );
    }


    if (
        proveedoresTotal
    ) {
        proveedoresTotal.textContent =
            String(
                total
            );
    }


    tablaProveedores
        ?.classList
        .toggle(
            "d-none",
            total === 0
        );


    sinProveedores
        ?.classList
        .toggle(
            "d-none",
            total > 0
        );


    renderizarPaginadorProveedores(
        total
    );
}


// =========================================================
// EVENTOS PROVEEDORES
// =========================================================

buscarProveedor
    ?.addEventListener(
        "input",
        function () {

            paginaProveedorActual =
                1;

            renderizarProveedores();
        }
    );


limpiarProveedor
    ?.addEventListener(
        "click",
        function () {

            if (
                buscarProveedor
            ) {
                buscarProveedor.value =
                    "";

                buscarProveedor.focus();
            }

            paginaProveedorActual =
                1;

            renderizarProveedores();
        }
    );


        renderizarProveedores();

    }
);