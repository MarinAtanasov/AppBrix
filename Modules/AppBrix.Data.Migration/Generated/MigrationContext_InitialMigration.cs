// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AppBrix.Data.Migration.Generated
{
    /// <summary>
    /// Auto-generated.
    /// </summary>
    public partial class MigrationContext_InitialMigration : Microsoft.EntityFrameworkCore.Migrations.Migration
    {
        /// <summary>
        /// Auto-generated.
        /// </summary>
        /// <param name="migrationBuilder">Migration builder.</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Migrations",
                columns: table => new
                {
                    Context = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: false),
                    Metadata = table.Column<string>(type: "ntext", nullable: false),
                    Migration = table.Column<string>(type: "ntext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Migrations", x => new { x.Context, x.Version });
                });

            migrationBuilder.CreateTable(
                name: "Snapshots",
                columns: table => new
                {
                    Context = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: false),
                    Snapshot = table.Column<string>(type: "ntext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshots", x => x.Context);
                });
        }

        /// <summary>
        /// Auto-generated.
        /// </summary>
        /// <param name="migrationBuilder">Migration builder.</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Migrations");

            migrationBuilder.DropTable(name: "Snapshots");
        }
    }
}
