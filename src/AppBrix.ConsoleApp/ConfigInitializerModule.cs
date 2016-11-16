using AppBrix.Caching;
using AppBrix.Caching.Json;
using AppBrix.Caching.Memory;
using AppBrix.Cloning;
using AppBrix.Configuration;
using AppBrix.Container;
using AppBrix.Events;
using AppBrix.Events.Async;
using AppBrix.Factory;
using AppBrix.Lifecycle;
using AppBrix.Logging;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Console;
using AppBrix.Logging.File;
using AppBrix.Modules;
using AppBrix.Text;
using AppBrix.Time;
using AppBrix.Web.Client;
using AppBrix.Web.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.ConsoleApp
{
    /// <summary>
    /// Initializes application configuration.
    /// This module should be first on the list in order to configure the application's configuration.
    /// </summary>
    public class ConfigInitializerModule : IModule, IInstallable
    {
        #region Public and overriden methods
        public void Install(IInstallContext context)
        {
            this.InitializeAppConfig(context.App.ConfigManager);
            this.InitializeLoggingConfig(context.App.ConfigManager);
            context.RequestedAction = RequestedAction.Restart;
        }

        public void Upgrade(IUpgradeContext context)
        {
        }

        public void Uninstall(IUninstallContext context)
        {
            throw new NotSupportedException($@"Module {nameof(ConfigInitializerModule)} does not support uninstallation.");
        }

        public void Initialize(IInitializeContext context)
        {
        }

        public void Uninitialize()
        {
        }
        #endregion

        #region Private methods
        private void InitializeAppConfig(IConfigManager manager)
        {
            var config = manager.Get<AppConfig>();
            if (config.Modules.Count > 1)
                throw new InvalidOperationException($@"Module {nameof(ConfigInitializerModule)} found other modules registered besides itself.");

            foreach (var module in ConfigInitializerModule.Modules)
            {
                var element = ModuleConfigElement.Create(module);
                if (ConfigInitializerModule.DisabledModules.Contains(module))
                    element.Status = ModuleStatus.Disabled;
                config.Modules.Add(element);
            }
        }

        private void InitializeLoggingConfig(IConfigManager manager)
        {
            manager.Get<LoggingConfig>().Async = false;
        }
        #endregion

        #region Private fields and constants
        private static readonly IEnumerable<Type> Modules = new List<Type>()
        {
            typeof(AsyncEventsModule),
            typeof(CachingModule),
            typeof(JsonCachingModule),
            typeof(MemoryCachingModule),
            typeof(CloningModule),
            typeof(ContainerModule),
            typeof(EventsModule),
            typeof(FactoryModule),
            typeof(LoggingModule),
            typeof(ConsoleLoggerModule),
            typeof(FileLoggerModule),
            typeof(TextModule),
            typeof(TimeModule),
            typeof(WebClientModule),
            typeof(WebServerModule)
        };

        private static readonly HashSet<Type> DisabledModules = new HashSet<Type>()
        {
            typeof(FileLoggerModule),
            typeof(WebServerModule)
        };
        #endregion
    }
}
