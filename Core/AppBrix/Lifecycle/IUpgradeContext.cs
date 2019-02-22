// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    /// <summary>
    /// Context passed down during application installation, upgrade or uninstall.
    /// </summary>
    public interface IUpgradeContext : IInstallContext
    {
        /// <summary>
        /// Gets the previously installed version of the module being upgraded.
        /// </summary>
        Version PreviousVersion { get; }
    }
}
