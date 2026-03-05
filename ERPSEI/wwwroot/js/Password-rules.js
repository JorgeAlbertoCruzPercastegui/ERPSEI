// wwwroot/js/password-rules.js
(function () {

    // Asegura que exista jQuery
    if (typeof window.jQuery === "undefined") {
        console.error("password-rules.js requiere jQuery.");
        return;
    }

    function setRule(ruleId, ok) {
        const el = $("#" + ruleId);
        el.toggleClass("ok", !!ok);
    }

    function evalPasswordRules(pwd) {
        pwd = pwd || "";

        // Si está vacío, NO marques nada como OK
        if (pwd.length === 0) {
            setRule("rule-digit", false);
            setRule("rule-special", false);
            setRule("rule-upper", false);
            setRule("rule-no-repeat", false);
            setRule("rule-length", false);
            return;
        }

        const hasDigit = /[0-9]/.test(pwd);
        const hasSpecial = new RegExp("[!@#$%^&*]").test(pwd);
        const hasUpper = /[A-ZÁÉÍÓÚÑ]/.test(pwd);
        const noRepeatConsecutive = !/(.)\1/.test(pwd);
        const minLen = pwd.length >= 8;

        setRule("rule-digit", hasDigit);
        setRule("rule-special", hasSpecial);
        setRule("rule-upper", hasUpper);
        setRule("rule-no-repeat", noRepeatConsecutive);
        setRule("rule-length", minLen);
    }

    function initPasswordUI() {
        const $pwd = $("#userPassword");
        const $btn = $("#btnTogglePwd");

        // Si no existe el input en la página, no hagas nada
        if ($pwd.length === 0) return;

        // Toggle ver/ocultar contraseña (si existe el botón)
        if ($btn.length > 0) {
            $btn.off("click.passwordRules").on("click.passwordRules", function () {
                const input = $pwd[0];
                const $icon = $(this).find("i");

                if (!input) return;

                if (input.type === "password") {
                    input.type = "text";
                    $icon.removeClass("bi-eye").addClass("bi-eye-slash");
                } else {
                    input.type = "password";
                    $icon.removeClass("bi-eye-slash").addClass("bi-eye");
                }
            });
        }

        // Checklist en vivo
        $pwd.off("input.passwordRules focus.passwordRules")
            .on("input.passwordRules focus.passwordRules", function () {
                evalPasswordRules($(this).val());
            });

        // Inicial
        evalPasswordRules($pwd.val());
    }

    // Inicializa cuando el DOM esté listo
    $(document).ready(initPasswordUI);

})();