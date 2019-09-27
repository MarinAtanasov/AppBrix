// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.Impl
{
    internal sealed class DefaultOnConfiguringDbContext : IOnConfiguringDbContext
    {
        #region Construction
        public DefaultOnConfiguringDbContext(DbContext context, DbContextOptionsBuilder builder, string? migrationsAssembly = null, string? migrationsHistoryTable = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            this.Context = context;
            this.MigrationsAssembly = migrationsAssembly;
            this.MigrationsHistoryTable = migrationsHistoryTable;
            this.OptionsBuilder = builder;
        }
        #endregion

        #region Properties
        public DbContext Context { get; }

        public string? MigrationsAssembly { get; }

        public string? MigrationsHistoryTable { get; }

        public DbContextOptionsBuilder OptionsBuilder { get; }
        #endregion
    }
}
