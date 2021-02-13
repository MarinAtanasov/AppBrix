// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Logging.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace AppBrix.Logging.Impl
{
    internal sealed class Logger : ILogger
    {
        #region Construction
        public Logger(IApp app, string categoryName, bool enabled)
        {
            this.app = app;
            var dotIndex = categoryName.LastIndexOf('.');
            this.categoryName = categoryName.Substring(dotIndex + 1);
            this.config = this.app.ConfigService.GetLoggingConfig();
            this.Enabled = enabled;
        }
        #endregion

        #region Properties
        public bool Enabled { get; set; }
        #endregion

        #region Public and overriden methods
        public bool IsEnabled(LogLevel logLevel) =>
            this.Enabled ? this.config.LogLevel <= this.ToAppBrixLogLevel(logLevel) : false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception?, string> formatter)
        {
            if (!this.Enabled)
                return;

            this.app.GetLogHub().Log(this.ToAppBrixLogLevel(logLevel), formatter(state, null), exception, this.categoryName, eventId.Name ?? this.categoryName, eventId.Id);
        }

        public IDisposable? BeginScope<TState>(TState state) => null;
        #endregion

        #region Private methods
        private Entries.LogLevel ToAppBrixLogLevel(LogLevel logLevel) => logLevel switch
        {
            LogLevel.Trace => Entries.LogLevel.Trace,
            LogLevel.Debug => Entries.LogLevel.Debug,
            LogLevel.Information => Entries.LogLevel.Info,
            LogLevel.Warning => Entries.LogLevel.Warning,
            LogLevel.Error => Entries.LogLevel.Error,
            LogLevel.Critical => Entries.LogLevel.Critical,
            LogLevel.None => Entries.LogLevel.None,
            _ => Entries.LogLevel.Trace
        };
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        private readonly string categoryName;
        private readonly LoggingConfig config;
        #endregion
    }
}
