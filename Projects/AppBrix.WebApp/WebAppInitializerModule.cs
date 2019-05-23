using AppBrix.Caching;
using AppBrix.Caching.Memory;
using AppBrix.Cloning;
using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Yaml;
using AppBrix.Container;
using AppBrix.Data;
using AppBrix.Data.Migration;
using AppBrix.Data.Sqlite;
using AppBrix.Events;
using AppBrix.Events.Async;
using AppBrix.Events.Schedule;
using AppBrix.Events.Schedule.Cron;
using AppBrix.Events.Schedule.Timer;
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.File;
using AppBrix.Modules;
using AppBrix.Permissions;
using AppBrix.Text;
using AppBrix.Time;
using AppBrix.Web.Client;
using AppBrix.Web.Server;
using AppBrix.WebApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.WebApp
{
    /// <summary>
    /// Initializes web application configuration.
    /// </summary>
    public sealed class WebAppInitializerModule : ModuleBase
    {
        #region Properties
        public override IEnumerable<Type> Dependencies => new[]
        {
            //typeof(CachingModule),
            typeof(MemoryCachingModule),
            typeof(CloningModule),
            typeof(ContainerModule),
            typeof(DataModule),
            //typeof(InMemoryDataModule),
            typeof(MigrationDataModule),
            typeof(SqliteDataModule),
            //typeof(SqlServerDataModule),
            typeof(EventsModule),
            typeof(AsyncEventsModule),
            typeof(ScheduledEventsModule),
            typeof(CronScheduledEventsModule),
            typeof(TimerScheduledEventsModule),
            typeof(Factory.FactoryModule),
            typeof(Logging.LoggingModule),
            //typeof(Logging.Console.ConsoleLoggerModule),
            typeof(FileLoggerModule),
            typeof(PermissionsModule),
            typeof(TextModule),
            typeof(TimeModule),
            typeof(WebClientModule),
            typeof(WebServerModule)
        };
        #endregion

        #region Public and overriden methods
        public static IApp CreateApp()
        {
            var configService = new ConfigService(new FilesConfigProvider("./Config", "yaml"), new YamlConfigSerializer());
            if (configService.Get<AppConfig>().Modules.Count == 0)
                configService.Get<AppConfig>().Modules.Add(ModuleConfigElement.Create<WebAppInitializerModule>());

            var app = AppBrix.App.Create(configService);
            app.Start();
            return app;
        }

        protected override void Install(IInstallContext context)
        {
            this.InitializeAppConfig(context.App.ConfigService);
            this.InitializeLoggingConfig(context.App.ConfigService);
            context.RequestedAction = RequestedAction.Restart;
        }

        protected override void Uninstall(IUninstallContext context)
        {
            throw new NotSupportedException($@"Module {nameof(WebAppInitializerModule)} does not support uninstallation.");
        }

        protected override void Initialize(IInitializeContext context)
        {
            this.booksService.Initialize(context);
            this.App.Container.Register(this.booksService);
            this.App.GetEventHub().Subscribe<IConfigureWebHost>(webHost => webHost.Builder.ConfigureServices(this.ConfigureServices));
            this.App.GetEventHub().Subscribe<IConfigureApplication>(this.Configure);
        }

        protected override void Uninitialize()
        {
            this.booksService.Uninitialize();
        }
        #endregion

        #region Private methods
        private void InitializeAppConfig(IConfigService service)
        {
            var config = service.Get<AppConfig>();
            if (config.Modules.Count > 1)
                throw new InvalidOperationException($@"Module {nameof(WebAppInitializerModule)} found other modules registered besides itself.");

            this.GetAllDependencies()
                .OrderBy(x => x.Namespace)
                .ThenBy(x => x.Name)
                .Select(ModuleConfigElement.Create)
                .ToList()
                .ForEach(config.Modules.Add);
        }

        private void InitializeLoggingConfig(IConfigService service)
        {
            service.Get<LoggingConfig>().Async = false;
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        private void Configure(IConfigureApplication application)
        {
            //if (env.IsDevelopment())
            //{
            //    application.Builder.UseDeveloperExceptionPage();
            //}
            //else
            //{
            //    application.Builder.UseHsts();
            //}

            application.Builder.UseHttpsRedirection();
            application.Builder.UseMvc();
        }
        #endregion

        #region Private fields and constants
        private BooksService booksService = new BooksService();
        #endregion
    }
}
