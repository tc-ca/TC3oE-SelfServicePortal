namespace OurAzureDevops.Models;

public record Project
{

	public Guid id {get; init;}
	public string name {get; init;}
	public string description {get; init;}
	public Uri url {get; init;}
	public string state {get; init;}
	public int revision {get; init;}
	public string visibility {get; init;}
	public DateTime lastUpdateTime {get; init;}
}