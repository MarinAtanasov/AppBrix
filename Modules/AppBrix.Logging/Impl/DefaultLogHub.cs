// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Logging.Configuration;
using AppBrix.Logging.Entries;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AppBrix.Logging.Impl
{
    internal sealed class DefaultLogHub : ILogHub, IApplicationLifecycle
    {
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

        #region ILogHub implementation
        public void Subscribe(Action<ILogEntry> logger)
        {
            if (this.app.GetConfig<LoggingConfig>().Async)
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

        public void Log(LogLevel level, string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var config = (LoggingConfig)this.app.GetConfig(typeof(LoggingConfig));
            if (config.LogLevel <= level)
            {
                this.app.GetEventHub()
                    .Raise(new DefaultLogEntry(this.app, level, this.app.GetTime(), message, error,
                        callerFile: callerFile, callerMember: callerMember, callerLineNumber: callerLineNumber));
            }
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        #endregion
    }
}
