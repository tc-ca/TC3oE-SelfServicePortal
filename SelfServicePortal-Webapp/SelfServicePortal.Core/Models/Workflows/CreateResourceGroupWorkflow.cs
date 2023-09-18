using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.Extensions.DependencyInjection;

namespace SelfServicePortal.Core.Models.Workflows;

public class CreateResourceGroupWorkflow : Workflow {

	public override string Id => nameof(CreateResourceGroupWorkflow);
	
	public string SubscriptionName {get; init;}
	public string LocationName {get; init;}
	public string ResourceGroupName {get; init;}
	public Dictionary<string, string> ResourceGroupTags {get; init; }
	public static readonly AzureLocation[] ValidLocations = new[] { AzureLocation.CanadaCentral, AzureLocation.CanadaEast };

	public override async Task Complete(IServiceProvider services)
	{
		var armClient = services.GetRequiredService<ArmClient>();
		var sub = await armClient.GetSubscriptionByName(SubscriptionName);
		var location = ValidLocations.First(l => l.Name == LocationName);
		var data = new ResourceGroupData(location);
		foreach (var tag in ResourceGroupTags)
		{
			data.Tags.Add(tag.Key, tag.Value ?? "");
		}
		data.Tags.Add("Resource-Group-Creation-Date", DateTime.Now.ToString("yyyy-MM-dd"));
		await sub.GetResourceGroups().CreateOrUpdateAsync(Azure.WaitUntil.Completed, ResourceGroupName, data );
	}
}