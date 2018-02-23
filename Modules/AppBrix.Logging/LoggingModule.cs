// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
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
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Container.Register(this);
            this.logHub.Initialize(context);
            this.App.Container.Register(this.logHub);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            this.logHub.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly DefaultLogHub logHub = new DefaultLogHub();
        #endregion
    }
}
