using AppBrix.Configuration;
using AppBrix.Data.Migration;
using AppBrix.Data.Sqlite;
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.File;
using AppBrix.Modules;
using AppBrix.Text;
using AppBrix.Web.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.WebApp
{
    /// <summary>
    /// Initializes application configuration.
    /// This module should be first on the list in order to configure the application's configuration.
    /// </summary>
    public sealed class ConfigInitializerModule : IModule, IInstallable
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

            ConfigInitializerModule.Modules
                .SelectMany(module => module.GetTypeInfo().Assembly.GetAllReferencedAssemblies())
                .Distinct()
                .SelectMany(assembly => assembly.ExportedTypes)
                .Where(type => type.GetTypeInfo().IsClass && !type.GetTypeInfo().IsAbstract)
                .Where(type => typeof(IModule).IsAssignableFrom(type))
                .Concat(ConfigInitializerModule.Modules)
                .Distinct()
                .OrderBy(type => type.Namespace)
                .ThenBy(type => type.Name)
                .ToList()
                .ForEach(type => config.Modules.Add(ModuleConfigElement.Create(type)));
        }

        private void InitializeLoggingConfig(IConfigManager manager)
        {
            manager.Get<LoggingConfig>().Async = false;
        }
        #endregion

        #region Private fields and constants
        private static readonly IEnumerable<Type> Modules = new List<Type>()
        {
            typeof(MigrationDataModule),
            typeof(SqliteDataModule),
            typeof(FileLoggerModule),
            typeof(TextModule),
            typeof(WebServerModule)
        };
        #endregion
    }
}
