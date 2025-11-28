using CQRS.Core.Events;
using MediatR;

namespace Post.Common.Events;

public class PostRemovedEvent : BaseEvent, INotification
{
    public PostRemovedEvent() : base(nameof(PostRemovedEvent))
    {
    }
}