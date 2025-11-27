using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers;

public class LikePostCommandHandler : IRequestHandler<LikePostCommand, Unit>
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public LikePostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.LikePost();
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}
