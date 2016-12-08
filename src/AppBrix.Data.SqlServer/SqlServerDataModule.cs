// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.SqlServer.Configuration;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using Microsoft.EntityFrameworkCore;
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
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            this.configurer.Value.Initialize(context);
            this.App.GetContainer().Register(this.configurer.Value);
        }

        protected override void UninitializeModule()
        {
            this.configurer.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<SqlServerDbContextConfigurer> configurer = new Lazy<SqlServerDbContextConfigurer>();
        #endregion
    }
}
