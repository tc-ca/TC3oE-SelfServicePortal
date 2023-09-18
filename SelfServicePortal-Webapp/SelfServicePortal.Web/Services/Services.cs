using Azure.Monitor.Query;
using Azure.ResourceManager;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using SelfServicePortal.Core;
using SelfServicePortal.Core.Services;
using OurAzure.Api.Services;
using OurAzureDevops;
using OurTeams.Api;

namespace SelfServicePortal.Web.Services;

/**
Collects common dependency injection targets into a single class
Allows injecting only one thing in our pages to get access to multiple services
*/
public class Services<T> where T: class {
	public Services(
		IStringLocalizer<T> localizer,
		ILogger<T> logger,
		GraphServiceClient graphClient,
		ArmClient armClient,
		AppSettings config,
		WorkflowClient workflowClient,
		ValidationHelper validationHelper,
		AzureRestClient azureClient,
		AzureDevopsRestClient devopsClient,
		IWebHostEnvironment webEnv,
		IAntiforgery antiforgery,
		LogsQueryClient logsClient,
		TelemetryClient telemetryClient,
		TeamsClient teamsClient
	) {
		Localizer = localizer;
		Logger = logger;
		GraphClient = graphClient;
		ArmClient = armClient;
		Config = config;
		WorkflowClient = workflowClient;
		ValidationHelper = validationHelper;
		AzureRestClient = azureClient;
		AzureDevopsRestClient = devopsClient;
		WebHostEnvironment = webEnv;
		Antiforgery = antiforgery;
		LogsQueryClient = logsClient;
		TelemetryClient = telemetryClient;
		TeamsClient = teamsClient;
	}

	public readonly IStringLocalizer<T> Localizer;
	public readonly GraphServiceClient GraphClient;

	public readonly ArmClient ArmClient;
	public readonly AppSettings Config;
	public readonly WorkflowClient WorkflowClient;
	public readonly ValidationHelper ValidationHelper;

	public readonly AzureRestClient AzureRestClient;
	public readonly AzureDevopsRestClient AzureDevopsRestClient;

	public readonly IWebHostEnvironment WebHostEnvironment;
	public readonly IAntiforgery Antiforgery;
	public readonly LogsQueryClient LogsQueryClient;
	public readonly TelemetryClient TelemetryClient;
	public readonly TeamsClient TeamsClient;
	public readonly ILogger<T> Logger;
}