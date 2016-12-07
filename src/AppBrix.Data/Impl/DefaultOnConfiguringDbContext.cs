// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.Impl
{
    internal class DefaultOnConfiguringDbContext : IOnConfiguringDbContext
    {
        #region Construction
        public DefaultOnConfiguringDbContext(DbContextOptionsBuilder builder)
        {
            this.OptionsBuilder = builder;
        }
        #endregion

        #region Properties
        public DbContextOptionsBuilder OptionsBuilder { get; }
        #endregion
    }
}
