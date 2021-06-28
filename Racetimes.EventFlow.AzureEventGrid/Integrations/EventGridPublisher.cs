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
        private readonly IEventGridClient _client;
        private readonly IEventGridConfiguration _configuration;
        private readonly ITransientFaultHandler<IEventGridRetryStrategy> _transientFaultHandler;

        public EventGridPublisher(
            ILog log,
            IEventGridConfiguration configuration,
            ITransientFaultHandler<IEventGridRetryStrategy> transientFaultHandler,
            IEventGridClient client
            )
        {
            _log = log;
            _client = client;
            _configuration = configuration;
            _transientFaultHandler = transientFaultHandler;
        }

        public async Task PublishAsync(IEnumerable<EventGridMessage> eventGridMessages, CancellationToken cancellationToken)
        {
            await _transientFaultHandler.TryAsync(
                (cancelToken) => PublishAsync(_client, _configuration.Hostname, eventGridMessages, cancelToken),
                Label.Named("eventgrid-publish"),
                cancellationToken
                ).ConfigureAwait(false);
            return;
        }

        private Task PublishAsync(IEventGridClient client, string endpoint, IEnumerable<EventGridMessage> domainEvents, CancellationToken cancellationToken)
        {
            _log.Debug($"Publishing {domainEvents.Count()} message to {endpoint} ...");
            return client.PublishEventsAsync(endpoint, domainEvents.Select(de => PublishSingleMessage(de)).ToList(), cancellationToken);
        }

        private EventGridEvent PublishSingleMessage(EventGridMessage de)
        {
            return new EventGridEvent()
            {
                Id = de.Id,
                EventType = de.EventType,

                Topic = _configuration.TopicRoot + "racetimes",
                Data = de.Data,
                EventTime = de.Timestamp,
                Subject = de.Subject,
                DataVersion = "1.0"
            };
        }
    }
}