﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Factory.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Factory
{
    /// <summary>
    /// A module used for registering a default object factory.
    /// The object factory should be used only for simple objects which cannot be made immutable.
    /// </summary>
    public sealed class FactoryModule : ModuleBase
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
            this.factoryService.Initialize(context);
            this.App.Container.Register(this.factoryService);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void Uninitialize()
        {
            this.factoryService.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly DefaultFactoryService factoryService = new DefaultFactoryService();
        #endregion
    }
}
