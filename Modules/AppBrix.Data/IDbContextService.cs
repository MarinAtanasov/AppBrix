// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data
{
    /// <summary>
    /// Defines a database context service to be used when initializing context deriving from <see cref="DbContext"/>.
    /// </summary>
    public interface IDbContextService
    {
        /// <summary>
        /// Gets an instance of a <see cref="DbContext"/>.
        /// </summary>
        /// <param name="type">The type of the context.</param>
        /// <returns>A databse context of the provided type.</returns>
        DbContext Get(Type type);
    }
}
