// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Container;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Application
{
    internal sealed class DefaultApp : IApp
    {
        #region Construction
        public DefaultApp(IConfigService configService)
        {
            if (configService is null)
                throw new ArgumentNullException(nameof(configService));

            this.ConfigService = configService;
        }
        #endregion

        #region Properties
        #nullable disable
        public IContainer Container { get; set; }
        #nullable restore

        public IConfigService ConfigService { get; }

        public bool IsStarted { get; private set; }

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
                this.ConfigService.Save();
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
                this.ConfigService.Save();
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
        #endregion

        #region Private methods
        private void InitializeInternal()
        {
            this.IsInitialized = true;

            for (var i = 0; i < this.modules.Count; i++)
            {
                var moduleInfo = this.modules[i];
                if (moduleInfo.Status != ModuleStatus.Enabled)
                    continue;

                var requestedAction = this.InstallOrUpgradeModule(moduleInfo);
                if (requestedAction == RequestedAction.None)
                {
                    var initializeContext = new InitializeContext(this);
                    moduleInfo.Module.Initialize(initializeContext);
                    requestedAction = initializeContext.RequestedAction;
                }
                switch (requestedAction)
                {
                    case RequestedAction.None:
                        break;
                    case RequestedAction.Reinitialize:
                        this.ConfigService.Save();
                        this.UninitializeInternal(i - 1);
                        this.Initialize();
                        return;
                    case RequestedAction.Restart:
                        this.ConfigService.Save();
                        this.UninitializeInternal(i - 1);
                        this.UnregisterModules();
                        this.Start();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException($@"{nameof(RequestedAction)}: {requestedAction}");
                }
            }
        }

        private RequestedAction InstallOrUpgradeModule(ModuleInfo moduleInfo)
        {
            RequestedAction requestedAction = RequestedAction.None;
            var version = moduleInfo.Module.GetType().Assembly.GetName().Version;
            if (moduleInfo.Config.Version is null)
            {
                var context = new InstallContext(this);
                moduleInfo.Module.Install(context);
                requestedAction = context.RequestedAction;
                moduleInfo.Config.Version = version;
            }
            else if (moduleInfo.Config.Version < version)
            {
                var context = new UpgradeContext(this, moduleInfo.Config.Version);
                moduleInfo.Module.Upgrade(context);
                requestedAction = context.RequestedAction;
                moduleInfo.Config.Version = version;
            }

            return requestedAction;
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
                    moduleInfo.Module.Uninstall(new UninstallContext(this));
                    moduleInfo.Config.Status = ModuleStatus.Disabled;
                    moduleInfo.Config.Version = null;
                    this.ConfigService.Save<AppConfig>();
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

        private IEnumerable<ModuleInfo> GetModuleInfos() =>
            this.ConfigService.GetAppConfig().Modules
                .Where(m => m.Status != ModuleStatus.Disabled || m.Version != null)
                .Select(m => new ModuleInfo(this.CreateModule(m), m));

        private IModule CreateModule(ModuleConfigElement element) => (IModule)Type.GetType(element.Type).CreateObject();
        #endregion

        #region Private fields and constants
        private readonly List<ModuleInfo> modules = new List<ModuleInfo>();
        #endregion
    }
}
