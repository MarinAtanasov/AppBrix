// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.InMemory.Configuration;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data.InMemory
{
    /// <summary>
    /// Module used for regitering an InMemory data provider.
    /// </summary>
    public sealed class InMemoryDataModule : ModuleBase
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
            context.OptionsBuilder.UseInMemoryDatabase(this.App.GetConfig<InMemoryDataConfig>().ConnectionString);
        }
        #endregion
    }
}
