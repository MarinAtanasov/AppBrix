// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Microsoft.EntityFrameworkCore;
using System;

namespace AppBrix.Data;

/// <summary>
/// Defines a database context service to be used when initializing context deriving from <see cref="DbContext"/>.
/// </summary>
public interface IDbContextService
{
    /// <summary>
    /// Gets an instance of a <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="T">The type of the context.</typeparam>
    /// <returns>A database context of the provided type.</returns>
    T Get<T>() where T : DbContext => (T)this.Get(typeof(T));

    /// <summary>
    /// Gets an instance of a <see cref="DbContext"/>.
    /// </summary>
    /// <param name="type">The type of the context.</param>
    /// <returns>A database context of the provided type.</returns>
    DbContext Get(Type type);
}
