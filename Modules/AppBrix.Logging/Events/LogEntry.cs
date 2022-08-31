// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Logging.Contracts;
using System;
using System.Text;

namespace AppBrix.Logging.Events;

internal sealed class LogEntry : ILogEntry
{
    #region Construciton
    public LogEntry(IApp app, LogLevel level, DateTime created, string message, Exception? exception = null,
        string? callerFile = null, string? callerMember = null, int callerLineNumber = 0)
    {
        this.app = app;
        this.Level = level;
        this.Exception = exception;
        this.Message = message;
        this.CallerFile = callerFile ?? string.Empty;
        this.CallerMember = callerMember ?? string.Empty;
        this.CallerLineNumber = callerLineNumber;
        this.ThreadId = Environment.CurrentManagedThreadId;
        this.Created = created;

        for (var i = this.CallerFile.Length - 2; i >= 0; i--)
        {
            if (this.CallerFile[i] is '/' or '\\')
            {
                this.callerFileNameIndex = i + 1;
                break;
            }
        }
    }
    #endregion

    #region Properties
    public LogLevel Level { get; }

    public string CallerFile { get; }

    public ReadOnlySpan<char> CallerFileName => this.CallerFile.AsSpan(this.callerFileNameIndex);

    public string CallerMember { get; }

    public int CallerLineNumber { get; }

    public Exception? Exception { get; }

    public string Message { get; }

    public DateTime Created { get; }

    public int ThreadId { get; }
    #endregion

    #region Public and overriden methods
    public override int GetHashCode() => this.Message.GetHashCode();

    public override string ToString()
    {
        var result = new StringBuilder();
        result.Append(this.app.GetTimeService().ToString(this.Created));
        result.Append(LogEntry.Separator);
        result.Append($"{this.Level,-5}");
        result.Append(LogEntry.Separator);
        result.Append(this.ThreadId);
        result.Append(LogEntry.Separator);
        result.Append(this.CallerFileName);
        result.Append(LogEntry.LineNumberSeparator);
        result.Append(this.CallerLineNumber);
        result.Append(LogEntry.Separator);
        result.Append(this.CallerMember);
        result.Append(LogEntry.Separator);
        result.Append(this.Message);

        if (this.Exception is not null)
        {
            result.Append(Environment.NewLine);
            result.Append(this.Exception);
        }

        return result.ToString();
    }
    #endregion

    #region Private fields and constants
    private const string Separator = " | ";
    private const char LineNumberSeparator = ':';
    private readonly IApp app;
    private readonly int callerFileNameIndex;
    #endregion
}
