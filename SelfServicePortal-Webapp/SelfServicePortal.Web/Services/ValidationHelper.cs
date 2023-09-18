#pragma warning disable CS1998

using System.Collections.Generic;
using Azure.Core;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Microsoft.Extensions.Localization;
using Microsoft.Graph;
//using System.Net.Http;
using OurAzure.Api.Services;

namespace SelfServicePortal.Web.Services;

public class ValidationHelper
{
	private readonly IStringLocalizer<ValidationHelper> _localizer;
	private readonly GraphServiceClient _graphClient;
	private readonly ArmClient _armClient;

	public ValidationHelper(
		IStringLocalizer<ValidationHelper> localizer,
		GraphServiceClient graphClient,
		ArmClient armClient
	)
	{
		_localizer = localizer;
		_graphClient = graphClient;
		_armClient = armClient;
	}

	
	public async IAsyncEnumerable<string> GetEmailLookupValidationErrors(string emails, bool allowEmpty = false)
	{
		var emailList = emails.SplitEmails();
		if (emails.Length == 0 && !allowEmpty)
		{
			yield return _localizer["EmptyInput"];
		}
		foreach (var email in emailList)
		{
			var matches = await _graphClient.Users.GetFromEmails(email).CountAsync();
			if (matches == 0) {
				yield return _localizer["UserDoesNotExist", email];
			}
			if (matches > 1) {
				yield return _localizer["UserAmbiguous", email];
			}
		}
	}

	public async IAsyncEnumerable<string> GetSecurityGroupLookupValidationErrors(params string[] displayNames)
	{
		foreach (var groupName in displayNames)
		{
			var matches = await _graphClient.Groups.GetFromDisplayNames(displayNames).CountAsync();
			if (matches == 0)
			{
				yield return _localizer["GroupDoesNotExist", groupName];
			}
			if (matches > 1)
			{
				yield return _localizer["GroupAmbiguous", groupName];
			}
		}
	}

	public async IAsyncEnumerable<string> GetSecurityGroupNameAvailableValidationErrors(params string[] displayNames)
	{
		foreach (var groupName in displayNames)
		{
			var exists = await _graphClient.Groups.GetFromDisplayNames(groupName).AnyAsync();
			if (exists)
			{
				yield return _localizer["GroupExists", groupName];
			}
		}
	}

	public async IAsyncEnumerable<string> GetResourceGroupNameInputVailidationErrors(string rgName, params SubscriptionResource[] subscriptions)
	{
		bool foundRGFlag = false;
        foreach(var sub in subscriptions)
        {
            if (sub.GetResourceGroups().Exists(rgName))
            {
                foundRGFlag = true;
                break;
            }

        }
		if (foundRGFlag == false)
        {
            yield return _localizer["ResourceGroupDoesNotExist", rgName];
        }
	}

	public async IAsyncEnumerable<string> GetResourceGroupNameAvailableValidationErrors(SubscriptionResource subscription, params string[] resourceGroupNames)
	{
		foreach (var rgName in resourceGroupNames)
		{
			if (subscription.GetResourceGroups().Exists(rgName))
			{
				yield return _localizer["ResourceGroupExists", rgName];
			}
		}
	}

	public async IAsyncEnumerable<string> GetResourceGroupLookupValidationErrors(SubscriptionResource subscription, params string[] resourceGroupNames)
	{
		foreach (var rgName in resourceGroupNames)
		{
			if (!subscription.GetResourceGroups().Exists(rgName))
			{
				yield return _localizer["ResourceGroupDoesNotExist", rgName];
			}
		}
	}

	public async IAsyncEnumerable<string> GetSubscriptionValidationErrors(string subscriptionName)
	{
		var subscription = await _armClient.GetSubscriptionByName(subscriptionName);
		if (subscription == null)
		{
			yield return _localizer["SubscriptionDoesNotExist", subscriptionName];
		}
	}

	public async IAsyncEnumerable<string> GetLocationValidationErrors(AzureLocation[] validLocations, params string[] locationNames)
	{
		foreach (var name in locationNames)
		{
			if (!validLocations.Any(l => l.Name == name))
			{
				yield return _localizer["LocationNotAllowed", name];
			}
		}
	}

	public async IAsyncEnumerable<string> GetSCEDLocationValidationErrors(string subscriptionName, string locationName)
	{
		if (subscriptionName.Contains("SCED") & locationName == "canadaeast")
		{
			yield return _localizer["NoEastLocationForSCED"];
		}

	}

	
}