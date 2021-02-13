// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Lifecycle
{
    internal class InstallContext : IInstallContext
    {
        #region Construction
        public InstallContext(IApp app)
        {
            if (app is null)
                throw new ArgumentNullException(nameof(app));

            this.App = app;
        }
        #endregion

        #region Properties
        public IApp App { get; }

        public RequestedAction RequestedAction { get; set; }
        #endregion
    }
}
