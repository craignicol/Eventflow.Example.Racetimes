using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    public interface IEventGridConnection : IDisposable
    {
        Task<int> WithModelAsync(Func<Microsoft.Azure.EventGrid.Models.EventGridEvent, Task> action, CancellationToken cancellationToken);
    }
}