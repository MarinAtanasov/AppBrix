// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Lifecycle;

/// <summary>
/// Context passed down during application installation to install an <see cref="IInstallable"/> object.
/// </summary>
public interface IInstallContext
{
    /// <summary>
    /// Gets the current application.
    /// </summary>
    IApp App { get; }

    /// <summary>
    /// Gets the previously installed version of the module.
    /// </summary>
    Version PreviousVersion { get; }

    /// <summary>
    /// Gets or sets the requested by the <see cref="IInstallable"/> object application action.
    /// </summary>
    RequestedAction RequestedAction { get; set; }
}
