// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

namespace AppBrix.Lifecycle;

internal sealed class UninstallContext : IUninstallContext
{
    #region Construction
    public UninstallContext(IApp app)
    {
        this.App = app;
    }
    #endregion

    #region Properties
    public IApp App { get; }
    #endregion
}
