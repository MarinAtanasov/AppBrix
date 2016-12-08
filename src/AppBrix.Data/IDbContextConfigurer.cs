// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data
{
    /// <summary>
    /// Defines a class which can be called during <see cref="DbContext.OnConfiguring(DbContextOptionsBuilder)"/>.
    /// This is called automatically when initializing <see cref="DbContextBase"/> or <see cref="Migrations.MigrationsContext"/>.
    /// </summary>
    public interface IDbContextConfigurer
    {
        /// <summary>
        /// Configures a DB context.
        /// This is called automatically when initializing <see cref="DbContextBase"/> or <see cref="Migrations.MigrationsContext"/>.
        /// </summary>
        /// <param name="optionsBuilder">The database context options builder.</param>
        void Configure(IOnConfiguringDbContext context);
    }
}
