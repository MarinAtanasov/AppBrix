using AppBrix.Configuration;
using AppBrix.Configuration.Files;
using AppBrix.Configuration.Yaml;
using AppBrix.Data.Migration;
using AppBrix.Data.Sqlite;
using AppBrix.Events.Schedule.Cron;
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.File;
using AppBrix.Modules;
using AppBrix.Text;
using AppBrix.Web.Client;
using AppBrix.Web.Server;
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
    public sealed class WebAppInitializerModule : ModuleBase, IInstallable
    {
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
            throw new NotSupportedException($@"Module {nameof(WebAppInitializerModule)} does not support uninstallation.");
        }

        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetEventHub().Subscribe<IConfigureWebHost>(webHost => webHost.Builder.ConfigureServices(services => services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)));
            this.App.GetEventHub().Subscribe<IConfigureApplication>(application =>
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
            });
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private methods
        private void InitializeAppConfig(IConfigService service)
        {
            var config = service.Get<AppConfig>();
            if (config.Modules.Count > 1)
                throw new InvalidOperationException($@"Module {nameof(WebAppInitializerModule)} found other modules registered besides itself.");

            this.GetType()
                .GetModuleDependencies()
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
        /// <summary>
        /// This is required in order for the assembly to reference the project references.
        /// When there are no code references, the project reference does not become an assembly reference.
        /// </summary>
        private static readonly IEnumerable<Type> Modules = new List<Type>
        {
            typeof(MigrationDataModule),
            typeof(SqliteDataModule),
            typeof(FileLoggerModule),
            typeof(CronScheduledEventsModule),
            typeof(TextModule),
            typeof(WebClientModule),
            typeof(WebServerModule)
        };
        #endregion
    }
}
