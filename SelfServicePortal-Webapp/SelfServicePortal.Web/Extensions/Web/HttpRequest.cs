using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace SelfServicePortal.Web.Extensions;

public static class HttpRequestExtensions
{
	public static string GetCulture(this HttpRequest request)
	{
		return request.HttpContext.Features.Get<IRequestCultureFeature>()!
		.RequestCulture.Culture.TwoLetterISOLanguageName;
	}

}