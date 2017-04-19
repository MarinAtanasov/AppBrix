// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using AppBrix.Lifecycle;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace AppBrix.Web.Server
{
    /// <summary>
    /// Modules used for working with Mvc or Web Api controllers.
    /// For dependency injection of the current app inside the controllers' constructors,
    /// use the <see cref="T:IServiceCollection.AddApp"/> extension method inside the
    /// <see cref="M:ConfigureServices"/> method.
    /// </summary>
    public sealed class WebServerModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            var defaultLoggerProvider = this.loggerProvider.Value;
            defaultLoggerProvider.Initialize(context);
            this.App.GetContainer().Register(defaultLoggerProvider);
        }

        protected override void UninitializeModule()
        {
            this.loggerProvider.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultLoggerProvider> loggerProvider = new Lazy<DefaultLoggerProvider>();
        #endregion
    }
}
