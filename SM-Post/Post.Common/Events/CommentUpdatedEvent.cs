using CQRS.Core.Events;
using MediatR;

namespace Post.Common.Events;

public class CommentUpdatedEvent : BaseEvent, INotification
{
    public CommentUpdatedEvent() : base(nameof(CommentUpdatedEvent))
    {
    }

    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string Username { get; set; }
    public DateTime EditDate { get; set; }
}