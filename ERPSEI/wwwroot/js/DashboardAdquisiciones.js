document.addEventListener(
    "DOMContentLoaded",
    function () {

        const botonesCarpeta =
            document.querySelectorAll(
                ".adq-folder-card"
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


        const total =
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


        let carpetaActual =
            "solicitudes";


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
                    "Órdenes que fueron rechazadas durante su proceso de autorización."
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
                    "Órdenes que ya cuentan con una Solicitud de Pago oficial generada."
            }
        };


        function perteneceACarpeta(
            fila,
            carpeta
        ) {

            const estatusId =
                Number(
                    fila.dataset.estatusId
                    ??
                    0
                );


            switch (
            carpeta
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


                case "solicitudes":
                default:

                    return true;
            }
        }


        function aplicarFiltros() {

            const termino =
                (
                    buscar?.value
                    ??
                    ""
                )
                    .trim()
                    .toLowerCase();


            let visibles =
                0;


            filas.forEach(
                function (
                    fila
                ) {

                    const pertenece =
                        perteneceACarpeta(
                            fila,
                            carpetaActual
                        );


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


                    const coincideBusqueda =
                        !termino
                        ||
                        texto.includes(
                            termino
                        );


                    const mostrar =
                        pertenece
                        &&
                        coincideBusqueda;


                    fila.classList.toggle(
                        "d-none",
                        !mostrar
                    );


                    if (
                        mostrar
                    ) {
                        visibles++;
                    }
                }
            );


            if (
                total
            ) {
                total.textContent =
                    String(
                        visibles
                    );
            }


            if (
                tabla
            ) {
                tabla.classList.toggle(
                    "d-none",
                    visibles === 0
                );
            }


            if (
                vacio
            ) {
                vacio.classList.toggle(
                    "d-none",
                    visibles > 0
                );
            }
        }


        function seleccionarCarpeta(
            boton
        ) {

            carpetaActual =
                boton.dataset.folder
                ??
                "solicitudes";


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


            if (
                buscar
            ) {
                buscar.value =
                    "";
            }


            aplicarFiltros();


            document
                .getElementById(
                    "seccionOrdenesDashboardAdq"
                )
                ?.scrollIntoView(
                    {
                        behavior:
                            "smooth",

                        block:
                            "start"
                    }
                );
        }


        botonesCarpeta.forEach(
            function (
                boton
            ) {

                boton.addEventListener(
                    "click",
                    function () {

                        seleccionarCarpeta(
                            boton
                        );
                    }
                );
            }
        );


        buscar
            ?.addEventListener(
                "input",
                aplicarFiltros
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


                    aplicarFiltros();
                }
            );


        const carpetaInicial =
            document.querySelector(
                '.adq-folder-card[data-folder="solicitudes"]'
            );


        if (
            carpetaInicial
        ) {
            carpetaInicial.classList.add(
                "active"
            );
        }


        aplicarFiltros();
    }
);