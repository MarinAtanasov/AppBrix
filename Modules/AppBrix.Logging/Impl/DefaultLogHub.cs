// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
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
        public void Log(LogLevel level, string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var config = (LoggingConfig)this.app.GetConfig(DefaultLogHub.ConfigType);
            if (config.LogLevel <= level)
            {
                this.app.GetEventHub()
                    .Raise<ILogEntry>(new DefaultLogEntry(this.app, level, this.app.GetTime(), message, error,
                        callerFile: callerFile, callerMember: callerMember, callerLineNumber: callerLineNumber));
            }
        }
        #endregion
        
        #region Private fields and constants
        private static readonly Type ConfigType = typeof(LoggingConfig);
        private IApp app;
        #endregion
    }
}
