// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Application
{
    /// <summary>
    /// The default implementation of an application.
    /// </summary>
    internal sealed class DefaultApp : IApp
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of the default app with the specified configuration manager.
        /// </summary>
        /// <param name="configManager">The configuration manager.</param>
        public DefaultApp(IConfigManager configManager)
        {
            this.ConfigManager = configManager;
            this.Id = Guid.NewGuid();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the id of the application.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the application's configuration manager.
        /// </summary>
        public IConfigManager ConfigManager { get; }
        
        /// <summary>
        /// Indicates whether the application is in an initialized state.
        /// </summary>
        public bool IsInitialized { get; private set; }
        #endregion

        #region IPublic and overriden methods
        public void Start()
        {
            this.RegisterModules();
            this.Initialize();
            this.ConfigManager.SaveAll();
        }

        public void Stop()
        {
            this.Uninitialize();
            this.UnregisterModules();
            this.ConfigManager.SaveAll();
        }

        public void Initialize()
        {
            if (this.IsInitialized)
                throw new InvalidOperationException("The application is already initialized.");

            this.IsInitialized = true;

            var initializeContext = new DefaultInitializeContext(this);
            for (var i = 0; i < this.modules.Count; i++)
            {
                var moduleInfo = this.modules[i];
                if (moduleInfo.Status != ModuleStatus.Enabled)
                    continue;

                var module = moduleInfo.Module as IInstallable;
                if (module != null)
                {
                    var requestedAction = this.InstallOrUpgradeModule(module, moduleInfo);
                    switch (requestedAction)
                    {
                        case RequestedAction.None:
                            break;
                        case RequestedAction.Reinitialize:
                            this.ConfigManager.SaveAll();
                            this.Uninitialize(i - 1);
                            this.Initialize();
                            return;
                        case RequestedAction.Restart:
                            this.ConfigManager.SaveAll();
                            this.Uninitialize(i - 1);
                            this.UnregisterModules();
                            this.Start();
                            return;
                        default:
                            throw new ArgumentOutOfRangeException($@"{nameof(requestedAction)}: {requestedAction}");
                    }
                }

                moduleInfo.Module.Initialize(initializeContext);
            }
        }

        public void Uninitialize()
        {
            if (!this.IsInitialized)
                throw new InvalidOperationException("The application is not initialized.");

            this.Uninitialize(this.modules.Count - 1);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
        #endregion

        #region Private methods
        private RequestedAction InstallOrUpgradeModule(IInstallable module, ModuleInfo moduleInfo)
        {
            RequestedAction? requestedAction = null;
            var version = module.GetType().GetTypeInfo().Assembly.GetName().Version;
            if (moduleInfo.Config.Version == null)
            {
                var context = new DefaultInstallContext(this);
                module.Install(context);
                requestedAction = context.RequestedAction;
            }
            else if (moduleInfo.Config.Version < version)
            {
                var context = new DefaultUpgradeContext(this, moduleInfo.Config.Version);
                module.Upgrade(context);
                requestedAction = context.RequestedAction;
            }

            if (requestedAction.HasValue)
            {
                moduleInfo.Config.Version = version;
                this.ConfigManager.Save<AppConfig>();
            }

            return requestedAction ?? RequestedAction.None;
        }

        private void Uninitialize(int lastInitialized)
        {
            for (var i = lastInitialized; i >= 0; i--)
            {
                var moduleInfo = this.modules[i];
                if (moduleInfo.Status == ModuleStatus.Enabled)
                    moduleInfo.Module.Uninitialize();

                var module = moduleInfo.Module as IInstallable;
                if (module != null && moduleInfo.Config.Status == ModuleStatus.Uninstalling)
                {
                    module.Uninstall(new DefaultUninstallContext(this));
                    moduleInfo.Config.Status = ModuleStatus.Disabled;
                    moduleInfo.Config.Version = null;
                    this.ConfigManager.Save<AppConfig>();
                }
            }

            this.IsInitialized = false;
        }

        private void RegisterModules()
        {
            var moduleInfos = this.GetModuleInfos();
            moduleInfos = ModuleInfo.SortByPriority(moduleInfos);
            foreach (var module in moduleInfos)
            {
                this.modules.Add(module);
            }
        }

        private void UnregisterModules()
        {
            this.modules.Clear();
        }

        private IEnumerable<ModuleInfo> GetModuleInfos()
        {
            return this.ConfigManager.Get<AppConfig>().Modules.Where(m => m.Status != ModuleStatus.Disabled).Select(m => new ModuleInfo(this.CreateModule(m), m));
        }

        private IModule CreateModule(ModuleConfigElement element)
        {
            return Type.GetType(element.Type).CreateObject<IModule>();
        }
        #endregion

        #region Private fields and constants
        private readonly IList<ModuleInfo> modules = new List<ModuleInfo>();
        #endregion
    }
}
