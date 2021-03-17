// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Web.Server.Events;
using AppBrix.Web.Server.Impl;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix web servers.
    /// </summary>
    public static class WebServerExtensions
    {
        /// <summary>
        /// Raises <see cref="IConfigureWebHost"/> event which can be used to attach to
        /// the web host which will use the current <see cref="IApp"/>.
        /// </summary>
        /// <param name="builder">The web host builder.</param>
        /// <param name="app">The current application.</param>
        /// <returns>The web host builder.</returns>
        public static IWebHostBuilder UseApp(this IWebHostBuilder builder, IApp app)
        {
            app.GetEventHub().Raise(new ConfigureWebHost(builder));
            return builder;
        }

        /// <summary>
        /// Raises <see cref="IConfigureHost"/> event which can be used to attach to
        /// the host which will use the current <see cref="IApp"/>.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <param name="app">The current application.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseApp(this IHostBuilder builder, IApp app)
        {
            app.GetEventHub().Raise(new ConfigureHost(builder));
            return builder;
        }

        /// <summary>
        /// Raises <see cref="IConfigureWebHost"/> event which can be used to attach to
        /// the web host which will use the current <see cref="IApp"/>.
        /// </summary>
        /// <param name="builder">The host builder.</param>
        /// <param name="app">The current application.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseWebApp(this IHostBuilder builder, IApp app) =>
            builder.ConfigureWebHostDefaults(x => app.GetEventHub().Raise(new ConfigureWebHost(x)));
    }
}
