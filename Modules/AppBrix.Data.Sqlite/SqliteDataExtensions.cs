// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Data.Sqlite;
using AppBrix.Data.Sqlite.Configuration;

namespace AppBrix;

/// <summary>
/// Extension methods for the <see cref="SqliteDataModule"/>.
/// </summary>
public static class SqliteDataExtensions
{
    /// <summary>
    /// Gets the <see cref="SqliteDataConfig"/> from <see cref="IConfigService"/>.
    /// </summary>
    /// <param name="service">The configuration service.</param>
    /// <returns>The <see cref="SqliteDataConfig"/>.</returns>
    public static SqliteDataConfig GetSqliteDataConfig(this IConfigService service) => (SqliteDataConfig)service.Get(typeof(SqliteDataConfig));
}
