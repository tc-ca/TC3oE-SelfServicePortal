namespace OurAzureDevops.Models.UserEntitlements;
public record ProjectEntitlement
{
	// Assignment Source (e.g. Group or Unknown).
	public AssignmentSource assignmentSource { get; init; }

	// Project Group (e.g. Contributor, Reader etc.)
	public Group group { get; init; }

	// Whether the user is inheriting permissions to a project through a Azure DevOps or AAD group membership.
	public ProjectPermissionInherited projectPermissionInherited { get; init; }

	// Project Ref
	public ProjectRef projectRef { get; init; }

	// Team Ref.
	public TeamRef[] teamRefs { get; init; }

	public bool IsAdmin => group.groupType switch
	{
		GroupType.projectContributor => true,
		GroupType.projectAdministrator => true,
		GroupType.custom => true,
		_ => false
	};
}