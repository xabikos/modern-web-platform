using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Identity.Pages.Account.ConfirmEmail
{
	[SecurityHeaders]
	[AllowAnonymous]
	public class Index : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public Index(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}
		public async Task<IActionResult> OnGet(string token, string email)
        {
			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
				return RedirectToPage("Error");
			var result = await _userManager.ConfirmEmailAsync(user, token);
			return result.Succeeded ? Page() : RedirectToPage("Error");
		}
    }
}
