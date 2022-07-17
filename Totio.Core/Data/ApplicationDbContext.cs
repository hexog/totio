using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Totio.Core.Data;

public class ApplicationDbContext : DbContext
{
	private static readonly Type[] EntityTypes = AppDomain.CurrentDomain
	   .GetAssemblies()
	   .SelectMany(x => x.GetTypes())
	   .Where(x => x.GetCustomAttribute<TableAttribute>() is not null)
	   .ToArray();

	public ApplicationDbContext(DbContextOptions options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		foreach (var entityType in EntityTypes)
		{
			modelBuilder.Entity(entityType);
		}
	}
}
