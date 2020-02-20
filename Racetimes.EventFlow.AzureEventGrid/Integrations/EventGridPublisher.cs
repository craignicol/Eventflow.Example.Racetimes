using EventFlow.Core;
using EventFlow.Logs;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    internal class EventGridPublisher : IEventGridPublisher
    {
        private readonly ILog _log;
        private readonly IEventGridConnectionFactory _connectionFactory;
        private readonly IEventGridConfiguration _configuration;
        private readonly ITransientFaultHandler<IEventGridRetryStrategy> _transientFaultHandler;

        public EventGridPublisher(
            ILog log,
            IEventGridConnectionFactory connectionFactory,
            IEventGridConfiguration configuration,
            ITransientFaultHandler<IEventGridRetryStrategy> transientFaultHandler
            )
        {
            _log = log;
            _connectionFactory = connectionFactory;
            _configuration = configuration;
            _transientFaultHandler = transientFaultHandler;
        }

        public async Task PublishAsync(IEnumerable<EventGridMessage> eventGridMessages, CancellationToken cancellationToken)
        {
            // TODO : logging
            var client = await _connectionFactory.CreateConnectionAsync(cancellationToken);
            await PublishAsync(client, _configuration.Hostname, eventGridMessages, cancellationToken);
            return;
        }

        private Task PublishAsync(IEventGridConnection client, string endpoint, IEnumerable<EventGridMessage> domainEvents, CancellationToken cancellationToken)
        {
            return client.PublishEventsAsync(endpoint, domainEvents.Select(de => PublishSingleMessage(de)).ToList(), cancellationToken);
        }

        private EventGridEvent PublishSingleMessage(EventGridMessage de)
        {
            return new EventGridEvent()
            {
                Id = de.Id,
                EventType = de.EventType,

                // TODO: Specify the name of the topic (under the domain) to which this event is destined for.
                // Currently using a topic name "domaintopic0"
                Topic = _configuration.TopicRoot + "racetimes",
                Data = de.Data,
                EventTime = de.Timestamp,
                Subject = de.Subject,
                DataVersion = "1.0"
            };
        }
    }
}