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
        private EventGridClient _client;

        public EventGridConnection(ILog log, EventGridClient client)
        {
            _log = log;
            _client = client;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<int> WithModelAsync(Func<EventGridEvent, Task> action, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}