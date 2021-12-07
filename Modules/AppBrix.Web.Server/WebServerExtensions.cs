// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Web.Server.Impl;
using Microsoft.AspNetCore.Builder;

namespace AppBrix;

/// <summary>
/// Extension methods for easier manipulation of AppBrix web servers.
/// </summary>
public static class WebServerExtensions
{
    /// <summary>
    /// Builds the web application using the current <see cref="IApp"/>.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="app">The current application.</param>
    /// <returns>The web application.</returns>
    public static WebApplication Build(this WebApplicationBuilder builder, IApp app)
    {
        app.GetEventHub().Raise(new ConfigureWebAppBuilder(builder));
        var webApp = builder.Build();
        app.GetEventHub().Raise(new ConfigureWebApp(webApp));
        return webApp;
    }
}
