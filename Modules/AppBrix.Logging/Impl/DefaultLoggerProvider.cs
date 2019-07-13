// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Logging.Impl
{
    internal sealed class DefaultLoggerProvider : ILoggerProvider, IApplicationLifecycle
    {
        #region Public and overriden methods
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
        }

        public void Uninitialize()
        {
            foreach (var logger in this.loggers.Values)
            {
                logger.Enabled = false;
            }
            this.loggers.Clear();
            this.app = null;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (!this.loggers.TryGetValue(categoryName, out var logger))
            {
                lock (loggers)
                {
                    if (!this.loggers.TryGetValue(categoryName, out logger))
                    {
                        logger = new DefaultLogger(this.app, categoryName, this.app != null);
                        loggers.Add(categoryName, logger);
                    }
                }
            }

            return logger;
        }
        #endregion

        #region Private fields and constants
        private readonly Dictionary<string, DefaultLogger> loggers = new Dictionary<string, DefaultLogger>();
        private IApp app;
        #endregion
    }
}
