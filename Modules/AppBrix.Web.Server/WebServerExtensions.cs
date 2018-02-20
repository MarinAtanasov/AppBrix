// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Web.Server.Impl;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Adds extension methods for using MVC and Web Api controllers.
    /// </summary>
    public static class WebServerExtensions
    {
        #region Public and overriden methods
        /// <summary>
        /// Raises <see cref="IConfigureWebHost"/> event which can be used to attach to
        /// the web host which will use the current <see cref="IApp"/>.
        /// </summary>
        /// <param name="builder">The web host builder.</param>
        /// <param name="app">The current application.</param>
        /// <returns>The web host builder.</returns>
        public static IWebHostBuilder UseApp(this IWebHostBuilder builder, IApp app)
        {
            app.GetEventHub().Raise(new DefaultConfigureWebHost(builder));
            return builder;
        }
        #endregion
    }
}
