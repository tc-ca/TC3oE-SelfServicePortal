using SelfServicePortal.Web.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;

namespace SelfServicePortal.Web.Pages;

[Authorize(Policy = AuthorizationPolicies.ApplicationAdministrator)]
public class Test : PageModel
{
	private readonly IAuthorizationService _auth;
	private readonly GraphServiceClient _graphClient;

	public Test(IAuthorizationService auth, GraphServiceClient graphClient)
	{
		_auth = auth;
		_graphClient = graphClient;
	}

	[BindProperty]
	public string MyInput {get; set;}

	public async Task OnGetAsync()
	{
		MyInput = (await _auth.AuthorizeAsync(User, AuthorizationPolicies.ApplicationAdministrator)).Succeeded ? "Admin" : "User";
		await Task.CompletedTask;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		await Task.CompletedTask;
		return Page();
	}
}