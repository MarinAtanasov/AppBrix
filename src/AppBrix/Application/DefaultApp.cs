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
        /// Indicates whether the application has been started.
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Indicates whether the application is in an initialized state.
        /// </summary>
        public bool IsInitialized { get; private set; }
        #endregion

        #region IPublic and overriden methods
        public void Start()
        {
            lock (this.modules)
            {
                if (this.IsStarted)
                    throw new InvalidOperationException("The application is already started.");

                this.RegisterModules();
                this.Initialize();
                this.ConfigManager.SaveAll();
            }
        }

        public void Stop()
        {
            lock (this.modules)
            {
                if (!this.IsStarted)
                    throw new InvalidOperationException("The application is not started.");

                this.Uninitialize();
                this.UnregisterModules();
                this.ConfigManager.SaveAll();
            }
        }

        public void Initialize()
        {
            lock (this.modules)
            {
                if (!this.IsStarted)
                    throw new InvalidOperationException("The application is stopped.");
                if (this.IsInitialized)
                    return; // The application is already initialized.

                this.InitializeInternal();
            }
        }

        public void Uninitialize()
        {
            lock (this.modules)
            {
                if (!this.IsStarted)
                    throw new InvalidOperationException("The application is stopped.");
                if (!this.IsInitialized)
                    return; // The application is not initialized.

                this.UninitializeInternal(this.modules.Count - 1);
            }
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
        private void InitializeInternal()
        {
            this.IsInitialized = true;

            var initializeContext = new DefaultInitializeContext(this);
            for (var i = 0; i < this.modules.Count; i++)
            {
                var moduleInfo = this.modules[i];
                if (moduleInfo.Status != ModuleStatus.Enabled)
                    continue;

                var requestedAction = this.InstallOrUpgradeModule(moduleInfo);
                switch (requestedAction)
                {
                    case RequestedAction.None:
                        break;
                    case RequestedAction.Reinitialize:
                        this.ConfigManager.SaveAll();
                        this.UninitializeInternal(i - 1);
                        this.Initialize();
                        return;
                    case RequestedAction.Restart:
                        this.ConfigManager.SaveAll();
                        this.UninitializeInternal(i - 1);
                        this.UnregisterModules();
                        this.Start();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException($@"{nameof(RequestedAction)}: {requestedAction}");
                }

                moduleInfo.Module.Initialize(initializeContext);
            }
        }

        private RequestedAction InstallOrUpgradeModule(ModuleInfo moduleInfo)
        {
            RequestedAction? requestedAction = null;
            var version = moduleInfo.Module.GetType().GetTypeInfo().Assembly.GetName().Version;
            if (moduleInfo.Config.Version == null)
            {
                var context = new DefaultInstallContext(this);
                moduleInfo.Module.Install(context);
                requestedAction = context.RequestedAction;
            }
            else if (moduleInfo.Config.Version < version)
            {
                var context = new DefaultUpgradeContext(this, moduleInfo.Config.Version);
                moduleInfo.Module.Upgrade(context);
                requestedAction = context.RequestedAction;
            }

            if (requestedAction.HasValue)
            {
                moduleInfo.Config.Version = version;
                this.ConfigManager.Save<AppConfig>();
            }

            return requestedAction ?? RequestedAction.None;
        }

        private void UninitializeInternal(int lastInitialized)
        {
            for (var i = lastInitialized; i >= 0; i--)
            {
                var moduleInfo = this.modules[i];
                if (moduleInfo.Status == ModuleStatus.Enabled)
                    moduleInfo.Module.Uninitialize();

                if (moduleInfo.Config.Status == ModuleStatus.Uninstalling)
                {
                    moduleInfo.Module.Uninstall(new DefaultUninstallContext(this));
                    moduleInfo.Config.Status = ModuleStatus.Disabled;
                    moduleInfo.Config.Version = null;
                    this.ConfigManager.Save<AppConfig>();
                }
            }

            for (var i = 0; i < this.modules.Count; i++)
            {
                var moduleInfo = this.modules[i];
                if (moduleInfo.Status != moduleInfo.Config.Status)
                {
                    this.modules[i] = new ModuleInfo(moduleInfo.Module, moduleInfo.Config);
                }
            }

            this.IsInitialized = false;
        }

        private void RegisterModules()
        {
            this.IsStarted = true;

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

            this.IsStarted = false;
        }

        private IEnumerable<ModuleInfo> GetModuleInfos()
        {
            return this.ConfigManager.Get<AppConfig>().Modules
                .Where(m => m.Status != ModuleStatus.Disabled || m.Version != null)
                .Select(m => new ModuleInfo(this.CreateModule(m), m));
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
