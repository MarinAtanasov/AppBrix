// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Linq;

namespace AppBrix.Logging
{
    /// <summary>
    /// A writer which serializes the log to the console, file, etc.
    /// Used by the LoggerBase classes: Logger, AsyncLogger.
    /// </summary>
    public interface ILogWriter : IApplicationLifecycle
    {
        /// <summary>
        /// Serializes and writes the log entry to the console, file, etc.
        /// </summary>
        /// <param name="entry">The log entry.</param>
        void WriteEntry(ILogEntry entry);
    }
}
