namespace OurAzureDevops.Models.ServiceEndpoint;

public record JObject
{

	// Represents an abstract JSON token.
	public JToken item { get; set; }

	// Gets the node type for this JToken.
	public string type { get; set; }

}