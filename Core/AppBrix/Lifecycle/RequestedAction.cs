// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Lifecycle;

/// <summary>
/// Enumeration used for storing the requested action during module installation or upgrade.
/// </summary>
public enum RequestedAction
{
    /// <summary>
    /// No action has been requested. The lifecycle workflow will continue normally.
    /// </summary>
    None,

    /// <summary>
    /// Requested reinitialization of the partially loaded application.
    /// </summary>
    Reinitialize,

    /// <summary>
    /// Requested full restart of the partially loaded application.
    /// </summary>
    Restart
}
