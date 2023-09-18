using SelfServicePortal.Web.Auth;
using SelfServicePortal.Core.Models.TableStorage;
using SelfServicePortal.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure.Monitor.Query.Models;
using Microsoft.Extensions.Hosting;
using SelfServicePortal.Core.Models.Workflows;
using Microsoft.Extensions.Logging;
using JsonDiffPatchDotNet;
using Newtonsoft.Json.Linq;

namespace SelfServicePortal.Web.Pages.WorkflowRequests;

[Authorize(Policy = AuthorizationPolicies.ApplicationAdministrator)]
public class EditModel : PageModel
{
	private readonly Services<EditModel> _services;

	[BindProperty(SupportsGet = true)]
	public string Initiator { get; set; }

	[BindProperty(SupportsGet = true)]
	public string Timestamp { get; set; }

	[BindProperty]
	public string Data {get; set;}

	public bool Success = false;

	public WorkflowRequestTableEntry Entry { get; private set; }

	public LogsQueryResult LogEntries {get; private set;}

	public EditModel(Services<EditModel> services)
	{
		_services = services;
	}

	public async Task OnGetAsync()
	{
		await _services.TelemetryClient.FlushAsync(System.Threading.CancellationToken.None);
		Entry = await _services.WorkflowClient.GetRequest(Initiator, Timestamp);
		Data = Entry.Data;
		var query = $@"
			AppTraces
				| where Properties.AspNetCoreEnvironment == '{(_services.WebHostEnvironment.IsDevelopment() ? "Development" : "Production")}'
				| where Properties.WorkflowRequestPartitionKey == '{Entry.PartitionKey}'
				| where Properties.WorkflowRequestRowKey == todatetime('{Entry.RowKey}')
				| project TimeGenerated, SeverityLevel, Message
				| order by TimeGenerated asc
		";
		LogEntries = await _services.LogsQueryClient.QueryWorkspaceAsync(
			_services.Config.LogWorkspaceId,
			query,
			Azure.Monitor.Query.QueryTimeRange.All
		);
	}

	public async Task<IActionResult> OnPostAsync()
	{
		// store data before overwriting
		var newData = Data;

		// fetch data from remote
		await OnGetAsync();

		// activate log scope for the workflow
		using var _ = _services.Logger.BeginWorkflowLogScope(Entry);

		// log differences
		var jdp = new JsonDiffPatch();
		var left = JToken.Parse(Entry.Data);
		var right = JToken.Parse(newData);
		var patch = jdp.Diff(left, right);
		_services.Logger.LogInformation("Updated by {Name}: {Patch}", User.GetName(), patch);

		// update data field
		Entry.Data = newData;
		// apply update
		await Entry.ApplyUpdateAsync();

		// fetch entity with updates
		await OnGetAsync();

		// return to page
		Success = true;
		return Page();
	}
}