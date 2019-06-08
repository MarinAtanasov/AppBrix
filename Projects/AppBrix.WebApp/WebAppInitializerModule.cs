using AppBrix.Caching.Memory;
using AppBrix.Cloning;
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
    public sealed class WebAppInitializerModule : MainModuleBase
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
        protected override void Install(IInstallContext context)
        {
            base.Install(context);
            this.App.GetConfig<LoggingConfig>().Async = false;
            context.RequestedAction = RequestedAction.Restart;
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
