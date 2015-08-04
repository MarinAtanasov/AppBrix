// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    internal class DefaultUpgradeContext : DefaultInstallContext, IUpgradeContext
    {
        #region Construction
        public DefaultUpgradeContext(IApp app, Version upgradeFrom) : base(app)
        {
            this.UpgradeFrom = upgradeFrom;
        }
        #endregion

        #region Properties
        public Version UpgradeFrom { get; private set; }
        #endregion
    }
}
