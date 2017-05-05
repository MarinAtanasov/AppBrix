// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace AppBrix.Web.Server
{
    internal class DefaultLogger : ILogger
    {
        #region Construction
        public DefaultLogger(IApp app, string categoryName)
        {
            this.app = app;
            var dotIndex = categoryName.LastIndexOf('.');
            this.categoryName = categoryName.Substring(dotIndex + 1);
        }
        #endregion

        #region Public and overriden methods
        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            this.app.GetLog().Log(this.ToAppBrixLogLevel(logLevel), formatter(state, null), exception, this.categoryName, eventId.Name ?? this.categoryName, eventId.Id);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
        #endregion

        #region Private methods
        private Logging.Entries.LogLevel ToAppBrixLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                    return Logging.Entries.LogLevel.Trace;
                case LogLevel.Debug:
                    return Logging.Entries.LogLevel.Debug;
                case LogLevel.Information:
                    return Logging.Entries.LogLevel.Info;
                case LogLevel.Warning:
                    return Logging.Entries.LogLevel.Warning;
                case LogLevel.Error:
                    return Logging.Entries.LogLevel.Error;
                case LogLevel.Critical:
                    return Logging.Entries.LogLevel.Critical;
                default:
                    return Logging.Entries.LogLevel.Trace;
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        private readonly string categoryName;
        #endregion
    }
}
