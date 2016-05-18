// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Logging.Entries;
using AppBrix.Logging.Loggers;
using System;
using System.Linq;

namespace AppBrix.Logging.Console
{
    /// <summary>
    /// A log writer which writes entries to the console.
    /// </summary>
    internal sealed class ConsoleLogWriter : ILogWriter
    {
        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
        }

        public void Uninitialize()
        {
        }

        public void WriteEntry(ILogEntry entry)
        {
            System.Console.WriteLine(entry.ToString().Replace(Environment.NewLine, "\n"));
        }
        #endregion
    }
}
