// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using AppBrix.Resolve;
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
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the application's configuration.
        /// </summary>
        public IAppConfig AppConfig { get; private set; }

        /// <summary>
        /// Gets or sets the currently loaded object resolver.
        /// </summary>
        public IResolver Resolver { get; set; }

        /// <summary>
        /// Indicates whether the application is in an initialized state.
        /// </summary>
        private bool IsInitialized { get; set; }
        #endregion

        #region IAppModule implementation
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
        #endregion

        #region IApplicationLifecycle implementation
        public void Initialize()
        {
            if (this.IsInitialized)
                throw new ApplicationException("The application is already initialized.");

            this.IsInitialized = true;
            foreach (var module in this.modules)
            {
                module.Initialize(new InitializeContext(this));
            }
        }

        public void Uninitialize()
        {
            foreach (var module in this.modules.Reverse())
            {
                module.Uninitialize();
            }
            this.IsInitialized = false;
        }

        private IEnumerable<Type> GetModuleTypes()
        {
            return this.AppConfig.Modules.Select(m => Type.GetType(m.Type));
        }
        #endregion

        #region Private methods
        private void RegisterModules()
        {
            var moduleTypes = this.GetModuleTypes();
            var modules = this.CreateModules(moduleTypes);
            foreach (var module in modules.OrderByDescending(m => m.LoadPriority))
            {
                this.modules.Add(module);
            }
        }

        private void UnregisterModules()
        {
            this.modules.Clear();
        }

        private IEnumerable<ModuleBase> CreateModules(IEnumerable<Type> moduleTypes)
        {
            return moduleTypes.Select(type => (ModuleBase)type.CreateObject());
        }
        #endregion

        #region Private fields and constants
        private readonly ICollection<ModuleBase> modules = new List<ModuleBase>();
        #endregion
    }
}
