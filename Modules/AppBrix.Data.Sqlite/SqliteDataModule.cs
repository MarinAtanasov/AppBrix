// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.Sqlite.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Data.Sqlite
{
    /// <summary>
    /// Module used for regitering a SqlServer provider.
    /// </summary>
    public sealed class SqliteDataModule : ModuleBase
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
            this.configurer.Value.Initialize(context);
            this.App.Container.Register(this.configurer.Value);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            this.configurer.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<SqliteDbContextConfigurer> configurer = new Lazy<SqliteDbContextConfigurer>();
        #endregion
    }
}
