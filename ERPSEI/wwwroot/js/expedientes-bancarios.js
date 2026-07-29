document.addEventListener("DOMContentLoaded", function () {
    document.body.classList.add(
        "module-main-theme",
        "expedientes-bancarios-page"
    );

    inicializarRfc();
    inicializarFormularios();
    inicializarMenuRapido();
});

function inicializarRfc() {
    const rfcInputs = document.querySelectorAll("[data-eb-rfc]");

    rfcInputs.forEach(function (input) {
        input.addEventListener("input", function () {
            this.value = this.value
                .toUpperCase()
                .replace(/\s+/g, "");
        });
    });
}

function inicializarFormularios() {
    const forms = document.querySelectorAll("form[data-eb-form]");

    forms.forEach(function (form) {
        form.addEventListener("submit", function () {
            if (!form.checkValidity()) {
                return;
            }

            const submitButton = form.querySelector(
                "button[type='submit']"
            );

            if (!submitButton) {
                return;
            }

            submitButton.disabled = true;
            submitButton.innerHTML =
                '<i class="fa-solid fa-spinner fa-spin me-1"></i>' +
                " Guardando...";
        });
    });
}

function inicializarMenuRapido() {
    const toggle = document.querySelector(".quick-nav-toggle");
    const menu = document.querySelector(".quick-nav");

    if (toggle && menu) {
        toggle.addEventListener("click", function (event) {
            event.preventDefault();
            event.stopPropagation();

            menu.classList.toggle("is-open");

            const expanded = menu.classList.contains("is-open");
            toggle.setAttribute("aria-expanded", expanded.toString());
        });
    }

    const dropdowns = document.querySelectorAll(".quick-dd");

    dropdowns.forEach(function (dropdown) {
        const trigger = dropdown.querySelector(":scope > .quick-link");

        if (!trigger) {
            return;
        }

        trigger.addEventListener("click", function (event) {
            if (window.innerWidth > 991.98) {
                return;
            }

            const dropdownMenu = dropdown.querySelector(
                ":scope > .quick-dd-menu"
            );

            if (!dropdownMenu) {
                return;
            }

            event.preventDefault();
            event.stopPropagation();

            dropdowns.forEach(function (otherDropdown) {
                if (otherDropdown !== dropdown) {
                    otherDropdown.classList.remove("open");
                }
            });

            dropdown.classList.toggle("open");
        });
    });

    document.addEventListener("click", function (event) {
        if (!event.target.closest(".quick-dd")) {
            dropdowns.forEach(function (dropdown) {
                dropdown.classList.remove("open");
            });
        }
    });
}