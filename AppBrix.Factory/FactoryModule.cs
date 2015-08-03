// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Factory
{
    /// <summary>
    /// A module used for registering a default object factory.
    /// </summary>
    public sealed class FactoryModule : ModuleBase
    {
        #region Properties
        public override int LoadPriority
        {
            get
            {
                return (int)ModuleLoadPriority.Factory;
            }
        }
        #endregion

        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            var factory = this.factory.Value;
            factory.Initialize(context);
            this.App.GetResolver().Register(this);
            this.App.GetResolver().Register(factory);
        }

        protected override void UninitializeModule()
        {
            this.factory.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private Lazy<DefaultFactory> factory = new Lazy<DefaultFactory>();
        #endregion
    }
}
