// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Logging.File.Impl;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Logging.File
{
    /// <summary>
    /// Class that registers a console logger.
    /// </summary>
    public sealed class FileLoggerModule : ModuleBase
    {
        #region Public and overriden methods
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Container.Register(this);
            this.logger.Initialize(context);
            this.App.Container.Register(this.logger);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            this.logger.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private FileLogger logger = new FileLogger();
        #endregion
    }
}
