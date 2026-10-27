/* =====================================================
   ERPSEI - PERFIL RESPONSIVE V4
   Panel independiente y cierre funcional
   ===================================================== */

document.addEventListener("DOMContentLoaded", function () {

    const btn = document.getElementById("btnSidebar");
    const sidebar = document.getElementById("sidebarProfile");
    const layout = document.getElementById("layoutWrapper");
    const topbar = document.querySelector(".topbar-links");

    if (!btn || !sidebar || !layout) return;

    const mobileQuery = window.matchMedia(
        "(max-width: 991.98px)"
    );

    const isMobile = () => mobileQuery.matches;

    // Conservar las ubicaciones originales
    const btnMarker = document.createComment("perfil-boton");
    const sidebarMarker = document.createComment("perfil-panel");

    btn.parentNode.insertBefore(btnMarker, btn);
    sidebar.parentNode.insertBefore(sidebarMarker, sidebar);

    let abierto = false;

    // ============================================
    // CREAR BOTÓN X
    // ============================================

    let closeBtn = sidebar.querySelector(
        ".mobile-profile-close"
    );

    if (!closeBtn) {
        closeBtn = document.createElement("button");
        closeBtn.className = "mobile-profile-close";
        closeBtn.type = "button";
        closeBtn.innerHTML = '<i class="bi bi-x-lg"></i>';
        sidebar.prepend(closeBtn);
    }

    closeBtn.setAttribute(
        "aria-label",
        "Cerrar datos del colaborador"
    );

    // ============================================
    // CREAR FONDO OSCURO
    // ============================================

    let backdrop = document.querySelector(
        ".mobile-profile-backdrop"
    );

    if (!backdrop) {
        backdrop = document.createElement("div");
        backdrop.className = "mobile-profile-backdrop";
        document.body.appendChild(backdrop);
    }

    // ============================================
    // ACCESIBILIDAD
    // ============================================

    btn.type = "button";
    btn.setAttribute("aria-controls", "sidebarProfile");
    btn.setAttribute("aria-expanded", "false");
    btn.setAttribute("aria-label", "Ver mis datos");

    sidebar.setAttribute("aria-label", "Mis datos personales");

    // ============================================
    // UBICAR COMPONENTES
    // ============================================

    function configurarUbicacion() {

        if (isMobile()) {

            // Botón dentro de la barra azul
            if (topbar && btn.parentNode !== topbar) {
                topbar.insertBefore(btn, topbar.firstChild);
            }

            // Panel directamente en body para evitar
            // conflictos de capas y contenedores.
            if (sidebar.parentNode !== document.body) {
                document.body.appendChild(sidebar);
            }

        } else {

            // Restaurar botón en escritorio
            if (btn.parentNode !== btnMarker.parentNode) {
                btnMarker.parentNode.insertBefore(
                    btn,
                    btnMarker.nextSibling
                );
            }

            // Restaurar panel en escritorio
            if (sidebar.parentNode !== sidebarMarker.parentNode) {
                sidebarMarker.parentNode.insertBefore(
                    sidebar,
                    sidebarMarker.nextSibling
                );
            }
        }
    }

    // ============================================
    // ICONO DEL BOTÓN
    // ============================================

    function actualizarIcono() {

        const icon = btn.querySelector("i");
        if (!icon) return;

        if (isMobile()) {
            icon.className = "bi bi-person-lines-fill";
        } else {
            icon.className = abierto
                ? "bi bi-x-lg"
                : "bi bi-list";
        }
    }

    // ============================================
    // ABRIR PERFIL
    // ============================================

    function abrirPerfil() {

        abierto = true;

        sidebar.classList.add("active");
        btn.classList.add("active");

        if (isMobile()) {

            layout.classList.remove("shifted");

            backdrop.classList.add("active");

            document.body.classList.add(
                "mobile-profile-open"
            );

            closeBtn.focus();

        } else {

            layout.classList.add("shifted");

        }

        btn.setAttribute("aria-expanded", "true");

        actualizarIcono();
    }

    // ============================================
    // CERRAR PERFIL
    // ============================================

    function cerrarPerfil() {

        abierto = false;

        sidebar.classList.remove("active");
        btn.classList.remove("active");
        layout.classList.remove("shifted");

        backdrop.classList.remove("active");

        document.body.classList.remove(
            "mobile-profile-open"
        );

        btn.setAttribute("aria-expanded", "false");

        actualizarIcono();
    }

    // ============================================
    // EVENTO DEL BOTÓN IZQUIERDO
    // ============================================

    btn.addEventListener("click", function (e) {

        e.preventDefault();
        e.stopPropagation();

        if (abierto) {
            cerrarPerfil();
        } else {
            abrirPerfil();
        }
    });

    // ============================================
    // CERRAR CON X
    // ============================================

    closeBtn.addEventListener("click", function (e) {

        e.preventDefault();
        e.stopPropagation();

        cerrarPerfil();

        if (isMobile()) btn.focus();
    });

    // ============================================
    // CERRAR TOCANDO EL FONDO
    // ============================================

    backdrop.addEventListener("click", cerrarPerfil);

    // ============================================
    // CERRAR CON ESCAPE
    // ============================================

    document.addEventListener("keydown", function (e) {

        if (e.key === "Escape" && abierto) {
            cerrarPerfil();
            btn.focus();
        }
    });

    // ============================================
    // CAMBIO ENTRE ESCRITORIO Y MÓVIL
    // ============================================

    function actualizarVista() {
        cerrarPerfil();
        configurarUbicacion();
        actualizarIcono();
    }

    if (mobileQuery.addEventListener) {
        mobileQuery.addEventListener(
            "change",
            actualizarVista
        );
    } else {
        mobileQuery.addListener(actualizarVista);
    }

    // ============================================
    // INICIAR
    // ============================================

    configurarUbicacion();
    cerrarPerfil();
});