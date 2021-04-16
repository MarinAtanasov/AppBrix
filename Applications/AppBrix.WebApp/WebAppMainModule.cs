using AppBrix.Caching.Memory;
using AppBrix.Cloning;
using AppBrix.Container;
using AppBrix.Data;
using AppBrix.Data.Migrations;
using AppBrix.Data.Sqlite;
using AppBrix.Events;
using AppBrix.Events.Async;
using AppBrix.Events.Schedule;
using AppBrix.Events.Schedule.Cron;
using AppBrix.Events.Schedule.Timer;
using AppBrix.Lifecycle;
using AppBrix.Logging.File;
using AppBrix.Modules;
using AppBrix.Permissions;
using AppBrix.Random;
using AppBrix.Text;
using AppBrix.Time;
using AppBrix.Web.Client;
using AppBrix.Web.Server;
using AppBrix.Web.Server.Events;
using AppBrix.WebApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

namespace AppBrix.WebApp
{
    /// <summary>
    /// Initializes web application configuration.
    /// </summary>
    public sealed class WebAppMainModule : MainModuleBase
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
            typeof(MigrationsDataModule),
            typeof(SqliteDataModule),
            //typeof(SqlServerDataModule),
            typeof(EventsModule),
            typeof(AsyncEventsModule),
            typeof(ScheduledEventsModule),
            typeof(CronScheduledEventsModule),
            typeof(TimerScheduledEventsModule),
            typeof(Factory.FactoryModule),
            typeof(Logging.LoggingModule),
            //typeof(Logging.Console.ConsoleLoggingModule),
            typeof(FileLoggingModule),
            typeof(PermissionsModule),
            typeof(RandomModule),
            typeof(TextModule),
            typeof(TimeModule),
            typeof(WebClientModule),
            typeof(WebServerModule)
        };
        #endregion

        #region Public and overriden methods
        protected override void Initialize(IInitializeContext context)
        {
            this.booksService.Initialize(context);
            this.App.Container.Register(this.booksService);
            this.App.GetEventHub().Subscribe<IConfigureHost>(webHost => webHost.Builder.ConfigureServices(this.ConfigureServices));
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
            services.AddControllers();
        }

        private void Configure(IConfigureApplication application)
        {
            var app = application.Builder;
            var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        #endregion

        #region Private fields and constants
        private readonly BooksService booksService = new BooksService();
        #endregion
    }
}
