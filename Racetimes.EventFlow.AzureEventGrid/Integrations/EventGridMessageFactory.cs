using EventFlow.Aggregates;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    internal class EventGridMessageFactory : IEventGridMessageFactory
    {
        public EventGridMessage CreateMessage(IDomainEvent evt)
        {
            return new EventGridMessage(
                id : evt.GetIdentity().Value,
                eventType : evt.EventType.Name,
                data : evt,
                timestamp : evt.Timestamp.UtcDateTime,
                subject : evt.IdentityType.Name
            );
        }
    }
}