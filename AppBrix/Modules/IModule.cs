// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using System;
using System.Linq;

namespace AppBrix.Modules
{
    /// <summary>
    /// Interface for an application module.
    /// </summary>
    public interface IModule : IApplicationLifecycle
    {
        /// <summary>
        /// The priority in which the module will be installed or initialized.
        /// Higher priority means that the module will load sooner and unload later than the rest of the modules.
        /// </summary>
        int LoadPriority { get; }
    }
}
