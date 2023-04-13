// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;

namespace AppBrix.Data.Migrations.Configuration;

/// <summary>
/// Configures the Migrations data module.
/// </summary>
public sealed class MigrationsDataConfig : IConfig
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="MigrationsDataConfig"/>.
    /// </summary>
    public MigrationsDataConfig()
    {
        this.EntryAssembly = string.Empty;
        this.MigrationsHistoryTablePrefix = "__MH_";
        this.MigrationsHistoryTableSuffixes = new []
        {
            "DbContext",
            "Context"
        };
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets or sets the entry assembly for the application. Leave blank to get from the runtime.
    /// This is used during creation of migration scripts in order to find necessary assembly references.
    /// </summary>
    public string EntryAssembly { get; set; }

    /// <summary>
    /// Gets or sets the prefix used for migrations history table names.
    /// This should not be changed once the application has been initialized.
    /// </summary>
    public string MigrationsHistoryTablePrefix { get; set; }

    /// <summary>
    /// Gets or sets the suffixes to trim from the migrations history table names.
    /// This should not be changed once the application has been initialized.
    /// </summary>
    public string[] MigrationsHistoryTableSuffixes { get; set; }
    #endregion
}
