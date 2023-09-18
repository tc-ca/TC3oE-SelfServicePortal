
namespace OurAzure.Api.Models.Authorization;

public record RoleAssignment
{
	// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles
	public static readonly string Contributor = "b24988ac-6180-42a0-ab88-20f7382dd24c";
	public static readonly string Owner = "8e3af657-a8ff-443c-a75c-2fe8c4bcb635";
	public static readonly string Reader = "acdd72a7-3385-48ef-bd42-f606fba81ae7";

	// The role assignment ID.
	public string id { get; set; }

	// The role assignment name.
	public string name { get; set; }

	// Role assignment properties.
	public RoleAssignmentPropertiesWithScope properties { get; set; }

	// The role assignment type.
	public string type { get; set; }
}