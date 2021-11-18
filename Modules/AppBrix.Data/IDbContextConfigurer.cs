// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data;

/// <summary>
/// Defines a class which can be called during <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
/// This is called automatically when initializing <see cref="DbContextBase"/>.
/// </summary>
public interface IDbContextConfigurer
{
    /// <summary>
    /// Configures a DB context.
    /// This is called automatically when initializing <see cref="DbContextBase"/>.
    /// </summary>
    /// <param name="context">The database context options builder.</param>
    void Configure(IOnConfiguringDbContext context);
}
