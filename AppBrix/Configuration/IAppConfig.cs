// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Configuration;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Defines an application configuration manager.
    /// </summary>
    public interface IAppConfig
    {
        /// <summary>
        /// Gets the configuration properties.
        /// </summary>
        KeyValueConfigurationCollection Properties { get; }

        /// <summary>
        /// Gets the application modules.
        /// </summary>
        ConfigElementCollection<ModuleConfigElement> Modules { get; }

        /// <summary>
        /// Saves any changes done to the configuration.
        /// </summary>
        void Save();
    }
}
