using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OurAzureDevops.Models.UserEntitlements;
using Azure.Core;
using SelfServicePortal.Web.Services;
using SelfServicePortal.Core.Models.Workflows;

namespace SelfServicePortal.Web.Pages.Forms;

public class NewServiceConnection : PageModel
{
	private readonly Services<NewServiceConnection> _services;

	//todo: fix localization
	public NewServiceConnection( Services<NewServiceConnection> services )
	{
		_services = services;
	}

	[BindProperty]
	public string TicketId { get; set; }

	[BindProperty]
	public string ProjectId { get; set; }

	[BindProperty]
	public string ResourceGroupId { get; set; }


	[BindProperty]
	public string? ServiceConnectionName { get; set; }

	public ProjectEntitlement[] Projects { get; private set; }

	public string[] ResourceGroupIds { get; private set; }

	public async Task OnGet()
	{
		Projects = (await _services.AzureDevopsRestClient.GetProjectsForUser(User.GetName())).ToArray();
		var objectId = Request.HttpContext.User.Claims.First(x => x.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
		var Subscriptions = _services.ArmClient.GetSubscriptions().ToArray();
		ResourceGroupIds = await _services.AzureRestClient.GetResoruceGroupIdsForUser(
			objectId,
			Subscriptions.Select(x => x.Id.ToString()).ToArray()
		).ToArrayAsync();
	}

	public async Task<IActionResult> OnPost()
	{
		await OnGet();

		var rgId = new ResourceIdentifier(ResourceGroupId);

		// Ensure project exists
		var project = Projects.First(p => p.projectRef.id == ProjectId);
		if (project == null)
			ModelState.AddModelError(nameof(ProjectId), _services.Localizer["Error-ProjectNotFound"]);

		// Ensure subscription exists
		var sub = await _services.ArmClient.GetSubscriptionResource(new ResourceIdentifier("/subscriptions/" + rgId.SubscriptionId)).GetAsync();
		if (sub == null)
			ModelState.AddModelError(nameof(ResourceGroupId), _services.Localizer["Error-SubscriptionNotFound"]);

		// Ensure resource group exists
		await foreach (var error in _services.ValidationHelper.GetResourceGroupLookupValidationErrors(sub!, rgId.Name))
			ModelState.AddModelError(nameof(ResourceGroupId), error);
		
		if (string.IsNullOrWhiteSpace(ServiceConnectionName))
			ServiceConnectionName = rgId.Name;

		if (!ModelState.IsValid) return Page();

		var request = new WorkflowRequest()
		{
			Title = $"New Service Connection - \"{rgId.Name}\"",
			DateInitiated = DateTime.Now,
			Initiator = User.GetName(),
			TicketId = TicketId,
			Workflows = new Workflow[] {
				new CreateServiceConnectionWorkflow()
				{
					ProjectId = ProjectId,
					ProjectName = project!.projectRef.name,
					TenantId = _services.Config.AzureAd.TenantId,
					SubscriptionId = rgId.SubscriptionId!,
					SubscriptionName = sub!.Value.Data.DisplayName,
					ServiceConnectionName = ServiceConnectionName,
					PrincipalDisplayName = "[DevOps] " + rgId.ResourceGroupName + " Service Connection",
					Scope = ResourceGroupId,
				}
			}
		};

		await _services.WorkflowClient.Enqueue(request);
		return this.RedirectToPageSameCulture("/Success");
	}
}