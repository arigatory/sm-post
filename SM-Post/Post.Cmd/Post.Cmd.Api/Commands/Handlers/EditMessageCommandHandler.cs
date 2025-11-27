using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Commands.Handlers;

public class EditMessageCommandHandler : IRequestHandler<EditMessageCommand, Unit>
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public EditMessageCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await _eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.EditMessage(request.Message);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}
