using Microsoft.Rest;
using System;

namespace Racetimes.EventFlow.AzureEventGrid
{
    public class EventGridConfiguration : IEventGridConfiguration
    {
        private EventGridConfiguration(string hostname, string apiKey, string topicRoot)
        {
            Hostname = hostname ?? throw new ArgumentNullException(nameof(hostname));
            ApiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            TopicRoot = topicRoot ?? throw new ArgumentNullException(nameof(topicRoot)); ;
        }

        public string Hostname { get; }

        public string ApiKey { get; }
        public string TopicRoot { get; }

        public int MaxRetries => 3;

        public double RetryDelayInMilliseconds => 25;

        public static IEventGridConfiguration With(
            string topicEndpoint,
            string apiKey,
            string topicRoot)
        {
            return new EventGridConfiguration(new Uri(topicEndpoint).Host, apiKey, topicRoot);
        }
    }
}