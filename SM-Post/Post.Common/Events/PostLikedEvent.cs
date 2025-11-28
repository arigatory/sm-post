using CQRS.Core.Events;
using MediatR;

namespace Post.Common.Events
{
    public class PostLikedEvent : BaseEvent, INotification
    {
        public PostLikedEvent() : base(nameof(PostLikedEvent))
        {
        }
    }
}