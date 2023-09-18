namespace OurAzureDevops.Models.Graph;

public record GraphDescriptorResult
{
	// This field contains zero or more interesting links about the graph descriptor. These links may be invoked to obtain additional relationships or more detailed information about this graph descriptor.
	public ReferenceLinks _links { get; init; }

	// nodesc
	public string value { get; init; }
}