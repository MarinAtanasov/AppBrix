// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using AppBrix.Logging.Entries;
using System;
using System.Linq;

namespace AppBrix.Logging.Loggers
{
    /// <summary>
    /// A logger which logs the entries on the same thread on which they were created.
    /// </summary>
    internal sealed class SyncLogger : ILogger
    {
        #region Construciton
        /// <summary>
        /// Creates a synchronous logger using the provider log writer.
        /// </summary>
        /// <param name="writer">The log writer that will log the log entries.</param>
        public SyncLogger(ILogWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            this.writer = writer;
        }
        #endregion

        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
            this.writer.Initialize(context);
            this.app.GetEventHub().Subscribe<ILogEntry>(this.LogEntry);
        }

        public void Uninitialize()
        {
            this.app.GetEventHub().Unsubscribe<ILogEntry>(this.LogEntry);
            this.writer.Uninitialize();
            this.app = null;
        }
        
        public void LogEntry(ILogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));

            this.writer.WriteEntry(entry);
        }
        #endregion

        #region Private fields and constants
        private IApp app;
        private readonly ILogWriter writer;
        #endregion
    }
}
