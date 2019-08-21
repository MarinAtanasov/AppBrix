// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Logging.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace AppBrix.Logging.Impl
{
    internal sealed class DefaultLogger : ILogger
    {
        #region Construction
        public DefaultLogger(IApp app, string categoryName, bool enabled)
        {
            this.app = app;
            var dotIndex = categoryName.LastIndexOf('.');
            this.categoryName = categoryName.Substring(dotIndex + 1);
            this.config = this.app.GetConfig<LoggingConfig>();
            this.Enabled = enabled;
        }
        #endregion

        #region Properties
        public bool Enabled { get; set; }
        #endregion

        #region Public and overriden methods
        public bool IsEnabled(LogLevel logLevel) =>
            this.Enabled ? this.config.LogLevel <= this.ToAppBrixLogLevel(logLevel) : false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!this.Enabled)
                return;

            this.app.GetLogHub().Log(this.ToAppBrixLogLevel(logLevel), formatter(state, null), exception, this.categoryName, eventId.Name ?? this.categoryName, eventId.Id);
        }

        public IDisposable BeginScope<TState>(TState state) => null;
        #endregion

        #region Private methods
        private Entries.LogLevel ToAppBrixLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return Entries.LogLevel.Trace;
                case LogLevel.Debug:
                    return Entries.LogLevel.Debug;
                case LogLevel.Information:
                    return Entries.LogLevel.Info;
                case LogLevel.Warning:
                    return Entries.LogLevel.Warning;
                case LogLevel.Error:
                    return Entries.LogLevel.Error;
                case LogLevel.Critical:
                    return Entries.LogLevel.Critical;
                default:
                    return Entries.LogLevel.Trace;
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        private readonly string categoryName;
        private readonly LoggingConfig config;
        #endregion
    }
}
