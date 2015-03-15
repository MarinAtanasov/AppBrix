// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Loggers;
using AppBrix.Modules;
using AppBrix.Resolve;
using System;
using System.Linq;

namespace AppBrix.Logging.File
{
    /// <summary>
    /// Class that registers a console logger.
    /// </summary>
    public sealed class FileLoggerModule : ModuleBase
    {
        #region Properties
        public override int LoadPriority
        {
            get
            {
                return (int)ModuleLoadPriority.Logger;
            }
        }
        #endregion

        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Resolver.Register(this);
            var async = this.App.GetConfig<LoggingConfig>().Async;
            this.logger = Logger.Create(new FileLogWriter(), async);
            this.App.Resolver.Register(this.logger, this.logger.GetType());
        }

        protected override void UninitializeModule()
        {
            this.logger.Uninitialize();
            this.logger = null;
        }
        #endregion

        #region Private fields and constants
        private Logger logger;
        #endregion
    }
}
