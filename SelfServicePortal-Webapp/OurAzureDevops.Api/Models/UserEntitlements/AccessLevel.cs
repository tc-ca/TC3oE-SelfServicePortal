namespace OurAzureDevops.Models.UserEntitlements;

public record AccessLevel
{
	// Type of Account License (e.g. Express, Stakeholder etc.)
	public AccountLicenseType accountLicenseType {get; init;}

	// Assignment Source of the License (e.g. Group, Unknown etc.
	public AssignmentSource assignmentSource {get; init;}

	// Display name of the License
	public string licenseDisplayName {get; init;}

	// Licensing Source (e.g. Account. MSDN etc.)
	public LicensingSource licensingSource {get; init;}

	// Type of MSDN License (e.g. Visual Studio Professional, Visual Studio Enterprise etc.)
	public MsdnLicenseType msdnLicenseType {get; init;}

	// User status in the account
	public AccountUserStatus status {get; init;}

	// Status message.
	public string statusMessage {get; init;}
}