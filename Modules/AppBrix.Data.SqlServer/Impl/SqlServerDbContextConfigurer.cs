// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Events;
using AppBrix.Data.Services;
using AppBrix.Lifecycle;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.SqlServer.Impl;

internal sealed class SqlServerDbContextConfigurer : IDbContextConfigurer, IApplicationLifecycle
{
    public void Initialize(IInitializeContext context)
    {
        this.connectionString = context.App.ConfigService.GetSqlServerDataConfig().ConnectionString;
    }

    public void Uninitialize()
    {
        this.connectionString = string.Empty;
    }

    public void Configure(IConfigureDbContext context)
    {
        context.OptionsBuilder.UseSqlServer(
            this.connectionString,
            builder =>
            {
                if (!string.IsNullOrEmpty(context.MigrationsAssembly))
                    builder = builder.MigrationsAssembly(context.MigrationsAssembly);
                if (!string.IsNullOrEmpty(context.MigrationsHistoryTable))
                    builder = builder.MigrationsHistoryTable(context.MigrationsHistoryTable);
            }
        );
    }

    private string connectionString = string.Empty;
}
