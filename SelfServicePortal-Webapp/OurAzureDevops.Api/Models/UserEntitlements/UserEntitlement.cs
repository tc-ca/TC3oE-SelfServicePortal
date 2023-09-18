using OurAzureDevops.Models.Graph;

namespace OurAzureDevops.Models.UserEntitlements;
public record UserEntitlement
{
	// User's access level denoted by a license.
	public AccessLevel accessLevel {get; init;}

	// [Readonly] Date the user was added to the collection.
	public string dateCreated {get; init;}

	// [Readonly] GroupEntitlements that this user belongs to.
	public GroupEntitlement[] groupAssignments {get; init;}

	// The unique identifier which matches the Id of the Identity associated with the GraphMember.
	public string id {get; init;}

	// [Readonly] Date the user last accessed the collection.
	public string lastAccessedDate {get; init;}

	// Relation between a project and the user's effective permissions in that project.
	public ProjectEntitlement[] projectEntitlements {get; init;}

	// User reference.
	public GraphUser user {get; init;}
}