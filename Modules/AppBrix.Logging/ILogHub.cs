// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Logging.Entries;
using System;
using System.Runtime.CompilerServices;

namespace AppBrix.Logging
{
    /// <summary>
    /// Defines a hub used for logging messages, errors and stack traces.
    /// </summary>
    public interface ILogHub
    {
        /// <summary>
        /// Subscribes a logging handler to be called when raising a logging event.
        /// </summary>
        /// <param name="logger">The logging handler.</param>
        void Subscribe(Action<ILogEntry> logger);

        /// <summary>
        /// Unsubscribes a logging handler so that it will no longer be called when raising a logging event.
        /// </summary>
        /// <param name="logger">The logging handler.</param>
        void Unsubscribe(Action<ILogEntry> logger);

        /// <summary>
        /// Creates a log entry and fires an <see cref="ILogEntry"/> event.
        /// </summary>
        /// <param name="level">The log severity level.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        void Log(LogLevel level, string message, Exception? error = null,
            [CallerFilePath] string? callerFile = null,
            [CallerMemberName] string? callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0);
    }
}
