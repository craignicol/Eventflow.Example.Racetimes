using System;

namespace Racetimes.EventFlow.AzureEventGrid
{
    public interface IEventGridConfiguration
    {
        Uri Uri { get; }
    }
}