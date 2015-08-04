// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Application
{
    /// <summary>
    /// The default app used when no app has been specified.
    /// </summary>
    internal sealed class DefaultApp : IApp
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of the default app with the specified configuration.
        /// </summary>
        /// <param name="config"></param>
        public DefaultApp(IAppConfig config)
        {
            this.AppConfig = config;
            this.Id = Guid.NewGuid();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the id of the application.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets the application's configuration.
        /// </summary>
        public IAppConfig AppConfig { get; private set; }
        
        /// <summary>
        /// Indicates whether the application is in an initialized state.
        /// </summary>
        private bool IsInitialized { get; set; }
        #endregion

        #region IPublic and overriden methods
        public void Start()
        {
            this.RegisterModules();
            this.Initialize();
        }

        public void Stop()
        {
            this.Uninitialize();
            this.UnregisterModules();
            this.AppConfig.Save();
        }

        public void Initialize()
        {
            if (this.IsInitialized)
                throw new ApplicationException("The application is already initialized.");

            this.IsInitialized = true;

            foreach (var moduleInfo in this.modules.Where(m => m.Status == ModuleStatus.Enabled))
            {
                if (moduleInfo.Module is IInstallable)
                {
                    if (moduleInfo.Config.Version == null)
                    {
                        ((IInstallable)moduleInfo.Module).Install(new DefaultInstallContext(this));
                        moduleInfo.Config.Version = moduleInfo.Module.GetType().Assembly.GetName().Version;
                    }
                    else if (moduleInfo.Config.Version < moduleInfo.Module.GetType().Assembly.GetName().Version)
                    {
                        ((IInstallable)moduleInfo.Module).Upgrade(new DefaultUpgradeContext(this, moduleInfo.Config.Version));
                        moduleInfo.Config.Version = moduleInfo.Module.GetType().Assembly.GetName().Version;
                    }
                }
                    
                moduleInfo.Module.Initialize(new DefaultInitializeContext(this));
            }
        }

        public void Uninitialize()
        {
            foreach (var moduleInfo in this.modules.Reverse())
            {
                if (moduleInfo.Status == ModuleStatus.Enabled)
                    moduleInfo.Module.Uninitialize();

                if (moduleInfo.Module is IInstallable && moduleInfo.Config.Status == ModuleStatus.Uninstalling)
                {
                    ((IInstallable)moduleInfo.Module).Uninstall(new DefaultInstallContext(this));
                    moduleInfo.Config.Status = ModuleStatus.Disabled;
                    moduleInfo.Config.Version = null;
                }
            }
            this.IsInitialized = false;
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
        private void RegisterModules()
        {
            var modules = this.GetModuleInfos();
            foreach (var module in modules.OrderByDescending(m => m.Module.LoadPriority))
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
            return this.AppConfig.Modules.Where(m => m.Status != ModuleStatus.Disabled).Select(m => new ModuleInfo(this.CreateModule(m), m));
        }

        private IModule CreateModule(ModuleConfigElement element)
        {
            return Type.GetType(element.Type).CreateObject<IModule>();
        }
        #endregion

        #region Private fields and constants
        private readonly ICollection<ModuleInfo> modules = new List<ModuleInfo>();
        #endregion
    }
}
