namespace OurAzure.Api.Models.Policy;

public class PolicyAssignment : Resource
{
	public class Properties {
		public string displayName {get; init;}
		public string policyDefinitionId {get; init;}
		public string scope {get; init;}
		public string[] notScopes {get; init;}
		public Dictionary<string, object> parameters {get; init;}
		public Dictionary<string, object> metadata {get; init;}
		public string enforcementMode {get; init;}
		public string[] nonComplianceMessages {get; init;}
	}
	public Identity identity {get; init;}
	public Properties properties {get; init;}
}