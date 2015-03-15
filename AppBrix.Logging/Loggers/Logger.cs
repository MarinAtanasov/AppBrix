// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Events;
using AppBrix.Lifecycle;
using AppBrix.Logging.Entries;
using System;
using System.Linq;

namespace AppBrix.Logging.Loggers
{
    /// <summary>
    /// Common base class for the loggers.
    /// Can be used to create a default sync/async logger.
    /// </summary>
    public abstract class Logger : IApplicationLifecycle
    {
        #region Construction
        /// <summary>
        /// Creates a LoggerBase with the provided log writer.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        protected Logger(ILogWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }

        /// <summary>
        /// Creates a new instance of a logger using the provided log writer.
        /// This logger should be initialized and later uninitialized during
        /// application/module uninitialization.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="async">If true, the logger will be asynchronous.</param>
        /// <returns></returns>
        public static Logger Create(ILogWriter writer, bool async)
        {
            return async ? (Logger)new AsyncLogger(writer) : new SyncLogger(writer);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the current app.
        /// </summary>
        protected IApp App { get; private set; }
        #endregion

        #region Public and abstract methods
        /// <summary>
        /// Initializes the log writer.
        /// Subscribes the logger to the log entry events.
        /// </summary>
        /// <param name="context">The initialization context.</param>
        public virtual void Initialize(IInitializeContext context)
        {
            this.App = context.App;
            this.writer.Initialize(context);
            this.App.GetEventHub().Subscribe<ILogEntry>(this.LogEntry);
        }

        /// <summary>
        /// Unsubscribes the logger from the log entry events.
        /// Uninitializes the log writer.
        /// </summary>
        public virtual void Uninitialize()
        {
            this.App.GetEventHub().Unsubscribe<ILogEntry>(this.LogEntry);
            this.writer.Uninitialize();
            this.App = null;
        }

        /// <summary>
        /// Logs the entry using the log writer.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public abstract void LogEntry(ILogEntry entry);
        #endregion

        #region Private fields and constants
        protected readonly ILogWriter writer;
        #endregion
    }
}
