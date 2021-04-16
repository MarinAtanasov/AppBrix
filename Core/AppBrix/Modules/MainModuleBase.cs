// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Lifecycle;
using System;
using System.Linq;

namespace AppBrix.Modules
{
    /// <summary>
    /// Base class for main application modules. Can be inherited by modules inside console or web apps.
    /// This class will add all dependent modules to the application config during configuration.
    /// </summary>
    public abstract class MainModuleBase : ModuleBase
    {
        #region Protected methods
        /// <summary>
        /// Adds and sorts all module dependencies of the current module recursively in the <see cref="AppConfig"/>.
        /// </summary>
        /// <param name="context">The configure context.</param>
        protected override void Configure(IConfigureContext context)
        {
            var configModules = this.App.ConfigService.GetAppConfig().Modules;
            var modules = this.GetAllDependencies().Select(ModuleConfigElement.Create).Reverse();

            var previousIndex = configModules.Count;
            foreach (var module in modules)
            {
                var index = -1;
                for (var i = 0; i < configModules.Count; i++)
                {
                    if (configModules[i].Type == module.Type)
                    {
                        index = i;
                        break;
                    }
                }

                if (index < 0)
                {
                    configModules.Insert(previousIndex, module);
                    context.RequestedAction = RequestedAction.Restart;
                }
                else if (index > previousIndex)
                {
                    var oldModule = configModules[index];
                    configModules.RemoveAt(index);
                    configModules.Insert(previousIndex, oldModule);
                    context.RequestedAction = RequestedAction.Restart;
                }
                else
                {
                    previousIndex = index;
                }
            }
        }
        #endregion
    }
}
