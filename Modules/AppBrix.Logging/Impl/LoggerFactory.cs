﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using Microsoft.Extensions.Logging;

namespace AppBrix.Logging.Impl;

internal sealed class LoggerFactory : ILoggerFactory, IApplicationLifecycle
{
    #region Public and overriden methods
    public void Initialize(IInitializeContext context)
    {
        this.app = context.App;
    }

    public void Uninitialize()
    {
        this.app = null!;
        this.Dispose();
    }

    public void Dispose()
    {
    }

    public void AddProvider(ILoggerProvider provider) => this.app.Container.Register(provider);

    public ILogger CreateLogger(string categoryName) => this.app.GetLoggerProvider().CreateLogger(categoryName);
    #endregion

    #region Private fields and constants
    private IApp app = null!;
    #endregion
}
