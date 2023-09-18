using System.Globalization;
using Microsoft.Extensions.Logging;
using SelfServicePortal.Core.Models.Approvals;
using SelfServicePortal.Core.Models.TableStorage;
using SelfServicePortal.Core.Models.Workflows;

namespace SelfServicePortal.Core.Extensions;

public static partial class ILoggerExtensions
{
	public static IDisposable BeginWorkflowLogScope(this ILogger self, string partitionKey, string rowKey)
	{
		return self.BeginScope(new Dictionary<string, object> {
			["WorkflowRequestPartitionKey"] = partitionKey,
			["WorkflowRequestRowKey"] = rowKey,
		});
	}
	public static IDisposable BeginWorkflowLogScope(this ILogger self, WorkflowRequest request)
	{
		return self.BeginWorkflowLogScope(request.PartitionKey, request.RowKey);
	}
	public static IDisposable BeginWorkflowLogScope(this ILogger self, ApprovalResponse response)
	{
		return self.BeginWorkflowLogScope(response.TargetPartitionKey, response.TargetRowKey);
	}
	public static IDisposable BeginWorkflowLogScope(this ILogger self, WorkflowRequestTableEntry entry)
	{
		return self.BeginWorkflowLogScope(entry.PartitionKey, entry.RowKey);
	}
}