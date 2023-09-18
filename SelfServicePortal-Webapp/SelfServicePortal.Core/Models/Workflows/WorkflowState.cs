namespace SelfServicePortal.Core.Models;

public static class WorkflowState {
	public static readonly string PendingApproval = "PendingApproval";
	public static readonly string Approved = "Approved";
	public static readonly string Denied = "Denied";
	public static readonly string Completed = "Completed";
	public static readonly string Failed = "Failed";

}
