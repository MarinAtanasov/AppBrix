// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    /// <summary>
    /// Context passed down during application installation.
    /// </summary>
    public interface IInstallContext
    {
        /// <summary>
        /// Gets the current application.
        /// </summary>
        IApp App { get; }

        /// <summary>
        /// Gets or sets the requested by the <see cref="IInstallable"/> object application action.
        /// </summary>
        RequestedAction RequestedAction { get; set; }
    }
}
