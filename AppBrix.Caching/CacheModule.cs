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
    public sealed class CacheModule : ModuleBase
    {
        #region Properties
        public override int LoadPriority
        {
            get
            {
                return (int)ModuleLoadPriority.Config;
            }
        }
        #endregion

        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Resolver.Register(this);
            this.cache = new MemoryCache(this.App.Id.ToString());
            this.App.Resolver.Register(this.cache);
        }

        protected override void UninitializeModule()
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
