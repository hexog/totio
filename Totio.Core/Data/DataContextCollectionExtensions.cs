using Microsoft.EntityFrameworkCore;
using Totio.Core;
using Totio.Core.Data;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class DataContextCollectionExtensions
{
	public static IServiceCollection AddTotioDbContext(this IServiceCollection services,
													   Action<DbContextOptionsBuilder>? configure = null)
	{
		return services
		   .AddDbContext<ApplicationDbContext>(configure);
	}

	public static IServiceCollection AddEntityHandler<THandler, TEntity>(this IServiceCollection services)
		where THandler : EntityHandlerBase<TEntity>
		where TEntity : class
	{
		return services
		   .AddTotioDbContext()
		   .AddScoped<THandler>();
	}
}
