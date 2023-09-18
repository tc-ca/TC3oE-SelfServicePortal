
using Azure.Core;
using Azure.ResourceManager;
using Microsoft.Extensions.DependencyInjection;

namespace SelfServicePortal.Core.Models.Workflows;

public class AddTagsWorkflow : Workflow
{
	public override string Id => nameof(AddTagsWorkflow);

	public string ResourceId { get; init; }
	public Dictionary<string, string> Tags { get; init; }

	public override async Task Complete(IServiceProvider services)
	{
		var armClient = services.GetRequiredService<ArmClient>();
		var resource = armClient.GetGenericResource(new ResourceIdentifier(ResourceId));
		foreach (var tag in Tags)
		{
			await resource.AddTagAsync(tag.Key, tag.Value);
		}
	}
}