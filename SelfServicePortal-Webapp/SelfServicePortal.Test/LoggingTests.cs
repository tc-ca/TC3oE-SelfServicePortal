using System;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;


namespace SelfServicePortal.Test;

public class LoggingTests : BaseTest
{
	public LoggingTests(ITestOutputHelper helper) : base(helper) {}
	// [Fact]
	// public async Task LogScopeTest()
	// {
	// 	// this test is supposed to see how long it takes for a message to appear in the log analytics workspace
	// 	// for some reason the logs aren't being sent so this test can't work for now

	// 	Assert.True(Services.WebHostEnvironment.IsDevelopment());
	// 	Assert.True(Services.TelemetryClient.IsEnabled());
	// 	var nonce = Guid.NewGuid().ToString();
	// 	Services.Logger.LogInformation("heehaw {nonce}", nonce);

	// 	await Services.TelemetryClient.FlushAsync(System.Threading.CancellationToken.None);
	// 	await Task.Delay(5000);

	// 	LogsQueryResult logs = await Services.LogsQueryClient.QueryWorkspaceAsync(
	// 		Services.Config.LogWorkspaceId,
	// 		$@"
	// 		AppTraces
	// 			| where Properties.AspNetCoreEnvironment == '{(Services.WebHostEnvironment.IsDevelopment() ? "Development" : "Production")}'
	// 			| where Message == 'heehaw {nonce}'
	// 		",
	// 		new QueryTimeRange(TimeSpan.FromMinutes(5))
	// 	);
	// 	Assert.NotEmpty(logs.Table.Rows);
	// }
}