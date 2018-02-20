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
            this.logger.Value.Initialize(context);
            this.App.Container.Register(this.logger.Value);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            this.logger.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private Lazy<FileLogger> logger = new Lazy<FileLogger>();
        #endregion
    }
}
