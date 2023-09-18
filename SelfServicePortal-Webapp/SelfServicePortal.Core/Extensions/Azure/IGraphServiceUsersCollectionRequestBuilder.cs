using Microsoft.Graph;

namespace SelfServicePortal.Core.Extensions;

public static class IGraphServiceUsersCollectionRequestBuilderExtensions
{
	public static async IAsyncEnumerable<User> GetFromEmails(this IGraphServiceUsersCollectionRequestBuilder users, params string[] emails)
	{
		foreach (var email in emails)
		{
			var matches = await users.Request().Filter($"userPrincipalName eq '{email}' or mail eq '{email}'").GetAsync();
			foreach (var user in matches)
			{
				yield return user;
			}
		}
	}
}