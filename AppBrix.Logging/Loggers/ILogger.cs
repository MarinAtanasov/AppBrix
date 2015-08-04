// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Logging.Entries;
using System;
using System.Linq;

namespace AppBrix.Logging.Loggers
{
    /// <summary>
    /// Logs the log entry.
    /// </summary>
    /// <param name="entry">The log entry.</param>
    public interface ILogger
    {
        void LogEntry(ILogEntry entry);
    }
}
