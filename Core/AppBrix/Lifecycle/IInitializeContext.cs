// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Lifecycle
{
    /// <summary>
    /// Context passed down during application initialization.
    /// </summary>
    public interface IInitializeContext
    {
        /// <summary>
        /// Gets the current application.
        /// </summary>
        IApp App { get; }
    }
}
