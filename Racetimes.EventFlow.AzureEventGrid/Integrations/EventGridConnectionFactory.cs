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
        private readonly IEventGridClient _client;

        public EventGridConnectionFactory(
            ILog log,
            IEventGridConfiguration configuration)
        {
            _log = log;
            _configuration = configuration;

            _client = CreateEventGridClient(_configuration.ApiKey);
        }

        public Task<IEventGridConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEventGridConnection>(new EventGridConnection(_log, _client));
        }

        private IEventGridClient CreateEventGridClient(string eventGridApiKey)
        {
            TopicCredentials domainKeyCredentials = new TopicCredentials(eventGridApiKey);
            EventGridClient client = new EventGridClient(domainKeyCredentials);
            return client;
        }
    }
}