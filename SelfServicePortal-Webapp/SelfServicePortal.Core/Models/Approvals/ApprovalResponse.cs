using Azure.Storage.Queues.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SelfServicePortal.Core.Models.Approvals;

public record ApprovalResponse
{
	public string TargetPartitionKey { get; init; }
	public string TargetRowKey { get; init; }
	public bool Approved { get; init; }
	public string Responders { get; init; }

	[JsonIgnore]
	public string MessageId {get; init;}
	[JsonIgnore]
	public string PopReceipt {get; init;}

	public static ApprovalResponse? FromQueueMessage(QueueMessage message)
	{
		ApprovalResponse? resp = null;
		try {
			var data = message.Body.ConvertToUTF8FromBase64();
			resp = JsonSerializer.Deserialize<ApprovalResponse>(data);
		} catch (Exception) {
			try {
				resp = JsonSerializer.Deserialize<ApprovalResponse>(message.Body.ToString());
			} catch (Exception) {}
		}

		if (resp == null) {
			Console.Error.WriteLine("Failed to deserialize approval response from {Body}", message.Body);
			return null;
		}
		
		return resp with {
			MessageId = message.MessageId,
			PopReceipt = message.PopReceipt,
		};
	}
}