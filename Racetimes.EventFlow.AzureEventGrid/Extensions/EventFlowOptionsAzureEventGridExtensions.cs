using EventFlow.Configuration;
using EventFlow.Subscribers;
using EventFlow;
using System.Net.Http;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;
using EventFlow.Logs;

namespace Racetimes.EventFlow.AzureEventGrid.Extensions
{
    public static class EventFlowOptionsAzureEventGridExtensions
    {
        public static IEventFlowOptions PublishToAzureEventGrid(
            this IEventFlowOptions eventFlowOptions,
            IEventGridConfiguration configuration,
            HttpClient httpClient)
        {
            return eventFlowOptions.RegisterServices(sr =>
            {
                sr.Register(rc => configuration, Lifetime.Singleton);

                sr.Register<IEventGridClient>(s => { return new EventGridClient(new TopicCredentials(configuration.ApiKey), httpClient, false); }, Lifetime.Singleton);
                sr.Register<Integrations.IEventGridConnection>(s => { return new Integrations.EventGridConnection(s.Resolver.Resolve<ILog>(), s.Resolver.Resolve<IEventGridClient>()); }, Lifetime.Singleton);

                sr.Register<Integrations.IEventGridMessageFactory, Integrations.EventGridMessageFactory>(Lifetime.Singleton);
                sr.Register<Integrations.IEventGridPublisher, Integrations.EventGridPublisher>(Lifetime.Singleton);
                sr.Register<Integrations.IEventGridRetryStrategy, Integrations.EventGridRetryStrategy>(Lifetime.Singleton);

                sr.Register(rc => configuration, Lifetime.Singleton);

                sr.Register<ISubscribeSynchronousToAll, EventGridDomainEventPublisher>();
            });
        }
    }
}