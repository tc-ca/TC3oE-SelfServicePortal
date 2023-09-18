using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Extensions.Logging;
using OurAzure.Api.Services;
using Azure.Core;

namespace SelfServicePortal.Core.Models.Workflows;

// security group does not exist at the time of this workflow being queued
// this makes it hard to reference the new security group by ID since we don't have a variable/output system for workflows
// so instead, getting it by display name should be enough for now
public class CreateSecurityGroupRoleAssignmentWorkflow : Workflow {
	public override string Id => nameof(CreateSecurityGroupRoleAssignmentWorkflow);

	public string SecurityGroupName {get; init;}
	public string Scope {get; init;}
	public string RoleName {get; init;}

	public override async Task Complete(IServiceProvider services)
	{
		var logger = services.GetRequiredService<ILogger<CreateSecurityGroupRoleAssignmentWorkflow>>();
		var graphClient = services.GetRequiredService<GraphServiceClient>();
		var restClient = services.GetRequiredService<AzureRestClient>();

		var scope = new ResourceIdentifier(Scope);

		var groupId = await graphClient.Groups.GetId(SecurityGroupName);
		if (groupId == null) 
		{
			logger.LogWarning("Could not find group id for \"{0}\"", SecurityGroupName);
			return;
		}

		var role = await restClient.GetRoleDefinitionAsync(scope.SubscriptionId!, RoleName);
		
		var maxAttempts = 10;
		for (int i=1; i<=maxAttempts; i++) {
			try {
				await restClient.CreateRoleAssignmentAsync(Scope, groupId, role.id, "Group");
				break;
			} catch (Exception e)
			{
				if (i == maxAttempts)
				{
					throw;
				} 
				else 
				{
					logger.LogWarning("Failed to create role assignment, waiting before trying again.\n{0}", e);
					System.Threading.Thread.Sleep(TimeSpan.FromSeconds(20));
				}
			}
		}
	}
}