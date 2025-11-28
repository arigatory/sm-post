using CQRS.Core.Events;
using MediatR;

namespace Post.Common.Events;

public class PostCreatedEvent : BaseEvent, INotification
{
    public PostCreatedEvent() : base(nameof(PostCreatedEvent))
    {
    }

    public string Author { get; set; }
    public string Message { get; set; }
    public DateTime DatePosted { get; set; }
}