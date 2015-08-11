// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace AppBrix.Logging.Entries
{
    internal sealed class DefaultLogEntry : ILogEntry
    {
        #region Construciton
        public DefaultLogEntry(IApp app, LogLevel level, DateTime created, string message, Exception error = null, StackTrace trace = null,
            string callerFile = null, string callerMember = null, int callerLineNumber = 0)
        {
            this.app = app;
            this.Level = level;
            this.Error = error;
            this.Message = message;
            this.trace = trace;
            this.CallerFile = callerFile;
            this.CallerMember = callerMember;
            this.CallerLineNumber = callerLineNumber;
            this.ThreadId = Thread.CurrentThread.ManagedThreadId;
            this.Created = created;
        }
        #endregion

        #region Properties
        public LogLevel Level { get; private set; }

        public string CallerFile { get; private set; }

        public string CallerMember { get; private set; }

        public int CallerLineNumber { get; private set; }

        public Exception Error { get; private set; }

        public string Message { get; private set; }

        public DateTime Created { get; private set; }

        public int ThreadId { get; private set; }

        public string Trace
        {
            get
            {
                return this.trace != null ? this.trace.ToString().TrimEnd() : null;
            }
        }
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
            result.Append(this.CallerFile.Substring(this.CallerFile.LastIndexOf(Path.DirectorySeparatorChar) + 1));
            result.Append(DefaultLogEntry.LineNumberSeparator);
            result.Append(this.CallerLineNumber);
            result.Append(DefaultLogEntry.Separator);
            result.Append(this.CallerMember);
            result.Append(DefaultLogEntry.Separator);
            result.Append(this.Message);

            if (this.trace != null)
            {
                result.Append(Environment.NewLine);
                result.Append(this.Trace);
            }
            if (this.Error != null)
            {
                result.Append(Environment.NewLine);
                result.Append(this.Error);
            }
            
            return result.ToString();
        }
        #endregion

        #region Private fields and constants
        private const string Separator = " | ";
        private const string LineNumberSeparator = ":";
        private readonly IApp app;
        private readonly StackTrace trace;
        #endregion
    }
}
