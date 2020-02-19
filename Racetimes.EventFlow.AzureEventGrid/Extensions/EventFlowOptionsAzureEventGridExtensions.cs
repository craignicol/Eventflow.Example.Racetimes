using EventFlow.Configuration;
using EventFlow.Subscribers;
using EventFlow;

namespace Racetimes.EventFlow.AzureEventGrid.Extensions
{
    public static class EventFlowOptionsAzureEventGridExtensions
    {
        public static IEventFlowOptions PublishToAzureEventGrid(
            this IEventFlowOptions eventFlowOptions,
            Racetimes.EventFlow.AzureEventGrid.IEventGridConfiguration configuration)
        {
            return eventFlowOptions.RegisterServices(sr =>
            {
                sr.Register<Racetimes.EventFlow.AzureEventGrid.Integrations.IEventGridConnectionFactory, Racetimes.EventFlow.AzureEventGrid.Integrations.EventGridConnectionFactory>(Lifetime.Singleton);
                sr.Register<Racetimes.EventFlow.AzureEventGrid.Integrations.IEventGridMessageFactory, Racetimes.EventFlow.AzureEventGrid.Integrations.EventGridMessageFactory>(Lifetime.Singleton);
                sr.Register<Racetimes.EventFlow.AzureEventGrid.Integrations.IEventGridPublisher, Racetimes.EventFlow.AzureEventGrid.Integrations.EventGridPublisher>(Lifetime.Singleton);
                sr.Register<Racetimes.EventFlow.AzureEventGrid.Integrations.IEventGridRetryStrategy, Racetimes.EventFlow.AzureEventGrid.Integrations.EventGridRetryStrategy>(Lifetime.Singleton);

                sr.Register(rc => configuration, Lifetime.Singleton);

                sr.Register<ISubscribeSynchronousToAll, EventGridDomainEventPublisher>();
            });
        }
    }
}