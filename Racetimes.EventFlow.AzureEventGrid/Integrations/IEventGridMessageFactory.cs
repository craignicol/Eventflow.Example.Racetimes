using EventFlow.Aggregates;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    internal interface IEventGridMessageFactory
    {
        EventGridMessage CreateMessage(IDomainEvent e);
    }
}