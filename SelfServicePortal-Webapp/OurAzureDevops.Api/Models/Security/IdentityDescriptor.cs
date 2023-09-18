namespace OurAzureDevops.Models.Security;

public class IdentityDescriptor
{
	// The unique identifier for this identity, not exceeding 256 chars, which will be persisted.
	public string identifier { get; init; }

	// Type of descriptor (for example, Windows, Passport, etc.).
	public string identityType { get; init; }

}