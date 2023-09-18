using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;

namespace SelfServicePortal.Web.Extensions;

public static class PageModelExtensions
{
	/**
	 * Helper shortcut method for one of the more common redirect operations.
	 */
	public static IActionResult RedirectToPageSameCulture(this PageModel model, string pageName)
	{
		return model.ApplyCulture(model.RedirectToPage(pageName));
	}

	/**
	 * Add the current culture to the redirect/whatever.
	 * When used, ensures that methods like RedirectToPage preserve the user's culture.
	 */
	public static IActionResult ApplyCulture(this PageModel model, dynamic redirect)
	{
		if (redirect.RouteValues == null)
			redirect.RouteValues = new RouteValueDictionary();
		redirect.RouteValues["culture"] =
			redirect.RouteValues["culture"]
			?? model.Request.GetCulture();
		return redirect;
	}
}