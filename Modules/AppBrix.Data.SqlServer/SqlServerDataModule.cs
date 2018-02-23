// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.SqlServer.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Data.SqlServer
{
    /// <summary>
    /// Module used for regitering a SqlServer provider.
    /// </summary>
    public sealed class SqlServerDataModule : ModuleBase
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
            this.configurer.Initialize(context);
            this.App.Container.Register(this.configurer);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            this.configurer.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly SqlServerDbContextConfigurer configurer = new SqlServerDbContextConfigurer();
        #endregion
    }
}
