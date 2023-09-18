namespace OurAzureDevops.Models.UserEntitlements;

public record TeamRef
{
	// Team ID
	public string id { get; init; }

	// Team Name
	public string name { get; init; }
}