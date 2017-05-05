// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
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
            this.configurer.Value.Initialize(context);
            this.App.GetContainer().Register(this.configurer.Value);
        }

        protected override void UninitializeModule()
        {
            this.configurer.Value.Uninitialize();
        }
        #endregion
        
        #region Private fields and constants
        private readonly Lazy<InMemoryDbContextConfigurer> configurer = new Lazy<InMemoryDbContextConfigurer>();
        #endregion
    }
}
