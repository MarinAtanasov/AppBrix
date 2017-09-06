// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Caching.Memory.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Caching.Memory
{
    /// <summary>
    /// Module used for caching objects locally in-memory.
    /// This can be used for objects which are long running and should
    /// be disposed after absolute or sliding expiration time.
    /// </summary>
    public sealed class MemoryCachingModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            this.cache.Value.Initialize(context);
            this.App.GetContainer().Register(this.cache.Value);
        }

        protected override void UninitializeModule()
        {
            this.cache.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultMemoryCache> cache = new Lazy<DefaultMemoryCache>();
        #endregion
    }
}
