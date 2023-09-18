using SelfServicePortal.Web.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using SelfServicePortal.Web.Services;
using SelfServicePortal.Core.Models.Workflows;

namespace SelfServicePortal.Web.Pages;

[Authorize(Policy = AuthorizationPolicies.ApplicationAdministrator)]
public class JsonFormModel : PageModel
{
	private readonly Services<JsonFormModel> _services;

	[BindProperty]
	public string Data {get; set;}
	public JsonFormModel(Services<JsonFormModel> services)
	{
		_services = services;
	}

	public void OnGet()
	{
		Data = new WorkflowRequest(){
			DateInitiated = DateTime.Now,
			Title = "JSON Workflow Request",
			Initiator = User.GetName(),
			TicketId = "none",
			Workflows = new Workflow[]{}
		}.Serialize();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		try {
			await _services.WorkflowClient.Enqueue(WorkflowRequest.Deserialize(Data));
		} catch (Exception e)
		{
			ModelState.AddModelError(nameof(Data), e.ToString());
			return Page();
		}
		return this.RedirectToPageSameCulture("/Success");
	}
}