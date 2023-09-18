using SelfServicePortal.Core.Models.Workflows;
using Azure.Data.Tables;

namespace SelfServicePortal.Core.Models.TableStorage;

public class WorkflowRequestTableEntry
{
	public TableClient Client { get; init; }

	private TableEntity _entity { get; init; }
	public TableEntity Entity
	{
		get => _entity;
		init {
			_entity = value;
			Request = WorkflowRequest.Deserialize(Data);
		}
	}
	public WorkflowRequest Request { get; init; }

	public string Data
	{
		get => (string)Entity["Data"];
		set => Entity["Data"] = value;
	}
	public string State
	{
		get => (string)Entity["State"];
		set => Entity["State"] = value;
	}
	
	#nullable enable
	public string? Error
	{
		get => (string)Entity["Error"];
		set => Entity["Error"] = value;
	}
	#nullable restore

	public string PartitionKey => Entity.PartitionKey;
	public string RowKey => Entity.RowKey;

	public string Initiator => PartitionKey;
	public DateTime Timestamp => DateTime.Parse(RowKey);

	public async Task ApplyUpdateAsync()
	{
		await Client.UpdateEntityAsync<TableEntity>(Entity, Azure.ETag.All, TableUpdateMode.Merge);
	}
}