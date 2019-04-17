// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
namespace AppBrix.Configuration
{
    /// <summary>
    /// Enumeration used for storing a module's lifecycle status.
    /// If the module is <see cref="Disabled"/>, it will not be loaded during application initialization.
    /// If the module is <see cref="Uninstalling"/>, it will be uninstalled during the next applicaiton uninitialization.
    /// </summary>
    public enum ModuleStatus
    {
        /// <summary>
        /// The module is enabled.
        /// If it has not been installed yet, it will be installed.
        /// If the module has been installed from a previous version of the module assembly, it will be upgraded.
        /// </summary>
        Enabled,
        /// <summary>
        /// The module is disabled and will not be loaded during application initialization.
        /// </summary>
        Disabled,
        /// <summary>
        /// Schedules a module for uninstallation.
        /// The module will be uninstalled during the next application uninitialization.
        /// </summary>
        Uninstalling
    }
}
