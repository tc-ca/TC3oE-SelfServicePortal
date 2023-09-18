using OurAzureDevops.Models;

namespace OurAzureDevops;

public record GraphMembership
{
	// This field contains zero or more interesting links about the graph membership. These links may be invoked to obtain additional relationships or more detailed information about this graph membership.
	public ReferenceLinks _links { get; init; }

	public string containerDescriptor { get; init; }

	public string memberDescriptor { get; init; }

}