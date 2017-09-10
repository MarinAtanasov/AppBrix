// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Caching.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Caching
{
    /// <summary>
    /// Module used for caching objects.
    /// </summary>
    public sealed class CachingModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Container.Register(this);
            this.cache.Value.Initialize(context);
            this.App.Container.Register(this.cache.Value);
        }

        protected override void UninitializeModule()
        {
            this.cache.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultCache> cache = new Lazy<DefaultCache>();
        #endregion
    }
}
