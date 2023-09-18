using OurAzure.Api.Models;
using OurAzure.Api.Models.Policy;
using OurAzure.Api.Models.Authorization;
using System.Text.RegularExpressions;

namespace OurAzure.Api.Services;

/**
 * Custom client for making REST calls to the Azure portal API.
 * Most of these methods are created by looking at the networking tab when just using the azure portal in the browser.
 */
public class AzureRestClient : RestClient
{
	public AzureRestClient(Azure.Core.TokenCredential cred) : base(cred)
	{
	}

	public async Task<Role> GetRoleDefinitionAsync(string subscriptionId, string roleName)
	{
		var url = $"https://management.azure.com/subscriptions/{subscriptionId}/providers/Microsoft.Authorization/roleDefinitions?api-version=2018-01-01-preview&$filter=roleName eq '{roleName}'";
		var resp = await GetAsync<ValueArray<Role>>(url);
		return resp.value.First();
	}

	public async Task CreateRoleAssignmentAsync(string scope, string principalId, string roleDefinitionId, string principalType) 
	{
		// https://aka.ms/docs-principaltype
		var url = $"https://management.azure.com{scope}/providers/Microsoft.Authorization/roleAssignments/{Guid.NewGuid()}?api-version=2018-09-01-preview";
		object payload = new
		{
			properties = new
			{
				roleDefinitionId,
				principalId,
				principalType,
			}
		};
		await PutAsync<object>(url, payload);
	}

	private async IAsyncEnumerable<Response> InvokeBatchRequestAsync(BatchRequest request, int batchSize = 20)
	{
		// Convert batch request into multiple requests of size batchSize
		var requests = request.requests
			.ToList()
			.SplitList(batchSize)
			.Select(reqs => new BatchRequest(){
				requests = reqs.ToArray()
			});

		var url = $"https://management.azure.com/batch?api-version=2020-06-01";

		foreach (var req in requests)
		{
			var resp = await PostAsync<BatchResponse>(url, req);
			foreach (var r in resp.responses)
			{
				yield return r;
			}
		}
	}

	public async IAsyncEnumerable<PolicyAssignment> GetPolicyAssignmentsAsync(string policyDefinitionId, string[] subscriptionIds, int limit = 1000)
	{
		var request = new BatchRequest()
		{
			requests = subscriptionIds.Select(sub => new Request()
			{
				httpMethod = "GET",
				name = sub,
				requestHeaderDetails = new Dictionary<string, string>{
					{"commandName", "Microsoft_Azure_Policy."}
				},
				url = "https://management.azure.com/subscriptions/"
					+ sub
					+ "/providers/Microsoft.Authorization/policyAssignments"
					+ "?api-version=2021-06-01"
					+ $"&$filter=policyDefinitionId eq '{policyDefinitionId}'"
					+ $"&$top={limit}"
			}).ToArray()
		};

		await foreach (var response in InvokeBatchRequestAsync(request))
		{
			foreach (var policyAssignment in response.GetContent<ValueArray<PolicyAssignment>>().value)
			{
				yield return policyAssignment;
			}
		}
	}

	public async Task<PolicyExemption.Properties> CreatePolicyExemptionAsync(
		string resourceId,
		PolicyExemption.Properties properties
	)
	{
		var url = "https://management.azure.com"
				+ resourceId 
				+ "/providers/Microsoft.Authorization/policyExemptions/"
				+ Guid.NewGuid()
				+ "?api-version=2020-07-01-preview";
		
		var resp = await PutAsync<PolicyExemption.Properties>(url, new { properties });
		return resp;
	}

	public async Task<RoleAssignmentListResult> GetRoleAssignments(string subscriptionId, string assigneeObjectId)
	{
		var url = "https://management.azure.com"
			+ subscriptionId
			+ "/providers/Microsoft.Authorization/roleAssignments"
			+ $"?$filter=assignedTo('{assigneeObjectId}')"
			+ "&api-version=2020-04-01-preview";
		var resp = await GetAsync<RoleAssignmentListResult>(url);
		return resp;
	}

	public async IAsyncEnumerable<string> GetResoruceGroupIdsForUser(string userObjectId, params string[] subscriptionIds)
	{
		var validScopes = new Regex(@"^/subscriptions/[^/]+/resourceGroups/[^/]+$");
		var validRoles = new[]{RoleAssignment.Owner, RoleAssignment.Contributor};
		foreach (var sub in subscriptionIds)
		{
			var assignments = await GetRoleAssignments(sub, userObjectId);
			foreach (var ass in assignments.value)
			{
				if (validScopes.IsMatch(ass.properties.scope))
				{
					foreach (var role in validRoles)
					{
						if (ass.properties.roleDefinitionId == $"{sub}/providers/Microsoft.Authorization/roleDefinitions/{role}")
						{
							yield return ass.properties.scope;
							break;
						}
					}
				}
			}
		}
	}
}