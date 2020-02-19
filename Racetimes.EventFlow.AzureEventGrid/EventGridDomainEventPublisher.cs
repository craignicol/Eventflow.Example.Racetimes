using EventFlow.Aggregates;
using EventFlow.Subscribers;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest;
using Racetimes.EventFlow.AzureEventGrid.Integrations;
using System;
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
            //var eventGridMessages = domainEvents.Select(e => _eventGridMessageFactory.CreateMessage(e));
            //return _eventGridPublisher.PublishAsync(eventGridMessages, cancellationToken);
            // TODO : logging
            var client = new EventGridClient(new BasicAuthenticationCredentials());
            return PublishAsync(client, "http://localhost", domainEvents);
        }

        private async static Task PublishAsync(IEventGridClient client, string endpoint, IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            client.PublishEventsAsync(endpoint, GetEventsList(domainEvents)).GetAwaiter().GetResult();
            Console.Write("Published events to Event Grid domain.");
            Console.ReadLine();
        }

        private static IList<EventGridEvent> GetEventsList(IReadOnlyCollection<IDomainEvent> domainEvents)
        {
            List<EventGridEvent> eventsList = new List<EventGridEvent>();

            foreach (var evt in domainEvents)
            {
                eventsList.Add(new EventGridEvent()
                {
                    Id = Guid.NewGuid().ToString(),
                    EventType = evt.EventType.Name,

                    // TODO: Specify the name of the topic (under the domain) to which this event is destined for.
                    // Currently using a topic name "domaintopic0"
                    Topic = "racetimes",
                    Data = evt,
                    EventTime = evt.Timestamp.UtcDateTime,
                    Subject = evt.IdentityType.Name,
                    DataVersion = "1.0"
                });
            }

            return eventsList;
        }
    }
}