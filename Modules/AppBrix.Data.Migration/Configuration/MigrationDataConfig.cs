// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;

namespace AppBrix.Data.Migration.Configuration
{
    /// <summary>
    /// Configures the Migration data module.
    /// </summary>
    public sealed class MigrationDataConfig : IConfig
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="MigrationDataConfig"/>.
        /// </summary>
        public MigrationDataConfig()
        {
            this.EntryAssembly = string.Empty;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the entry assembly for the application. Leave blank to get from the runtime.
        /// This is used during creation of migration scripts in order to find necessary assembly references.
        /// </summary>
        public string EntryAssembly { get; set; }
        #endregion
    }
}
