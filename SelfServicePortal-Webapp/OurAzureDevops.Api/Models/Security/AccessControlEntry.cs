namespace OurAzureDevops.Models.Security;

public record AccessControlEntry
{
	// The set of permission bits that represent the actions that the associated descriptor is allowed to perform.
	public int allow { get; init; }

	// The set of permission bits that represent the actions that the associated descriptor is not allowed to perform.
	public int deny { get; init; }

	// The descriptor for the user this AccessControlEntry applies to.
	public IdentityDescriptor descriptor { get; init; }

	// This value, when set, reports the inherited and effective information for the associated descriptor. This value is only set on AccessControlEntries returned by the QueryAccessControlList(s) call when its includeExtendedInfo parameter is set to true.
	public AceExtendedInformation extendedInfo { get; init; }

}