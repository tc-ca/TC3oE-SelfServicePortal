namespace OurAzureDevops.Models.ServiceEndpoint;

public record IdentityRef
{
	// This field contains zero or more interesting links about the graph subject. These links may be invoked to obtain additional relationships or more detailed information about this graph subject.
	public ReferenceLinks _links { get; set; }

	// The descriptor is the primary way to reference the graph subject while the system is running. This field will uniquely identify the same graph subject across both Accounts and Organizations.
	public string descriptor { get; set; }

	// Deprecated - Can be retrieved by querying the Graph user referenced in the "self" entry of the IdentityRef "_links" dictionary
	public string directoryAlias { get; set; }

	// This is the non-unique display name of the graph subject. To change this field, you must alter its value in the source provider.
	public string displayName { get; set; }

	public string id { get; set; }

	// Deprecated - Available in the "avatar" entry of the IdentityRef "_links" dictionary
	public string imageUrl { get; set; }

	// Deprecated - Can be retrieved by querying the Graph membership state referenced in the "membershipState" entry of the GraphUser "_links" dictionary
	public bool inactive { get; set; }

	// Deprecated - Can be inferred from the subject type of the descriptor (Descriptor.IsAadUserType/Descriptor.IsAadGroupType)
	public bool isAadIdentity { get; set; }

	// Deprecated - Can be inferred from the subject type of the descriptor (Descriptor.IsGroupType)
	public bool isContainer { get; set; }

	public bool isDeletedInOrigin { get; set; }
	
	// Deprecated - not in use in most preexisting implementations of ToIdentityRef
	public string profileUrl { get; set; }

	// Deprecated - use Domain+PrincipalName instead
	public string uniqueName { get; set; }

	// This url is the full route to the source resource of this graph subject.
	public string url { get; set; }

}