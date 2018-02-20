// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Data
{
    /// <summary>
    /// Module used for enabling database functionality.
    /// </summary>
    public sealed class DataModule : ModuleBase
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
            this.contextService.Value.Initialize(context);
            this.App.Container.Register(this.contextService.Value);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            this.contextService.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultDbContextService> contextService = new Lazy<DefaultDbContextService>();
        #endregion
    }
}
