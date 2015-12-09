// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using Microsoft.Framework.Caching.Distributed;
using Microsoft.Framework.Caching.Memory;
using System;
using System.Linq;

namespace AppBrix.Caching
{
    /// <summary>
    /// Module used for caching objects.
    /// </summary>
    public sealed class CacheModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetResolver().Register(this);
            this.App.GetResolver().Register(this.serializer.Value);

            var distributedCache = new LocalCache(new MemoryCache(new MemoryCacheOptions()));
            var cache = new DefaultCache(this.App, distributedCache);
            this.App.GetResolver().Register(cache);
        }

        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private fields and constants
        private Lazy<DefaultCacheSerializer> serializer = new Lazy<DefaultCacheSerializer>();
        #endregion
    }
}
