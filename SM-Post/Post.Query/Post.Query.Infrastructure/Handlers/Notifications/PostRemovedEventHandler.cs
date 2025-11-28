using MediatR;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers.Notifications;

public class PostRemovedEventHandler : INotificationHandler<PostRemovedEvent>
{
    private readonly IPostRepository _postRepository;

    public PostRemovedEventHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task Handle(PostRemovedEvent notification, CancellationToken cancellationToken)
    {
        await _postRepository.DeleteAsync(notification.Id);
    }
}
