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


/* =====================================================
   MENU DROPDOWN GLOBAL
   ===================================================== */

document.addEventListener("DOMContentLoaded", function () {

    const btn = document.querySelector(".quick-nav-toggle");
    const menu = document.getElementById("quickNavMenu");

    if (!btn || !menu) return;

    const isMobile = () => window.matchMedia("(max-width: 991.98px)").matches;

    function cerrarDropdowns(excepto = null) {
        document.querySelectorAll(".quick-dd.open").forEach(x => {
            if (x !== excepto) {
                x.classList.remove("open");
            }
        });
    }

    document.querySelectorAll(".quick-dd-toggle").forEach(toggle => {

        toggle.addEventListener("click", function (e) {

            e.preventDefault();
            e.stopPropagation();

            const drop = this.closest(".quick-dd");

            if (!drop) return;

            const yaAbierto = drop.classList.contains("open");

            cerrarDropdowns(drop);

            if (!yaAbierto) {
                drop.classList.add("open");
            }
        });

    });

    document.addEventListener("click", function (e) {

        if (!menu.contains(e.target) && !btn.contains(e.target)) {
            cerrarDropdowns();
        }

    });

});