// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Logging.Contracts;
using AppBrix.Logging.Events;
using System;
using System.Runtime.CompilerServices;

namespace AppBrix.Logging.Services;

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
    /// Determines whether a given log severity level is enabled.
    /// </summary>
    /// <param name="level">The log severity level.</param>
    /// <returns>True if the severity level is enabled.</returns>
    bool IsEnabled(LogLevel level);

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

    /// <summary>
    /// Creates and fires a critical level log entry.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="error">The error message to be logged. Optional.</param>
    /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
    /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
    /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
    void Critical(string message, Exception? error = null,
        [CallerFilePath] string? callerFile = null,
        [CallerMemberName] string? callerMember = null,
        [CallerLineNumber] int callerLineNumber = 0
    ) => this.Log(LogLevel.Critical, message, error, callerFile, callerMember, callerLineNumber);

    /// <summary>
    /// Creates and fires a debug level log entry.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="error">The error message to be logged. Optional.</param>
    /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
    /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
    /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
    void Debug(string message, Exception? error = null,
        [CallerFilePath] string? callerFile = null,
        [CallerMemberName] string? callerMember = null,
        [CallerLineNumber] int callerLineNumber = 0
    ) => this.Log(LogLevel.Debug, message, error, callerFile, callerMember, callerLineNumber);

    /// <summary>
    /// Creates and fires an error level log entry.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="error">The error message to be logged. Optional.</param>
    /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
    /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
    /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
    void Error(string message, Exception? error = null,
        [CallerFilePath] string? callerFile = null,
        [CallerMemberName] string? callerMember = null,
        [CallerLineNumber] int callerLineNumber = 0
    ) => this.Log(LogLevel.Error, message, error, callerFile, callerMember, callerLineNumber);

    /// <summary>
    /// Creates and fires an info level log entry.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="error">The error message to be logged. Optional.</param>
    /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
    /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
    /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
    void Info(string message, Exception? error = null,
        [CallerFilePath] string? callerFile = null,
        [CallerMemberName] string? callerMember = null,
        [CallerLineNumber] int callerLineNumber = 0
    ) => this.Log(LogLevel.Info, message, error, callerFile, callerMember, callerLineNumber);

    /// <summary>
    /// Creates and fires a trace level log entry.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="error">The error message to be logged. Optional.</param>
    /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
    /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
    /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
    void Trace(string message, Exception? error = null,
        [CallerFilePath] string? callerFile = null,
        [CallerMemberName] string? callerMember = null,
        [CallerLineNumber] int callerLineNumber = 0
    ) => this.Log(LogLevel.Trace, message, error, callerFile, callerMember, callerLineNumber);

    /// <summary>
    /// Creates and fires a warning level log entry.
    /// </summary>
    /// <param name="message">The message to be logged.</param>
    /// <param name="error">The error message to be logged. Optional.</param>
    /// <param name="callerFile">Full path to the caller's file. Automatically filled.</param>
    /// <param name="callerMember">The caller's member name (function name). Automatically filled.</param>
    /// <param name="callerLineNumber">The caller's executing line number. Automatically filled.</param>
    void Warning(string message, Exception? error = null,
        [CallerFilePath] string? callerFile = null,
        [CallerMemberName] string? callerMember = null,
        [CallerLineNumber] int callerLineNumber = 0
    ) => this.Log(LogLevel.Warning, message, error, callerFile, callerMember, callerLineNumber);
}
