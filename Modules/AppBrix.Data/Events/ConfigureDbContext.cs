// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.Events;

internal sealed class ConfigureDbContext : IConfigureDbContext
{
    #region Construction
    public ConfigureDbContext(DbContext context, DbContextOptionsBuilder builder, string? migrationsAssembly = null, string? migrationsHistoryTable = null)
    {
        this.Context = context;
        this.MigrationsAssembly = migrationsAssembly;
        this.MigrationsHistoryTable = migrationsHistoryTable;
        this.OptionsBuilder = builder;
    }
    #endregion

    #region Properties
    public DbContext Context { get; }

    public string? MigrationsAssembly { get; }

    public string? MigrationsHistoryTable { get; }

    public DbContextOptionsBuilder OptionsBuilder { get; }
    #endregion
}
