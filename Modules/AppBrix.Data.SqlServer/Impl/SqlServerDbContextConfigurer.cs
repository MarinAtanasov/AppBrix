﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.SqlServer.Impl
{
    internal sealed class SqlServerDbContextConfigurer : IDbContextConfigurer, IApplicationLifecycle
    {
        public void Initialize(IInitializeContext context)
        {
            this.connectionString = context.App.ConfigService.GetSqlServerDataConfig().ConnectionString;
        }

        public void Uninitialize()
        {
            this.connectionString = null;
        }

        public void Configure(IOnConfiguringDbContext context)
        {
            context.OptionsBuilder.UseSqlServer(
                this.connectionString,
                builder => builder
                    .MigrationsAssembly(context.MigrationsAssembly)
                    .MigrationsHistoryTable(context.MigrationsHistoryTable)
            );
        }

        private string? connectionString;
    }
}
