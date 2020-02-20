using Microsoft.Rest;
using System;

namespace Racetimes.EventFlow.AzureEventGrid
{
    public interface IEventGridConfiguration
    {
        string Hostname { get; }
        string ApiKey { get; }
        string TopicRoot { get; }
    }
}