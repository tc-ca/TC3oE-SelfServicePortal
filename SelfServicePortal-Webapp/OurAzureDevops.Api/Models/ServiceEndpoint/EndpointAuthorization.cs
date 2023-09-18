namespace OurAzureDevops.Models.ServiceEndpoint;

public record EndpointAuthorization
{
	// Gets or sets the parameters for the selected authorization scheme.
	public object parameters { get; set; }

	// Gets or sets the scheme used for service endpoint authentication.
	public string scheme { get; set; }
}