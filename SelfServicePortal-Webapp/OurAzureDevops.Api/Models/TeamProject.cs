namespace OurAzureDevops.Models;

public record TeamProject
{
	// The links to other objects related to this object.
	public ReferenceLinks _links { get; set; }

	// Project abbreviation.
	public string abbreviation { get; set; }

	// Set of capabilities this project has (such as process template & version control).
	public object capabilities { get; set; }

	// The shallow ref to the default team.
	public WebApiTeamRef defaultTeam { get; set; }

	// Url to default team identity image.
	public string defaultTeamImageUrl { get; set; }

	// The project's description (if any).
	public string description { get; set; }

	// Project identifier.
	public string id { get; set; }

	// Project last update time.
	public string lastUpdateTime { get; set; }

	// Project name.
	public string name { get; set; }

	// Project revision.
	public int revision { get; set; }

	// Project state.
	public ProjectState state { get; set; }

	// Url to the full version of the object.
	public string url { get; set; }

	// Project visibility.
	public ProjectVisibility visibility { get; set; }
}