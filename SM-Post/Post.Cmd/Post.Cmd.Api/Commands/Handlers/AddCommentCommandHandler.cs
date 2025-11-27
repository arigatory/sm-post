using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, Unit>
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public AddCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.AddComment(request.Comment, request.Username);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}
