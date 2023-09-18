using System.Security.Claims;

namespace SelfServicePortal.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
	public static string GetName(this ClaimsPrincipal User)
	{
		return User.Identity!.Name!;
	}
}