// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Logging.Contracts;
using System;

namespace AppBrix.Logging.Events;

/// <summary>
/// Base interface for all logging entry events.
/// </summary>
public interface ILogEntry : IEvent
{
    /// <summary>
    /// Gets the severity level of the log entry.
    /// </summary>
    LogLevel Level { get; }

    /// <summary>
    /// Gets the message being logged.
    /// </summary>
    string Message { get; }

    /// <summary>
    /// The exception being logged. Could be null.
    /// </summary>
    Exception? Exception { get; }

    /// <summary>
    /// Gets the time when the log entry was created.
    /// </summary>
    DateTime Created { get; }

    /// <summary>
    /// Gets the full path to the file of the caller.
    /// </summary>
    string CallerFile { get; }

    /// <summary>
    /// Gets the name of the file of the caller.
    /// </summary>
    ReadOnlySpan<char> CallerFileName { get; }

    /// <summary>
    /// Gets the member name of the caller.
    /// </summary>
    string CallerMember { get; }

    /// <summary>
    /// Gets the caller's line number where the the log entry was created.
    /// </summary>
    int CallerLineNumber { get; }

    /// <summary>
    /// Gets the id of the thread which was used to create this log entry.
    /// </summary>
    int ThreadId { get; }
}
