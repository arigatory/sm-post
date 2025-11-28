using MediatR;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers.Notifications;

public class PostLikedEventHandler : INotificationHandler<PostLikedEvent>
{
    private readonly IPostRepository _postRepository;

    public PostLikedEventHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task Handle(PostLikedEvent notification, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(notification.Id);
        if (post == null) return;

        post.Likes++;
        await _postRepository.UpdateAsync(post);
    }
}
