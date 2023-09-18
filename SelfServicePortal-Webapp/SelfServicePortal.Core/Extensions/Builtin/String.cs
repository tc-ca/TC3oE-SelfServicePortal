
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SelfServicePortal.Core.Extensions;

public static class StringExtensions
{
	public static string[] SplitEmails(this string self)
	{
		var regex = new Regex(@"([^\s,;]+)");
		var matches = regex.Matches(self);
		return matches
			.Select(match => match.Groups[1].Value)
			.ToArray();
	}

	public static Guid CreateGuidFromHash(this string self)
	{
		using var md5 = MD5.Create();
		var hash = md5.ComputeHash(Encoding.Default.GetBytes(self));
		return new Guid(hash);
	}
}