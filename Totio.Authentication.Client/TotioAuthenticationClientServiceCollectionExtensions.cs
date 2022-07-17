using Totio.Authentication.Client;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class TotioAuthenticationClientServiceCollectionExtensions
{
	public static IServiceCollection AddTotioAuthenticationClients(this IServiceCollection services)
	{
		return services
		   .AddScoped<AccountClient>();
	}
}
