using Azure.ResourceManager.Resources;
using SelfServicePortal.Core.Models.Workflows;
using Xunit.Abstractions;


namespace SelfServicePortal.Test;

public class WorkflowTests : BaseTest
{
	public WorkflowTests(ITestOutputHelper helper) : base(helper) {}
	
	[Fact]
	public async Task ServiceConnectionWorkflow()
	{
		var sub = await Services.ArmClient.GetSubscriptionByName("MySubscriptionHere");
		ResourceGroupResource rg = await sub.GetResourceGroupAsync("MyResourceGroup");
		var scope = rg.Id.ToString();
		var wf = new CreateServiceConnectionWorkflow()
		{
			PrincipalDisplayName = "SelfServicePortal Test Connection",
			ProjectId = "55555-5555-555-5555", // azure devops project id
			ProjectName = "MyProject",
			Scope = scope,
			ServiceConnectionName = "SelfServicePortal Test Connection",
			SubscriptionName = sub.Data.DisplayName,
			SubscriptionId = sub.Id.SubscriptionId!,
			TenantId = sub.Data.TenantId.ToString()!,
		};
		await wf.Complete(ServiceProvider);
	}
}