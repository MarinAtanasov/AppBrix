// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Container
{
    /// <summary>
    /// Module which registers the default object container.
    /// </summary>
    public sealed class ContainerModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            var defaultContainer = this.container.Value;
            defaultContainer.Initialize(context);
            defaultContainer.Register(this);
            this.App.SetContainer(defaultContainer);
        }

        protected override void UninitializeModule()
        {
            this.container.Value.Uninitialize();
            this.App.SetContainer(null);
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultContainer> container = new Lazy<DefaultContainer>();
        #endregion
    }
}
