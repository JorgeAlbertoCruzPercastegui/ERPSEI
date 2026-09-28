document.addEventListener("DOMContentLoaded", function () {

    const rows = Array.from(
        document.querySelectorAll(".metrica-row")
    );

    const input =
        document.getElementById("txtBuscarMetrica");

    const btnPrev =
        document.getElementById("btnMetricasPrev");

    const btnNext =
        document.getElementById("btnMetricasNext");

    const pageNumbers =
        document.getElementById("metricasPageNumbers");

    const lblResumen =
        document.getElementById("lblMetricasResumen");

    const pageSize = 10;

    let paginaActual = 1;

    let filasFiltradas = [...rows];


    // =====================================================
    // NORMALIZAR TEXTO
    // =====================================================

    function normalizarTexto(texto) {

        return (texto || "")
            .toString()
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "");
    }


    // =====================================================
    // CREAR BOTÓN DE PÁGINA
    // =====================================================

    function crearBotonPagina(numero) {

        const button =
            document.createElement("button");

        button.type = "button";

        button.className =
            "metrica-page-number";

        button.textContent =
            numero;

        if (numero === paginaActual) {

            button.classList.add("active");
        }

        button.addEventListener(
            "click",
            function () {

                paginaActual =
                    numero;

                renderTabla();
            }
        );

        return button;
    }


    // =====================================================
    // CREAR PUNTOS SUSPENSIVOS
    // =====================================================

    function crearEllipsis() {

        const span =
            document.createElement("span");

        span.className =
            "metrica-page-ellipsis";

        span.textContent =
            "...";

        return span;
    }


    // =====================================================
    // RENDERIZAR NÚMEROS DEL PAGINADOR
    // =====================================================

    function renderPaginador(totalPaginas) {

        if (!pageNumbers) {
            return;
        }

        pageNumbers.innerHTML = "";

        if (totalPaginas <= 1) {

            pageNumbers.appendChild(
                crearBotonPagina(1)
            );

            return;
        }


        // Hasta 7 páginas las mostramos todas
        if (totalPaginas <= 7) {

            for (
                let i = 1;
                i <= totalPaginas;
                i++
            ) {

                pageNumbers.appendChild(
                    crearBotonPagina(i)
                );
            }

            return;
        }


        // Primera página
        pageNumbers.appendChild(
            crearBotonPagina(1)
        );


        // Páginas cercanas al inicio
        if (paginaActual <= 4) {

            for (
                let i = 2;
                i <= 5;
                i++
            ) {

                pageNumbers.appendChild(
                    crearBotonPagina(i)
                );
            }

            pageNumbers.appendChild(
                crearEllipsis()
            );
        }

        // Páginas cercanas al final
        else if (
            paginaActual >=
            totalPaginas - 3
        ) {

            pageNumbers.appendChild(
                crearEllipsis()
            );

            for (
                let i = totalPaginas - 4;
                i < totalPaginas;
                i++
            ) {

                pageNumbers.appendChild(
                    crearBotonPagina(i)
                );
            }
        }

        // Estamos en medio
        else {

            pageNumbers.appendChild(
                crearEllipsis()
            );

            for (
                let i = paginaActual - 1;
                i <= paginaActual + 1;
                i++
            ) {

                pageNumbers.appendChild(
                    crearBotonPagina(i)
                );
            }

            pageNumbers.appendChild(
                crearEllipsis()
            );
        }


        // Última página
        pageNumbers.appendChild(
            crearBotonPagina(
                totalPaginas
            )
        );
    }


    // =====================================================
    // RENDERIZAR TABLA
    // =====================================================

    function renderTabla() {

        const totalRegistros =
            filasFiltradas.length;

        const totalPaginas =
            Math.max(
                Math.ceil(
                    totalRegistros /
                    pageSize
                ),
                1
            );


        if (
            paginaActual >
            totalPaginas
        ) {

            paginaActual =
                totalPaginas;
        }


        // Ocultar todas
        rows.forEach(function (row) {

            row.style.display =
                "none";
        });


        const inicio =
            (paginaActual - 1) *
            pageSize;

        const fin =
            Math.min(
                inicio + pageSize,
                totalRegistros
            );


        // Mostrar solo 10
        filasFiltradas
            .slice(
                inicio,
                fin
            )
            .forEach(function (row) {

                row.style.display =
                    "";
            });


        // =================================================
        // RESUMEN
        // =================================================

        if (lblResumen) {

            if (
                totalRegistros === 0
            ) {

                lblResumen.textContent =
                    "Mostrando 0 de 0 registros";
            }
            else {

                lblResumen.textContent =
                    `Mostrando ${inicio + 1} a ${fin} de ${totalRegistros} registros`;
            }
        }


        // =================================================
        // BOTONES ANTERIOR / SIGUIENTE
        // =================================================

        if (btnPrev) {

            btnPrev.disabled =
                paginaActual <= 1;
        }

        if (btnNext) {

            btnNext.disabled =
                paginaActual >=
                totalPaginas;
        }


        // =================================================
        // NÚMEROS
        // =================================================

        renderPaginador(
            totalPaginas
        );
    }


    // =====================================================
    // FILTRAR
    // =====================================================

    function filtrar() {

        const filtro =
            normalizarTexto(
                input?.value
            );


        filasFiltradas =
            rows.filter(
                function (row) {

                    const contenido =
                        normalizarTexto(
                            row.getAttribute(
                                "data-search"
                            )
                        );

                    return (
                        filtro.length === 0 ||
                        contenido.includes(
                            filtro
                        )
                    );
                }
            );


        paginaActual = 1;

        renderTabla();
    }


    // =====================================================
    // BUSCADOR
    // =====================================================

    input?.addEventListener(
        "input",
        filtrar
    );


    // =====================================================
    // ANTERIOR
    // =====================================================

    btnPrev?.addEventListener(
        "click",
        function () {

            if (
                paginaActual > 1
            ) {

                paginaActual--;

                renderTabla();
            }
        }
    );


    // =====================================================
    // SIGUIENTE
    // =====================================================

    btnNext?.addEventListener(
        "click",
        function () {

            const totalPaginas =
                Math.max(
                    Math.ceil(
                        filasFiltradas.length /
                        pageSize
                    ),
                    1
                );

            if (
                paginaActual <
                totalPaginas
            ) {

                paginaActual++;

                renderTabla();
            }
        }
    );


    // =====================================================
    // CARGA INICIAL
    // =====================================================

    renderTabla();

});