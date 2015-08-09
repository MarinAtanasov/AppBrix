// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;
using System.Runtime.Caching;

namespace AppBrix.Caching
{
    public sealed class CacheModule : ModuleBase, IDisposable
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetResolver().Register(this);
            this.cache = new MemoryCache(this.App.Id.ToString());
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
            this.cache.Dispose();
            this.cache = null;
        }
        #endregion

        #region Private fields and constants
        private MemoryCache cache;
        #endregion
    }
}
