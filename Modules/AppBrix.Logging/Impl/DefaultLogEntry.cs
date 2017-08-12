// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Logging.Entries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AppBrix.Logging.Impl
{
    internal sealed class DefaultLogEntry : ILogEntry
    {
        #region Construciton
        public DefaultLogEntry(IApp app, LogLevel level, DateTime created, string message, Exception exception = null,
            string callerFile = null, string callerMember = null, int callerLineNumber = 0)
        {
            this.app = app;
            this.Level = level;
            this.Exception = exception;
            this.Message = message;
            this.CallerFile = callerFile;
            this.CallerMember = callerMember;
            this.CallerLineNumber = callerLineNumber;
            this.ThreadId = Thread.CurrentThread.ManagedThreadId;
            this.Created = created;
        }
        #endregion

        #region Properties
        public LogLevel Level { get; }

        public string CallerFile { get; }

        public string CallerMember { get; }

        public int CallerLineNumber { get; }

        public Exception Exception { get; }

        public string Message { get; }

        public DateTime Created { get; }

        public int ThreadId { get; }
        #endregion

        #region Public and overriden methods
        public override int GetHashCode()
        {
            return this.Message.GetHashCode();
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.Append(app.GetTimeService().ToString(this.Created));
            result.Append(DefaultLogEntry.Separator);
            result.Append(string.Format("{0,-5}", this.Level));
            result.Append(DefaultLogEntry.Separator);
            result.Append(this.ThreadId);
            result.Append(DefaultLogEntry.Separator);
            result.Append(this.CallerFile.Split(DefaultLogEntry.DirectorySeparatorChars, StringSplitOptions.RemoveEmptyEntries).LastOrDefault());
            result.Append(DefaultLogEntry.LineNumberSeparator);
            result.Append(this.CallerLineNumber);
            result.Append(DefaultLogEntry.Separator);
            result.Append(this.CallerMember);
            result.Append(DefaultLogEntry.Separator);
            result.Append(this.Message);

            if (this.Exception != null)
            {
                result.Append(Environment.NewLine);
                result.Append(this.Exception);
            }

            return result.ToString();
        }
        #endregion

        #region Private fields and constants
        private const string Separator = " | ";
        private const string LineNumberSeparator = ":";
        private static readonly char[] DirectorySeparatorChars = new HashSet<char> { '/', '\\', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }.ToArray();
        private readonly IApp app;
        #endregion
    }
}
