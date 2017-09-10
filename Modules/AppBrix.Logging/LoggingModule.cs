// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Impl;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Logging
{
    /// <summary>
    /// Module used for registering a logs hub.
    /// </summary>
    public sealed class LoggingModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Container.Register(this);
            var defaultLogHub = this.logHub.Value;
            defaultLogHub.Initialize(context);
            this.App.Container.Register(defaultLogHub);
            this.App.GetFactory().Register(() =>
                this.App.GetConfig<LoggingConfig>().Async ? (ILogger)
                new AsyncLogger(this.App.GetFactory().Get<ILogWriter>()) :
                new SyncLogger(this.App.GetFactory().Get<ILogWriter>()));
        }

        protected override void UninitializeModule()
        {
            this.logHub.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultLogHub> logHub = new Lazy<DefaultLogHub>();
        #endregion
    }
}
