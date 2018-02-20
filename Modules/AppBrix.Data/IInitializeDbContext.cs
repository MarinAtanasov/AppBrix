// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Data
{
    /// <summary>
    /// Context passed down during <see cref="DbContextBase"/> initialization.
    /// </summary>
    public interface IInitializeDbContext
    {
        /// <summary>
        /// Gets the current application.
        /// </summary>
        IApp App { get; }

        /// <summary>
        /// Gets the migrations assembly.
        /// </summary>
        string MigrationsAssembly { get; }
    }
}
