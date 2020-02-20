using EventFlow.Aggregates;
using EventFlow.Subscribers;
using Racetimes.EventFlow.AzureEventGrid.Integrations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid
{
    internal class EventGridDomainEventPublisher : ISubscribeSynchronousToAll
    {
        private readonly IEventGridPublisher _eventGridPublisher;
        private readonly IEventGridMessageFactory _eventGridMessageFactory;

        public EventGridDomainEventPublisher(
            IEventGridPublisher eventGridPublisher,
            IEventGridMessageFactory eventGridMessageFactory)
        {
            _eventGridPublisher = eventGridPublisher;
            _eventGridMessageFactory = eventGridMessageFactory;
        }

        public Task HandleAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            var eventGridMessages = domainEvents.Select(e => _eventGridMessageFactory.CreateMessage(e));
            return _eventGridPublisher.PublishAsync(eventGridMessages, cancellationToken);
        }
    }
}