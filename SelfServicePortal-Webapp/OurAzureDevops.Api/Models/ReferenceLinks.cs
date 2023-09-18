namespace OurAzureDevops.Models;
public record ReferenceLinks
{
	// The readonly view of the links. Because Reference links are readonly, we only want to expose them as read only.
	public object links {get; init;}
}