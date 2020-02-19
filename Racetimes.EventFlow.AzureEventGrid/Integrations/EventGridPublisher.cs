using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    internal class EventGridPublisher : IEventGridPublisher
    {
        public Task PublishAsync(IEnumerable<EventGridMessage> eventGridMessages, CancellationToken cancellationToken)
        {
            // TODO : logging
            var client = new EventGridClient(new BasicAuthenticationCredentials());
            return PublishAsync(client, "http://localhost", eventGridMessages, cancellationToken);
        }

        private static Task PublishAsync(IEventGridClient client, string endpoint, IEnumerable<EventGridMessage> domainEvents, CancellationToken cancellationToken)
        {
            return client.PublishEventsAsync(endpoint, domainEvents.Select(de => PublishSingleMessage(de)).ToList(), cancellationToken);
        }

        private static EventGridEvent PublishSingleMessage(EventGridMessage de)
        {
            return new EventGridEvent()
            {
                Id = de.Id,
                EventType = de.EventType,

                // TODO: Specify the name of the topic (under the domain) to which this event is destined for.
                // Currently using a topic name "domaintopic0"
                Topic = "racetimes",
                Data = de.Data,
                EventTime = de.Timestamp,
                Subject = de.Subject,
                DataVersion = "1.0"
            };
        }
    }
}