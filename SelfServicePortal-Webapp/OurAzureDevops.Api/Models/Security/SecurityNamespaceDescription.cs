namespace OurAzureDevops.Models.Security;

public record SecurityNamespaceDescription
{
	// The list of actions that this Security Namespace is responsible for securing.
	public ActionDefinition[] actions { get; init; }

	// This is the dataspace category that describes where the security information for this SecurityNamespace should be stored.
	public string dataspaceCategory { get; init; }

	// This localized name for this namespace.
	public string displayName { get; init; }

	// If the security tokens this namespace will be operating on need to be split on certain character lengths to determine its elements, that length should be specified here. If not, this value will be -1.
	public int elementLength { get; init; }

	// This is the type of the extension that should be loaded from the plugins directory for extending this security namespace.
	public string extensionType { get; init; }

	// If true, the security namespace is remotable, allowing another service to proxy the namespace.
	public bool isRemotable { get; init; }

	// This non-localized for this namespace.
	public string name { get; init; }

	// The unique identifier for this namespace.
	public string namespaceId { get; init; }

	// The permission bits needed by a user in order to read security data on the Security Namespace.
	public int readPermission { get; init; }

	// If the security tokens this namespace will be operating on need to be split on certain characters to determine its elements that character should be specified here. If not, this value will be the null character.
	public string separatorValue { get; init; }

	// Used to send information about the structure of the security namespace over the web service.
	public int structureValue { get; init; }

	// The bits reserved by system store
	public int systemBitMask { get; init; }

	// If true, the security service will expect an ISecurityDataspaceTokenTranslator plugin to exist for this namespace
	public bool useTokenTranslator { get; init; }

	// The permission bits needed by a user in order to modify security data on the Security Namespace.
	public int writePermission { get; init; }


}