// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using AppBrix.Permissions.Impl;
using System;
using System.Linq;

namespace AppBrix.Permissions
{
    /// <summary>
    /// A module which enables working with permissions.
    /// </summary>
    public sealed class PermissionsModule : ModuleBase
    {
        #region Public and overriden methods
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Container.Register(this);
            this.permissionsService.Initialize(context);
            this.App.Container.Register(this.permissionsService);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            this.permissionsService.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly CachedPermissionsService permissionsService = new CachedPermissionsService(new DefaultPermissionsService());
        #endregion
    }
}
