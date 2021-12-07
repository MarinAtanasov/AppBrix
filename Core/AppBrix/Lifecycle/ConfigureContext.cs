// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Lifecycle;

internal class ConfigureContext : IConfigureContext
{
    #region Construction
    public ConfigureContext(IApp app, Version previousVersion)
    {
        if (app is null)
            throw new ArgumentNullException(nameof(app));
        if (previousVersion is null)
            throw new ArgumentNullException(nameof(previousVersion));

        this.App = app;
        this.PreviousVersion = previousVersion;
    }
    #endregion

    #region Properties
    public IApp App { get; }

    public Version PreviousVersion { get; set; }

    public RequestedAction RequestedAction { get; set; }
    #endregion
}
