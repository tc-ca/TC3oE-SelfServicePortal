using System;
using System.Net;
using SelfServicePortal.Web.Auth;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using OurAzureDevops;
using OurAzure.Api.Services;
using Microsoft.Extensions.Logging;
using Azure.Monitor.Query;
using SelfServicePortal.Web.Middleware;
using SelfServicePortal.Web.Services;
using SelfServicePortal.Core.Services;
using SelfServicePortal.Core;
using OurTeams.Api;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

AddConfig(builder.Configuration);
AddServices(builder.Configuration, builder.Services, builder.Environment);
// Logging configured to dump to Azure and stdout
builder.Logging.AddAzureWebAppDiagnostics();
builder.Logging.AddConsole();

// Create the app
var app = builder.Build();
app.MapHealthChecks("/healthz");

// Configure the app to work properly when running behind the application gateway
// https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-3.1#deal-with-path-base-and-proxies-that-change-the-request-path
app.UseForwardedHeaders();
// The app is reverse proxied a subpath
app.UsePathBase("/ssp-pls");

// Redirect to the proper localized error page outside of development
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler(o =>
	{
		o.Run(ctx =>
		{
			var language = ctx.Request.Path.Value?.Length >= 3 ? ctx.Request.Path.Value.Substring(1, 2) : "en";
			if (language.Equals("en")) ctx.Response.Redirect($"{ctx.Request.PathBase.Value}/{language}/Error");
			if (language.Equals("fr")) ctx.Response.Redirect($"{ctx.Request.PathBase.Value}/{language}/Error");
			return Task.CompletedTask;
		});
	});
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRequestLocalization();
app.UseRouting();

// https://stackoverflow.com/a/70754029/11141271
// https://stackoverflow.com/a/64874175/11141271
// // Add this before any other middleware that might write cookies
app.UseCookiePolicy(new CookiePolicyOptions
{
    // HttpOnly =  HttpOnlyPolicy.Always,
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always
	// MinimumSameSitePolicy = SameSiteMode.Lax
});

app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Needed for Identity auth stuff to work
// https://docs.microsoft.com/en-us/azure/active-directory/develop/web-app-quickstart?pivots=devlang-aspnet-core#startup-class
app.MapControllers();

app.Run();

// Make this importable by tests
public partial class Program {
	public static void AddConfig(ConfigurationManager config)
	{
		var cfg = config.Get<AppSettings>()!;
		var options = new DefaultAzureCredentialOptions {
				ExcludeSharedTokenCacheCredential = true,
				ManagedIdentityClientId = cfg.ManagedIdentityClientId, // from mi_client_id terraform output
		};
		// if (!string.IsNullOrEmpty(cfg.ManagedIdentityClientId))
		// {
		// 	options.ManagedIdentityClientId = cfg.ManagedIdentityClientId;
		// 	Console.WriteLine("Found managed identity " + cfg.ManagedIdentityClientId);
		// }
		// else
		// {
		// 	Console.WriteLine("Could not find managed identity!");
		// }
		// throw new Exception("found mid " + cfg.ManagedIdentityClientId);
		config.AddAzureKeyVault(
			new Uri($"https://{cfg.KeyVaultName}.vault.azure.net/"),
			new DefaultAzureCredential(options)
		);
	}
	/**
	Helper method to allow populating services in the program and in tests
	*/
	public static void AddServices(IConfiguration config, IServiceCollection services, IWebHostEnvironment environment)
	{
		var cfg = config.Get<AppSettings>();
		services.AddSingleton<AppSettings>(cfg);

		// https://learn.microsoft.com/en-us/azure/azure-monitor/app/asp-net-core?source=recommendations&tabs=netcore6#enable-application-insights-server-side-telemetry-no-visual-studio
		// Pulls ApplicationInsights:ConnectionString from keyvault config
		services.AddApplicationInsightsTelemetry();

		services.AddSingleton<TeamsClient>(new TeamsClient(cfg.TeamsWebhookUrl));

		services.AddHealthChecks();

		// Configure headers to allow gateway reverse proxy
		services.Configure<ForwardedHeadersOptions>(options =>
		{
			options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost;
			options.AllowedHosts.Add("cloud.org.gc.ca");

			// public IP of the gateway being used
			// this is one of the older ones
			options.KnownProxies.Add(System.Net.IPAddress.Parse("555.555.555.555"));
		});

		// needed to get login working with gateway/k8s SSL termination
		// https://stackoverflow.com/a/68193168/11141271
		// https://stackoverflow.com/a/70754029/11141271
		// services.Configure<CookiePolicyOptions>(options =>
		// {
		// 	// options.Secure = CookieSecurePolicy.Always;
		// 	options.MinimumSameSitePolicy = SameSiteMode.Lax;
		// });

		// Configure AAD sign in
		services.AddMicrosoftIdentityWebAppAuthentication(config);
		services.AddAuthorization(options =>
		{
			options.FallbackPolicy = options.DefaultPolicy;
			AuthorizationPolicies.Configure(options);
		});
		// When in prod we are running behind the application gateway
		// so we need to override the RedirectUri to point to the correct url
		// since the app doesn't know its url when running behind the gateway
		if (!environment.IsDevelopment())
		{
			services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
			{
				// options.CorrelationCookie.Path = "/ssp-pls/signin-oidc";
				options.Events = new OpenIdConnectEvents
				{
					OnRedirectToIdentityProvider = ctxt => {
						ctxt.ProtocolMessage.RedirectUri = cfg.RedirectUri;
						return Task.CompletedTask;
					}
				};
			});
		}

		// Configure translations
		services.AddLocalization(options =>
		{
			options.ResourcesPath = "Resources";
		});
		services.Configure<RequestLocalizationOptions>(options =>
		{
			options.SetDefaultCulture("en");
			options.AddSupportedCultures("en", "fr");
			options.AddSupportedUICultures("en", "fr");
			options.FallBackToParentCultures = true;
			options.RequestCultureProviders = new[]{
				new UrlRequestCultureProvider() { Options = options }
			};
		});
		// Used by the culture anchor tag helper
		services.AddHttpContextAccessor();

		services.AddRazorPages(options =>
		{
			options.Conventions.Add(new CultureTemplatePageRouteModelConvention());
		})
			.AddMicrosoftIdentityUI() // might not be needed?
			.AddViewLocalization();

		// Create credential object for authenticating as our service principal
		// Uses values pulled from Azure key vault
		// Adds the cred obj as a service for dependency injection into our other services
		services.AddScoped<Azure.Core.TokenCredential>(x => new ClientSecretCredential(
			cfg.AzureAd.TenantId,
			cfg.AzureAd.ClientId,
			cfg.AzureAd.ClientSecret
		));

		// Create our clients, they get the above cred using dependency injection
		services.AddScoped<AzureRestClient>();
		services.AddScoped<Azure.ResourceManager.ArmClient>();
		services.AddScoped<Microsoft.Graph.GraphServiceClient>(builder =>
		{
			var cred = builder.GetRequiredService<Azure.Core.TokenCredential>();
			var graphClient = new Microsoft.Graph.GraphServiceClient(new Microsoft.Graph.DelegateAuthenticationProvider(req =>
			{
				req.Headers.Authorization = cred.GetAuthHeader("https://graph.microsoft.com/.default");
				return Task.CompletedTask;
			}));
			return graphClient;
		});

		// Devops client uses different auth
		// The personal access token is kinda bad since it relies on a specific user, but it is more effort than it's worth to change.
		services.AddScoped<AzureDevopsRestClient>(x => new AzureDevopsRestClient(
			cfg.PersonalAccessToken,
			cfg.DevopsOrg
		));

		// Used for querying the Log Analytics Workspace that the app writes to
		// There's an Admin webpage that helps viewing logs without using the portal
		services.AddScoped<LogsQueryClient>(builder => 
		{
			var cred = builder.GetRequiredService<Azure.Core.TokenCredential>();
			var queryClient = new LogsQueryClient(cred);
			return queryClient;
		});

		// Common util to help with form validation
		services.AddScoped<ValidationHelper>();


		// Service for interacting with the "workflow engine"
		services.AddScoped<WorkflowClient>();
		// Create background service to poll for approval results
		services.AddHostedService<WorkflowFulfillmentService>();

		// Helper object to reduce duplicate code in constructors using dependency injection
		services.AddScoped(typeof(Services<>), typeof(Services<>));

	}


}