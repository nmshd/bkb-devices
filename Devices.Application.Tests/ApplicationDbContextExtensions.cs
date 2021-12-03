using Devices.Infrastructure.Persistence.Database;

namespace Devices.Application.Tests
{
    public static class ApplicationDbContextExtensions
    {
        public static TEntity SaveEntity<TEntity>(this ApplicationDbContext dbContext, TEntity entity) where TEntity : class
        {
            dbContext.Set<TEntity>().Add(entity);
            dbContext.SaveChanges();
            return entity;
        }
    }
}
