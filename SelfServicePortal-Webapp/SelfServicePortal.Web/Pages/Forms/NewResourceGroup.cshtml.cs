using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SelfServicePortal.Core.Models.Workflows;
using SelfServicePortal.Web.Services;
using Azure.Core;
using Azure.ResourceManager.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OurAzureDevops.Models.UserEntitlements;

namespace SelfServicePortal.Web.Pages.Forms;

public class NewResourceGroup : PageModel
{
	public readonly AzureLocation[] ValidLocations = CreateResourceGroupWorkflow.ValidLocations;
	private readonly Services<NewResourceGroup> _services;

	public NewResourceGroup(Services<NewResourceGroup> services)
	{
		_services = services;
	}
	public SubscriptionResource[] Subscriptions { get; private set; }


	public ProjectEntitlement[] Projects { get; private set; }

	[BindProperty]
	public string TicketId { get; set; }

	[BindProperty]
	public string SubscriptionName { get; set; }
	[BindProperty]
	public string LocationName { get; set; }

	[BindProperty]
	public string ResourceGroupName { get; set; }
	[BindProperty]
	public Dictionary<string, string> ResourceGroupTags { get; set; }

	
	[BindProperty]
	public bool ShouldAssignSecurityGroup { get; set; } = true;
	[BindProperty]
	[DisplayFormat(ConvertEmptyStringToNull = false)]
	public string ExistingSecurityGroupName { get; set; }
	[BindProperty]
	[DisplayFormat(ConvertEmptyStringToNull = false)]
	public string NewSecurityGroupName { get; set; }
	[BindProperty]
	public bool SecurityGroupExists { get; set; }

	[BindProperty]
	[DisplayFormat(ConvertEmptyStringToNull = false)]
	public string SecurityGroupMembers { get; set; }
	[BindProperty]
	[DisplayFormat(ConvertEmptyStringToNull = false)]
	public string SecurityGroupOwners { get; set; }

	[BindProperty]
	public bool ShouldCreateServiceConnection { get; set; }

	// not bound since we don't actually use the user input
	// still needed to make form validation work properly
	public bool ShouldCostRecovery { get; set; }

	[BindProperty]
	[DisplayFormat(ConvertEmptyStringToNull = false)]
	public string ServiceConnectionName { get; set; }

	[BindProperty]
	[DisplayFormat(ConvertEmptyStringToNull = false)]
	public string ServiceConnectionProjectId { get; set; }

	public async Task OnGetAsync()
	{
		Subscriptions = _services.ArmClient.GetSubscriptions().GetAll().ToArray();

		Projects = (await _services.AzureDevopsRestClient.GetProjectsForUser(User.GetName())).ToArray();
		if (_services.WebHostEnvironment.IsDevelopment()) // pre-fill for testing
		{
			_services.Logger.LogDebug("Pre-filling debug info");
			TicketId = "debug";
			SubscriptionName = "MySubscription";
			LocationName = "canadacentral";
			ResourceGroupName = "TEST-PLS-DELETE";
			ResourceGroupTags = new Dictionary<string, string> {
				{"Business-Unit", "Unit1"},
				{"Business-Value", "Business Critical"},
				{"Environment", "Development"},
				{"Project-Name", "MyProject"},
				{"Solution-Name", "MyProject"},
				{"Sensitivity", "Unclassified"},
				{"RC-Manager-Name", "Joe Schmoe"},
				{"RC-Manager-Email", "joe.schmoe@org.gc.ca"},
				{"Tech-Contact-Name", "Joe Schmoe"},
				{"Tech-Contact-Email", "joe.schmoe@org.gc.ca"},
				{"Tech-Owner-Name", "Jane Schmoe"},
				{"Tech-Owner-Email", "jane.schmoe@org.gc.ca"},
				{"Cost-Center-Code", "Cost-Center-Code"},
				{"Cost-Center-Name", "Cost-Center-Name"},
			};
			ShouldAssignSecurityGroup = true;
			ShouldCostRecovery = true;
			ExistingSecurityGroupName = "";
			NewSecurityGroupName = "mytestsecgroup";
			SecurityGroupOwners = "joe.schmoe@org.gc.ca";
			SecurityGroupMembers = "";
			SecurityGroupExists = false;
		}
	}


	public async Task<IActionResult> OnPostAsync()
	{
		// Validate subscription and location
		await foreach (var error in _services.ValidationHelper.GetSubscriptionValidationErrors(SubscriptionName))
		{
			ModelState.AddModelError(nameof(SubscriptionName), error);
		}
		await foreach (var error in _services.ValidationHelper.GetLocationValidationErrors(ValidLocations, LocationName))
		{
			ModelState.AddModelError(nameof(LocationName), error);
		}
		await foreach (var error in _services.ValidationHelper.GetSCEDLocationValidationErrors(SubscriptionName, LocationName))
		{
			ModelState.AddModelError(nameof(LocationName), error);
		}
		if (!ModelState.IsValid)
		{
			await OnGetAsync();
			return Page();
		}

		// validate resource group name
		var sub = await _services.ArmClient.GetSubscriptionByName(SubscriptionName);
		var location = ValidLocations.First(l => l.Name == LocationName);
		ResourceGroupName = GetResourceGroupName(sub, location, ResourceGroupName);
		await foreach (var error in _services.ValidationHelper.GetResourceGroupNameAvailableValidationErrors(sub, ResourceGroupName))
		{
			ModelState.AddModelError(
				nameof(ResourceGroupName),
				error
			);
		}

		// validate security group
		if (ShouldAssignSecurityGroup)
		{
			if (SecurityGroupExists)
			{
				// Client wants to use an existing group
				await foreach (var error in _services.ValidationHelper.GetSecurityGroupLookupValidationErrors(ExistingSecurityGroupName))
				{
					ModelState.AddModelError(nameof(ExistingSecurityGroupName), error);
				}
			}
			else
			{
				// Client wants to create a new group
				NewSecurityGroupName = "MyOrg-" + NewSecurityGroupName;
				await foreach (var error in _services.ValidationHelper.GetSecurityGroupNameAvailableValidationErrors(NewSecurityGroupName))
				{
					ModelState.AddModelError(nameof(NewSecurityGroupName), error);
				}
				await foreach (var error in _services.ValidationHelper.GetEmailLookupValidationErrors(SecurityGroupOwners, allowEmpty: true))
				{
					ModelState.AddModelError(nameof(SecurityGroupOwners), error);
				}
				await foreach (var error in _services.ValidationHelper.GetEmailLookupValidationErrors(SecurityGroupMembers, allowEmpty: true))
				{
					ModelState.AddModelError(nameof(SecurityGroupMembers), error);
				}
			}
		}

		if (!ModelState.IsValid)
		{
			await OnGetAsync();
			return Page();
		}

		List<Workflow> workflows = new List<Workflow> {
			new CreateResourceGroupWorkflow() {
				LocationName = LocationName,
				SubscriptionName = SubscriptionName,
				ResourceGroupName = ResourceGroupName,
				ResourceGroupTags = ResourceGroupTags
			},
		};

		if (ShouldAssignSecurityGroup)
		{
			if (!SecurityGroupExists)
			{
				var members = SecurityGroupMembers.SplitEmails();
				var owners = SecurityGroupOwners.SplitEmails();
				workflows.Add(new CreateSecurityGroupWorkflow()
				{
					SecurityGroupName = NewSecurityGroupName,
				});
				if (owners.Count() > 0)
					workflows.Add(new AddSecurityGroupOwnersWorkflow()
					{
						SecurityGroupName = NewSecurityGroupName,
						SecurityGroupOwnerEmails = owners.ToArray()
					});
				if (members.Count() > 0)
					workflows.Add(new AddSecurityGroupMembersWorkflow()
					{
						SecurityGroupName = NewSecurityGroupName,
						SecurityGroupMemberEmails = members.ToArray()
					});
			}
			workflows.Add(new CreateSecurityGroupRoleAssignmentWorkflow()
			{
				RoleName = "Contributor",
				Scope = $"/subscriptions/{sub.Id.SubscriptionId}/resourceGroups/{ResourceGroupName}",
				SecurityGroupName = SecurityGroupExists ? ExistingSecurityGroupName : NewSecurityGroupName,
			});
		}
		if (ShouldCreateServiceConnection)
		{
			var project = (await _services.AzureDevopsRestClient.GetProjectsForUser(User.GetName())).Where(x => x.projectRef.id == ServiceConnectionProjectId).FirstOrDefault((ProjectEntitlement?)null);
			if (project == null)
			{
				ModelState.AddModelError(nameof(ServiceConnectionProjectId), _services.Localizer["ProjectNotFound"]);
				await OnGetAsync();
				return Page();
			}
			workflows.Add(new CreateServiceConnectionWorkflow()
			{
				ProjectId = ServiceConnectionProjectId,
				ProjectName = project.projectRef.name,
				ServiceConnectionName = string.IsNullOrWhiteSpace(ServiceConnectionName) ? ResourceGroupName : ServiceConnectionName,
				PrincipalDisplayName = "[DevOps] " + ResourceGroupName + " Service Connection",
				SubscriptionId = sub.Data.SubscriptionId,
				SubscriptionName = sub.Data.DisplayName,
				TenantId = sub.Data.TenantId!.ToString()!,
				Scope = sub.Id.ToString() + "/resourceGroups/" + ResourceGroupName
			});
		}

		var request = new WorkflowRequest()
		{
			Title = $"New Resource Group - \"{ResourceGroupName}\"",
			DateInitiated = DateTime.Now,
			Initiator = User.GetName(),
			TicketId = TicketId,
			Workflows = workflows.ToArray(),
		};
		await _services.WorkflowClient.Enqueue(request);

		return this.RedirectToPageSameCulture("/Success");
	}

	private string GetResourceGroupName(SubscriptionResource sub, AzureLocation region, string rgName)
	{
		var name = "";
		name += sub.Data.DisplayName;
		name += "-";
		name += region.GetNamingPrefix();
		name += "-";
		name += rgName;
		name += "-RGP";
		return name;
	}
}