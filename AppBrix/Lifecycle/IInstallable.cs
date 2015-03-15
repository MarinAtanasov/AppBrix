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
        void Install();

        /// <summary>
        /// Used to upgrade any permanent changes to the latest version.
        /// </summary>
        /// <param name="upgradeFrom">The previous version of the object.</param>
        void Upgrade(Version upgradeFrom);

        /// <summary>
        /// Cleans up any permanent changes made by the install method.
        /// </summary>
        void Uninstall();
    }
}
