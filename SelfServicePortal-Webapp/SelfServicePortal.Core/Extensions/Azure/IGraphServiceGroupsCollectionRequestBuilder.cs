using Microsoft.Graph;

namespace SelfServicePortal.Core.Extensions;

public static class IGraphServiceGroupsCollectionRequestBuilderExtensions
{
	public static async Task<string?> GetId(this IGraphServiceGroupsCollectionRequestBuilder groups, string displayName)
	{
		var found = await groups.Request()
				.Filter($"displayName eq '{displayName}'")
				.Select(g => new { g.Id })
				.Top(1)
				.GetAsync();
		if (found.Count < 1) return null;
		return found.First().Id;
	}

	public static async IAsyncEnumerable<Group> GetFromDisplayNames(this IGraphServiceGroupsCollectionRequestBuilder groups, params string[] displayNames)
	{
		foreach (var name in displayNames)
		{
			var matches = await groups.Request()
				.Filter($"displayName eq '{name}'")
				.GetAsync();
			foreach (var group in matches)
			{
				yield return group;
			}
		}
	}
}