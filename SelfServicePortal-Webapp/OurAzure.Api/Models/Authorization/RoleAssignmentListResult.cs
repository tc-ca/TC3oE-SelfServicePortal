namespace OurAzure.Api.Models.Authorization;

public record RoleAssignmentListResult
{
	// The URL to use for getting the next set of results.
	public string nextLink { get; set; }

	// Role assignment list.
	public RoleAssignment[] value { get; set; }

}