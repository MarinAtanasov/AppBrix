// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace AppBrix.Logging.Impl;

internal sealed class LoggerProvider : ILoggerProvider, IApplicationLifecycle
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
        this.app = null!;
        this.Dispose();
    }

    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (this.loggers.TryGetValue(categoryName, out var logger))
            return logger;

        lock (this.loggers)
        {
            if (!this.loggers.TryGetValue(categoryName, out logger))
                this.loggers[categoryName] = logger = new Logger(this.app, categoryName, this.app is not null);
        }

        return logger;
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();
    private IApp app = null!;
    #endregion
}
