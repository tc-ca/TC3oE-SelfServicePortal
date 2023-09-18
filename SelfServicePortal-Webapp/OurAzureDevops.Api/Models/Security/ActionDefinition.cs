namespace OurAzureDevops.Models.Security;

public record ActionDefinition
{
	// The bit mask integer for this action. Must be a power of 2.
	public int bit { get; init; }

	// The localized display name for this action.
	public string displayName { get; init; }

	// The non-localized name for this action.
	public string name { get; init; }

	// The namespace that this action belongs to. This will only be used for reading from the database.
	public string namespaceId { get; init; }
}