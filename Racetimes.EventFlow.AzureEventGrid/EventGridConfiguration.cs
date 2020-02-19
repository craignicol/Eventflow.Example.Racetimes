using Microsoft.Rest;
using System;

namespace Racetimes.EventFlow.AzureEventGrid
{
    public class EventGridConfiguration : IEventGridConfiguration
    {
        public EventGridConfiguration(Uri uri, ServiceClientCredentials credentials)
        {
            Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            Credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }

        public Uri Uri { get; }

        public ServiceClientCredentials Credentials { get; }

        public static IEventGridConfiguration With(
            Uri uri,
            ServiceClientCredentials credentials)
        {
            return new EventGridConfiguration(uri, credentials);
        }
    }
}