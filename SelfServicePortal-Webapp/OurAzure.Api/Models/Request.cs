namespace OurAzure.Api.Models;

public class Request
{
	public string httpMethod {get; init;}
	public string name {get; init;}
	public string url {get; init;}
	public Dictionary<string, string> requestHeaderDetails {get; init;}
}