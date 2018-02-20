// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Logging;
using AppBrix.Logging.Entries;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix logging.
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Gets the registered log hub.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The log hub.</returns>
        public static ILogHub GetLogHub(this IApp app)
        {
            return (ILogHub)app.Get(typeof(ILogHub));
        }

        /// <summary>
        /// Creates and fires a critical level log entry.
        /// </summary>
        /// <param name="logHub">The current log hub to log the entry.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        public static void Critical(this ILogHub logHub, string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logHub.Log(LogLevel.Critical, message, error, callerFile, callerMember, callerLineNumber);
        }

        /// <summary>
        /// Creates and fires a debug level log entry.
        /// </summary>
        /// <param name="logHub">The current log hub to log the entry.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        public static void Debug(this ILogHub logHub, string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logHub.Log(LogLevel.Debug, message, error, callerFile, callerMember, callerLineNumber);
        }

        /// <summary>
        /// Creates and fires an error level log entry.
        /// </summary>
        /// <param name="logHub">The current log hub to log the entry.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        public static void Error(this ILogHub logHub, string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logHub.Log(LogLevel.Error, message, error, callerFile, callerMember, callerLineNumber);
        }

        /// <summary>
        /// Creates and fires an info level log entry.
        /// </summary>
        /// <param name="logHub">The current log hub to log the entry.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        public static void Info(this ILogHub logHub, string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logHub.Log(LogLevel.Info, message, error, callerFile, callerMember, callerLineNumber);
        }

        /// <summary>
        /// Creates and fires a trace level log entry.
        /// </summary>
        /// <param name="logHub">The current log hub to log the entry.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        public static void Trace(this ILogHub logHub, string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logHub.Log(LogLevel.Trace, message, error, callerFile, callerMember, callerLineNumber);
        }

        /// <summary>
        /// Creates and fires a warning level log entry.
        /// </summary>
        /// <param name="logHub">The current log hub to log the entry.</param>
        /// <param name="message">The message to be logged.</param>
        /// <param name="error">The error message to be logged. Optional.</param>
        /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
        /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
        /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
        public static void Warning(this ILogHub logHub, string message, Exception error = null,
            [CallerFilePath] string callerFile = null,
            [CallerMemberName] string callerMember = null,
            [CallerLineNumber] int callerLineNumber = 0)
        {
            logHub.Log(LogLevel.Warning, message, error, callerFile, callerMember, callerLineNumber);
        }
    }
}
