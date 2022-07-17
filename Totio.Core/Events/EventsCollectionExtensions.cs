using Microsoft.EntityFrameworkCore;
using Totio.Core;
using Totio.Core.Data;
using Totio.Core.Events;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class EventsCollectionExtensions
{
	public static IServiceCollection AddProducer<TProducer, TMessage>(this IServiceCollection services)
		where TProducer : ProducerBase<TMessage>
	{
		return services
		   .AddScoped<TProducer>();
	}

	public static IServiceCollection AddConsumer<TConsumer, TMessage>(this IServiceCollection services)
		where TConsumer : ConsumerServiceBase<TMessage>
	{
		return services
		   .AddHostedService<TConsumer>();
	}
}
