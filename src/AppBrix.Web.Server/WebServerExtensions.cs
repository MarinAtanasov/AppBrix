// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Adds extension methods for using MVC and Web Api controllers.
    /// </summary>
    public static class WebServerExtensions
    {
        #region Public methods
        /// <summary>
        /// Adds the current application to be resolved in MVC and Web Api controllers.
        /// This method must be called from <see cref="M:ConfigureServices"/> method.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="app"></param>
        public static void AddApp(this IServiceCollection services, IApp app)
        {
            services.AddSingleton(app);
        }

        /// <summary>
        /// Adds the currently registered <see cref="ILoggerProvider"/> to the logging system.
        /// </summary>
        /// <param name="loggerFactory">The logger factory where to add the provider.</param>
        /// <param name="app">The app where the provider has been registerd.</param>
        public static void AddProvider(this ILoggerFactory loggerFactory, IApp app)
        {
            loggerFactory.AddProvider(app.Get<ILoggerProvider>());
        }
        #endregion
    }
}
