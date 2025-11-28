using MediatR;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers.Notifications;

public class PostCreatedEventHandler : INotificationHandler<PostCreatedEvent>
{
    private readonly IPostRepository _postRepository;

    public PostCreatedEventHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
    {
        var post = new PostEntity
        {
            PostId = notification.Id,
            Author = notification.Author,
            DatePosted = notification.DatePosted,
            Message = notification.Message
        };

        await _postRepository.CreateAsync(post);
    }
}
