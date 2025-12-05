// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Localization;
using ERPSEI.Email;
using ERPSEI.Data.Entities.Empleados;
using ERPSEI.Data.Managers.Empleados;
using ERPSEI.Data.Entities.Usuarios;
using ERPSEI.Data.Managers.Usuarios;

namespace ERPSEI.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmpleadoManager _empleadoManager;
        private readonly AppUserManager _userManager;
        private readonly IUserStore<AppUser> _userStore;
        private readonly IUserEmailStore<AppUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IStringLocalizer<RegisterModel> _localizer;


        public RegisterModel(
            IEmpleadoManager empleadoManager,
            AppUserManager userManager,
            IUserStore<AppUser> userStore,
            SignInManager<AppUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IStringLocalizer<RegisterModel> localizer)
        {
            _empleadoManager = empleadoManager;
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
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
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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
            [StringLength(100, ErrorMessage = "FieldLength", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "PasswordField")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "ConfirmPasswordField")]
            [Compare("Password", ErrorMessage = "FieldsComparison")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        /*public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                Empleado empleado = await _empleadoManager.GetByEmailAsync(Input.Email);

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                //Si ya existe empleado.
                if(empleado != null)
                {
					//Se considera que el preregistro ya está autorizado
					user.IsPreregisterAuthorized = true;
                    //Se liga el empleado al usuario.
                    user.EmpleadoId = empleado.Id;
                }

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("El usuario creó una nueva cuenta con contraseña.");

                    var userId = await _userManager.GetUserIdAsync(user);

                    //Si existe empleado que tenga el correo asignado
                    if (empleado != null) {
						//Se agrega la cuenta de usuario como usuario.
						await _userManager.AddToRoleAsync(user, ServicesConfiguration.RolUsuario);

						//Liga al usuario creado con el empleado existente.
						empleado.UserId = userId;
						await _empleadoManager.UpdateAsync(empleado);
					}
                    else
                    {
						//Se agrega la cuenta de usuario como candidato.
						await _userManager.AddToRoleAsync(user, ServicesConfiguration.RolCandidato);
					}

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    string emailBody = $"{_localizer["EmailBodyFP"]} <a href='{callbackUrl}'>{_localizer["EmailBodySP"]}</a>";

                    _emailSender.SendEmailAsync(Input.Email, _localizer["EmailSubject"], emailBody);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    string localizedError = _localizer[error.Code];
                    ModelState.AddModelError(string.Empty, localizedError);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }*/

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                Empleado empleado = await _empleadoManager.GetByEmailAsync(Input.Email);

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // Si ya existe empleado
                if (empleado != null)
                {
                    user.IsPreregisterAuthorized = true;
                    user.EmpleadoId = empleado.Id;
                }

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("El usuario creó una nueva cuenta con contraseña.");

                    var userId = await _userManager.GetUserIdAsync(user);

                    // CONFIRMAR EMAIL AUTOMÁTICAMENTE 
                    // ---------------------------------------------------------------
                    var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    await _userManager.ConfirmEmailAsync(user, confirmToken);
                    // ---------------------------------------------------------------

                    // Asignar roles dependiendo si existe empleado
                    if (empleado != null)
                    {
                        await _userManager.AddToRoleAsync(user, ServicesConfiguration.RolUsuario);

                        empleado.UserId = userId;
                        await _empleadoManager.UpdateAsync(empleado);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, ServicesConfiguration.RolCandidato);
                    }

                    // BLOQUE ANTERIOR PARA ENVIAR CORREO — YA NO ES NECESARIO
                    // -------------------------------------------------------------------
                    /*
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    string emailBody = $"{_localizer["EmailBodyFP"]} <a href='{callbackUrl}'>{_localizer["EmailBodySP"]}</a>";
                    _emailSender.SendEmailAsync(Input.Email, _localizer["EmailSubject"], emailBody);
                    */
                    // -------------------------------------------------------------------

                    // TAMPOCO ES NECESARIO VERIFICAR "RequireConfirmedAccount"
                    // IDENTIDAD YA SABE QUE EL EMAIL ESTÁ CONFIRMADO
                    // -------------------------------------------------------------------
                    /*
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    */
                    // -------------------------------------------------------------------

                    // INICIO DE SESIÓN INMEDIATO
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    string localizedError = _localizer[error.Code];
                    ModelState.AddModelError(string.Empty, localizedError);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


        private AppUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<AppUser>();
            }
            catch
            {
                throw new InvalidOperationException($"No se puede crear una instancia de '{nameof(AppUser)}'. " +
                    $"Asegurese que '{nameof(AppUser)}' no es una clase abstracta y tiene un constructor sin parámetros, o alternativamente " +
                    $"sobrecargue la página de registro en /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<AppUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("La UI default requiere almacenar un usuario con correo electrónico.");
            }
            return (IUserEmailStore<AppUser>)_userStore;
        }
    }
}
