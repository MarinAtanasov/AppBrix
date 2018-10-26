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
using System.Net.Http;

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
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Container.Register(this);
            this.loggerProvider.Initialize(context);
            this.App.Container.Register(this.loggerProvider);

            this.App.GetEventHub().Subscribe<IConfigureWebHost>(webHost => webHost.Builder
                .ConfigureServices(services => services.AddSingleton(this.App))
                .ConfigureLogging(logging => logging.AddProvider(this.App.Get<ILoggerProvider>()))
                .Configure(appBuilder =>
                {
                    appBuilder.ApplicationServices.GetRequiredService<IApplicationLifetime>().ApplicationStopped.Register(this.App.Stop);
                    var client = appBuilder.ApplicationServices.GetService<IHttpClientFactory>();
                    if (client != null)
                    {
                        this.App.Container.Register(client);
                        this.App.GetFactoryService().Register(() => this.App.Get<IHttpClientFactory>().CreateClient());
                    }
                    this.App.GetEventHub().Raise(new DefaultConfigureApplication(appBuilder));
                })
                .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly().GetName().Name)
            );
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            this.loggerProvider.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly DefaultLoggerProvider loggerProvider = new DefaultLoggerProvider();
        #endregion
    }
}
