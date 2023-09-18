using Azure.Core;

namespace SelfServicePortal.Core.Extensions;

public static class LocationExtensions {
	#nullable enable
	public static string? GetNamingPrefix(this AzureLocation Location)
	{
		if (Location == AzureLocation.CanadaCentral)
			return "CACN";
		if (Location == AzureLocation.CanadaEast)
			return "CAEA";
		return null;
	}
}