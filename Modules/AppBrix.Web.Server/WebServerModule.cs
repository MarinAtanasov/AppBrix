// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Modules;
using AppBrix.Lifecycle;
using AppBrix.Web.Server.Impl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Reflection;

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

            this.App.GetEventHub().Subscribe<IConfigureWebHost>(webHost => webHost.Builder
                .ConfigureServices(services => services.AddSingleton(this.App))
                .ConfigureLogging(logging => logging.AddProvider(this.App.Get<ILoggerProvider>()))
                .Configure(appBuilder =>
                {
                    appBuilder.ApplicationServices.GetRequiredService<IApplicationLifetime>().ApplicationStopped.Register(this.App.Stop);
                    this.App.GetEventHub().Raise(new DefaultConfigureApplication(appBuilder));
                })
                .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly().GetName().Name)
            );
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
