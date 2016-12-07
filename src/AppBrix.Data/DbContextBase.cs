// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Linq;
using AppBrix.Data.Impl;

namespace AppBrix.Data
{
    /// <summary>
    /// Base class for database contexts in the scope of an <see cref="IApp"/>.
    /// </summary>
    public abstract class DbContextBase : DbContext
    {
        #region Properties
        /// <summary>
        /// Gets the current <see cref="IApp"/>.
        /// </summary>
        protected IApp App { get; private set; }

        /// <summary>
        /// Gets the migrations assembly to be used during migrations.
        /// </summary>
        protected string MigrationsAssembly { get; private set; }
        #endregion

        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeDbContext context)
        {
            this.App = context.App;
            this.MigrationsAssembly = context.MigrationsAssembly;
        }
        #endregion

        #region Public and overriden methods
        public override void Dispose()
        {
            base.Dispose();
            this.App = null;
            this.MigrationsAssembly = null;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: Call object to initialize server / migration assembly (last could be hardcoded)
            //optionsBuilder.UseSqlServer(@"Server=.\sqlexpress;Database=MyDatabase;Trusted_Connection=True;");
            this.App.GetEventHub().Raise<IOnConfiguringDbContext>(new DefaultOnConfiguringDbContext(optionsBuilder));
            optionsBuilder.Options.Extensions.OfType<RelationalOptionsExtension>().Single().MigrationsAssembly = this.MigrationsAssembly;
        }
        #endregion

    }
}
