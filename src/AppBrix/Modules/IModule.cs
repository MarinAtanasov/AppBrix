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
    public interface IModule : IApplicationLifecycle, IInstallable
    {
    }
}
