using MediatR;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers.Notifications;

public class MessageUpdatedEventHandler : INotificationHandler<MessageUpdatedEvent>
{
    private readonly IPostRepository _postRepository;

    public MessageUpdatedEventHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task Handle(MessageUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(notification.Id);
        if (post == null) return;

        post.Message = notification.Message;
        await _postRepository.UpdateAsync(post);
    }
}
