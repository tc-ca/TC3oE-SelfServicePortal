using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using SelfServicePortal.Web.Services;
using SelfServicePortal.Core.Models.Workflows;

namespace SelfServicePortal.Web.Pages.Forms;

public class AddAccountInactivityPolicyExemption : PageModel
{
	private readonly Services<AddAccountInactivityPolicyExemption> _services;

	public AddAccountInactivityPolicyExemption(
		Services<AddAccountInactivityPolicyExemption> services
	)
	{
		_services = services;
	}

	[BindProperty]
	public string TicketId { get; set; }

	[BindProperty]
	public string MemberEmails { get; set; }
	public async Task<IActionResult> OnPost()
	{
		// don't validate group name to the client, if it's wrong the app should hard error.
		var GroupName = _services.Config.InactivityExemptionGroupName;

		// Validate all emails exist
		await foreach (var error in _services.ValidationHelper.GetEmailLookupValidationErrors(MemberEmails))
		{
			ModelState.AddModelError(nameof(MemberEmails), error);
		}

		if (!ModelState.IsValid) return Page();

		var request = new WorkflowRequest()
		{
			Title = $"Add Inactivity-Exemption members - \"{MemberEmails}\"",
			DateInitiated = DateTime.Now,
			Initiator = User.GetName(),
			TicketId = TicketId,
			Workflows = new Workflow[] {
				new AddSecurityGroupMembersWorkflow()
				{
					SecurityGroupName = GroupName,
					SecurityGroupMemberEmails = MemberEmails.SplitEmails()
				}
			}
		};

		await _services.WorkflowClient.Enqueue(request);
		return this.RedirectToPageSameCulture("/Success");
	}
}