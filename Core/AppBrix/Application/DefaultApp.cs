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

        #region Public and overriden methods
        public void Start()
        {
            lock (this.modules)
            {
                if (this.IsStarted)
                    throw new InvalidOperationException("The application is already started.");

                this.RegisterModules();
                this.Initialize();
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

                try
                {
                    this.InitializeInternal();
                }
                finally
                {
                    this.ConfigService.Save();
                }
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

                try
                {
                    this.UninitializeInternal(this.modules.Count - 1);
                }
                finally
                {
                    this.ConfigService.Save();
                }
            }
        }
        #endregion

        #region Application lifecycle
        private void RegisterModules()
        {
            this.IsStarted = true;

            var moduleInfos = this.ConfigService.GetAppConfig().Modules
                .Where(m => m.Status != ModuleStatus.Disabled || m.Version != null)
                .Select(m => new ModuleInfo((IModule)Type.GetType(m.Type).CreateObject(), m))
                .SortByPriority();
            this.modules.AddRange(moduleInfos);
        }

        private void UnregisterModules()
        {
            this.modules.Clear();

            this.IsStarted = false;
        }

        private void InitializeInternal()
        {
            this.IsInitialized = true;

            this.ConfigureModules();
            this.InstallAndInitializeModules();
        }

        private void UninitializeInternal(int lastInitialized)
        {
            for (var i = lastInitialized; i >= 0; i--)
            {
                var moduleInfo = this.modules[i];
                this.UninitializeModule(moduleInfo);
                this.UninstallModule(moduleInfo);
            }

            for (var i = 0; i < this.modules.Count; i++)
            {
                var moduleInfo = this.modules[i];
                if (moduleInfo.Status != moduleInfo.Config.Status)
                    this.modules[i] = new ModuleInfo(moduleInfo.Module, moduleInfo.Config);
            }

            this.IsInitialized = false;
        }
        #endregion

        #region Modules lifecycle
        private void ConfigureModules()
        {
            for (var i = 0; i < this.modules.Count; i++)
            {
                var moduleInfo = this.modules[i];
                if (moduleInfo.Status != ModuleStatus.Enabled)
                    continue;

                var requestedAction = this.ConfigureModule(moduleInfo);
                switch (requestedAction)
                {
                    case RequestedAction.None:
                        break;
                    case RequestedAction.Reinitialize:
                        i = -1;
                        break;
                    case RequestedAction.Restart:
                        this.UnregisterModules();
                        this.Start();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException($@"{nameof(RequestedAction)}: {requestedAction}");
                }
            }
        }

        private RequestedAction ConfigureModule(ModuleInfo moduleInfo)
        {
            var version = moduleInfo.Module.GetType().Assembly.GetName().Version;
            if (moduleInfo.Config.Version is null || moduleInfo.Config.Version < version)
            {
                var context = new ConfigureContext(this, moduleInfo.Config.Version ?? DefaultApp.EmptyVersion);
                moduleInfo.Module.Configure(context);
                return context.RequestedAction;
            }
            return RequestedAction.None;
        }

        private void InstallAndInitializeModules()
        {
            for (var i = 0; i < this.modules.Count; i++)
            {
                var moduleInfo = this.modules[i];
                if (moduleInfo.Status != ModuleStatus.Enabled)
                    continue;

                var requestedAction = this.InstallModule(moduleInfo);
                if (requestedAction == RequestedAction.None)
                    requestedAction = this.InitializeModule(moduleInfo);
                switch (requestedAction)
                {
                    case RequestedAction.None:
                        break;
                    case RequestedAction.Reinitialize:
                        this.UninitializeInternal(i - 1);
                        this.InitializeInternal();
                        return;
                    case RequestedAction.Restart:
                        this.UninitializeInternal(i - 1);
                        this.UnregisterModules();
                        this.Start();
                        return;
                    default:
                        throw new ArgumentOutOfRangeException($@"{nameof(RequestedAction)}: {requestedAction}");
                }
            }
        }

        private RequestedAction InstallModule(ModuleInfo moduleInfo)
        {
            var version = moduleInfo.Module.GetType().Assembly.GetName().Version;
            if (moduleInfo.Config.Version is null || moduleInfo.Config.Version < version)
            {
                var context = new InstallContext(this, moduleInfo.Config.Version ?? DefaultApp.EmptyVersion);
                moduleInfo.Module.Install(context);
                moduleInfo.Config.Version = version;
                return context.RequestedAction;
            }
            return RequestedAction.None;
        }

        private RequestedAction InitializeModule(ModuleInfo moduleInfo)
        {
            var initializeContext = new InitializeContext(this);
            moduleInfo.Module.Initialize(initializeContext);
            return initializeContext.RequestedAction;
        }

        private void UninitializeModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo.Status == ModuleStatus.Enabled)
                moduleInfo.Module.Uninitialize();
        }

        private void UninstallModule(ModuleInfo moduleInfo)
        {
            if (moduleInfo.Config.Status == ModuleStatus.Uninstalling)
            {
                moduleInfo.Module.Uninstall(new UninstallContext(this));
                moduleInfo.Config.Status = ModuleStatus.Disabled;
                moduleInfo.Config.Version = null;
            }
        }
        #endregion

        #region Private fields and constants
        private static readonly Version EmptyVersion = new Version();
        private readonly List<ModuleInfo> modules = new List<ModuleInfo>();
        #endregion
    }
}
