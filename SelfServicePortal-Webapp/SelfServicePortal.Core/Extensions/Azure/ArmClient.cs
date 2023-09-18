using Azure.ResourceManager;
using Azure.ResourceManager.Resources;

namespace SelfServicePortal.Core.Extensions;

public static class ArmClientExtensions
{
	public static async Task<SubscriptionResource> GetSubscriptionByName(this ArmClient armClient, string subscriptionName)
	{
		return await armClient.GetSubscriptions().FirstAsync(s => s.Data.DisplayName == subscriptionName);
	}
}