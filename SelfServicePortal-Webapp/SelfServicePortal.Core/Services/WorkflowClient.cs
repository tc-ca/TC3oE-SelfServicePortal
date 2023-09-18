using System.Globalization;
using System.Text.Json;
using SelfServicePortal.Core.Models.Approvals;
using SelfServicePortal.Core.Models.TableStorage;
using SelfServicePortal.Core.Models.Workflows;
using Azure.Core;
using Azure.Data.Tables;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SelfServicePortal.Core.Extensions;
using SelfServicePortal.Core.Models;
using OurTeams.Api;

namespace SelfServicePortal.Core.Services;

public class WorkflowClient
{

	private readonly ILogger<WorkflowClient> _logger;
	private readonly IServiceProvider _services;
	private readonly QueueClient _approvalResponsesQueue;
	private readonly QueueClient _approvalRequestsQueue;
	private readonly TableClient _workflowTable;
	private readonly bool _disableWorkflowCompletion;
	private readonly TeamsClient _teamsClient;

	public WorkflowClient(
		AppSettings cfg,
		ILogger<WorkflowClient> logger,
		IServiceProvider services,
		TokenCredential cred,
		TeamsClient teamsClient
	)
	{
		this._logger = logger;
		this._services = services;
		this._disableWorkflowCompletion = cfg.DisableWorkflowCompletion;
		this._teamsClient = teamsClient;
		this._approvalResponsesQueue = new QueueClient(
			new Uri(cfg.Approvals.ResponsesQueueUri),
			cred,
			null
		);
		this._approvalRequestsQueue = new QueueClient(
			new Uri(cfg.Approvals.RequestsQueueUri),
			cred,
			null
		);
		this._workflowTable = new TableClient(
			new Uri(cfg.WorkflowRequestStorage.WorkflowsTableUri),
			cfg.WorkflowRequestStorage.WorkflowsTableName,
			cred,
			null
		);
	}

	public async Task Enqueue(WorkflowRequest request)
	{
		using var _ = (_logger.BeginWorkflowLogScope(request));
		_logger.LogInformation("Enqueueing request [{Title}]", request.Title);
		_logger.LogDebug("Creating table entity for request");
		var data = request.Serialize();
		var entity = new TableEntity();
		entity.PartitionKey = request.PartitionKey;
		entity.RowKey = request.RowKey;
		entity["State"] = WorkflowState.PendingApproval;
		entity["Data"] = data;

		_logger.LogDebug(data);

		_logger.LogDebug("Appending entity to table");
		await _workflowTable.AddEntityAsync(entity);

		await SubmitApprovalRequest(new WorkflowRequestTableEntry(){
			Request = request,
			Entity = entity,
		});
	}

	
	public async Task HandleApproval(ApprovalResponse response)
	{
		using var _ = (_logger.BeginWorkflowLogScope(response));

		_logger.LogInformation("Received approval response approved={Approved}", response.Approved);
		var entity = _workflowTable.Query<TableEntity>(e => e.PartitionKey == response.TargetPartitionKey && e.RowKey == response.TargetRowKey).FirstOrDefault((TableEntity?) null);
		if (entity == null)
		{
			_logger.LogWarning("Approval response pointed to a nonexistant table entry");
			await _approvalResponsesQueue.DeleteMessageAsync(response.MessageId, response.PopReceipt);
			return;
		}
		var entry = new WorkflowRequestTableEntry
		{
			Client = _workflowTable,
			Entity = entity
		};
		if (entry.State != WorkflowState.PendingApproval) {
			_logger.LogWarning("Approval response pointed to a non-pending table entry");
			await _approvalResponsesQueue.DeleteMessageAsync(response.MessageId, response.PopReceipt);
			return;
		}
		_logger.LogDebug("Updating table entry state");
		entry.State = response.Approved ? WorkflowState.Approved : WorkflowState.Denied;
		await entry.ApplyUpdateAsync();
		_logger.LogDebug("Deleting approval response message from queue");
		await _approvalResponsesQueue.DeleteMessageAsync(response.MessageId, response.PopReceipt);

		if (!response.Approved) {
			_logger.LogInformation("Approval response came back denied. No action required.");
			return;
		}

		try
		{
			_logger.LogInformation("Completing workflow \"{0}\"", entry.Request.Title);
			if (!_disableWorkflowCompletion)
			{
				await entry.Request.Complete(_services);
				entry.State = WorkflowState.Completed;
				await entry.ApplyUpdateAsync();
			}
			else
			{
				_logger.LogWarning("Skipping completion because DisableWorkflowCompletion app setting is set to true.");
			}
			_logger.LogInformation("Workflow completed successfully");
		}
		catch (Exception e)
		{
			_logger.LogError("Failed to complete workflow: {0}", e);
			_logger.LogDebug("Updating table entry state to Failed.");
			entry.State = WorkflowState.Failed;
			entry.Error = e.Message;
			await entry.ApplyUpdateAsync();
			try {
				await _teamsClient.SendMessage($"Failed to complete workflow {entry.Request.Title} with error: {e.Message}");
			} catch (Exception ee)
			{
				_logger.LogError("Exception occurred during workflow exception handling. Failed to send message to teams: {0}", ee);
			}
		}
	}

	public async Task SubmitApprovalRequest(WorkflowRequestTableEntry entry)
	{
		using var _ = (_logger.BeginWorkflowLogScope(entry));

		_logger.LogDebug("Serializing workflows to JSON to build approval message");
		var serializedWorkflows = JsonSerializer.Serialize((object[])entry.Request.Workflows, new JsonSerializerOptions { WriteIndented = true });
		var message = $@"Ticket ID = `{entry.Request.TicketId}`
```
{serializedWorkflows}
```";

		var approval = new ApprovalRequest
		{
			Title = entry.Request.Title,
			Message = message,
			TargetPartitionKey = entry.Entity.PartitionKey,
			TargetRowKey = entry.Entity.RowKey,
		};
		
		var json = JsonSerializer.Serialize(approval);
		_logger.LogInformation("Sending approval request to queue");
		await _approvalRequestsQueue.SendMessageAsync(json);
		try {
			await _teamsClient.SendMessage($"A new workflow needs approval: \"{entry.Request.Title}\"\n{message}");
		} catch (Exception ee)
		{
			_logger.LogError("Exception occurred during workflow approval teams notification handling. Failed to send message to teams: {0}", ee);
		}
	}

	public async Task<Azure.Response> DeleteApprovalRequest(string messageId, string popReceipt)
	{
		return await _approvalRequestsQueue.DeleteMessageAsync(messageId, popReceipt);
	}

	public async Task SubmitApprovalResponse(ApprovalResponse response)
	{
		using var _ = _logger.BeginWorkflowLogScope(response);

		var json = JsonSerializer.Serialize(response);
		_logger.LogInformation("Submitting approval response approved={Approved}", response.Approved);
		await _approvalResponsesQueue.SendMessageAsync(json);
	}



	public async Task<IEnumerable<ApprovalRequest>> GetApprovalRequests()
	{
		var messages = await _approvalRequestsQueue.ReceiveMessagesAsync(10);
		if (messages.Value == null) return new ApprovalRequest[]{};
		return messages.Value
			.Select(msg => ApprovalRequest.FromQueueMessage(msg))
			.Where(req => req != null)
			.Cast<ApprovalRequest>();
	}

	public async Task Tick()
	{
		QueueMessage[] messages = await _approvalResponsesQueue.ReceiveMessagesAsync();
		foreach (var message in messages)
		{
			var response = ApprovalResponse.FromQueueMessage(message);
			if (response == null)
			{
				_logger.LogWarning("Failed to parse approval response message {msg}", message);
			} 
			else 
			{
				await HandleApproval(response);
			}

		}
	}

	public async IAsyncEnumerable<WorkflowRequestTableEntry> GetRequests(string? initiator = null)
	{
		Azure.AsyncPageable<TableEntity> query;
		if (string.IsNullOrWhiteSpace(initiator))
			query = _workflowTable.QueryAsync<TableEntity>();
		else
			query = _workflowTable.QueryAsync<TableEntity>(e => e.PartitionKey == initiator);

		await foreach (var entity in query)
		{
			yield return new()
			{
				Client = _workflowTable,
				Entity = entity
			};
		}
	}

	public async Task<WorkflowRequestTableEntry> GetRequest(string initiatorPartitionKey, string timestampRowKey)
	{
		var entry = await _workflowTable.QueryAsync<TableEntity>(e => e.PartitionKey == initiatorPartitionKey && e.RowKey == timestampRowKey).FirstAsync();
		return new()
		{
			Client = _workflowTable,
			Entity = entry
		};
	}
}