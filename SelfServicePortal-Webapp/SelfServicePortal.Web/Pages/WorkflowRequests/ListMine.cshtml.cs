using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SelfServicePortal.Web.Services;
using SelfServicePortal.Core.Models.TableStorage;

namespace SelfServicePortal.Web.Pages;

public class ListMineModel : PageModel
{
	private readonly Services<ListMineModel> _services;

	public ListMineModel(Services<ListMineModel> services)
	{
		_services = services;
	}

	public IEnumerable<WorkflowRequestTableEntry> Requests {get; private set;}

	public async Task OnGet()
	{
		Requests = await _services.WorkflowClient.GetRequests(User.GetName())
			.OrderByDescending(x => x.Request.DateInitiated)
			.ToListAsync();
	}
}