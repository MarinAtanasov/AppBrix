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
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void Initialize(IInitializeContext context)
        {
            this.App.Container.Register(this);
            this.cache.Initialize(context);
            this.App.Container.Register(this.cache);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void Uninitialize()
        {
            this.cache.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly DefaultMemoryCache cache = new DefaultMemoryCache();
        #endregion
    }
}
