// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Lifecycle;
using AppBrix.Logging.Entries;
using AppBrix.Logging.File.Configuration;
using AppBrix.Logging.Loggers;
using System;
using System.IO;
using System.Linq;

namespace AppBrix.Logging.File
{
    /// <summary>
    /// A log writer which writes entries to the console.
    /// </summary>
    internal sealed class FileLogWriter : ILogWriter
    {
        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
            var config = context.App.GetConfig<FileLoggerConfig>();
            this.writer = System.IO.File.AppendText(config.Path);
        }

        public void Uninitialize()
        {
            this.writer.Dispose();
            this.writer = null;
        }

        public void WriteEntry(ILogEntry entry)
        {
            this.writer.WriteLine(entry.ToString());
        }
        #endregion

        #region Private fields and constants
        private TextWriter writer;
        #endregion
    }
}
