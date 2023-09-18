namespace OurAzureDevops.Models.ServiceEndpoint;

public record JToken
{
	// Get the first child token of this token.
	public JToken first { get; set; }

	// Gets a value indicating whether this token has child tokens.
	public bool hasValues { get; set; }

	// Represents an abstract JSON token.
	public JToken item { get; set; }

	// Get the last child token of this token.
	public JToken last { get; set; }

	// Gets the next sibling token of this node.
	public JToken next { get; set; }

	// Gets or sets the parent.
	public string parent { get; set; }

	// Gets the path of the JSON token.
	public string path { get; set; }

	// Gets the previous sibling token of this node.
	public JToken previous { get; set; }

	// Gets the root JToken of this JToken.
	public JToken root { get; set; }

	// Gets the node type for this JToken.
	public string type { get; set; }

}