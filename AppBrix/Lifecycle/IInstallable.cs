// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    /// <summary>
    /// Used by modules and other objects which require permanent changes:
    /// Configuration, database, files, etc.
    /// </summary>
    public interface IInstallable
    {
        /// <summary>
        /// Used for permanent changes required by the object.
        /// </summary>
        /// <param name="context">The install context.</param>
        void Install(IInstallContext context);

        /// <summary>
        /// Used to upgrade any permanent changes to the latest version.
        /// </summary>
        /// <param name="context">The upgrade context.</param>
        void Upgrade(IUpgradeContext context);

        /// <summary>
        /// Cleans up any permanent changes made by the install method.
        /// </summary>
        /// <param name="context">The uninstall context.</param>
        void Uninstall(IInstallContext context);
    }
}
