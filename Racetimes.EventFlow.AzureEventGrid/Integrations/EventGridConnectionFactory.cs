using EventFlow.Logs;
using Microsoft.Azure.EventGrid;
using Microsoft.Rest;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    public class EventGridConnectionFactory : IEventGridConnectionFactory
    {
        private readonly ILog _log;
        private readonly IEventGridConfiguration _configuration;

        public EventGridConnectionFactory(
            ILog log,
            IEventGridConfiguration configuration)
        {
            _log = log;
            _configuration = configuration;
        }

        public async Task<IEventGridConnection> CreateConnectionAsync(Uri uri, CancellationToken cancellationToken)
        {
            var client = new EventGridClient(_configuration.Credentials);

            return new EventGridConnection(_log, client);
        }
    }
}