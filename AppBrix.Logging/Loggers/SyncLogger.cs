// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Logging.Entries;
using System;
using System.Linq;

namespace AppBrix.Logging.Loggers
{
    /// <summary>
    /// A logger which logs the entries on the same thread on which they were created.
    /// </summary>
    internal sealed class SyncLogger : Logger
    {
        #region Construciton
        /// <summary>
        /// Creates a synchronous logger using the provider log writer.
        /// </summary>
        /// <param name="writer">The log writer that will log the log entries.</param>
        public SyncLogger(ILogWriter writer) : base(writer)
        {
        }
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Logs the entry synchronously.
        /// </summary>
        /// <param name="entry">The log entry which will be logged.</param>
        /// <exception cref="ArgumentNullException">entry</exception>
        public override void LogEntry(ILogEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");
            this.writer.WriteEntry(entry);
        }
        #endregion
    }
}
