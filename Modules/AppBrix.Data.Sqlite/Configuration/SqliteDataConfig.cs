// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;

namespace AppBrix.Data.Sqlite.Configuration;

/// <summary>
/// Configures the Sqlite data provider.
/// </summary>
public sealed class SqliteDataConfig : IConfig
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="SqliteDataConfig"/> with default property values.
    /// </summary>
    public SqliteDataConfig()
    {
        this.ConnectionString = "Data Source=AppBrix.sqlite3;";
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the connection string to the Sqlite database instance.
    /// </summary>
    public string ConnectionString { get; set; }
    #endregion
}
