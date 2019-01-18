// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using AppBrix.Permissions.Configuration;
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
            this.permissionsService = this.App.GetConfig<PermissionsConfig>().EnableCaching ?
                new CachedPermissionsService(new DefaultPermissionsService()) :
                (IApplicationLifecycle)new DefaultPermissionsService();
            this.permissionsService.Initialize(context);
            this.App.Container.Register(this.permissionsService, this.permissionsService.GetType());
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase"/>.<see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void UninitializeModule()
        {
            this.permissionsService?.Uninitialize();
            this.permissionsService = null;
        }
        #endregion

        #region Private fields and constants
        private IApplicationLifecycle permissionsService;
        #endregion
    }
}
