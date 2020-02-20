using Microsoft.Rest;
using System;

namespace Racetimes.EventFlow.AzureEventGrid
{
    public class EventGridConfiguration : IEventGridConfiguration
    {
        public EventGridConfiguration(string hostname, string apiKey)
        {
            Hostname = hostname ?? throw new ArgumentNullException(nameof(hostname));
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        }

        public string Hostname { get; }

        public string ApiKey { get; }

        public static IEventGridConfiguration With(
            string hostname,
            string apiKey)
        {
            return new EventGridConfiguration(hostname, apiKey);
        }
    }
}