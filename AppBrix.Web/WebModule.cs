// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using AppBrix.Lifecycle;
using AppBrix.Web.Impl;
using System;
using System.Linq;

namespace AppBrix.Web
{
    /// <summary>
    /// Modules which registers a factory for creating <see cref="IRestCall"/> objects.
    /// </summary>
    public class WebModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetFactory().Register<IRestCall>(() => new DefaultRestCall());
        }

        protected override void UninitializeModule()
        {
        }
        #endregion
    }
}
