namespace OurAzureDevops.Models;

public enum ProjectState
{
	// All projects regardless of state.
	all,

	// Project has been queued for creation, but the process has not yet started.
	createPending,

	// Project has been deleted.
	deleted,

	// Project is in the process of being deleted.
	deleting,

	// Project is in the process of being created.
	NEW,

	// Project has not been changed.
	unchanged,

	// Project is completely created and ready to use.
	wellFormed,
}