// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Data.Sqlite.Configuration;
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.Sqlite.Impl
{
    internal sealed class SqliteDbContextConfigurer : IDbContextConfigurer, IApplicationLifecycle
    {
        public void Initialize(IInitializeContext context)
        {
            this.connectionString = context.App.GetConfig<SqliteDataConfig>().ConnectionString;
        }

        public void Uninitialize()
        {
            this.connectionString = null;
        }

        public void Configure(IOnConfiguringDbContext context)
        {
            context.OptionsBuilder.UseSqlite(
                this.connectionString,
                builder => builder
                    .MigrationsAssembly(context.MigrationsAssembly)
                    .MigrationsHistoryTable(context.MigrationsHistoryTable)
            );
        }

        private string? connectionString;
    }
}
