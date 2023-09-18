namespace OurAzureDevops.Models.UserEntitlements;
public record Group
{
	// Display Name of the Group
	public string displayName {get; init;}

	// Group Type
	public GroupType groupType {get; init;}
}