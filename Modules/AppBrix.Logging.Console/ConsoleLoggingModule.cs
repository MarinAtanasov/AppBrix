// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using AppBrix.Logging.Console.Impl;
using AppBrix.Modules;
using System;
using System.Collections.Generic;

namespace AppBrix.Logging.Console;

/// <summary>
/// Class that registers a console logger.
/// </summary>
public sealed class ConsoleLoggingModule : ModuleBase
{
    #region Properties
    /// <summary>
    /// Gets the types of the modules which are direct dependencies for the current module.
    /// This is used to determine the order in which the modules are loaded.
    /// </summary>
    public override IEnumerable<Type> Dependencies => [typeof(LoggingModule)];
    #endregion

    #region Public and overriden methods
    /// <summary>
    /// Initializes the module.
    /// Automatically called by <see cref="ModuleBase.Initialize"/>
    /// </summary>
    /// <param name="context">The initialization context.</param>
    protected override void Initialize(IInitializeContext context)
    {
        this.logger.Initialize(context);
        this.App.Container.Register(this.logger);
    }

    /// <summary>
    /// Uninitializes the module.
    /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
    /// </summary>
    protected override void Uninitialize()
    {
        this.logger.Uninitialize();
    }
    #endregion

    #region Private fields and constants
    private readonly ConsoleLogger logger = new ConsoleLogger();
    #endregion
}
