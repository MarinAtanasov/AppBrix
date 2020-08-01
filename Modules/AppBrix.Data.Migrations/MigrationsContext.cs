// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.Migrations.Data;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.Migrations
{
    /// <summary>
    /// Database context used for database migrations.
    /// </summary>
    public sealed class MigrationsContext : DbContextBase
    {
        #nullable disable
        /// <summary>
        /// Gets or sets the database migration data.
        /// </summary>
        public DbSet<MigrationData> Migrations { get; set; }

        /// <summary>
        /// Gets or sets the database migration snapshots.
        /// </summary>
        public DbSet<SnapshotData> Snapshots { get; set; }
        #nullable restore

        /// <summary>
        /// Configures the creation of the database migration models.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MigrationData>(entity =>
            {
                entity.Property(x => x.Context).IsUnicode().HasMaxLength(64).ValueGeneratedNever();
                entity.Property(x => x.Version).IsUnicode().HasMaxLength(32).ValueGeneratedNever();
                entity.Property(x => x.Migration).IsUnicode();
                entity.Property(x => x.Metadata).IsUnicode();
                entity.HasKey(x => new { x.Context, x.Version });
            });

            modelBuilder.Entity<SnapshotData>(entity =>
            {
                entity.Property(x => x.Context).IsUnicode().HasMaxLength(64).ValueGeneratedNever();
                entity.Property(x => x.Version).IsUnicode().HasMaxLength(32);
                entity.Property(x => x.Snapshot).IsUnicode();
                entity.HasKey(x => x.Context);
            });
        }
    }
}
