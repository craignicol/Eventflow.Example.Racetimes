using System.Threading;
using Racetimes.Domain.Command;
using Racetimes.Domain.CommandHandler;
using Racetimes.Domain.Event;
using Racetimes.Domain.Identity;
using EventFlow;
using EventFlow.Extensions;
using EventFlow.MsSql;
using EventFlow.MsSql.EventStores;
using EventFlow.MsSql.Extensions;
using Racetimes.Domain.Aggregate;
using log4net;
using System.Reflection;
using log4net.Config;
using System.IO;
using Racetimes.ReadModel.MsSql;
using Racetimes.ReadModel.EntityFramework;
using EventFlow.Snapshots.Strategies;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Racetimes.CommandLine
{
    class Program
    {
        static async Task Main(string[] args)
        {
            /* 
             * SETUP
             */
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            using (var resolver = EventFlowOptions.New
                .AddEvents(Assembly.GetAssembly(typeof(CompetitionRegisteredEvent)))
                .AddCommands(Assembly.GetAssembly(typeof(RegisterCompetitionCommand)), t => true)
                .AddCommandHandlers(Assembly.GetAssembly(typeof(RegisterCompetitionHandler)))
                .AddSnapshots(Assembly.GetAssembly(typeof(CompetitionSnapshot)), t => true)
                .RegisterServices(sr => sr.Register(i => SnapshotEveryFewVersionsStrategy.Default))
                .UseMssqlEventStore()
                .UseMsSqlSnapshotStore()
                .AddMsSqlReadModel()
                .AddEntityFrameworkReadModel()
                .CreateResolver())
            {
                var msSqlDatabaseMigrator = resolver.Resolve<IMsSqlDatabaseMigrator>();
                EventFlowEventStoresMsSql.MigrateDatabase(msSqlDatabaseMigrator);
                // var sql = EventFlowEventStoresMsSql.GetSqlScripts().Select(s => s.Content).ToArray();

                /* 
                 * USAGE
                 */

                // Create a new identity for our aggregate root
                var exampleId = CompetitionId.New;

                // Define some important value
                const string name = "test-competition";
                const string name2 = "new-name";
                const string user = "test-user";

                // Resolve the command bus and use it to publish a command
                var commandBus = resolver.Resolve<ICommandBus>();
                var executionResult = commandBus.Publish(new RegisterCompetitionCommand(exampleId, user, name), CancellationToken.None);

                executionResult = commandBus.Publish(new CorrectCompetitionCommand(exampleId, name2), CancellationToken.None);

                ReadModel.MsSql.ReadModelConfiguration.Query(resolver, exampleId);
                var compModel = ReadModel.EntityFramework.ReadModelConfiguration.Query(resolver, exampleId);
                Console.WriteLine($"{compModel.Competitionname} : {compModel.Id} @ {compModel.Version} // by {compModel.Username}");

                using (var http = new HttpClient())
                {
                    var comp = exampleId.ToString();

                    var entry1Id = await RecordEntry(http, new { CompetitionId = comp, Discipline = "Discipline 1", Name = "Name 1", TimeInMillis = 11111 });
                    var entry2Id = await RecordEntry(http, new { CompetitionId = comp, Discipline = "Discipline 2", Name = "Name 2", TimeInMillis = 22222 });

                    var getEntry = await GetEntry(http, entry1Id);

                    await CorrectEntryTime(http, entry1Id, new { CompetitionId = comp, TimeInMillis = 10000 });
                    await CorrectEntryTime(http, entry2Id, new { CompetitionId = comp, TimeInMillis = 20000 });

                    for (int x = 1; x < 100; x++)
                    {
                        await CorrectEntryTime(http, entry2Id, new { CompetitionId = comp, TimeInMillis = 2000 + x });
                    }

                }
                executionResult = commandBus.Publish(new DeleteCompetitionCommand(exampleId), CancellationToken.None);
            }
        }

        private static async Task<string> RecordEntry(HttpClient http, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            Console.WriteLine($"Record Entry SEND: {json}");

            var httpResult = await http.PostAsync("http://localhost:7071/api/entry", stringContent, CancellationToken.None);
            httpResult.EnsureSuccessStatusCode();
            var content = await httpResult.Content.ReadAsStringAsync();

            Console.WriteLine($"Record Entry RECV: {content}");           
            return content;
        }

        private static async Task<string> CorrectEntryTime(HttpClient http, string id, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
            Console.WriteLine($"Correct Entry SEND: {json}");

            var httpResult = await http.PutAsync($"http://localhost:7071/api/entry/{id}", stringContent, CancellationToken.None);
            httpResult.EnsureSuccessStatusCode();
            var content = JsonConvert.DeserializeObject<dynamic>(await httpResult.Content.ReadAsStringAsync());

            Console.WriteLine($"Correct Entry RECV: {content}");
            return content.entryId.ToString();
        }

        private static async Task<dynamic> GetEntry(HttpClient http, string id)
        {
            var httpResult = await http.GetAsync($"http://localhost:7071/api/entry/{id}", CancellationToken.None);
            httpResult.EnsureSuccessStatusCode();
            var content = JsonConvert.DeserializeObject<dynamic>(await httpResult.Content.ReadAsStringAsync());

            Console.WriteLine($"Get Entry RECV: {content}");
            return content;
        }
    }
}