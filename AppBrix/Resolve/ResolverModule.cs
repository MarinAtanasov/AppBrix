// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Resolve
{
    /// <summary>
    /// Modules which registers the default object resolver.
    /// </summary>
    public sealed class ResolverModule : ModuleBase
    {
        #region Properties
        /// <summary>
        /// The resolver module should load first and unload last
        /// because all other modules depend on the resolver module.
        /// </summary>
        public override int LoadPriority
        {
            get
            {
                return (int)ModuleLoadPriority.Resolver;
            }
        }
        #endregion

        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            var resolver = this.resolver.Value; ;
            resolver.Initialize(context);
            resolver.Register(this);
            resolver.Register(resolver);
            this.App.Resolver = resolver;
        }

        protected override void UninitializeModule()
        {
            this.resolver.Value.Uninitialize();
            this.App.Resolver = null;
        }
        #endregion

        #region Private fields and constants
        private Lazy<DefaultResolver> resolver = new Lazy<DefaultResolver>();
        #endregion
    }
}
