using EventFlow.Logs;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    public class EventGridConnection : IEventGridConnection
    {
        private ILog _log;
        private IEventGridClient _client;

        public EventGridConnection(ILog log, IEventGridClient client)
        {
            _log = log;
            _client = client;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        public Task PublishEventsAsync(string topicHostname, System.Collections.Generic.IList<EventGridEvent> events, CancellationToken cancellationToken)
        {
            _log.Debug($"Sending {events.Count} events to {topicHostname}");
            return _client.PublishEventsAsync(topicHostname, events, cancellationToken);
        }
    }
}