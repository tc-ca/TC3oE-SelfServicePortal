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

public class NewSecurityGroup : PageModel
{
    private readonly Services<NewSecurityGroup> _services;

    public NewSecurityGroup(Services<NewSecurityGroup> services)
    {
        _services = services;
    }
    public ProjectEntitlement[] Projects { get; private set; }

    [BindProperty]
    public string TicketId { get; set; }
    [BindProperty]
    public string ResourceGroupName { get; set; }

    [BindProperty]
    public string NewSecurityGroupName { get; set; }
    [BindProperty]
    public string SecurityGroupRole { get; set; }
    [BindProperty]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string SecurityGroupMembers { get; set; }
    [BindProperty]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string SecurityGroupOwners { get; set; }

    public async Task OnGetAsync()
    {
        Projects = (await _services.AzureDevopsRestClient.GetProjectsForUser(User.GetName())).ToArray();
    }


    public async Task<IActionResult> OnPostAsync()
    {
        // validate security group
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
    
        // The assigned RG must in these 3 subsciptions
 		SubscriptionResource[] Subscriptions = _services.ArmClient.GetSubscriptions().GetAll().Where(p => (p.Data.DisplayName.ToUpper() == "SUB1" || p.Data.DisplayName.ToUpper() == "SUB2" || p.Data.DisplayName.ToUpper() == "SUB3")).ToArray();
        await foreach (var error in _services.ValidationHelper.GetResourceGroupNameInputVailidationErrors(ResourceGroupName,Subscriptions))
        {
            ModelState.AddModelError(nameof(ResourceGroupName), error);
        }

        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }


        var members = SecurityGroupMembers.SplitEmails();
        var owners = SecurityGroupOwners.SplitEmails();
        List<Workflow> workflows = new List<Workflow> {
            new CreateSecurityGroupWorkflow()
                {
                    SecurityGroupName = NewSecurityGroupName,
                },
        };
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

        if (ResourceGroupName.Length > 0)
        {
            foreach(var sub in Subscriptions)
            {
                if (sub.GetResourceGroups().Exists(ResourceGroupName))
                {
                    workflows.Add(new CreateSecurityGroupRoleAssignmentWorkflow()
                    {
                        RoleName = SecurityGroupRole,
                        Scope = $"/subscriptions/{sub.Id.SubscriptionId}/resourceGroups/{ResourceGroupName}",
                        SecurityGroupName = NewSecurityGroupName,
                    });
                }
            }
        }

        var request = new WorkflowRequest()
        {
            Title = $"New Security Group - \"{NewSecurityGroupName}\"",
            DateInitiated = DateTime.Now,
            Initiator = User.GetName(),
            TicketId = TicketId,
            Workflows = workflows.ToArray(),
        };
        await _services.WorkflowClient.Enqueue(request);

        return this.RedirectToPageSameCulture("/Success");
    }
}