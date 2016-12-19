// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Data
{
    /// <summary>
    /// Defines a database context loader to be used when initializing context deriving from <see cref="DbContextBase"/>.
    /// </summary>
    public interface IDbContextLoader
    {
        /// <summary>
        /// Gets an instance of a <see cref="DbContextBase"/> of type <see cref="T"/>.
        /// </summary>
        /// <param name="type">The type of the context.</param>
        /// <returns>A databse context of the provided type.</returns>
        DbContextBase Get(Type type);
    }
}
