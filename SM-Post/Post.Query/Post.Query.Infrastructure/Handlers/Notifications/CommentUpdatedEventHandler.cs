using MediatR;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers.Notifications;

public class CommentUpdatedEventHandler : INotificationHandler<CommentUpdatedEvent>
{
    private readonly ICommentRepository _commentRepository;

    public CommentUpdatedEventHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task Handle(CommentUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(notification.CommentId);
        if (comment == null) return;

        comment.Comment = notification.Comment;
        comment.Edited = true;
        comment.CommentDate = notification.EditDate;

        await _commentRepository.UpdateAsync(comment);
    }
}
