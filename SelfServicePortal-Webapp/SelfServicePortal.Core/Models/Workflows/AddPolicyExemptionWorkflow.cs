using OurAzure.Api.Services;
using OurAzure.Api.Models.Policy;
using Microsoft.Extensions.DependencyInjection;

namespace SelfServicePortal.Core.Models.Workflows;

public class AddPolicyExemptionWorkflow : Workflow
{
	public override string Id => nameof(AddPolicyExemptionWorkflow);

	public string PolicyId { get; init; }
	public string ResourceId { get; init; }

	public override async Task Complete(IServiceProvider services)
	{
		var restClient = services.GetRequiredService<AzureRestClient>();
		await restClient.CreatePolicyExemptionAsync(ResourceId, new PolicyExemption.Properties
		{
			policyAssignmentId = PolicyId,
			displayName = "SelfServicePortal Exemption",
			description = "Exemption created via SelfServicePortal",
			exemptionCategory = "Waiver"
		});
	}
}