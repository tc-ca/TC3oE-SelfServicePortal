namespace OurAzureDevops.Models.Security;

public record AceExtendedInformation
{
	// This is the combination of all of the explicit and inherited permissions for this identity on this token. These are the permissions used when determining if a given user has permission to perform an action.
	public int effectiveAllow { get; init; }

	// This is the combination of all of the explicit and inherited permissions for this identity on this token. These are the permissions used when determining if a given user has permission to perform an action.
	public int effectiveDeny { get; init; }

	// These are the permissions that are inherited for this identity on this token. If the token does not inherit permissions this will be 0. Note that any permissions that have been explicitly set on this token for this identity, or any groups that this identity is a part of, are not included here.
	public int inheritedAllow { get; init; }

	// These are the permissions that are inherited for this identity on this token. If the token does not inherit permissions this will be 0. Note that any permissions that have been explicitly set on this token for this identity, or any groups that this identity is a part of, are not included here.
	public int inheritedDeny { get; init; }
}