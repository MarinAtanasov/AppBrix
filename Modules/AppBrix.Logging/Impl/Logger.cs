// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Logging.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace AppBrix.Logging.Impl;

internal sealed class Logger : ILogger
{
    #region Construction
    public Logger(IApp app, string categoryName, bool enabled)
    {
        this.app = app;
        var dotIndex = categoryName.LastIndexOf('.');
        this.categoryName = categoryName[(dotIndex + 1)..];
        this.config = this.app.ConfigService.GetLoggingConfig();
        this.Enabled = enabled;
    }
    #endregion

    #region Properties
    public bool Enabled { get; set; }
    #endregion

    #region Public and overriden methods
    public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) => this.Enabled && this.config.LogLevel <= this.ToAppBrixLogLevel(logLevel);

    public void Log<TState>(Microsoft.Extensions.Logging.LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!this.Enabled)
            return;

        this.app.GetLogHub().Log(this.ToAppBrixLogLevel(logLevel), formatter(state, null), exception, this.categoryName, eventId.Name ?? this.categoryName, eventId.Id);
    }

    public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
    #endregion

    #region Private methods
    private LogLevel ToAppBrixLogLevel(Microsoft.Extensions.Logging.LogLevel logLevel) => logLevel switch
    {
        Microsoft.Extensions.Logging.LogLevel.Trace => LogLevel.Trace,
        Microsoft.Extensions.Logging.LogLevel.Debug => LogLevel.Debug,
        Microsoft.Extensions.Logging.LogLevel.Information => LogLevel.Info,
        Microsoft.Extensions.Logging.LogLevel.Warning => LogLevel.Warning,
        Microsoft.Extensions.Logging.LogLevel.Error => LogLevel.Error,
        Microsoft.Extensions.Logging.LogLevel.Critical => LogLevel.Critical,
        Microsoft.Extensions.Logging.LogLevel.None => LogLevel.None,
        _ => LogLevel.Trace
    };
    #endregion

    #region Private fields and constants
    private readonly IApp app;
    private readonly string categoryName;
    private readonly LoggingConfig config;
    #endregion

    #region Private classes
    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }
    #endregion
}
