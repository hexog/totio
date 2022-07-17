using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Totio.Core.Data;
using Totio.Core.Events;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

[AttributeUsage(AttributeTargets.Class)]
public class ScopedServiceAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public class TransientServiceAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public class SingleTonServiceAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public class CustomDiAttribute : Attribute
{
}

public static class AssemblyCollectionExtensions
{
	public static readonly Type[] IgnoredDiTypes =
	{
		typeof(EntityHandlerBase<>),
		typeof(ProducerBase<>),
		typeof(ConsumerServiceBase<>),
		typeof(ControllerBase),
	};

	public static IServiceCollection AddTypesFromAssembly(this IServiceCollection services, Assembly assembly)
	{
		void AddTypeToDi(Type type, Type? @interface = null)
		{
			if (type.GetCustomAttribute<CustomDiAttribute>() is not null)
			{
				return;
			}

			if (IgnoredDiTypes.Any(ignoredDiType => type.IsAssignableTo(ignoredDiType)))
			{
				return;
			}

			var isSingleTon = type.GetCustomAttribute<SingleTonServiceAttribute>() is not null;

			if (isSingleTon)
			{
				if (@interface is not null)
				{
					services.AddSingleton(@interface, type);
				}
				else
				{
					services.AddScoped(type);
				}

				return;
			}

			var isTransient = type.GetCustomAttribute<TransientServiceAttribute>() is not null;

			if (isTransient)
			{
				if (@interface is not null)
				{
					services.AddTransient(@interface, type);
				}
				else
				{
					services.AddTransient(type);
				}

				return;
			}

			if (@interface is not null)
			{
				services.AddScoped(@interface, type);
			}
			else
			{
				services.AddScoped(type);
			}
		}

		var classes = assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract);

		foreach (var type in classes)
		{
			var interfaces = type.GetInterfaces();

			foreach (var @interface in interfaces)
			{
				AddTypeToDi(type, @interface);
			}

			AddTypeToDi(type);
		}

		return services;
	}
}
