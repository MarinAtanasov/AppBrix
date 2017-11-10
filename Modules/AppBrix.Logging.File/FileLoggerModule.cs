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
        private Lazy<FileLogger> logger = new Lazy<FileLogger>();
        #endregion
    }
}
