// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using ERPSEI.Data.Entities;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ERPSEI.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly AppUserManager _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IStringLocalizer<LoginModel> _localizer;

        public LoginModel(
            AppUserManager userManager, 
            SignInManager<AppUser> signInManager, 
            ILogger<LoginModel> logger,
            IStringLocalizer<LoginModel> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _localizer = localizer;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Required")]
            [EmailAddress(ErrorMessage = "EmailFormat")]
            [Display(Name = "EmailField")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "Required")]
            [DataType(DataType.Password)]
            [Display(Name = "PasswordField")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "RememberField")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");

            if (_signInManager.IsSignedIn(User)) { return LocalRedirect("~/Identity/Account/Logout"); }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(_userManager.NormalizeEmail(Input.Email));
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, _localizer["LoginError"]);
                    return Page();
                }
                else if (user.IsBanned)
                {
                    //Si el usuario está baneado, restringe acceso.
                    _logger.LogWarning(_localizer["AccountBanned"]);
                    return RedirectToPage("./Lockout");
                }
                else if (user.EmailConfirmed && user.PasswordResetNeeded)
                {
                    //Si el usuario requiere resetear su password, redirige a pantalla de reset de password.
                    _logger.LogWarning(_localizer["ResetPasswordRequired"]);

					var code = await _userManager.GeneratePasswordResetTokenAsync(user);
					code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
					return RedirectToPage($"./ResetPassword", new { code = code });
                }
                else
                {
                    //De lo contrario se le permite el intento de iniciar sesión.
                    var result = await _signInManager.PasswordSignInAsync(user.Email, Input.Password, Input.RememberMe, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
						if (!await _userManager.IsInRoleAsync(user, ServicesConfiguration.RolMaster) && !user.EmpleadoId.HasValue)
						{
							//Si el usuario no está en el rol de Master y además no tiene empleado vinculado, entonces redirige a pantalla de preregistro.
							_logger.LogWarning(_localizer["PreregisterRequired"]);
							return RedirectToPage("./Manage/Index");
						}
                        else if (user.EmpleadoId.HasValue && !user.IsPreregisterAuthorized)
                        {
                            //Si el usuario ya cuenta con registro de empleado pero el preregistro no ha sido autorizado, entonces redirige a pantalla de espera de autorización.
                            _logger.LogWarning(_localizer["PendingUserAuthorization"]);
                            return RedirectToPage("./PendingUserAuthorization");
                        }
                        else
                        {
                            return LocalRedirect("/");
                        }
                    }
                    if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning(_localizer["AccountBanned"]);
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, _localizer["LoginError"]);
                        return Page();
                    }
                }
            }

            //Si el código llegó hasta este punto, algo salió mal. Vuelve a mostrar la pantalla de inicio de sesión.
            return Page();
        }
    }
}
