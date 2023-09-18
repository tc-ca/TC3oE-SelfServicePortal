using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;


// https://stackoverflow.com/a/59283426/11141271
// https://stackoverflow.com/questions/60397920/razorpages-anchortaghelper-does-not-remove-index-from-href
// https://talagozis.com/en/asp-net-core/razor-pages-localisation-seo-friendly-urls
namespace SelfServicePortal.Web.TagHelpers;

[HtmlTargetElement("a", Attributes = ActionAttributeName)]
[HtmlTargetElement("a", Attributes = ControllerAttributeName)]
[HtmlTargetElement("a", Attributes = AreaAttributeName)]
[HtmlTargetElement("a", Attributes = PageAttributeName)]
[HtmlTargetElement("a", Attributes = PageHandlerAttributeName)]
[HtmlTargetElement("a", Attributes = FragmentAttributeName)]
[HtmlTargetElement("a", Attributes = HostAttributeName)]
[HtmlTargetElement("a", Attributes = ProtocolAttributeName)]
[HtmlTargetElement("a", Attributes = RouteAttributeName)]
[HtmlTargetElement("a", Attributes = RouteValuesDictionaryName)]
[HtmlTargetElement("a", Attributes = RouteValuesPrefix + "*")]
public class CultureAnchorTagHelper : AnchorTagHelper
{
	private const string ActionAttributeName = "asp-action";
	private const string ControllerAttributeName = "asp-controller";
	private const string AreaAttributeName = "asp-area";
	private const string PageAttributeName = "asp-page";
	private const string PageHandlerAttributeName = "asp-page-handler";
	private const string FragmentAttributeName = "asp-fragment";
	private const string HostAttributeName = "asp-host";
	private const string ProtocolAttributeName = "asp-protocol";
	private const string RouteAttributeName = "asp-route";
	private const string RouteValuesDictionaryName = "asp-all-route-data";
	private const string RouteValuesPrefix = "asp-route-";
	private readonly IHttpContextAccessor _contextAccessor;

	public CultureAnchorTagHelper(IHttpContextAccessor contextAccessor, IHtmlGenerator generator) :
		base(generator)
	{
		this._contextAccessor = contextAccessor;
	}

	public override void Process(TagHelperContext context, TagHelperOutput output)
	{
		var culture = _contextAccessor.HttpContext!.Request.GetCulture();
		RouteValues["culture"] = culture;
		base.Process(context, output);
	}
}