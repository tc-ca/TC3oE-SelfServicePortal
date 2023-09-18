using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SelfServicePortal.Core.Models.Workflows;
using SelfServicePortal.Web.Services;

namespace SelfServicePortal.Web.Pages.Forms;

public class AddGroupOwners : PageModel
{
	private readonly Services<AddGroupOwners> _services;

	public AddGroupOwners(
		Services<AddGroupOwners> services
	)
	{
		_services = services;
	}

	[BindProperty]
	public string TicketId { get; set; }

	[BindProperty]
	public string GroupName { get; set; }

	[BindProperty]
	public string OwnerEmails { get; set; }

	public async Task<IActionResult> OnPost()
	{
		await foreach (var error in _services.ValidationHelper.GetSecurityGroupLookupValidationErrors(GroupName))
		{
			ModelState.AddModelError(nameof(GroupName), error);
		}

		await foreach (var error in _services.ValidationHelper.GetEmailLookupValidationErrors(OwnerEmails))
		{
			ModelState.AddModelError(nameof(OwnerEmails), error);
		}

		if (!ModelState.IsValid) return Page();

		var request = new WorkflowRequest()
		{
			Title = $"Add group owners - \"{GroupName}\"",
			DateInitiated = DateTime.Now,
			Initiator = User.GetName(),
			TicketId = TicketId,
			Workflows = new Workflow[] {
				new AddSecurityGroupOwnersWorkflow()
				{
					SecurityGroupName = GroupName,
					SecurityGroupOwnerEmails = OwnerEmails.SplitEmails()
				}
			}
		};
		await _services.WorkflowClient.Enqueue(request);
		return this.RedirectToPageSameCulture("/Success");
	}
}