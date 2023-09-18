namespace OurAzureDevops.Models.ServiceEndpoint;

public record RequestBody
{
	// Gets or sets the identity reference for the administrators group of the service endpoint.
	public IdentityRef administratorsGroup { get; set; }

	// Gets or sets the authorization data for talking to the endpoint.
	public EndpointAuthorization authorization { get; set; }

	// Gets or sets the identity reference for the user who created the Service endpoint.
	public IdentityRef createdBy { get; set; }

	public object data { get; set; }

	// Gets or sets the description of endpoint.
	public string description { get; set; }

	// This is a deprecated field.
	public string groupScopeId { get; set; }

	// Gets or sets the identifier of this endpoint.
	public string id { get; set; }

	// EndPoint state indicator
	public bool isReady { get; set; }

	// Indicates whether service endpoint is shared with other projects or not.
	public bool isShared { get; set; }

	// Gets or sets the friendly name of the endpoint.
	public string name { get; set; }

	// Error message during creation/deletion of endpoint
	public JObject operationStatus { get; set; }

	// Owner of the endpoint Supported values are "library", "agentcloud"
	public string owner { get; set; }

	// Gets or sets the identity reference for the readers group of the service endpoint.
	public IdentityRef readersGroup { get; set; }

	// All other project references where the service endpoint is shared.
	public ServiceEndpointProjectReference[] serviceEndpointProjectReferences { get; set; }

	// Gets or sets the type of the endpoint.
	public string type { get; set; }

	// Gets or sets the url of the endpoint.
	public string url { get; set; }

}