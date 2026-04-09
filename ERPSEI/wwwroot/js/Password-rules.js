(function () {

    if (typeof window.jQuery === "undefined") {
        console.error("password-rules.js requiere jQuery.");
        return;
    }

    function setRule(ruleId, ok) {
        const el = $("#" + ruleId);
        el.toggleClass("ok", !!ok);
    }

    function getPasswordValidation(pwd) {
        pwd = pwd || "";

        const hasDigit = /[0-9]/.test(pwd);
        const hasSpecial = /[!@#$%^&*]/.test(pwd);
        const hasUpper = /[A-ZÁÉÍÓÚÑ]/.test(pwd);
        const noRepeatConsecutive = !/(.)\1/.test(pwd);
        const minLen = pwd.length >= 8;

        return {
            hasDigit,
            hasSpecial,
            hasUpper,
            noRepeatConsecutive,
            minLen,
            isValid: hasDigit && hasSpecial && hasUpper && noRepeatConsecutive && minLen
        };
    }

    function evalPasswordRules(pwd) {
        const result = getPasswordValidation(pwd);

        setRule("rule-digit", result.hasDigit);
        setRule("rule-special", result.hasSpecial);
        setRule("rule-upper", result.hasUpper);
        setRule("rule-no-repeat", result.noRepeatConsecutive);
        setRule("rule-length", result.minLen);

        return result.isValid;
    }

    function showRules() {
        $("#pwdRules").removeClass("d-none").addClass("pwd-rules-visible");
    }

    function hideRules() {
        $("#pwdRules").addClass("d-none").removeClass("pwd-rules-visible");
    }

    function initPasswordUI() {
        const $form = $("#account");
        const $pwd = $("#userPassword");
        const $btn = $("#btnTogglePwd");

        if ($pwd.length === 0) return;

        hideRules();

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

        $pwd.off("input.passwordRules").on("input.passwordRules", function () {
            const isValid = evalPasswordRules($(this).val());

            if ($("#pwdRules").hasClass("pwd-rules-visible")) {
                if (isValid) {
                    hideRules();
                } else {
                    showRules();
                }
            }
        });

        $form.off("submit.passwordRules").on("submit.passwordRules", function (e) {
            const pwd = $pwd.val() || "";
            const isValid = evalPasswordRules(pwd);

            if (!isValid) {
                e.preventDefault();
                showRules();
                $pwd.trigger("focus");
                return false;
            }

            if (typeof showLoading === "function") {
                showLoading();
            }
        });
    }

    $(document).ready(initPasswordUI);

})();