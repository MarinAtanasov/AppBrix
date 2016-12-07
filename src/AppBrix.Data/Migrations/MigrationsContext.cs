// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Data.Impl;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AppBrix.Data.Migrations
{
    /// <summary>
    /// Database context used for database migrations.
    /// </summary>
    public class MigrationsContext : DbContext
    {
        public MigrationsContext(IApp app)
        {
            this.app = app;
        }

        public DbSet<MigrationData> Migrations { get; set; }

        public DbSet<SnapshotData> Snapshots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            this.app?.GetEventHub().Raise<IOnConfiguringDbContext>(new DefaultOnConfiguringDbContext(optionsBuilder));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MigrationData>(entity =>
            {
                entity.Property(x => x.Context).IsRequired().IsUnicode().ValueGeneratedNever();

                entity.Property(x => x.Version).IsRequired().IsUnicode().ValueGeneratedNever();

                entity.Property(x => x.Migration).IsRequired().IsUnicode().HasColumnType("ntext");

                entity.Property(x => x.Metadata).IsRequired().IsUnicode().HasColumnType("ntext");

                entity.HasKey(x => new { x.Context, x.Version });
            });

            modelBuilder.Entity<SnapshotData>(entity =>
            {
                entity.Property(x => x.Context).IsRequired().IsUnicode().ValueGeneratedNever();

                entity.Property(x => x.Version).IsRequired().IsUnicode();

                entity.Property(x => x.Snapshot).IsRequired().IsUnicode().HasColumnType("ntext");

                entity.HasKey(x => x.Context);
            });
        }

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
