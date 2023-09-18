namespace OurAzure.Api.Models.Policy;

public class PolicyExemption : Resource
{
	public class Properties {
		public string description {get; init;}
		public string displayName {get; init;}
		public string exemptionCategory {get; init;}
		public string policyAssignmentId {get; init;}
		public string[] policyDefinitionReferenceIds {get; init;}
		public Dictionary<string, object> metadata {get; init;}
	}
	public Identity identity {get; init;}
	public Properties properties {get; init;}
}