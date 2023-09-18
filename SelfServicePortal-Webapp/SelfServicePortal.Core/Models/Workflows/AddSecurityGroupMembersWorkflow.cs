using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace SelfServicePortal.Core.Models.Workflows;

public class AddSecurityGroupMembersWorkflow : Workflow
{
	public override string Id => nameof(AddSecurityGroupMembersWorkflow);

	public string SecurityGroupName { get; init; }
	public string[]? SecurityGroupMemberEmails { get; init; }

	public string[]? MemberGroupNames {get; init;}

	public override async Task Complete(IServiceProvider services)
	{
		var graphClient = services.GetRequiredService<GraphServiceClient>();
		var logger = services.GetRequiredService<ILogger<AddSecurityGroupMembersWorkflow>>();
		var groupId = await graphClient.Groups.GetId(SecurityGroupName);
		if (groupId == null) {
			throw new ArgumentException($"Group {SecurityGroupName} not found");
		}
		
		await foreach (var user in graphClient.Users.GetFromEmails(SecurityGroupMemberEmails ?? new string[]{}))
		{
			try
			{
				await graphClient.Groups[groupId].Members.References.Request().AddAsync(user);
			}
			catch (Exception e)
			{
				logger.LogWarning("Failed to add member {0} to group {1}: {2}", user, SecurityGroupName, e.ToString());
			}
		}

		foreach (var groupName in MemberGroupNames ?? new string[]{})
		{
			var found = await graphClient.Groups.Request().Filter($"displayName eq '{groupName}'").GetAsync();
			if (found.Count < 1)
			{
				logger.LogWarning("Failed to find group with name {0}", groupName);
			}
			else
			{
				try
				{
					await graphClient.Groups[groupId].Members.References.Request().AddAsync(found.First());
				}
				catch (Exception e)
				{
					logger.LogWarning("Failed to add member {0} to group {1}: {2}", groupName, SecurityGroupName, e.ToString());
				}
			}
		}
	}
}