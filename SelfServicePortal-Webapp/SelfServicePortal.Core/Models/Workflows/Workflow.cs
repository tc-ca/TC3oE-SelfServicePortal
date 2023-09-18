namespace SelfServicePortal.Core.Models.Workflows;

public abstract class Workflow
{
	abstract public string Id {get; }

	public static Dictionary<string, Type> WorkflowLookup = new[]{
		typeof(CreateResourceGroupWorkflow),
		typeof(CreateSecurityGroupWorkflow),
		typeof(AddSecurityGroupOwnersWorkflow),
		typeof(AddSecurityGroupMembersWorkflow),
		typeof(AddPolicyExemptionWorkflow),
		typeof(AddTagsWorkflow),
		typeof(CreateServiceConnectionWorkflow),
		typeof(CreateSecurityGroupRoleAssignmentWorkflow),
	}.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First());

	public abstract Task Complete(IServiceProvider services);
}