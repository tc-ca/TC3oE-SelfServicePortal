namespace SelfServicePortal.Core;

public record AppSettings
{
	public AzureAdAppSettings AzureAd {get; init;}
	public WorkflowRequestStorageAppSettings WorkflowRequestStorage {get; init;}
	public ApprovalsAppSettings Approvals {get; init;}
	public string ManagedIdentityClientId {get; init;}
	public string KeyVaultName {get; init;}
	public string LogWorkspaceId {get; init;}
	public string DevopsOrg {get; init;}
	public string PersonalAccessToken {get; init;} // stored in keyvault not json
	public bool DisableWorkflowCompletion {get; init;}// used when pentesting to avoid random things from happening
	public string InactivityExemptionGroupName {get; init;}
	public string AllowedHosts {get; init;}
	public LoggingAppSettings Logging {get; init;}

	public string TeamsWebhookUrl {get; init;}

	public ApplicationInsightsSettings ApplicationInsights {get; init;}

	public string RedirectUri {get; set;}
}

public record ApplicationInsightsSettings
{
	public string ConnectionString {get; init;}
}

public record AzureAdAppSettings
{
	public string Instance {get; init;}
	public string Domain {get; init;}
	public string CallbackPath {get; init;}
	public string TenantId {get; init;} // stored in keyvault not json
	public string ClientId {get; init;} // stored in keyvault not json
	public string ClientSecret {get; init;} // stored in keyvault not json
}

public record WorkflowRequestStorageAppSettings
{
	public string WorkflowsTableUri {get; init;}
	public string WorkflowsTableName {get; init;}
}

public record ApprovalsAppSettings
{
	public string RequestsQueueUri {get; init;}
	public string ResponsesQueueUri {get; init;}
}

public record LoggingAppSettings
{
	public Dictionary<string, string> LogLevel {get; init;}
}