// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Logging.Console.Impl;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Logging.Console
{
    /// <summary>
    /// Class that registers a console logger.
    /// </summary>
    public sealed class ConsoleLoggerModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Container.Register(this);
            this.logger.Value.Initialize(context);
            this.App.Container.Register(this.logger.Value);
        }

        protected override void UninitializeModule()
        {
            this.logger.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private Lazy<ConsoleLogger> logger = new Lazy<ConsoleLogger>();
        #endregion
    }
}
