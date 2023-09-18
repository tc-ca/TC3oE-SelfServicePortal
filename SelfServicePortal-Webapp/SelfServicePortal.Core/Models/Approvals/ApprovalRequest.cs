using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Queues.Models;

namespace SelfServicePortal.Core.Models.Approvals;

	public record ApprovalRequest
	{
		public string TargetPartitionKey { get; init; }
		public string TargetRowKey { get; init; }
		public string Title { get; init; }
		public string Message { get; init; }

		[JsonIgnore]
		public string MessageId {get; init;}
		[JsonIgnore]
		public string PopReceipt {get; init;}

		
	public static ApprovalRequest? FromQueueMessage(QueueMessage message)
	{
		ApprovalRequest? resp = null;
		try {
			var data = message.Body.ConvertToUTF8FromBase64();
			resp = JsonSerializer.Deserialize<ApprovalRequest>(data);
		} catch (Exception) {
			try {
				resp = JsonSerializer.Deserialize<ApprovalRequest>(message.Body.ToString());
			} catch (Exception) {}
		}

		if (resp == null) {
			Console.Error.WriteLine("Failed to deserialize approval request from {Body}", message.Body);
			return null;
		}
		
		return resp with {
			MessageId = message.MessageId,
			PopReceipt = message.PopReceipt,
		};
	}
	}