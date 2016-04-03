// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    internal class DefaultInstallContext : IInstallContext
    {
        #region Construction
        public DefaultInstallContext(IApp app)
        {
            this.App = app;
        }
        #endregion

        #region Properties
        public IApp App { get; }
        #endregion
    }
}
