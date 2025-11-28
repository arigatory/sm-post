using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore;
    private readonly IEventProducer _eventProducer;

    public EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer)
    {
        _eventStore = eventStore;
        _eventProducer = eventProducer;
    }
    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        var events = await _eventStore.GetEventsAsync(aggregateId);

        if (events == null || !events.Any()) return aggregate;

        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(x => x.Version).Max();

        return aggregate;
    }

    public async Task RepublishEventsAsync(DateTime? restoreToDateTime = null)
    {
        var aggreageIds = await _eventStore.GetAggregateIdsAsync();

        if (aggreageIds == null || !aggreageIds.Any()) return;

        foreach (var aggreageId in aggreageIds)
        {
            var aggregate = await GetByIdAsync(aggreageId);

            if (aggregate == null || !aggregate.Active) continue;

            var eventModels = await _eventStore.GetEventModelsAsync(aggreageId);
            
            // Filter events by datetime if specified (ensure UTC)
            var filterDateTime = restoreToDateTime.HasValue 
                ? (restoreToDateTime.Value.Kind == DateTimeKind.Utc ? restoreToDateTime.Value : restoreToDateTime.Value.ToUniversalTime())
                : (DateTime?)null;
            
            var eventsToReplay = filterDateTime.HasValue
                ? eventModels.Where(e => e.TimeStamp <= filterDateTime.Value).Select(e => e.EventData).ToList()
                : eventModels.Select(e => e.EventData).ToList();            foreach (var @event in eventsToReplay)
            {
                var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore.SaveEventAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);
        aggregate.MarkChangesAsCommitted();
    }
}