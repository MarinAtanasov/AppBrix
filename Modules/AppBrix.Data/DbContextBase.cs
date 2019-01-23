// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.Impl;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

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

        /// <summary>
        /// Gets the migrations history table name.
        /// This is used when creating automated DB migrations.
        /// </summary>
        protected string MigrationsHistoryTable { get; private set; }
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Initializes the <see cref="DbContextBase"/> using the provided <see cref="IInitializeDbContext"/>.
        /// This should be called right after creating the context inside <see cref="IDbContextService"/>.
        /// </summary>
        /// <param name="context">The <see cref="IInitializeDbContext"/>.</param>
        public void Initialize(IInitializeDbContext context)
        {
            this.App = context.App;
            this.MigrationsAssembly = context.MigrationsAssembly;
            this.MigrationsHistoryTable = context.MigrationsHistoryTable;
        }

        /// <summary>
        /// Disposes of the allocated resources.
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();
            this.App = null;
            this.MigrationsAssembly = null;
        }

        /// <summary>
        /// Configures the context for usage.
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            this.App.GetDbContextConfigurer().Configure(
                new DefaultOnConfiguringDbContext(this, optionsBuilder, this.MigrationsAssembly, this.MigrationsHistoryTable));
        }
        #endregion
    }
}
