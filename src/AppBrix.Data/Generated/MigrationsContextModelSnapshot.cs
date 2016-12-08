// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AppBrix.Data.Migrations;

namespace AppBrix.Data.Generated
{
    [DbContext(typeof(MigrationsContext))]
    partial class MigrationsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");
                //.HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AppBrix.Data.Migrations.MigrationData", b =>
                {
                    b.Property<string>("Context")
                        .IsUnicode(true);

                    b.Property<string>("Version")
                        .IsUnicode(true);

                    b.Property<string>("Metadata")
                        .IsRequired()
                        .HasColumnType("ntext")
                        .IsUnicode(true);

                    b.Property<string>("Migration")
                        .IsRequired()
                        .HasColumnType("ntext")
                        .IsUnicode(true);

                    b.HasKey("Context", "Version");

                    b.ToTable("Migrations");
                });

            modelBuilder.Entity("AppBrix.Data.Migrations.SnapshotData", b =>
                {
                    b.Property<string>("Context")
                        .IsUnicode(true);

                    b.Property<string>("Version")
                        .IsRequired()
                        .IsUnicode(true);

                    b.Property<string>("Snapshot")
                        .IsRequired()
                        .HasColumnType("ntext")
                        .IsUnicode(true);

                    b.HasKey("Context");

                    b.ToTable("Snapshots");
                });
        }
    }
}
