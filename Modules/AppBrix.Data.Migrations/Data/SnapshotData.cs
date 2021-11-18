// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//

namespace AppBrix.Data.Migrations.Data;

/// <summary>
/// Database snapshot created after a database migration.
/// </summary>
public sealed class SnapshotData
{
    /// <summary>
    /// Creates a new instance of <see cref="SnapshotData"/>.
    /// </summary>
    public SnapshotData()
    {
        this.Context = string.Empty;
        this.Version = string.Empty;
        this.Snapshot = string.Empty;
    }

    /// <summary>
    /// Gets or sets the name of the database context.
    /// </summary>
    public string Context { get; set; }

    /// <summary>
    /// Gets or sets the version of the database context.
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Gets or sets the snapshot code for the current version of the database context.
    /// </summary>
    public string Snapshot { get; set; }
}
