// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.Extensions.Localization;
using ERPSEI.Data.Managers.Usuarios;

namespace ERPSEI.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly AppUserManager _userManager;
        private readonly IStringLocalizer<ResetPasswordModel> _localizer;

        public ResetPasswordModel(
            AppUserManager userManager,
            IStringLocalizer<ResetPasswordModel> localizer)
        {
            _userManager = userManager;
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
            /*[Required(ErrorMessage = "Required")]
            [StringLength(100, ErrorMessage = "FieldLength", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "PasswordField")]
            public string Password { get; set; }*/
            [Required(ErrorMessage = "Required")]
            [StringLength(100, ErrorMessage = "FieldLength", MinimumLength = 8)]
            [RegularExpression(@"^(?=.*[A-ZÁÉÍÓÚÑ])(?=.*\d)(?=.*[!@#$%^&*]).{8,}$", ErrorMessage = "PasswordComplexity")]
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

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string Code { get; set; }

        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest(_localizer["CodeRequestMessage"]);
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                user.PasswordResetNeeded = false;
                await _userManager.UpdateAsync(user);

                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                string localizedError = _localizer[error.Code];
                ModelState.AddModelError(string.Empty, localizedError);
            }
            return Page();
        }
    }
}
