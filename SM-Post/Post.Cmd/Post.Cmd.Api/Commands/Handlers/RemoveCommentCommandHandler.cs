using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers;

public class RemoveCommentCommandHandler : IRequestHandler<RemoveCommentCommand, Unit>
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public RemoveCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(RemoveCommentCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.RemoveComment(request.CommentId, request.Username);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}
