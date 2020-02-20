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
        private readonly IEventGridConfiguration _configuration;

        public EventGridConnectionFactory(
            ILog log,
            IEventGridConfiguration configuration)
        {
            _log = log;
            _configuration = configuration;
        }

        public Task<IEventGridConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            var client = CreateEventGridClient(_configuration.ApiKey);

            return Task.FromResult<IEventGridConnection>(new EventGridConnection(_log, client));
        }

        // TODO : Move this to Singleton lifecycle in Startup?
        private IEventGridClient CreateEventGridClient(string eventGridApiKey)
        {
            TopicCredentials domainKeyCredentials = new TopicCredentials(eventGridApiKey);
            EventGridClient client = new EventGridClient(domainKeyCredentials);
            return client;
        }
    }
}