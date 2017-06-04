// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data.Migration.Impl
{
    internal sealed class DefaultOnConfiguringDbContext : IOnConfiguringDbContext
    {
        #region Construction
        public DefaultOnConfiguringDbContext(DbContext context, DbContextOptionsBuilder builder, string migrationsAssembly = null)
        {
            this.Context = context;
            this.MigrationsAssembly = migrationsAssembly;
            this.OptionsBuilder = builder;
        }
        #endregion

        #region Properties
        public DbContext Context { get; }

        public string MigrationsAssembly { get; }

        public DbContextOptionsBuilder OptionsBuilder { get; }
        #endregion
    }
}
