document.addEventListener("DOMContentLoaded", function () {

    const rows = Array.from(document.querySelectorAll(".metrica-row"));

    const input = document.getElementById("txtBuscarMetrica");

    const btnPrev = document.getElementById("btnMetricasPrev");

    const btnNext = document.getElementById("btnMetricasNext");

    const lblPagina = document.getElementById("lblMetricasPagina");

    const pageSize = 10;

    let paginaActual = 1;

    let filasFiltradas = [...rows];

    function normalizarTexto(texto) {
        return (texto || "")
            .toString()
            .toLowerCase()
            .normalize("NFD")
            .replace(/[\u0300-\u036f]/g, "");
    }

    function renderTabla() {

        const totalPaginas =
            Math.max(Math.ceil(filasFiltradas.length / pageSize), 1);

        if (paginaActual > totalPaginas) {
            paginaActual = totalPaginas;
        }

        rows.forEach(row => {
            row.style.display = "none";
        });

        const inicio = (paginaActual - 1) * pageSize;

        const fin = inicio + pageSize;

        filasFiltradas
            .slice(inicio, fin)
            .forEach(row => {
                row.style.display = "";
            });

        if (lblPagina) {
            lblPagina.textContent =
                `Página ${paginaActual} de ${totalPaginas}`;
        }

        if (btnPrev) {
            btnPrev.disabled = paginaActual <= 1;
        }

        if (btnNext) {
            btnNext.disabled = paginaActual >= totalPaginas;
        }
    }

    function filtrar() {

        const filtro =
            normalizarTexto(input?.value);

        filasFiltradas = rows.filter(row => {

            const contenido =
                normalizarTexto(row.getAttribute("data-search"));

            return filtro.length === 0 ||
                contenido.includes(filtro);
        });

        paginaActual = 1;

        renderTabla();
    }

    input?.addEventListener("input", filtrar);

    btnPrev?.addEventListener("click", function () {

        if (paginaActual > 1) {

            paginaActual--;

            renderTabla();
        }
    });

    btnNext?.addEventListener("click", function () {

        const totalPaginas =
            Math.max(Math.ceil(filasFiltradas.length / pageSize), 1);

        if (paginaActual < totalPaginas) {

            paginaActual++;

            renderTabla();
        }
    });

    renderTabla();

});