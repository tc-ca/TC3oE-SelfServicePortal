using Azure.ResourceManager.Resources;
namespace SelfServicePortal.Core.Extensions;

public static class SubscriptionExtensions {
	public static string GetNamingPrefix(this SubscriptionResource Subscription)
	{
		if (Subscription.Data.DisplayName == "My-Ugly-Subscription")
			return "UglySub";
		return Subscription.Data.DisplayName;
	}
}