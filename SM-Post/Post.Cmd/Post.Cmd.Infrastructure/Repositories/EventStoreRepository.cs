using CQRS.Core.Domain;
using CQRS.Core.Events;
using Marten;
using Microsoft.Extensions.Options;
using Post.Cmd.Infrastructure.Config;

namespace Post.Cmd.Infrastructure.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly IDocumentStore _documentStore;

        public EventStoreRepository(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task<List<EventModel>> FindAllAsync()
        {
            using var session = _documentStore.QuerySession();
            var events = await session.Query<EventModel>().ToListAsync();
            return events.ToList();
        }

        public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
        {
            using var session = _documentStore.QuerySession();
            var events = await session.Query<EventModel>()
                .Where(x => x.AggregateIdentifier == aggregateId)
                .OrderBy(x => x.Version)
                .ToListAsync();
            return events.ToList();
        }

        public async Task SaveAsync(EventModel @event)
        {
            using var session = _documentStore.LightweightSession();
            session.Store(@event);
            await session.SaveChangesAsync();
        }

        public async Task DeleteEventsAfterAsync(DateTime dateTime)
        {
            using var session = _documentStore.LightweightSession();
            
            // Ensure we're using UTC
            var utcDateTime = dateTime.Kind == DateTimeKind.Utc ? dateTime : dateTime.ToUniversalTime();
            
            // Use proper Marten syntax for DateTime comparison
            var eventsToDelete = await session.Query<EventModel>()
                .Where(x => x.TimeStamp > utcDateTime)
                .ToListAsync();
            
            foreach (var @event in eventsToDelete)
            {
                session.Delete(@event);
            }
            
            if (eventsToDelete.Any())
            {
                await session.SaveChangesAsync();
            }
        }
    }
}