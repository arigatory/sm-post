using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers;

public class NewPostCommandHandler : IRequestHandler<NewPostCommand, Unit>
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public NewPostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(NewPostCommand request, CancellationToken cancellationToken)
    {
        var aggregate = new PostAggregate(request.Id, request.Author, request.Message);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}
