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
    /// TODO: FIX THIS MAZALYAK!!!
    /// </summary>
    public sealed class CacheModule : ModuleBase, IDisposable
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetResolver().Register(this);

            this.serializer = new DefaultCacheSerializer();
            this.App.GetResolver().Register(this.serializer);

            var distributedCache = new LocalCache(new MemoryCache(new MemoryCacheOptions()));
            this.cache = new DefaultCache(this.App, distributedCache);
            this.App.GetResolver().Register(this.cache);
        }

        protected override void UninitializeModule()
        {
            this.Dispose();
        }

        /// <summary>
        /// Code analysis CA1001: Types that own disposable fields should be disposable.
        /// Do not use. Use Uninitialize instead.
        /// </summary>
        public void Dispose()
        {
            this.cache = null;
        }
        #endregion

        #region Private fields and constants
        private DefaultCacheSerializer serializer;
        private DefaultCache cache;
        #endregion
    }
}
