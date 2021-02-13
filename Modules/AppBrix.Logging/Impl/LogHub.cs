// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Entries;
using System;
using System.Runtime.CompilerServices;

namespace AppBrix.Logging.Impl
{
    internal sealed class LogHub : ILogHub, IApplicationLifecycle
    {
        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
            this.config = this.app.ConfigService.GetLoggingConfig();
        }

        public void Uninitialize()
        {
            this.config = null;
            this.app = null;
        }
        #endregion

        #region ILogHub implementation
        public void Subscribe(Action<ILogEntry> logger)
        {
            if (this.config.Async)
            {
                this.app.GetAsyncEventHub().Subscribe(logger);
            }
            else
            {
                this.app.GetEventHub().Subscribe(logger);
            }
        }

        public void Unsubscribe(Action<ILogEntry> logger)
        {
            this.app.GetEventHub().Unsubscribe(logger);
            this.app.GetAsyncEventHub().Unsubscribe(logger);
        }

        public void Log(LogLevel level, string message, Exception? error = null,
            [CallerFilePath] string? callerFile = null,
            [CallerMemberName] string? callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (this.config.LogLevel <= level)
            {
                this.app.GetEventHub()
                    .Raise(new LogEntry(this.app, level, this.app.GetTime(), message, error,
                        callerFile: callerFile, callerMember: callerMember, callerLineNumber: callerLineNumber));
            }
        }
        #endregion

        #region Private fields and constants
        #nullable disable
        private IApp app;
        private LoggingConfig config;
        #nullable restore
        #endregion
    }
}
