using EventFlow.Core;
using System;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    internal class EventGridRetryStrategy : IEventGridRetryStrategy
    {
        public Retry ShouldThisBeRetried(Exception exception, TimeSpan totalExecutionTime, int currentRetryCount)
        {
            throw new NotImplementedException();
        }
    }
}