document.addEventListener("DOMContentLoaded", function () {
    const input = document.getElementById("txtBuscarAuditoria");
    const selectAccion = document.getElementById("selAccionAuditoria");
    const rows = Array.from(document.querySelectorAll(".auditoria-row"));
    const empty = document.getElementById("auditoriaEmpty");

    const btnPrev = document.getElementById("btnAuditoriaPrev");
    const btnNext = document.getElementById("btnAuditoriaNext");
    const lblPagina = document.getElementById("lblAuditoriaPagina");

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
        const totalPaginas = Math.max(Math.ceil(filasFiltradas.length / pageSize), 1);

        if (paginaActual > totalPaginas) {
            paginaActual = totalPaginas;
        }

        rows.forEach(row => {
            row.style.display = "none";
        });

        const inicio = (paginaActual - 1) * pageSize;
        const fin = inicio + pageSize;

        filasFiltradas.slice(inicio, fin).forEach(row => {
            row.style.display = "";
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
        const texto = normalizarTexto(input?.value);
        const accion = selectAccion?.value || "";

        filasFiltradas = rows.filter(row => {
            const contenido = normalizarTexto(row.getAttribute("data-search"));
            const accionRow = row.getAttribute("data-accion") || "";

            const coincideTexto = texto.length === 0 || contenido.includes(texto);
            const coincideAccion = accion === "" || accionRow === accion;

            return coincideTexto && coincideAccion;
        });

        paginaActual = 1;
        renderTabla();
    }

    input?.addEventListener("input", filtrar);
    selectAccion?.addEventListener("change", filtrar);

    btnPrev?.addEventListener("click", function () {
        if (paginaActual > 1) {
            paginaActual--;
            renderTabla();
        }
    });

    btnNext?.addEventListener("click", function () {
        const totalPaginas = Math.max(Math.ceil(filasFiltradas.length / pageSize), 1);

        if (paginaActual < totalPaginas) {
            paginaActual++;
            renderTabla();
        }
    });

    renderTabla();
});