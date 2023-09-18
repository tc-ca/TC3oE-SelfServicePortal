using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SelfServicePortal.Core;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Xunit.Abstractions;

namespace SelfServicePortal.Test;

public class ConfigurationTests : BaseTest
{
	private readonly AppSettings _config;
	public ConfigurationTests(ITestOutputHelper helper) : base(helper)
	{
		_config = ServiceProvider.GetService<AppSettings>()!;
	}

	[Fact]
	public void IsKeyVaultLoaded()
	{
		Assert.NotEmpty(_config.AzureAd.ClientId);
	}

	[Fact]
	public void NoConfigValuesEmpty()
	{
		Assert.False(string.IsNullOrWhiteSpace(_config.AzureAd.CallbackPath), "AzureAd.CallbackPath");
		Assert.False(string.IsNullOrWhiteSpace(_config.AzureAd.ClientId), "AzureAd.ClientId");
		Assert.False(string.IsNullOrWhiteSpace(_config.AzureAd.ClientSecret), "AzureAd.ClientSecret");
		Assert.False(string.IsNullOrWhiteSpace(_config.AzureAd.Domain), "AzureAd.Domain");
		Assert.False(string.IsNullOrWhiteSpace(_config.WorkflowRequestStorage.WorkflowsTableName), "WorkflowRequestStorage.WorkflowsTableName");
		Assert.False(string.IsNullOrWhiteSpace(_config.WorkflowRequestStorage.WorkflowsTableUri), "WorkflowRequestStorage.WorkflowsTableUri");
		Assert.False(string.IsNullOrWhiteSpace(_config.Approvals.RequestsQueueUri), "Approvals.RequestsQueueUri");
		Assert.False(string.IsNullOrWhiteSpace(_config.Approvals.ResponsesQueueUri), "Approvals.ResponsesQueueUri");
		Assert.False(string.IsNullOrWhiteSpace(_config.ManagedIdentityClientId), "ManagedIdentityClientId");
		Assert.False(string.IsNullOrWhiteSpace(_config.KeyVaultName), "KeyVaultName");
		Assert.False(string.IsNullOrWhiteSpace(_config.LogWorkspaceId), "LogWorkspaceId");
		Assert.False(string.IsNullOrWhiteSpace(_config.DevopsOrg), "DevopsOrg");
		Assert.False(string.IsNullOrWhiteSpace(_config.PersonalAccessToken), "PersonalAccessToken");
		Assert.False(string.IsNullOrWhiteSpace(_config.InactivityExemptionGroupName), "InactivityExemptionGroupName");
		Assert.False(string.IsNullOrWhiteSpace(_config.AllowedHosts), "AllowedHosts");
		Assert.False(string.IsNullOrWhiteSpace(_config.ApplicationInsights.ConnectionString), "ApplicationInsights.ConnectionString");
		Assert.NotNull(_config.Logging);
	}
}