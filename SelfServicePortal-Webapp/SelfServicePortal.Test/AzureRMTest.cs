using Azure.Core;
using Xunit.Abstractions;

namespace SelfServicePortal.Test;

public class AzureRMTest : BaseTest
{
	public AzureRMTest(ITestOutputHelper helper) : base(helper) {}
	
	[Fact]
	public async Task GetRoleAssignmentsTest()
	{
		var user = "5555-5555-55555-5555"; // some user's guid
		var prod = await Services.ArmClient.GetSubscriptionByName("mytestsub");
		var roleAssignments = await Services.AzureRestClient.GetRoleAssignments(prod.Id!, user);
		Assert.NotEmpty(roleAssignments.value);
	}

	[Fact]
	public async Task GetResourceGroupIdsForUser()
	{
		var user = "5555-5555-55555-5555"; // some user's guid
		var subs = Services.ArmClient.GetSubscriptions().AsEnumerable().Select(x => x.Id.ToString()).ToArray();
		var resourceGroups = await Services.AzureRestClient.GetResoruceGroupIdsForUser(user,subs).ToArrayAsync();
		Assert.NotEmpty(resourceGroups);
	}
}