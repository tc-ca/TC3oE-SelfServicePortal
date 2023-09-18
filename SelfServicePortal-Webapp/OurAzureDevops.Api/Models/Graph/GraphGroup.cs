namespace OurAzureDevops.Models.Graph;
public record GraphGroup
{
	// This field contains zero or more interesting links about the graph subject. These links may be invoked to obtain additional relationships or more detailed information about this graph subject.
	public ReferenceLinks _links { get; init; }

	// A short phrase to help human readers disambiguate groups with similar names
	public string description { get; init; }

	// The descriptor is the primary way to reference the graph subject while the system is running. This field will uniquely identify the same graph subject across both Accounts and Organizations.
	public string descriptor { get; init; }

	// This is the non-unique display name of the graph subject. To change this field, you must alter its value in the source provider.
	public string displayName { get; init; }

	// This represents the name of the container of origin for a graph member. (For MSA this is "Windows Live ID", for AD the name of the domain, for AAD the tenantID of the directory, for VSTS groups the ScopeId, etc)
	public string domain { get; init; }

	// [Internal Use Only] The legacy descriptor is here in case you need to access old version IMS using identity descriptor.
	public string legacyDescriptor { get; init; }

	// The email address of record for a given graph member. This may be different than the principal name.
	public string mailAddress { get; init; }

	// The type of source provider for the origin identifier (ex:AD, AAD, MSA)
	public string origin { get; init; }

	// The unique identifier from the system of origin. Typically a sid, object id or Guid. Linking and unlinking operations can cause this value to change for a user because the user is not backed by a different provider and has a different unique id in the new provider.
	public string originId { get; init; }

	// This is the PrincipalName of this graph member from the source provider. The source provider may change this field over time and it is not guaranteed to be immutable for the life of the graph member by VSTS.
	public string principalName { get; init; }

	// This field identifies the type of the graph subject (ex: Group, Scope, User).
	public string subjectKind { get; init; }

	// This url is the full route to the source resource of this graph subject.
	public string url { get; init; }
}