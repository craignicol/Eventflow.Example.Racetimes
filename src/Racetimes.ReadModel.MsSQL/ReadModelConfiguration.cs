using System.Threading;
using System.Threading.Tasks;
using EventFlow;
using EventFlow.Configuration;
using EventFlow.Extensions;
using EventFlow.MsSql;
using EventFlow.MsSql.Extensions;
using EventFlow.Queries;
using Racetimes.Domain.Identity;

namespace Racetimes.ReadModel.MsSql
{
    public static class ReadModelConfiguration
    {
        public static IEventFlowOptions AddMsSqlReadModel(this IEventFlowOptions efo)
        {
            return efo
                .RegisterServices(sr => sr.Register<IEntryLocator, EntryLocator>())
                .UseMssqlReadModel<CompetitionReadModel>()
                .UseMssqlReadModel<EntryReadModel, IEntryLocator>()
                .AddQueryHandler<GetAllEntriesQueryHandler, GetAllEntriesQuery, EntryReadModel[]>()
                .ConfigureMsSql(MsSqlConfiguration.New.SetConnectionString(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TimesEF;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"));
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