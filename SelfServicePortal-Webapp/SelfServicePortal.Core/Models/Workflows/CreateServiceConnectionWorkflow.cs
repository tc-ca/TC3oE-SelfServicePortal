using Azure.Core;
using Azure.ResourceManager;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using OurAzure.Api.Services;
using OurAzureDevops;

namespace SelfServicePortal.Core.Models.Workflows;

public class CreateServiceConnectionWorkflow : Workflow
{
	public override string Id => nameof(CreateServiceConnectionWorkflow);

	public string ProjectId { get; init; }
	public string ProjectName { get; init; }
	public string TenantId { get; init; }
	public string SubscriptionId { get; init; }
	public string SubscriptionName { get; init; }
	public string ServiceConnectionName { get; init; }
	public string PrincipalDisplayName {get; init;}
	public string Scope { get; init; }

	public override async Task Complete(IServiceProvider services)
	{
		var logger = services.GetRequiredService<ILogger<CreateServiceConnectionWorkflow>>();
		logger.LogInformation("gathering services");

		var devopsClient = services.GetRequiredService<AzureDevopsRestClient>();
		var graphClient = services.GetRequiredService<GraphServiceClient>();
		var armClient = services.GetRequiredService<ArmClient>();
		var restClient = services.GetRequiredService<AzureRestClient>();

		logger.LogInformation("creating service principal");
		var app = await graphClient.Applications.Request().AddAsync(new(){
			DisplayName = PrincipalDisplayName,
		});
		var sp = await graphClient.ServicePrincipals.Request().AddAsync(new(){
			AppId = app.AppId,
			DisplayName = app.DisplayName,
		});

		logger.LogInformation("adding service principal credential");
		var cred = new PasswordCredential{
			DisplayName = "SelfServicePortal instantiated credential",
		};
		cred = await graphClient.ServicePrincipals[sp.Id].AddPassword(cred).Request().PostAsync();

		logger.LogInformation("creating role assignment");
		var role = await restClient.GetRoleDefinitionAsync(new ResourceIdentifier(Scope).SubscriptionId!, "Contributor");
		await restClient.CreateRoleAssignmentAsync(Scope, sp.Id, role.id, "ServicePrincipal");

		logger.LogInformation("creating service connection");
		await devopsClient.CreateManualServiceConnection(
			ProjectId,
			ProjectName,
			TenantId,
			SubscriptionId,
			SubscriptionName,
			ServiceConnectionName,
			sp.AppId,
			cred.SecretText,
			Scope
		);
	}
}