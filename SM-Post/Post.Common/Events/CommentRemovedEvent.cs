using CQRS.Core.Events;
using MediatR;

namespace Post.Common.Events;

public class CommentRemovedEvent : BaseEvent, INotification
{
    public CommentRemovedEvent() : base(nameof(CommentRemovedEvent))
    {
    }

    public Guid CommentId { get; set; }
}