namespace OurAzureDevops.Models;

public record WebApiTeamRef
{
	// Team (Identity) Guid. A Team Foundation ID.
	public string id { get; set; }

	// Team name
	public string name { get; set; }

	// Team REST API Url
	public string url { get; set; }
}