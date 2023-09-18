using System;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SelfServicePortal.Core;
using SelfServicePortal.Web.Auth;
using Microsoft.Extensions.Hosting;
using SelfServicePortal.Web.Services;
using Microsoft.ApplicationInsights;

namespace SelfServicePortal.Web.Pages;

[Authorize(Policy = AuthorizationPolicies.ApplicationAdministrator)]
public class LogsModel : PageModel
{
	private readonly Services<LogsModel> _services;

	public LogsModel(Services<LogsModel> services)
	{
		_services = services;
	}

	public LogsQueryResult LogEntries {get; private set;}

	public async Task OnGetAsync()
	{
		await _services.TelemetryClient.FlushAsync(System.Threading.CancellationToken.None);
		var query = $@"
			AppTraces
				| where Properties.AspNetCoreEnvironment == '{(_services.WebHostEnvironment.IsDevelopment() ? "Development" : "Production")}'
				| project TimeGenerated, SeverityLevel, scope=strcat(Properties.WorkflowRequestPartitionKey, ' ', Properties.WorkflowRequestRowKey), Message
				| order by TimeGenerated asc
		";
		LogEntries = await _services.LogsQueryClient.QueryWorkspaceAsync(
			_services.Config.LogWorkspaceId,
			query,
			new QueryTimeRange(TimeSpan.FromDays(30))
		);
	}
}