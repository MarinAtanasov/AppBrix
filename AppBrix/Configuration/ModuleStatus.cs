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
        Enabled,
        Disabled,
        Uninstalling
    }
}
