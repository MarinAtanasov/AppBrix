// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using AppBrix.Lifecycle;
using System;
using System.Linq;

namespace AppBrix.Web.Server
{
    /// <summary>
    /// Modules used for working with Mvc or Web Api controllers.
    /// For dependency injection of the current app inside the controllers' constructors,
    /// use the <see cref="IServiceCollection.AddApp"/> extension method inside the
    /// <see cref="ConfigureServices"/> method.
    /// </summary>
    public class WebServerModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
        }

        protected override void UninitializeModule()
        {
        }
        #endregion
    }
}
