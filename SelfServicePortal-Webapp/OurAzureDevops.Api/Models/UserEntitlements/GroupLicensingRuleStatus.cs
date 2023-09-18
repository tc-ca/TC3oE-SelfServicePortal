namespace OurAzureDevops.Models.UserEntitlements;

public enum GroupLicensingRuleStatus
{
	// Rule is applied
	applied,

	// Rule is created or updated, but apply is pending
	applyPending,

	// The group rule was incompatible
	incompatible,

	// Rule failed to apply unexpectedly and should be retried
	unableToApply,
}