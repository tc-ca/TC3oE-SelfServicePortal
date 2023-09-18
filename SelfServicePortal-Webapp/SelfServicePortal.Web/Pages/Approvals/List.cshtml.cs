using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SelfServicePortal.Web.Auth;
using SelfServicePortal.Core.Services;
using SelfServicePortal.Core.Models.Approvals;

namespace SelfServicePortal.Web.Pages.Approvals;

[Authorize(Policy = AuthorizationPolicies.ApplicationAdministrator)]
public class ListModel : PageModel
{
	private readonly WorkflowClient _client;

	public ListModel(
		WorkflowClient client
	)
	{
		this._client = client;
	}

	public ApprovalRequest[] Requests { get; private set; }

	public async Task OnGetAsync()
	{
		Requests = (await _client.GetApprovalRequests()).ToArray();
	}

	public async Task<ActionResult> OnPostApprovalResultAsync(
		string MessageId,
		string PopReceipt,
		string TargetPartitionKey,
		string TargetRowKey,
		bool Approved
	)
	{
		await _client.SubmitApprovalResponse(new()
		{
			Approved = Approved,
			TargetPartitionKey = TargetPartitionKey,
			TargetRowKey = TargetRowKey,
			Responders = User.GetName(),
		});
		await _client.DeleteApprovalRequest(MessageId, PopReceipt);
		await OnGetAsync();
		return Page();
	}
}