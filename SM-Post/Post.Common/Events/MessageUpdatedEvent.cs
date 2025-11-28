using CQRS.Core.Events;
using MediatR;

namespace Post.Common.Events
{
    public class MessageUpdatedEvent : BaseEvent, INotification
    {
        public MessageUpdatedEvent() : base(nameof(MessageUpdatedEvent))
        {
        }

        public string Message { get; set; }
    }
}