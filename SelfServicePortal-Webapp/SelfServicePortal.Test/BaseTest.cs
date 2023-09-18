using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using SelfServicePortal.Web.Services;
using System;
using System.Reflection;
using Xunit.Abstractions;


namespace SelfServicePortal.Test;

public class BaseTest
{
	public readonly ServiceProvider ServiceProvider;
	public readonly Services<object> Services;
	public readonly ITestOutputHelper Helper;
	public BaseTest(ITestOutputHelper helper)
	{
		Helper = helper;
		var config  = (ConfigurationManager) new ConfigurationManager()
				.SetBasePath(AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json")
				.AddJsonFile("appsettings.Development.json");
		Program.AddConfig(config);

		var services = new ServiceCollection();
		var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
		{
			EnvironmentName = Microsoft.Extensions.Hosting.Environments.Development,
		});
		services.AddSingleton<IHostingEnvironment>((IHostingEnvironment) builder.Environment);
		services.AddSingleton<IWebHostEnvironment>(builder.Environment);
		Program.AddServices(config, services, builder.Environment);

		this.ServiceProvider = services.BuildServiceProvider();
		this.Services = ServiceProvider.GetService<Services<object>>()!;
	}
}