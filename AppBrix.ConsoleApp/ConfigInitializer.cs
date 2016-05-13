using System;
using System.Collections.Generic;
using System.Linq;
using AppBrix.Caching;
using AppBrix.Cloning;
using AppBrix.Configuration;
using AppBrix.Events;
using AppBrix.Factory;
using AppBrix.Logging;
using AppBrix.Logging.Console;
using AppBrix.Logging.File;
using AppBrix.Resolver;
using AppBrix.Time;
using AppBrix.Web.Client;
using AppBrix.Web.Server;

namespace AppBrix.ConsoleApp
{
    /// <summary>
    /// Initializes the default configuration no configuration has been loaded.
    /// </summary>
    internal class ConfigInitializer
    {
        #region Public methods
        /// <summary>
        /// If a configuration has not been loaded, initializes the default configuration.
        /// </summary>
        /// <param name="manager">The configuration manager.</param>
        public void Initialize(IConfigManager manager)
        {
            if (manager.Get<AppConfig>().Modules.Count > 0)
                return;

            this.InitializeAppConfig(manager);
        }
        #endregion

        #region Private methods
        private void InitializeAppConfig(IConfigManager manager)
        {
            var config = manager.Get<AppConfig>();
            foreach (var module in ConfigInitializer.Modules)
            {
                var element = ModuleConfigElement.Create(module);
                if (ConfigInitializer.DisabledModules.Contains(module))
                    element.Status = ModuleStatus.Disabled;
                config.Modules.Add(element);
            }
        }
        #endregion

        #region Private fields and constants
        private static readonly IEnumerable<Type> Modules = new List<Type>()
        {
            typeof(CacheModule),
            typeof(CloningModule),
            typeof(EventsModule),
            typeof(FactoryModule),
            typeof(LoggingModule),
            typeof(ConsoleLoggerModule),
            typeof(FileLoggerModule),
            typeof(ResolverModule),
            typeof(TimeModule),
            typeof(WebClientModule),
            typeof(WebServerModule)
        };

        private static readonly HashSet<Type> DisabledModules = new HashSet<Type>()
        {
            typeof(FileLoggerModule),
            typeof(WebClientModule),
            typeof(WebServerModule)
        };
        #endregion
    }
}
