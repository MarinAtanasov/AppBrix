// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

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
    public bool IsEnabled(LogLevel logLevel) => this.Enabled && this.config.LogLevel <= this.ToAppBrixLogLevel(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (this.Enabled)
            this.app.GetLogHub().Log(this.ToAppBrixLogLevel(logLevel), formatter(state, null), exception, this.categoryName, eventId.Name ?? this.categoryName, eventId.Id);
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;
    #endregion

    #region Private methods
    private Contracts.LogLevel ToAppBrixLogLevel(LogLevel logLevel) => logLevel switch
    {
        //LogLevel.Trace => Contracts.LogLevel.Trace,
        LogLevel.Debug => Contracts.LogLevel.Debug,
        LogLevel.Information => Contracts.LogLevel.Info,
        LogLevel.Warning => Contracts.LogLevel.Warning,
        LogLevel.Error => Contracts.LogLevel.Error,
        LogLevel.Critical => Contracts.LogLevel.Critical,
        LogLevel.None => Contracts.LogLevel.None,
        _ => Contracts.LogLevel.Trace
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
