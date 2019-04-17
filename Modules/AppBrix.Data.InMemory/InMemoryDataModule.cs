// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.InMemory.Impl;
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
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void Initialize(IInitializeContext context)
        {
            this.App.Container.Register(this);
            this.configurer.Initialize(context);
            this.App.Container.Register(this.configurer);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void Uninitialize()
        {
            this.configurer.Uninitialize();
        }
        #endregion
        
        #region Private fields and constants
        private readonly InMemoryDbContextConfigurer configurer = new InMemoryDbContextConfigurer();
        #endregion
    }
}
