using System.Text.Json;

namespace OurAzure.Api.Models;

public class Response
{
	public string name {get; init;}
	public int httpStatusCode {get; init;}
	public object content {get; init;}
	public int contentLength {get; init;}
	public Dictionary<string, string> headers {get; init;}
	public T GetContent<T>() {
		return ((JsonElement) content).ToObject<T>();
	}
}