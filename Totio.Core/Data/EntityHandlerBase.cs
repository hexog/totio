using Microsoft.EntityFrameworkCore;

namespace Totio.Core.Data;

public abstract class EntityHandlerBase<TEntity>
	where TEntity : class
{
	protected EntityHandlerBase(ApplicationDbContext dbContext)
	{
		DbContext = dbContext;
	}

	protected ApplicationDbContext DbContext { get; }
	protected DbSet<TEntity> Set => DbContext.Set<TEntity>();
}
