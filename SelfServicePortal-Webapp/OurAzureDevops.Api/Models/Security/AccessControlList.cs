namespace OurAzureDevops.Models.Security;

public record AccessControlList
{
	// Storage of permissions keyed on the identity the permission is for.
	public Dictionary<string, AccessControlEntry> acesDictionary { get; init; }

	// True if this ACL holds ACEs that have extended information.
	public bool includeExtendedInfo { get; init; }

	// True if the given token inherits permissions from parents.
	public bool inheritPermissions { get; init; }

	// The token that this AccessControlList is for.
	public string token { get; init; }

}