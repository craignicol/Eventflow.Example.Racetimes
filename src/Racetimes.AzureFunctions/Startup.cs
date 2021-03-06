﻿using EventFlow;
using EventFlow.Extensions;
using EventFlow.MsSql.Extensions;
using EventFlow.Snapshots.Strategies;
using log4net;
using log4net.Config;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Racetimes.Domain.Aggregate;
using Racetimes.Domain.Command;
using Racetimes.Domain.CommandHandler;
using Racetimes.Domain.Event;
using Racetimes.ReadModel.EntityFramework;
using Racetimes.ReadModel.MsSql;
using System.IO;
using System.Reflection;

[assembly: FunctionsStartup(typeof(Racetimes.AzureFunctions.Startup))]

namespace Racetimes.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        private ICommandBus CreateEventFlow()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            // Let Singleton Scope in Configure() handle IDisposable
            var resolver = EventFlowOptions.New
                .UseConsoleLog()
                .AddEvents(Assembly.GetAssembly(typeof(CompetitionRegisteredEvent)))
                .AddCommands(Assembly.GetAssembly(typeof(RegisterCompetitionCommand)), t => true)
                .AddCommandHandlers(Assembly.GetAssembly(typeof(RegisterCompetitionHandler)))
                .AddSnapshots(Assembly.GetAssembly(typeof(CompetitionSnapshot)), t => true)
                .RegisterServices(sr => sr.Register(i => SnapshotEveryFewVersionsStrategy.Default))
                .UseMssqlEventStore()
                .UseMsSqlSnapshotStore()
                .AddMsSqlReadModel()
                .AddEntityFrameworkReadModel()
                .CreateResolver();
            
            // TODO: Get migration working without SqlClient configuration errors
            // TODO: Move migration into command line tool to be run on deployment
            // var msSqlDatabaseMigrator = resolver.Resolve<IMsSqlDatabaseMigrator>();
            // EventFlowEventStoresMsSql.MigrateDatabase(msSqlDatabaseMigrator);

            // TODO : Create factory to resolve this
            return resolver.Resolve<ICommandBus>();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(typeof(ICommandBus), CreateEventFlow());
        }
    }
}