// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Data.Migration.Data
{
    /// <summary>
    /// Data about a single database migration.
    /// </summary>
    public sealed class MigrationData
    {
        /// <summary>
        /// Creates a new instance of <see cref="MigrationData"/>.
        /// </summary>
        public MigrationData()
        {
            this.Context = string.Empty;
            this.Version = string.Empty;
            this.Migration = string.Empty;
            this.Metadata = string.Empty;
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
        /// Gets or sets the migration code for the current version of the database context.
        /// </summary>
        public string Migration { get; set; }

        /// <summary>
        /// Gets or sets the metadata for the current version of the database context.
        /// </summary>
        public string Metadata { get; set; }
    }
}
