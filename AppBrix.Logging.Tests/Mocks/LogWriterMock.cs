// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Logging.Entries;
using AppBrix.Logging.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Logging.Tests.Mocks
{
    internal class LogWriterMock : ILogWriter
    {
        #region Properties
        public bool IsInitialized { get; private set; }

        public IEnumerable<ILogEntry> LoggedEntries
        {
            get
            {
                return this.loggedEntries;
            }
        }
        #endregion

        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
            this.IsInitialized = true;
        }

        public void Uninitialize()
        {
            this.IsInitialized = false;
        }

        public void WriteEntry(ILogEntry entry)
        {
            this.loggedEntries.Add(entry);
        }
        #endregion

        #region Private fields and constants
        private ICollection<ILogEntry> loggedEntries = new List<ILogEntry>();
        #endregion
    }
}
