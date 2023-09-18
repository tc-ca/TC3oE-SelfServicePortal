using System.Net.Http.Headers;
using OurAzureDevops.Models;
using OurAzureDevops.Models.Graph;
using OurAzureDevops.Models.Security;
using OurAzureDevops.Models.UserEntitlements;
using OurAzureDevops.Models.ServiceEndpoint;

namespace OurAzureDevops;

public class AzureDevopsRestClient : RestClient
{
	public readonly string DevBaseUrl;
	public readonly string VsspsBaseUrl;

	public readonly string VsaexBaseUrl;
	public readonly string DevOpsOrgName;

	public AzureDevopsRestClient(
		string personalAccessToken,
		string devopsOrgName
	) : base(new AuthenticationHeaderValue("Basic",
				Convert.ToBase64String(
					System.Text.ASCIIEncoding.ASCII.GetBytes(
						string.Format("{0}:{1}", "", personalAccessToken)))))
	{
		DevOpsOrgName = devopsOrgName;
		DevBaseUrl = $"https://dev.azure.com/{devopsOrgName}/_apis";
		VsspsBaseUrl = $"https://vssps.dev.azure.com/{devopsOrgName}/_apis";
		VsaexBaseUrl = $"https://vsaex.dev.azure.com/{devopsOrgName}/_apis";
	}

	public async IAsyncEnumerable<Project> GetProjects()
	{
		await foreach (var x in GetAsyncPaginated<Project>($"{DevBaseUrl}/projects?api-version=6.0"))
			yield return x;
	}

	public async Task<TeamProject> GetProject(string projectId)
	{
		var url = $"{DevBaseUrl}/_apis/projects/{projectId}?api-version=6.0";
		return await GetAsync<TeamProject>(url);
	}

	public async IAsyncEnumerable<SecurityNamespaceDescription> GetSecurityNamespaces()
	{
		await foreach (var x in GetAsyncPaginated<SecurityNamespaceDescription>($"{DevBaseUrl}/securitynamespaces?api-version=6.0"))
			yield return x;
	}

	public async Task<PagedGraphMemberList> GetUserEntitlementsByName(string name, UserEntitlementProperty[] select)
	{
		var url = $"{VsaexBaseUrl}/UserEntitlements";
		url += $"?$filter=name eq '{name}'";
		url += "&$orderBy=name Ascending";
		url += "&select=" + string.Join(",", select);
		var rtn = await GetAsync<PagedGraphMemberList>(url);
		return rtn;
	}

	public async Task<GraphDescriptorResult> GetGraphDescriptor(string storageKey)
	{
		return await GetAsync<GraphDescriptorResult>($"{VsspsBaseUrl}/graph/descriptors/{storageKey}?api-version=6.0-preview.1");
	}

	public async Task<GraphMembership> GetMemberships(string subjectDescriptor)
	{
		return await GetAsync<GraphMembership>($"{DevBaseUrl}/graph/memberships/{subjectDescriptor}");
	}

	public async Task<GraphGroup> GetGroup(string groupDescriptor)
	{
		return await GetAsync<GraphGroup>($"{DevBaseUrl}/graph/groups/{groupDescriptor}");
	}

	public async Task<AccessControlList> GetAccessControlLists(string securityNamespace)
	{
		return await GetAsync<AccessControlList>($"{DevBaseUrl}/accesscontrollists/{securityNamespace}?api-version=6.0");
	}

	public async IAsyncEnumerable<GraphUser> GetUsers(string? scopeDescriptor = null)
	{
		var url = $"{VsspsBaseUrl}/graph/users?api-version=6.0-preview.1";
		if (scopeDescriptor != null)
			url += $"&scopeDescriptor={scopeDescriptor}";
		await foreach (var x in GetAsyncPaginated<GraphUser>(url))
		{
			yield return x;
		}
	}

	public async Task<IEnumerable<ProjectEntitlement>> GetProjectsForUser(string name)
	{
		return (await GetUserEntitlementsByName(name, new[] { UserEntitlementProperty.projects })).items
			.Where(user => user.user.principalName.ToLowerInvariant() == name.ToLowerInvariant())
			.SelectMany(x => x.projectEntitlements);
	}


	// automatic creation mode has some funky permissions for creating the AAD objects
	// better to manually create them since we know the selfserviceportal has permissions to do so
	// might cause the new service principals to not be auto-deleted when their corresponding service connections are tho
	// can make a cleanup script to scan for this later if we really care.
	[Obsolete("Use CreateManualServiceConnection instead")]
	public async Task<ServiceEndpoint> CreateAutomaticServiceConnection(
		string projectId,
		string projectName,
		string tenantId,
		string subscriptionId,
		string subscriptionName,
		string serviceConnectionName,
		string scope
	)
	{
		var url = $"{DevBaseUrl}/_apis/serviceendpoint/endpoints?api-version=6.0-preview.4";
		return await PostAsync<ServiceEndpoint>(url, new RequestBody
		{
			authorization = new()
			{
				parameters = new
				{
					tenantId,
					scope,
					authenticationType = "spnKey",
					serviceprincipalid = "",
					serviceprincipalkey = "",
				},
				scheme = "ServicePrincipal",
			},
			createdBy = new(),
			data = new
			{
				environment = "AzureCloud",
				scopeLevel = "Subscription",
				subscriptionId = subscriptionId,
				subscriptionName = subscriptionName,
				resourceGroupName = "",
				mlWorkspaceName = "",
				mlWorkspaceLocation = "",
				managementGroupId = "",
				managementGroupName = "",
				oboAuthorization = "",
				creationMode = "Automatic",
				azureSpnRoleAssignmentId = "",
				azureSpnPermissions = "",
				spnObjectId = "",
				appObjectId = "",
				resourceId = "",
			},
			id = new Guid().ToString(),
			isShared = false,
			name = serviceConnectionName,
			owner = "library",
			type = "azurerm",
			url = "https://management.azure.com/",
			administratorsGroup = null!,
			description = "",
			groupScopeId = null!,
			operationStatus = null!,
			readersGroup = null!,
			serviceEndpointProjectReferences = new ServiceEndpointProjectReference[]{
				new(){
					description = "",
					name = serviceConnectionName,
					projectReference = new(){
						id = projectId,
						name = projectName,
					}
				},
			},
		});
	}

	public async Task<ServiceEndpoint> CreateManualServiceConnection(
		string projectId,
		string projectName,
		string tenantId,
		string subscriptionId,
		string subscriptionName,
		string serviceConnectionName,
		string servicePrincipalId,
		string servicePrincipalKey,
		string scope
	)
	{
		var url = $"{DevBaseUrl}/serviceendpoint/endpoints?api-version=6.0-preview.4";
		return await PostAsync<ServiceEndpoint>(url, new RequestBody
		{
			authorization = new()
			{
				parameters = new
				{
					tenantid=tenantId, // lowercase matters here because azure devops api is evil
					scope,
					authenticationType = "spnKey",
					serviceprincipalid = servicePrincipalId,
					serviceprincipalkey = servicePrincipalKey,
				},
				scheme = "ServicePrincipal",
			},
			createdBy = new(),
			data = new
			{
				environment = "AzureCloud",
				scopeLevel = "Subscription",
				subscriptionId = subscriptionId,
				subscriptionName = subscriptionName,
			},
			isShared = false,
			isReady = true,
			name = serviceConnectionName,
			type = "azurerm",
			url = "https://management.azure.com/",
			serviceEndpointProjectReferences = new ServiceEndpointProjectReference[]{
				new(){
					// description = "",
					name = serviceConnectionName,
					projectReference = new(){
						id = projectId,
						name = projectName,
					}
				},
			},
		});
	}
}