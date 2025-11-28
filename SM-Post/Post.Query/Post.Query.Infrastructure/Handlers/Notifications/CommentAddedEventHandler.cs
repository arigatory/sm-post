using MediatR;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers.Notifications;

public class CommentAddedEventHandler : INotificationHandler<CommentAddedEvent>
{
    private readonly ICommentRepository _commentRepository;

    public CommentAddedEventHandler(ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
    {
        var comment = new CommentEntity
        {
            PostId = notification.Id,
            CommentId = notification.CommentId,
            CommentDate = notification.CommentDate,
            Comment = notification.Comment,
            Username = notification.Username,
            Edited = false
        };

        await _commentRepository.CreateAsync(comment);
    }
}
