namespace OurAzureDevops.Models.UserEntitlements;

public record ProjectRef
{
	// Project ID.
	public string id { get; init; }

	// Project Name.
	public string name { get; init; }
}