
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;

// https://stackoverflow.com/questions/44115248/ms-graph-api-c-sharp-add-user-to-group
// https://stackoverflow.com/questions/46467617/create-a-group-in-microsoft-graph-api-with-a-owner
// https://github.com/microsoftgraph/aspnet-snippets-sample/blob/3a11e44fe9fd8f20d84e2dd0762d71ab22f229d4/SnippetsApp/Controllers/BaseController.cs
// https://github.com/microsoftgraph/aspnet-snippets-sample/blob/main/SnippetsApp/Controllers/GroupsController.cs
// https://github.com/microsoftgraph/msgraph-sdk-dotnet
namespace SelfServicePortal.Core.Models.Workflows;

public class CreateSecurityGroupWorkflow : Workflow {
	public override string Id => nameof(CreateSecurityGroupWorkflow);

	public string SecurityGroupName {get; init;}

	public override async Task Complete(IServiceProvider services)
	{
		var graphClient = services.GetRequiredService<GraphServiceClient>();
		var group = new Group
		{
			DisplayName = SecurityGroupName,
			MailNickname = SecurityGroupName.Replace(" ","-"),
			SecurityEnabled = true,
			MailEnabled = false,
		};
		await graphClient.Groups.Request().AddAsync(group);
	}
}