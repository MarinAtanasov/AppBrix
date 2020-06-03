// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Modules
{
    /// <summary>
    /// Base class for main application modules. Can be inherited by modules inside console or web apps.
    /// This class will add all dependent modules to the application config during installation.
    /// </summary>
    public abstract class MainModuleBase : ModuleBase
    {
        #region Protected methods
        /// <summary>
        /// Installs all module dependencies of the current module recursively to the <see cref="AppConfig"/>.
        /// </summary>
        /// <param name="context">The install context.</param>
        protected override void Install(IInstallContext context)
        {
            var config = this.App.ConfigService.GetAppConfig();
            var installed = new HashSet<string>(config.Modules.Select(x => x.Type));
            var modules = this.GetAllDependencies()
                .Where(x => !installed.Contains(x.GetAssemblyQualifiedName()))
                .OrderBy(x => x.Namespace)
                .ThenBy(x => x.Name)
                .Select(ModuleConfigElement.Create);

            foreach (var module in modules)
            {
                config.Modules.Add(module);
                context.RequestedAction = RequestedAction.Restart;
            }
        }

        /// <summary>
        /// Adds all new module dependencies of the current module recursively to the <see cref="AppConfig"/>.
        /// </summary>
        /// <param name="context">The upgrade context.</param>
        protected override void Upgrade(IUpgradeContext context) => this.Install(context);

        /// <summary>
        /// Throws <see cref="NotSupportedException"/> unless overriden in a child class.
        /// </summary>
        /// <param name="context">The uninstall context.</param>
        protected override void Uninstall(IUninstallContext context) =>
            throw new NotSupportedException($"Module {this.GetType().GetAssemblyQualifiedName()} does not support uninstallation");
        #endregion
    }
}
