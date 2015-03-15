// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Configuration;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// In-memory application configuration manager.
    /// Can be used when a configuration file is not needed.
    /// </summary>
    public sealed class MemoryAppConfig : IAppConfig
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of the in-memory app config manager.
        /// </summary>
        public MemoryAppConfig()
        {
            this.Properties = new KeyValueConfigurationCollection();
            this.Modules = new ConfigElementCollection<ModuleConfigElement>();
        }
        #endregion

        #region IAppConfigManager implementation
        public KeyValueConfigurationCollection Properties { get; private set; }

        public ConfigElementCollection<ModuleConfigElement> Modules { get; private set; }

        public void Save()
        {
            // Do nothing.
        }
        #endregion
    }
}
