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
            var configModules = this.App.ConfigService.GetAppConfig().Modules;
            var modules = this.GetAllDependencies().Select(ModuleConfigElement.Create).Reverse();

            var previousIndex = configModules.Count;
            foreach (var module in modules)
            {
                var index = configModules.Count;
                for (var i = 0; i < index; i++)
                {
                    if (configModules[i].Type == module.Type)
                        index = i;
                }
                
                if (index == configModules.Count)
                {
                    configModules.Insert(previousIndex, module);
                    context.RequestedAction = RequestedAction.Restart;
                }
                else
                {
                    previousIndex = index;
                }
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
