using Xunit.Abstractions;

namespace SelfServicePortal.Test;

public class DevopsTests : BaseTest
{
	public DevopsTests(ITestOutputHelper helper) : base(helper) {}
	
	[Fact]
	public async Task CanAccessProjects()
	{
		var projects = await Services.AzureDevopsRestClient.GetProjects().ToListAsync();
		Assert.NotEmpty(projects);
	}

	[Fact]
	public async Task CanAccessUsers()
	{
		var results = Services.AzureDevopsRestClient.GetUsers();
		var count = await results.CountAsync();
		Assert.InRange(count, 100, 10000);
	}

	[Fact]
	public async Task CanAccessProjectsForUser()
	{
		var user = "first.last@org.gc.ca";
		var projects = await Services.AzureDevopsRestClient.GetProjectsForUser(user);
		Assert.NotEmpty(projects);
	}
}