using System;

namespace Banks.UpdatesSubscribers
{
    public class ClientSubscribedEvent
    {
        public ClientSubscribedEvent(EventType type)
        {
            Type = type;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; init; }
        public EventType Type { get; init; }
    }
}