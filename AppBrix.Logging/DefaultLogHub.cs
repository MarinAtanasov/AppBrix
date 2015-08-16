// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Entries;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AppBrix.Logging
{
    internal sealed class DefaultLogHub : ILogHub, IApplicationLifecycle
    {
        #region ILogHub implementation
        public void Critical(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (this.ShouldLog(LogLevel.Critical))
            {
                this.app.GetEventHub()
                    .Raise<ILogEntry>(new DefaultLogEntry(this.app, LogLevel.Critical, this.GetTime(), message, error,
                        callerFile: callerFile, callerMember: callerMember, callerLineNumber: callerLineNumber));
            }
        }

        public void Error(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (this.ShouldLog(LogLevel.Error))
            {
                this.app.GetEventHub()
                    .Raise<ILogEntry>(new DefaultLogEntry(this.app, LogLevel.Error, this.GetTime(), message, error,
                        callerFile: callerFile, callerMember: callerMember, callerLineNumber: callerLineNumber));
            }
        }

        public void Debug(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (this.ShouldLog(LogLevel.Debug))
            {
                this.app.GetEventHub()
                    .Raise<ILogEntry>(new DefaultLogEntry(this.app, LogLevel.Debug, this.GetTime(), message, error,
                        callerFile: callerFile, callerMember: callerMember, callerLineNumber: callerLineNumber));
            }
        }

        public void Info(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (this.ShouldLog(LogLevel.Info))
            {
                this.app.GetEventHub()
                    .Raise<ILogEntry>(new DefaultLogEntry(this.app, LogLevel.Info, this.GetTime(), message, error,
                        callerFile: callerFile, callerMember: callerMember, callerLineNumber: callerLineNumber));
            }
        }

        public void Trace(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (this.ShouldLog(LogLevel.Trace))
            {
                this.app.GetEventHub()
                    .Raise<ILogEntry>(new DefaultLogEntry(this.app, LogLevel.Trace, this.GetTime(), message, error, new StackTrace(1, true),
                        callerFile: callerFile, callerMember: callerMember, callerLineNumber: callerLineNumber));
            }
        }

        public void Warning(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (this.ShouldLog(LogLevel.Warning))
            {
                this.app.GetEventHub()
                    .Raise<ILogEntry>(new DefaultLogEntry(this.app, LogLevel.Warning, this.GetTime(), message, error,
                        callerFile: callerFile, callerMember: callerMember, callerLineNumber: callerLineNumber));
            }
        }
        #endregion

        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
        }

        public void Uninitialize()
        {
            this.app = null;
        } 
        #endregion

        #region Private methods
        /// <summary>
        /// Determines whether a log entry should be logged with the provided log level.
        /// </summary>
        /// <param name="level">The level of the log entry to be logged.</param>
        /// <returns></returns>
        private bool ShouldLog(LogLevel level)
        {
            return this.app.GetConfig<LoggingConfig>().LogLevel <= level;
        }

        private DateTime GetTime()
        {
            return this.app.GetTime();
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion
    }
}
