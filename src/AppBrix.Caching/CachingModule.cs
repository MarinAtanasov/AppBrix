// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
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
            this.App.GetContainer().Register(this);
            this.memoryCache = new MemoryCache(new MemoryCacheOptions());
            this.App.GetContainer().Register(new MemoryDistributedCache(memoryCache));
            this.cache.Value.Initialize(context);
            this.App.GetContainer().Register(this.cache.Value);
        }

        protected override void UninitializeModule()
        {
            this.cache.Value.Uninitialize();
            this.memoryCache?.Dispose();
            this.memoryCache = null;
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultCache> cache = new Lazy<DefaultCache>();
        private IMemoryCache memoryCache;
        #endregion
    }
}
