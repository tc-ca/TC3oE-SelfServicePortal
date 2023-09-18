using System;
using Microsoft.Graph;
using Xunit.Abstractions;

namespace SelfServicePortal.Test;

public class AzureADTest : BaseTest
{
	public AzureADTest(ITestOutputHelper helper) : base(helper) {}
	
	[Fact]
	public async Task FindGroupByName()
	{
		var found = await Services.GraphClient.Groups.Request().Filter("displayName eq 'MyGroupHere'").GetAsync();
		Assert.NotNull(found);
		Assert.NotEmpty(found);
	}

	[Fact]
	public async Task CanCreateServicePrincipals()
	{
		Console.WriteLine("Creating application");
		var app = new Application()
		{
			DisplayName = "SelfServicePortal Test Application",
		};
		app = await Services.GraphClient.Applications.Request().AddAsync(app);

		Console.WriteLine("Creating service principal");
		var sp = new ServicePrincipal()
		{
			AppId = app.AppId,
			DisplayName = app.DisplayName,
		};
		sp = await Services.GraphClient.ServicePrincipals.Request().AddAsync(sp);

		Console.WriteLine("Cleaning up service principal");
		await Services.GraphClient.ServicePrincipals[sp.Id].Request().DeleteAsync();
		
		Console.WriteLine("Cleaning up app registration");
		await Services.GraphClient.Applications[app.Id].Request().DeleteAsync();
	}
}