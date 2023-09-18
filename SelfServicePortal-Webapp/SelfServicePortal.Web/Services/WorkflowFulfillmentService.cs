using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SelfServicePortal.Core.Services;

namespace SelfServicePortal.Web.Services;

// https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-6.0&tabs=netcore-cli
public class WorkflowFulfillmentService : IHostedService
{
	private readonly ILogger<WorkflowFulfillmentService> _logger;
	private readonly IServiceScopeFactory _scopeFactory;
	private CancellationToken _cancellationToken;
	private Thread _thread;

	public WorkflowFulfillmentService(
		ILogger<WorkflowFulfillmentService> logger,
		IServiceScopeFactory scopeFactory
	)
	{
		_logger = logger;
		_scopeFactory = scopeFactory;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Starting workflow fulfillment service.");
		_cancellationToken = cancellationToken;
		// _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
		_thread = new Thread(new ThreadStart(this.DoWork));
		_thread.Start();
		return Task.CompletedTask;
	}

	private async void DoWork()
	{
		while (true)
		{
			if (_cancellationToken.IsCancellationRequested)
			{
				break;
			}
			else
			{
				Thread.Sleep(5000);
			}
			using (var scope = _scopeFactory.CreateScope())
			{
				var workflowClient = scope.ServiceProvider.GetRequiredService<WorkflowClient>();
				await workflowClient.Tick();
			}
		}
	}


	public Task StopAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("Stopping workflow fulfillment service.");
		return Task.CompletedTask;
	}
}