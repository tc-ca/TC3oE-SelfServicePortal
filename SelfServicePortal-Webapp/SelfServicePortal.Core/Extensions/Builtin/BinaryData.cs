using System.Text;

namespace SelfServicePortal.Core.Extensions;

public static class BinaryDataExtensions
{
	public static string ConvertToUTF8FromBase64(this BinaryData data) 
	{
		return Encoding.UTF8.GetString(Convert.FromBase64String(data.ToString()));
	}
}