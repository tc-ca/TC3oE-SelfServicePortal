using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authorization;
using SelfServicePortal.Web.Auth;

namespace SelfServicePortal.Web.Pages;

public class IndexModel : PageModel
{
	public bool IsDev;
	public bool IsAdmin;
	private readonly IAuthorizationService _auth;

	public IndexModel(IWebHostEnvironment env, IAuthorizationService auth)
	{
		IsDev = env.IsDevelopment();
		_auth = auth;
	}
	public async Task<IActionResult> OnGet()
	{
		// If user landed on index page from `/` instead of `/en/`
		if (Request.RouteValues["culture"] == null)
		{
			// Grab the backend fallback culture
			var fallback = Request.GetCulture();

			// Redirect to get the culture in the browser URI
			return Redirect($"~/{fallback}/");
		}

		IsAdmin = (await _auth.AuthorizeAsync(User, AuthorizationPolicies.ApplicationAdministrator)).Succeeded;
		return Page();
	}
}
