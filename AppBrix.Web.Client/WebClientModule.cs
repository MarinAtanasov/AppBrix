// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using AppBrix.Lifecycle;
using AppBrix.Web.Client.Impl;
using System;
using System.Linq;

namespace AppBrix.Web.Client
{
    /// <summary>
    /// Modules which registers a factory for creating <see cref="IHttpCall"/> objects.
    /// </summary>
    public class WebClientModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetFactory().Register<IHttpCall>(() => new DefaultHttpCall(this.App));
        }

        protected override void UninitializeModule()
        {
        }
        #endregion
    }
}
