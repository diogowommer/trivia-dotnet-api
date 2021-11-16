using CrossCutting.SeedWork.Classes;
using CrossCutting.SeedWork.Domain;
using MediatR;
using System.Linq;
using System.Threading.Tasks;


namespace CrossCutting.SeedWork.Extensions
{
    public static class MediatorExtension
    {
        public static async Task<int> DispatchDomainEventsAsync(this IMediator mediator, DbContextBase dbContext)
        {
            var domainEntities = dbContext.ChangeTracker
                .Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await mediator.Publish(domainEvent);
                });

            await Task.WhenAll(tasks);

            return domainEvents.Count;
        }
    }
}
