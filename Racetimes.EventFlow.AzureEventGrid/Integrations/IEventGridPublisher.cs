using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    internal interface IEventGridPublisher
    {
        Task PublishAsync(IEnumerable<EventGridMessage> eventGridMessages, CancellationToken cancellationToken);
    }
}