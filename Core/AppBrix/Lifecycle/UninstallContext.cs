// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Lifecycle;

internal sealed class UninstallContext : IUninstallContext
{
    #region Construction
    public UninstallContext(IApp app)
    {
        if (app is null)
            throw new ArgumentNullException(nameof(app));

        this.App = app;
    }
    #endregion

    #region Properties
    public IApp App { get; }
    #endregion
}
