// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Logging.Tests.Mocks
{
    /// <summary>
    /// Used for storing the logged entries in-memory for testing purposes.
    /// </summary>
    internal sealed class LoggerMock
    {
        #region Properties
        public IEnumerable<ILogEntry> LoggedEntries => this.loggedEntries;
        #endregion

        #region Public and overriden methods
        public void LogEntry(ILogEntry entry)
        {
            this.loggedEntries.Add(entry);
        }
        #endregion

        #region Private fields and constants
        private readonly List<ILogEntry> loggedEntries = new List<ILogEntry>();
        #endregion
    }
}
