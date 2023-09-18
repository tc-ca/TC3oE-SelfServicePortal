namespace OurAzureDevops.Models.UserEntitlements;
public enum AccountUserStatus
{
	//User has signed in at least once to the VSTS account
	active,

	//User is removed from the VSTS account by the VSTS account admin
	deleted,

	//User cannot sign in; primarily used by admin to temporarily remove a user due to absence or license reallocation
	disabled,

	//User can sign in; primarily used when license is in expired state and we give a grace period
	expired,

	none,

	//User is invited to join the VSTS account by the VSTS account admin, but has not signed up/signed in yet
	pending,

	//User is disabled; if reenabled, they will still be in the Pending state
	pendingDisabled,
}