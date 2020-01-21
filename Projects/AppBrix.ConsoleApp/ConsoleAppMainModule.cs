using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Modules;
using System;
using System.Collections.Generic;

namespace AppBrix.ConsoleApp
{
    /// <summary>
    /// Initializes application configuration.
    /// </summary>
    public sealed class ConsoleAppMainModule : MainModuleBase
    {
        #region Properties
        public override IEnumerable<Type> Dependencies => new[]
        {
            //typeof(Caching.CachingModule),
            typeof(Caching.Memory.MemoryCachingModule),
            typeof(Cloning.CloningModule),
            typeof(Container.ContainerModule),
            typeof(Data.DataModule),
            //typeof(Data.InMemory.InMemoryDataModule),
            typeof(Data.Migration.MigrationDataModule),
            typeof(Data.Sqlite.SqliteDataModule),
            //typeof(Data.SqlServer.SqlServerDataModule),
            typeof(Events.EventsModule),
            typeof(Events.Async.AsyncEventsModule),
            typeof(Events.Schedule.ScheduledEventsModule),
            typeof(Events.Schedule.Cron.CronScheduledEventsModule),
            typeof(Events.Schedule.Timer.TimerScheduledEventsModule),
            typeof(Factory.FactoryModule),
            typeof(Logging.LoggingModule),
            typeof(Logging.Console.ConsoleLoggingModule),
            //typeof(Logging.File.FileLoggerModule),
            typeof(Permissions.PermissionsModule),
            typeof(Random.RandomModule),
            typeof(Text.TextModule),
            typeof(Time.TimeModule),
            typeof(Web.Client.WebClientModule),
            //typeof(Web.Server.WebServerModule)
        };
        #endregion

        #region Public and overriden methods
        protected override void Install(IInstallContext context)
        {
            base.Install(context);
            this.App.GetConfig<LoggingConfig>().Async = false;
            context.RequestedAction = RequestedAction.Restart;
        }
        #endregion
    }
}
