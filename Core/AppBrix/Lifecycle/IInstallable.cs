// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Lifecycle
{
    /// <summary>
    /// Used by modules and other objects which require permanent changes:
    /// Configuration, database, files, etc.
    /// </summary>
    public interface IInstallable
    {
        /// <summary>
        /// Used for updating the configurations before installing the object.
        /// </summary>
        /// <param name="context">The configure context.</param>
        void Configure(IConfigureContext context);

        /// <summary>
        /// Used for permanent changes required by the object.
        /// </summary>
        /// <param name="context">The install context.</param>
        void Install(IInstallContext context);

        /// <summary>
        /// Cleans up any permanent changes made by the install method.
        /// </summary>
        /// <param name="context">The uninstall context.</param>
        void Uninstall(IUninstallContext context);
    }
}
