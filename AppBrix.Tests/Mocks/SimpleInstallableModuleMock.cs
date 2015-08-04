// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Linq;

namespace AppBrix.Tests.Mocks
{
    internal sealed class SimpleInstallableModuleMock : SimpleModuleMock, IInstallable
    {
        public bool IsInstalled { get; private set; }

        public bool IsUpgraded { get; private set; }

        public bool IsUninstalled { get; private set; }

        public void Install(IInstallContext context)
        {
            this.IsInstalled = true;
        }

        public void Uninstall(IInstallContext context)
        {
            this.IsUninstalled = true;
        }

        public void Upgrade(IUpgradeContext context)
        {
            this.IsUpgraded = true;
        }
    }
}
