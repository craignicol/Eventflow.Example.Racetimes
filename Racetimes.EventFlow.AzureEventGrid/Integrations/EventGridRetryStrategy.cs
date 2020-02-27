using EventFlow.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Racetimes.EventFlow.AzureEventGrid.Integrations
{
    internal class EventGridRetryStrategy : IEventGridRetryStrategy
    {
        private static readonly ISet<Type> TransientExceptions = new HashSet<Type>
            {
                typeof(TimeoutException),
                typeof(HttpRequestException)
            };
        private readonly IEventGridConfiguration _configuration;

        public EventGridRetryStrategy(IEventGridConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Retry ShouldThisBeRetried(Exception exception, TimeSpan totalExecutionTime, int currentRetryCount)
        {
            return currentRetryCount <= _configuration.MaxRetries && TransientExceptions.Contains(exception.GetType())
                ? Retry.YesAfter(TimeSpan.FromMilliseconds(_configuration.RetryDelayInMilliseconds))
                : Retry.No;
        }
    }
}