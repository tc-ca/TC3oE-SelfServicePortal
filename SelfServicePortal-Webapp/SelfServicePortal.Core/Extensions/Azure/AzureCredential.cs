using System.Net.Http.Headers;
using Azure.Core;

namespace SelfServicePortal.Core.Extensions;

public static class AzureCredentialExtensions
{
	public static AuthenticationHeaderValue GetAuthHeader(this TokenCredential cred, string scope)
	{
		var token = cred.GetToken(new Azure.Core.TokenRequestContext(new[] { scope }), default).Token;
		var header = new AuthenticationHeaderValue("bearer", token);
		return header;
	}
}