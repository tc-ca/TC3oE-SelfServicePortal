namespace OurAzure.Api.Models.Authorization;

public record RoleAssignmentPropertiesWithScope
{
	// The principal ID.
	public string principalId { get; set; }

	// The role definition ID.
	public string roleDefinitionId { get; set; }

	// The role assignment scope.
	public string scope { get; set; }
}