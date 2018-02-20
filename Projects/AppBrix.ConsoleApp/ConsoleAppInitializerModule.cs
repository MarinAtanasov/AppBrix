using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Json;
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.ConsoleApp
{
    /// <summary>
    /// Initializes application configuration.
    /// </summary>
    public sealed class ConsoleAppInitializerModule : ModuleBase, IInstallable
    {
        #region Public and overriden methods
        public static IApp CreateApp()
        {
            var configService = new ConfigService(new FilesConfigProvider("./Config", "json"), new JsonConfigSerializer());
            if (configService.Get<AppConfig>().Modules.Count == 0)
                configService.Get<AppConfig>().Modules.Add(ModuleConfigElement.Create<ConsoleAppInitializerModule>());

            var app = AppBrix.App.Create(configService);
            app.Start();
            return app;
        }

        public void Install(IInstallContext context)
        {
            this.InitializeAppConfig(context.App.ConfigService);
            this.InitializeLoggingConfig(context.App.ConfigService);
            context.RequestedAction = RequestedAction.Restart;
        }

        public void Upgrade(IUpgradeContext context)
        {
        }

        public void Uninstall(IUninstallContext context)
        {
            throw new NotSupportedException($@"Module {nameof(ConsoleAppInitializerModule)} does not support uninstallation.");
        }

        protected override void InitializeModule(IInitializeContext context)
        {
        }

        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private methods
        private void InitializeAppConfig(IConfigService service)
        {
            var config = service.Get<AppConfig>();
            if (config.Modules.Count > 1)
                throw new InvalidOperationException($@"Module {nameof(ConsoleAppInitializerModule)} found other modules registered besides itself.");

            this.GetType()
                .GetReferencedModules()
                .Select(ModuleConfigElement.Create)
                .ToList()
                .ForEach(config.Modules.Add);
        }

        private void InitializeLoggingConfig(IConfigService service)
        {
            service.Get<LoggingConfig>().Async = false;
        }
        #endregion

        #region Private fields and constants
        private static readonly IEnumerable<Type> Modules = new List<Type>
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
            typeof(Factory.FactoryModule),
            typeof(Logging.LoggingModule),
            typeof(Logging.Console.ConsoleLoggerModule),
            //typeof(Logging.File.FileLoggerModule),
            typeof(Text.TextModule),
            typeof(Time.TimeModule),
            typeof(Web.Client.WebClientModule),
            //typeof(Web.Server.WebServerModule)
        };
        #endregion
    }
}
