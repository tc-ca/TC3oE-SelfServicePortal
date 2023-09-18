using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace SelfServicePortal.Web.Middleware;

// Custom class since RouteDataRequestCultureProvider doesn't work.
// Route values aren't available I guess, so we grab from the request path.
// https://github.com/aspnet/Localization/blob/master/src/Microsoft.AspNetCore.Localization.Routing/RouteDataRequestCultureProvider.cs
public class UrlRequestCultureProvider : RequestCultureProvider
{
	// Suppress warning about "different" return type nullability.
	// https://github.com/dotnet/roslyn/issues/40757
#pragma warning disable CS8609
	public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext Context)
	{
		var culture = Context?.Request?.Path.Value?.Split('/').ElementAtOrDefault(1);
		if (culture == null)
			return Task.FromResult(new ProviderCultureResult(DefaultCulture));

		return Task.FromResult(
			IsValidCulture(culture)
			? new ProviderCultureResult(culture)
			: new ProviderCultureResult(DefaultCulture)
		);
	}

	private bool IsValidCulture(string culture)
	{
		return Options!.SupportedCultures!.Any(x => x.TwoLetterISOLanguageName.Equals(culture, StringComparison.InvariantCultureIgnoreCase));
	}

	private string DefaultCulture => Options!.DefaultRequestCulture.Culture.TwoLetterISOLanguageName;
}