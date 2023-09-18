using OurAzureDevops.Models.Graph;

namespace OurAzureDevops.Models.UserEntitlements;
public record GroupEntitlement
{
	// Member reference.
	public GraphGroup group {get; init;}

	// The unique identifier which matches the Id of the GraphMember.
	public string id {get; init;}

	// [Readonly] The last time the group licensing rule was executed (regardless of whether any changes were made).
	public string lastExecuted {get; init;}

	// License Rule.
	public AccessLevel licenseRule {get; init;}

	// Group members. Only used when creating a new group.
	public UserEntitlement[] members {get; init;}

	// Relation between a project and the member's effective permissions in that project.
	public ProjectEntitlement[] projectEntitlements {get; init;}

	// The status of the group rule.
	public GroupLicensingRuleStatus status {get; init;}

}