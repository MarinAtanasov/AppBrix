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
    public interface IContextLoader
    {
        /// <summary>
        /// Gets an instance of a <see cref="DbContextBase"/> of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the context.</typeparam>
        /// <returns>A databse context of the provided type.</returns>
        T Get<T>() where T : DbContextBase;
    }
}
