using System;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    public class EventGridMessage
    {
        public EventGridMessage(string id, string eventType, object data, DateTime timestamp, string subject)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            EventType = eventType ?? throw new ArgumentNullException(nameof(eventType));
            Data = data ?? throw new ArgumentNullException(nameof(data));
            Timestamp = timestamp;
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
        }

        public string Id { get; }
        public string EventType { get; }
        public object Data { get;  }
        public DateTime Timestamp { get; }
        public string Subject { get; }
    }
}