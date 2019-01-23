// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using System;
using System.Linq;

namespace AppBrix.Data.Migration.Configuration
{
    /// <summary>
    /// Configures the Migration data module.
    /// </summary>
    public sealed class MigrationDataConfig : IConfig
    {
        #region Properties
        /// <summary>
        /// Gets or sets the entry assembly for the application. Leave blank to get from the runtime.
        /// This is used during creation of migration scripts in order to find necessary assembly references.
        /// </summary>
        public string EntryAssembly { get; set; }
        #endregion
    }
}
