// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;

namespace AppBrix.Modules;

/// <summary>
/// Interface for an application module.
/// </summary>
public interface IModule : IApplicationLifecycle, IInstallable
{
    /// <summary>
    /// Gets the types of the modules which are direct dependencies for the current module.
    /// This is used to determine the order in which the modules are loaded.
    /// </summary>
    IEnumerable<Type> Dependencies { get; }
}
