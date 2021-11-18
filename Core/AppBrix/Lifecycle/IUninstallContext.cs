// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Lifecycle;

/// <summary>
/// Context passed down during application uninstallation.
/// </summary>
public interface IUninstallContext
{
    /// <summary>
    /// Gets the current application.
    /// </summary>
    IApp App { get; }
}
