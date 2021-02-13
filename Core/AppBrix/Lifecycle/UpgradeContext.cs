// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Lifecycle
{
    internal sealed class UpgradeContext : InstallContext, IUpgradeContext
    {
        #region Construction
        public UpgradeContext(IApp app, Version previousVersion) : base(app)
        {
            if (previousVersion is null)
                throw new ArgumentNullException(nameof(previousVersion));

            this.PreviousVersion = previousVersion;
        }
        #endregion

        #region Properties
        public Version PreviousVersion { get; }
        #endregion
    }
}
