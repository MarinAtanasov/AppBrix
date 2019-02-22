// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    internal sealed class DefaultUpgradeContext : DefaultInstallContext, IUpgradeContext
    {
        #region Construction
        public DefaultUpgradeContext(IApp app, Version previousVersion) : base(app)
        {
            if (previousVersion == null)
                throw new ArgumentNullException(nameof(previousVersion));

            this.PreviousVersion = previousVersion;
        }
        #endregion

        #region Properties
        public Version PreviousVersion { get; }
        #endregion
    }
}
