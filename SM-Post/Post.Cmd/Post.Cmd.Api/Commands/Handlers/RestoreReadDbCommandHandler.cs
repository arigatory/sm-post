using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers;

public class RestoreReadDbCommandHandler : IRequestHandler<RestoreReadDbCommand, Unit>
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public RestoreReadDbCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(RestoreReadDbCommand request, CancellationToken cancellationToken)
    {
        await _eventSourcingHandler.RepublishEventsAsync();
        return Unit.Value;
    }
}
