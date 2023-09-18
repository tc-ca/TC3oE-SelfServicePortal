
using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SelfServicePortal.Core.Models.Workflows;

public class WorkflowRequest
{
	public string Initiator { get; init; }
	public DateTime DateInitiated { get; init; }
	public string TicketId { get; init; }

	public Workflow[] Workflows { get; init; }

	public string Title { get; set; }

	public string RowKey => DateInitiated.ToString("o", CultureInfo.InvariantCulture);
	public string PartitionKey => Initiator;

	public string Serialize()
	{
		var me = new
		{
			Title,
			Initiator,
			DateInitiated,
			TicketId,
			Workflows = (object[])Workflows
		};
		var rtn = JsonSerializer.Serialize(me, new JsonSerializerOptions
		{
			WriteIndented = true,
		});
		return rtn;
	}

	public static WorkflowRequest Deserialize(string json)
	{
		var doc = JsonDocument.Parse(json).RootElement;
		var title = doc.GetProperty(nameof(Title)).GetString()!;
		var initiator = doc.GetProperty(nameof(Initiator)).GetString()!;
		var dateInitiated = doc.GetProperty(nameof(DateInitiated)).GetDateTime();
		string ticketId = "";
		if (doc.TryGetProperty(nameof(TicketId), out var ticketIdElem))
			ticketId = ticketIdElem.GetString()!;
		var workflows = doc.GetProperty(nameof(Workflows)).EnumerateArray()
		.Select(x =>
			{
				var id = x.GetProperty(nameof(Workflow.Id)).GetString()!;
				if (Workflow.WorkflowLookup.ContainsKey(id))
				{
					Type workflowType = Workflow.WorkflowLookup[id];
					return (Workflow)JsonSerializer.Deserialize(x.ToString(), workflowType)!;
				}
				else
				{
					Console.WriteLine("Failed to decode unknown workflow with id {0}", id);
					// TODO: find a way to get an ILogger here to log a warning
					return null;
				}
			})
		.Where(x => x != null)
		.Cast<Workflow>()
		.ToArray();
		return new WorkflowRequest
		{
			Title = title,
			Initiator = initiator,
			DateInitiated = dateInitiated,
			TicketId = ticketId,
			Workflows = workflows
		};
	}

	public async Task Complete(IServiceProvider services)
	{
		var logger = services.GetRequiredService<ILogger<WorkflowRequest>>();
		var i = 0;
		foreach (var workflow in Workflows)
		{
			logger.LogInformation("Completing workflow {i}: {Id}", i, workflow.Id);
			await workflow.Complete(services);
			logger.LogInformation("Successfully completed workflow {i}: {Id}", i, workflow.Id);
			i++;
		}
	}
}