// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using ERPSEI.Data.Entities;
using ERPSEI.Data.Entities.Usuarios;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ERPSEI.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<AppUser> signInManager, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            HttpContext.Session.Clear();

            _logger.LogInformation(
                "User logged out."
            );

            if (
                !string.IsNullOrWhiteSpace(returnUrl) &&
                Url.IsLocalUrl(returnUrl) &&
                !returnUrl.StartsWith(
                    "/Identity/Account/Logout",
                    StringComparison.OrdinalIgnoreCase
                )
            )
            {
                return LocalRedirect(
                    returnUrl
                );
            }

            return LocalRedirect(
                "~/Identity/Account/Login"
            );
        }
    }
}
