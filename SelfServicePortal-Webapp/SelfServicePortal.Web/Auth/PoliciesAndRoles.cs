using Microsoft.AspNetCore.Authorization;

namespace SelfServicePortal.Web.Auth;

public static class AppRoles
{
	public const string Admin = "admin";
}
public static class AuthorizationPolicies
{
	public const string ApplicationAdministrator = nameof(ApplicationAdministrator);

	public static void Configure(AuthorizationOptions options)
	{
		options.AddPolicy(
			AuthorizationPolicies.ApplicationAdministrator,
			policy => policy.RequireRole(AppRoles.Admin) //|| policy.RequireClaim("roles", AppRoles.Admin)
		);
	}
}