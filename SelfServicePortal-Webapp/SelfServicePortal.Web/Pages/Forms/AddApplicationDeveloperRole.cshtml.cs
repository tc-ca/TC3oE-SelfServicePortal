using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SelfServicePortal.Web.Services;
using SelfServicePortal.Core.Models.Workflows;

namespace SelfServicePortal.Web.Pages.Forms;

public class AddApplicationDeveloperRole : PageModel
{
	private readonly Services<AddApplicationDeveloperRole> _services;

	public AddApplicationDeveloperRole(
		Services<AddApplicationDeveloperRole> services
	)
	{
		_services = services;
	}

	[BindProperty]
	public string TicketId { get; set; }

	[BindProperty]
	public string Emails { get; set; }

	public async Task<IActionResult> OnPost()
	{
		await foreach (var error in _services.ValidationHelper.GetEmailLookupValidationErrors(Emails))
		{
			ModelState.AddModelError(nameof(Emails), error);
		}

		if (!ModelState.IsValid) return Page();

		var request = new WorkflowRequest()
		{
			Title = $"Add Application Developer Role",
			DateInitiated = DateTime.Now,
			Initiator = User.GetName(),
			TicketId = TicketId,
			Workflows = new Workflow[] {
				new AddSecurityGroupMembersWorkflow()
				{
					SecurityGroupName = "MyADGroupHere",
					SecurityGroupMemberEmails = Emails.SplitEmails()
				}
			}
		};
		await _services.WorkflowClient.Enqueue(request);
		return this.RedirectToPageSameCulture("/Success");
	}
}