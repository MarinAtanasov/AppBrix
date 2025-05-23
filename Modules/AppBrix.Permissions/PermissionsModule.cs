// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Container;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using AppBrix.Permissions.Impl;
using System;
using System.Collections.Generic;

namespace AppBrix.Permissions;

/// <summary>
/// A module which enables working with permissions.
/// </summary>
public sealed class PermissionsModule : ModuleBase
{
    #region Properties
    /// <summary>
    /// Gets the types of the modules which are direct dependencies for the current module.
    /// This is used to determine the order in which the modules are loaded.
    /// </summary>
    public override IEnumerable<Type> Dependencies => [typeof(ContainerModule)];
    #endregion

    #region Public and overriden methods
    /// <summary>
    /// Initializes the module.
    /// Automatically called by <see cref="ModuleBase.Initialize"/>
    /// </summary>
    /// <param name="context">The initialization context.</param>
    protected override void Initialize(IInitializeContext context)
    {
        this.permissionsService = this.App.ConfigService.GetPermissionsConfig().EnableCaching ?
            new CachedPermissionsService(new PermissionsService()) : new PermissionsService();
        this.permissionsService.Initialize(context);
        this.App.Container.Register(this.permissionsService);
    }

    /// <summary>
    /// Uninitializes the module.
    /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
    /// </summary>
    protected override void Uninitialize()
    {
        this.permissionsService?.Uninitialize();
        this.permissionsService = null;
    }
    #endregion

    #region Private fields and constants
    private IApplicationLifecycle? permissionsService;
    #endregion
}
