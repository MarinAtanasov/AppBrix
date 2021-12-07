// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Data.SqlServer;
using AppBrix.Data.SqlServer.Configuration;

namespace AppBrix;

/// <summary>
/// Extension methods for the <see cref="SqlServerDataModule"/>.
/// </summary>
public static class SqlServerDataExtensions
{
    /// <summary>
    /// Gets the <see cref="SqlServerDataConfig"/> from <see cref="IConfigService"/>.
    /// </summary>
    /// <param name="service">The configuration service.</param>
    /// <returns>The <see cref="SqlServerDataConfig"/>.</returns>
    public static SqlServerDataConfig GetSqlServerDataConfig(this IConfigService service) => (SqlServerDataConfig)service.Get(typeof(SqlServerDataConfig));
}
