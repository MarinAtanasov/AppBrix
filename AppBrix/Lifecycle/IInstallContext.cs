// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    /// <summary>
    /// Context passed down during application installation or uninstall.
    /// </summary>
    public interface IInstallContext
    {
        /// <summary>
        /// Gets the current application.
        /// </summary>
        IApp App { get; }
    }
}
