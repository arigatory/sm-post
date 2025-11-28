using MediatR;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers.Notifications;

public class CommentRemovedEventHandler : INotificationHandler<CommentRemovedEvent>
{
    private readonly ICommentRepository _commentRepository;

    public CommentRemovedEventHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task Handle(CommentRemovedEvent notification, CancellationToken cancellationToken)
    {
        await _commentRepository.DeleteAsync(notification.CommentId);
    }
}
