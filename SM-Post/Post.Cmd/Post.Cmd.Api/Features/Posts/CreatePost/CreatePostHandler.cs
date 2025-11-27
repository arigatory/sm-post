using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Api.Features.Posts.CreatePost;

public class CreatePostHandler : IRequestHandler<CreatePostCommand, Unit>
{
    private readonly IEventSourcingHandler<PostAggregate> _eventSourcingHandler;

    public CreatePostHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    {
        _eventSourcingHandler = eventSourcingHandler;
    }

    public async Task<Unit> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var aggregate = new PostAggregate(request.Id, request.Author, request.Message);
        await _eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}
