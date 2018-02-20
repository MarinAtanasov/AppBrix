// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data
{
    /// <summary>
    /// Defines interface which is passed down to <see cref="IDbContextConfigurer.Configure(IOnConfiguringDbContext)"/>.
    /// </summary>
    public interface IOnConfiguringDbContext
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        DbContext Context { get; }

        /// <summary>
        /// Gets the migration assembly.
        /// </summary>
        string MigrationsAssembly { get; }

        /// <summary>
        /// Gets the database context options builder.
        /// </summary>
        DbContextOptionsBuilder OptionsBuilder { get; }
    }
}
