// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Logging.Entries;
using System;
using System.Linq;

namespace AppBrix.Logging.Loggers
{
    /// <summary>
    /// A logger class which can be used to pass in log entries.
    /// </summary>
    public interface ILogger : IApplicationLifecycle
    {
        /// <summary>
        /// Logs the log entry.
        /// </summary>
        /// <param name="entry">The log entry.</param>
        void LogEntry(ILogEntry entry);
    }
}
