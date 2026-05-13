document.addEventListener("DOMContentLoaded", function () {
    const input = document.getElementById("txtBuscarDirectorio");
    const btnLimpiar = document.getElementById("btnLimpiarDirectorio");
    const rows = Array.from(document.querySelectorAll(".directorio-row"));
    const empty = document.getElementById("directorioEmpty");

    const btnPrev = document.getElementById("btnDirectorioPrev");
    const btnNext = document.getElementById("btnDirectorioNext");
    const lblPagina = document.getElementById("lblDirectorioPagina");

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

    function aplicarPaginacion() {
        const totalPaginas = Math.max(Math.ceil(filasFiltradas.length / pageSize), 1);

        if (paginaActual > totalPaginas) {
            paginaActual = totalPaginas;
        }

        rows.forEach(row => row.classList.add("d-none"));

        const inicio = (paginaActual - 1) * pageSize;
        const fin = inicio + pageSize;

        filasFiltradas.slice(inicio, fin).forEach(row => {
            row.classList.remove("d-none");
        });

        if (lblPagina) {
            lblPagina.textContent = `Página ${paginaActual} de ${totalPaginas}`;
        }

        if (btnPrev) {
            btnPrev.disabled = paginaActual <= 1;
        }

        if (btnNext) {
            btnNext.disabled = paginaActual >= totalPaginas;
        }

        empty?.classList.toggle("d-none", filasFiltradas.length > 0);
    }

    function filtrar() {
        const filtro = normalizarTexto(input.value);

        filasFiltradas = rows.filter(row => {
            const contenido = normalizarTexto(row.getAttribute("data-search"));
            return filtro.length === 0 || contenido.includes(filtro);
        });

        paginaActual = 1;
        aplicarPaginacion();
    }

    input?.addEventListener("input", filtrar);

    btnLimpiar?.addEventListener("click", function () {
        input.value = "";
        filtrar();
        input.focus();
    });

    btnPrev?.addEventListener("click", function () {
        if (paginaActual > 1) {
            paginaActual--;
            aplicarPaginacion();
        }
    });

    btnNext?.addEventListener("click", function () {
        const totalPaginas = Math.max(Math.ceil(filasFiltradas.length / pageSize), 1);

        if (paginaActual < totalPaginas) {
            paginaActual++;
            aplicarPaginacion();
        }
    });

    aplicarPaginacion();
});