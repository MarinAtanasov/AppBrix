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
    /// Static class used for creating default implementations of <see cref="ILogger"/>.
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// Creates a new instance of a logger using the provided log writer.
        /// This logger should be initialized and later uninitialized during
        /// application/module uninitialization.
        /// </summary>
        /// <param name="writer">The log writer.</param>
        /// <param name="async">If true, the logger will be asynchronous.</param>
        /// <returns></returns>
        public static ILogger Create(ILogWriter writer, bool async)
        {
            return async ? (ILogger)new AsyncLogger(writer) : new SyncLogger(writer);
        }
    }
}
