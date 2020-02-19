using System;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    public interface IEventGridConnectionFactory
    {
        Task<IEventGridConnection> CreateConnectionAsync(Uri uri, CancellationToken cancellationToken);
    }
}