// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.SqlServer.Configuration;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using Microsoft.EntityFrameworkCore;
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
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            this.App.GetEventHub().Subscribe<IOnConfiguringDbContext>(this.OnConfiguringDbContext);
        }

        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private methods
        private void OnConfiguringDbContext(IOnConfiguringDbContext context)
        {
            context.OptionsBuilder.UseSqlite(this.App.GetConfig<SqliteDataConfig>().ConnectionString);
        }
        #endregion
    }
}
