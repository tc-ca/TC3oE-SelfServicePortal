using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Extensions.Logging;

namespace SelfServicePortal.Core.Models.Workflows;

public class AddSecurityGroupOwnersWorkflow : Workflow
{
	public override string Id => nameof(AddSecurityGroupOwnersWorkflow);

	public string SecurityGroupName { get; init; }
	public string[] SecurityGroupOwnerEmails { get; init; }

	public override async Task Complete(IServiceProvider services)
	{
		var graphClient = services.GetRequiredService<GraphServiceClient>();
		var logger = services.GetRequiredService<ILogger<AddSecurityGroupOwnersWorkflow>>();
		var groupId = await graphClient.Groups.GetId(SecurityGroupName);
		if (groupId == null) {
			throw new ArgumentException($"Group {SecurityGroupName} not found");
		}
		
		await foreach (var user in graphClient.Users.GetFromEmails(SecurityGroupOwnerEmails))
		{
			try
			{
				await graphClient.Groups[groupId].Owners.References.Request().AddAsync(user);
			}
			catch (Exception e)
			{
				logger.LogWarning("Failed to add owner {0} to group {1}: {2}", user, SecurityGroupName, e);
			}
		}
	}
}