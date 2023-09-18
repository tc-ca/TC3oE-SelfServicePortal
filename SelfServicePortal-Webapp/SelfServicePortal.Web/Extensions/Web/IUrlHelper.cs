using Microsoft.AspNetCore.Mvc;

namespace SelfServicePortal.Web.Extensions;

public static class IUrlHelperExtensions {
	public static string? PageSameCulture(this IUrlHelper helper, string page)
	{
		return helper.Page(page, null, new { culture = helper.ActionContext.HttpContext.Request.GetCulture()});
	}
}