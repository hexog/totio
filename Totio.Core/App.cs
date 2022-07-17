using Microsoft.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Totio.Core;

public class App
{
	public static void Run(Action<IServiceCollection>? configureServices = null, Action<WebApplication>? configureApp = null)
	{
		var builder = WebApplication.CreateBuilder();

		builder.WebHost.ConfigureAppConfiguration(ConfigureAppConfiguration);

		ConfigureServices(builder.Services, configureServices);

		var app = builder.Build();

		configureApp?.Invoke(app);

		app.Run();
	}

	public static void ConfigureServices(IServiceCollection services,
										 Action<IServiceCollection>? configureServices = null)
	{
		services.AddHttpClient();
		configureServices?.Invoke(services);
	}

	public static void ConfigureAppConfiguration(WebHostBuilderContext webHostBuilderContext, IConfigurationBuilder builder)
	{
		var environment = webHostBuilderContext.HostingEnvironment;

		var rootSharedSettingsFolder = Path.Combine(environment.ContentRootPath, "..", "sharedsettings.json");

		builder.AddJsonFile("sharedsettings.json", optional: true)
		   .AddJsonFile(rootSharedSettingsFolder, optional: true)
		   .AddJsonFile("appsettings.json", optional: true);

		builder.AddEnvironmentVariables();
	}
}
