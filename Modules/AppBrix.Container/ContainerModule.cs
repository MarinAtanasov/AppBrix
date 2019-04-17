// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Container.Impl;
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
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void Initialize(IInitializeContext context)
        {
            var defaultContainer = this.container;
            defaultContainer.Initialize(context);
            defaultContainer.Register(this);
            this.App.Container = defaultContainer;
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void Uninitialize()
        {
            this.container.Uninitialize();
            this.App.Container = null;
        }
        #endregion

        #region Private fields and constants
        private readonly DefaultContainer container = new DefaultContainer();
        #endregion
    }
}
