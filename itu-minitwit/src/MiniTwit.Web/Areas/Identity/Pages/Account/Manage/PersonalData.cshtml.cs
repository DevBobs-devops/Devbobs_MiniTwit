// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Chirp.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<Author> _userManager;

        public PersonalDataModel(UserManager<Author> userManager, ILogger<PersonalDataModel> logger)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            var author = await _userManager.GetUserAsync(User);
            if (author == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
