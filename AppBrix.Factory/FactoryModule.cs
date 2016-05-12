﻿// Copyright (c) MarinAtanasov. All rights reserved.
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
    /// The object factory should be used only for simple objects which cannot be made immutable.
    /// </summary>
    public sealed class FactoryModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            var defaultFactory = this.factory.Value;
            defaultFactory.Initialize(context);
            this.App.GetResolver().Register(this);
            this.App.GetResolver().Register(defaultFactory);
        }

        protected override void UninitializeModule()
        {
            this.factory.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultFactory> factory = new Lazy<DefaultFactory>();
        #endregion
    }
}
