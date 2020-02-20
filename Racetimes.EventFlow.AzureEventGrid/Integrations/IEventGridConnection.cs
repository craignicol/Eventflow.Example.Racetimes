using Microsoft.Azure.EventGrid.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    public interface IEventGridConnection : IDisposable
    {
        Task PublishEventsAsync(string topicHostname, IList<EventGridEvent> events, CancellationToken cancellationToken);
    }
}