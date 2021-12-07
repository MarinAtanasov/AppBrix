// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Data.Services;
using AppBrix.Events.Contracts;
using Microsoft.EntityFrameworkCore;

namespace AppBrix.Data.Events;

/// <summary>
/// Defines interface which is passed down to <see cref="IDbContextConfigurer.Configure(IConfigureDbContext)"/>.
/// </summary>
public interface IConfigureDbContext : IEvent
{
    /// <summary>
    /// Gets the context.
    /// </summary>
    DbContext Context { get; }

    /// <summary>
    /// Gets the migrations assembly.
    /// </summary>
    string? MigrationsAssembly { get; }

    /// <summary>
    /// Gets the migrations history table name.
    /// This is used when creating automated DB migrations.
    /// </summary>
    string? MigrationsHistoryTable { get; }

    /// <summary>
    /// Gets the database context options builder.
    /// </summary>
    DbContextOptionsBuilder OptionsBuilder { get; }
}
