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
        public DefaultOnConfiguringDbContext(DbContext context, DbContextOptionsBuilder builder)
        {
            this.Context = context;
            this.OptionsBuilder = builder;
        }
        #endregion

        #region Properties
        public DbContext Context { get; }

        public DbContextOptionsBuilder OptionsBuilder { get; }
        #endregion
    }
}
