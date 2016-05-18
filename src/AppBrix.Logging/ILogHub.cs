// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AppBrix.Logging
{
    /// <summary>
    /// Defines a hub used for logging messages, errors and stack traces.
    /// </summary>
    public interface ILogHub
    {
        /// <summary>
        /// Creates a critical level log entry.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        void Critical(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0);

        /// <summary>
        /// Creates an error level log entry.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        void Error(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0);

        /// <summary>
        /// Creates a debug level log entry.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        void Debug(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0);

        /// <summary>
        /// Creates an info level log entry.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        void Info(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0);

        /// <summary>
        /// Creates a trace level log entry.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        void Trace(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0);

        /// <summary>
        /// Creates a warning level log entry.
        /// </summary>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        void Warning(string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0);
    }
}
