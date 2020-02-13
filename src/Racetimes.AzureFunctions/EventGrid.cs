using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Racetimes.AzureFunctions.Startup))]

namespace Racetimes.AzureFunctions
{
    /// <summary>
    /// IOptions collection for retrieving secrets
    /// </summary>
    public class EventGrid
    {
        public string Endpoint { get; private set; }
        public string ApiKey { get; private set; }
        public string Host => new Uri(Endpoint).Host;
    }
}