using EventFlow.Configuration;
using EventFlow.Subscribers;
using EventFlow;

namespace Racetimes.EventFlow.AzureEventGrid.Extensions
{
    public static class EventFlowOptionsAzureEventGridExtensions
    {
        public static IEventFlowOptions PublishToAzureEventGrid(
            this IEventFlowOptions eventFlowOptions,
            IEventGridConfiguration configuration)
        {
            return eventFlowOptions.RegisterServices(sr =>
            {
                sr.Register<Integrations.IEventGridConnectionFactory, Integrations.EventGridConnectionFactory>(Lifetime.Singleton);
                sr.Register<Integrations.IEventGridMessageFactory, Integrations.EventGridMessageFactory>(Lifetime.Singleton);
                sr.Register<Integrations.IEventGridPublisher, Integrations.EventGridPublisher>(Lifetime.Singleton);
                sr.Register<Integrations.IEventGridRetryStrategy, Integrations.EventGridRetryStrategy>(Lifetime.Singleton);

                sr.Register(rc => configuration, Lifetime.Singleton);

                sr.Register<ISubscribeSynchronousToAll, EventGridDomainEventPublisher>();
            });
        }
    }
}