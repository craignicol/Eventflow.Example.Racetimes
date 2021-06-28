using EventFlow.Logs;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    public class EventGridConnectionFactory : IEventGridConnectionFactory
    {
        private readonly ILog _log;
        private readonly IEventGridClient _client;

        public EventGridConnectionFactory(
            ILog log,
            IEventGridClient client)
        {
            _log = log;
            _client = client;
        }

        public Task<IEventGridConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEventGridConnection>(new EventGridConnection(_log, _client));
        }
    }
}