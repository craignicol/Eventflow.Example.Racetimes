using EventFlow;
using EventFlow.Configuration;
using EventFlow.EntityFramework;
using EventFlow.EntityFramework.Extensions;
using EventFlow.Extensions;
using EventFlow.Queries;
using Racetimes.Domain.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace Racetimes.ReadModel.EntityFramework
{
    public static class ReadModelConfiguration
    {
        public static IEventFlowOptions AddEntityFrameworkReadModel(this IEventFlowOptions efo)
        {
            return efo
                .RegisterServices(sr => sr.Register<IEntryLocator, EntryLocator>())
                .RegisterServices(sr => sr.Register(c => @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TimesEF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
                .UseEntityFrameworkReadModel<CompetitionReadModel, ExampleDbContext>()
                .UseEntityFrameworkReadModel<EntryReadModel, ExampleDbContext, IEntryLocator>()
                .AddQueryHandler<GetAllEntriesQueryHandler, GetAllEntriesQuery, EntryReadModel[]>()
                .ConfigureEntityFramework(EntityFrameworkConfiguration.New)
                .AddDbContextProvider<ExampleDbContext, DbContextProvider>();
        }

        public static Task<CompetitionReadModel> Query(IRootResolver resolver, CompetitionId exampleId)
        {
            // Resolve the query handler and use the built-in query for fetching
            // read models by identity to get our read model representing the
            // state of our aggregate root
            var queryProcessor = resolver.Resolve<IQueryProcessor>();
            return queryProcessor.ProcessAsync(new ReadModelByIdQuery<CompetitionReadModel>(exampleId), CancellationToken.None);
        }
    }
}