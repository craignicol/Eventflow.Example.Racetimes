using EventFlow.Aggregates;
using EventFlow.Subscribers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.EventFlow.AzureEventGrid
{
    internal class EventGridDomainEventPublisher : ISubscribeSynchronousToAll
    {
        public async Task HandleAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken)
        {
            // TODO : Something useful
            Console.WriteLine($"Writing events to EventGrid : {domainEvents}");
        }
    }
}