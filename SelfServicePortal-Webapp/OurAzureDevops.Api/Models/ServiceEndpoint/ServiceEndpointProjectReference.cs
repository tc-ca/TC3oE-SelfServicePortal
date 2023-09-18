namespace OurAzureDevops.Models.ServiceEndpoint;

public record ServiceEndpointProjectReference
{
	// Gets or sets description of the service endpoint.
	public string description { get; set; }

	// Gets or sets name of the service endpoint.
	public string name { get; set; }

	// Gets or sets project reference of the service endpoint.
	public ProjectReference projectReference { get; set; }

}