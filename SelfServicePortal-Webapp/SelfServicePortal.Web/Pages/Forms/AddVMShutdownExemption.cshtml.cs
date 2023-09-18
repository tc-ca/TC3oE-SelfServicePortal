using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using SelfServicePortal.Core.Models.Workflows;
using SelfServicePortal.Web.Services;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SelfServicePortal.Web.Pages.Forms;

public class AddVMShutdownExemption : PageModel
{
	private readonly Services<AddVMShutdownExemption> _services;

	public AddVMShutdownExemption(
		Services<AddVMShutdownExemption> services
	)
	{
		_services = services;
	}

	public void OnGet()
	{
		//SubscriptionCollection  class  SubscriptionResource
		Subscriptions = _services.ArmClient.GetSubscriptions().GetAll().ToArray();	
		string[] sublist = {"Sub1","Sub2","MySub3"};
		Subscriptions = (
							from item in Subscriptions
							where (Array.FindIndex(sublist, x => x == item.Data.DisplayName) > -1) 
							select item
						).ToArray();  
	}  

	[BindProperty]
	public string TicketId { get; set; }

	[BindProperty]
	public string SubscriptionName { get; set; }

	[BindProperty]
	public string ResourceGroupName { get; set; }

	[BindProperty]
	public string VMName { get; set; }

	[BindProperty]
	public string Justification { get; set; }
	public SubscriptionResource[] Subscriptions { get; private set; }

	private const string POLICY_DEFINITION_ID = "/providers/Microsoft.Management/managementGroups/5555-55555-55555-55555/providers/Microsoft.Authorization/policySetDefinitions/abc123";

	public async Task<IActionResult> OnPost()
	{
		// Validate subscription exists
		await foreach (var error in _services.ValidationHelper.GetSubscriptionValidationErrors(SubscriptionName))
		{
			ModelState.AddModelError(nameof(SubscriptionName), error);
		}
		if (!ModelState.IsValid) {
			OnGet();
			return Page();
		}
		
		// Validate resource group exists
		var sub = await _services.ArmClient.GetSubscriptionByName(SubscriptionName);
		await foreach (var error in _services.ValidationHelper.GetResourceGroupLookupValidationErrors(sub, ResourceGroupName))
		{
			ModelState.AddModelError(nameof(ResourceGroupName), error);
		}
		if (!ModelState.IsValid) {
			OnGet();
			return Page();
		}

		// Validate VM exists
		ResourceGroupResource rg = await sub.GetResourceGroups().GetAsync(ResourceGroupName);
		if (!rg.GetVirtualMachines().Exists(VMName))
		{
			ModelState.AddModelError(
				nameof(ResourceGroupName),
				_services.Localizer["Error-VMNotFound"]
			);
		}
		if (!ModelState.IsValid) {
			OnGet();
			return Page();
		}

		VirtualMachineResource vm = await rg.GetVirtualMachines().GetAsync(VMName);
		List<Workflow> workflows = new();

		// Create policy exemptions
		await foreach (var ass in _services.AzureRestClient.GetPolicyAssignmentsAsync(POLICY_DEFINITION_ID, new[] { sub.Id.SubscriptionId! }))
		{
			workflows.Add(new AddPolicyExemptionWorkflow()
			{
				PolicyId = ass.id,
				ResourceId = vm.Id.ToString()
			});
		}

		// Add tag update workflow
		workflows.Add(new AddTagsWorkflow()
		{
			ResourceId = vm.Id.ToString(),
			Tags = new Dictionary<string, string>()
			{
				{"AutoShutDown", "No"}
			}
		});

		// Create workflow request
		var request = new WorkflowRequest()
		{
			Title = $"Add VM Shutdown Exemption - {VMName}",
			DateInitiated = DateTime.Now,
			Initiator = User.GetName(),
			TicketId = TicketId,
			Workflows = workflows.ToArray()
		};

		// Submit and redirect
		await _services.WorkflowClient.Enqueue(request);
		return this.RedirectToPageSameCulture("/Success");
	}
}