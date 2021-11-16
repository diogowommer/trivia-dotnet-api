using CrossCutting.SeedWork.Classes;
using Microsoft.EntityFrameworkCore;

namespace CrossCutting.SeedWork.Extensions
{    public static class ContextExtension
    {
        /// <summary>
        /// Use with caution. If entity key is not auto generated you may have problem.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        /// <returns><see cref="EntityState"/></returns>
        public static EntityState AddOrUpdate<TEntity>(this DbContextBase context, TEntity entity) where TEntity : class
        {
            var entry = context.Entry(entity);

            if (context.Entry(entity).IsKeySet)
            {
                entry.State = EntityState.Modified;
            }
            else
            {
                entry.State = EntityState.Added;
            }

            return entry.State;
        }
    }
}
