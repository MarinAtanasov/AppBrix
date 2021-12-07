// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Services;

namespace AppBrix;

/// <summary>
/// Extension methods for easier manipulation of AppBrix logging.
/// </summary>
public static class LogExtensions
{
    /// <summary>
    /// Gets the registered log hub.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <returns>The log hub.</returns>
    public static ILogHub GetLogHub(this IApp app) => (ILogHub)app.Get(typeof(ILogHub));

    /// <summary>
    /// Gets the <see cref="LoggingConfig"/> from <see cref="IConfigService"/>.
    /// </summary>
    /// <param name="service">The configuration service.</param>
    /// <returns>The <see cref="LoggingConfig"/>.</returns>
    public static LoggingConfig GetLoggingConfig(this IConfigService service) => (LoggingConfig)service.Get(typeof(LoggingConfig));

    /// <summary>
    /// Gets the registered logger provider.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <returns>The logger provider.</returns>
    internal static Microsoft.Extensions.Logging.ILoggerProvider GetLoggerProvider(this IApp app) =>
        (Microsoft.Extensions.Logging.ILoggerProvider)app.Get(typeof(Microsoft.Extensions.Logging.ILoggerProvider));
}
