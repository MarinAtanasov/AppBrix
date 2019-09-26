﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Factory;
using AppBrix.Lifecycle;
using AppBrix.Logging;
using AppBrix.Modules;
using AppBrix.Web.Server.Impl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        #region Properties
        /// <summary>
        /// Gets the types of the modules which are direct dependencies for the current module.
        /// This is used to determine the order in which the modules are loaded.
        /// </summary>
        public override IEnumerable<Type> Dependencies => new[] { typeof(FactoryModule), typeof(LoggingModule) };
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void Initialize(IInitializeContext context)
        {
            this.App.Container.Register(this);

            this.App.GetEventHub().Subscribe<IConfigureWebHost>(webHost => webHost.Builder
                .ConfigureServices(services => services.AddSingleton(this.App))
                .ConfigureLogging(logging => logging.ClearProviders().AddProvider(this.App.Get<ILoggerProvider>()))
                .Configure(appBuilder =>
                {
                    appBuilder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>().ApplicationStopped.Register(this.App.Stop);
                    var client = appBuilder.ApplicationServices.GetService<IHttpClientFactory>();
                    if (client != null)
                    {
                        this.App.Container.Register(client);
                        this.App.GetFactoryService().Register(this.CreateClient);
                    }
                    this.App.GetEventHub().Raise(new DefaultConfigureApplication(appBuilder));
                })
                .UseSetting(WebHostDefaults.ApplicationKey, Assembly.GetEntryAssembly().GetName().Name)
            );
        }
        #endregion

        #region Private methods
        private HttpClient CreateClient() => this.App.Get<IHttpClientFactory>().CreateClient();
        #endregion
    }
}
