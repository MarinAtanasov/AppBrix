// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Caching.Json
{
    /// <summary>
    /// Module used to serialize object to json while caching.
    /// </summary>
    public sealed class JsonCachingModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            this.App.GetContainer().Register(this.serializer.Value);
        }

        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<JsonCacheSerializer> serializer = new Lazy<JsonCacheSerializer>();
        #endregion
    }
}
