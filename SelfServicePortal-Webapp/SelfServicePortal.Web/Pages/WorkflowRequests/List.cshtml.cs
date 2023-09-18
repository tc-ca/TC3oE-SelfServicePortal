using System.Collections.Generic;
using SelfServicePortal.Web.Auth;
using SelfServicePortal.Core.Models.TableStorage;
using SelfServicePortal.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace SelfServicePortal.Web.Pages.WorkflowRequests;

[Authorize(Policy = AuthorizationPolicies.ApplicationAdministrator)]
public class ListModel : PageModel
{
	private readonly Services<ListModel> _services;

	public AntiforgeryTokenSet AntiForgeryToken {get; private set;}

	public ListModel(Services<ListModel> services)
	{
		_services = services;
	}

	public IEnumerable<WorkflowRequestTableEntry> Requests {get; private set;}

	public async Task OnGet()
	{
		Requests = await _services.WorkflowClient.GetRequests()
			.OrderByDescending(x => x.Request.DateInitiated)
			.ToListAsync();
		AntiForgeryToken = _services.Antiforgery.GetAndStoreTokens(HttpContext);
	}

	public async Task<IActionResult> OnPostResubmitAsync(string partitionKey, string rowKey)
	{
		var request = await _services.WorkflowClient.GetRequest(partitionKey, rowKey);
		await _services.WorkflowClient.SubmitApprovalRequest(request);
		return StatusCode(200);
	}
}