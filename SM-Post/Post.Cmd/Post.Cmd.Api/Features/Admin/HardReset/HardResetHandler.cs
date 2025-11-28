using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Api.Features.Admin.HardReset;

public class HardResetHandler : IRequestHandler<HardResetCommand, Unit>
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;
    private readonly IEventStore _eventStore;

    public HardResetHandler(
        IEventSourcingHandler<PostAggregate> eventSourcingHandler,
        IEventStore eventStore)
    {
        _eventSourcingHandler = eventSourcingHandler;
        _eventStore = eventStore;
    }

    public async Task<Unit> Handle(HardResetCommand request, CancellationToken cancellationToken)
    {
        // First, republish events up to specified datetime to rebuild read side
        await _eventSourcingHandler.RepublishEventsAsync(request.ResetToDateTime);
        
        // Then delete events after specified datetime from event store
        await _eventStore.DeleteEventsAfterAsync(request.ResetToDateTime);
        
        return Unit.Value;
    }
}
