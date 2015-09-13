// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Reads and writes configurations from an outside source (file, DB, etc.)
    /// </summary>
    public interface IConfigProvider
    {
        /// <summary>
        /// Reads a configuration by a given configuration type.
        /// </summary>
        /// <param name="type">The type of the configuration to be read.</param>
        /// <returns>The read configuration.</returns>
        string ReadConfig(Type type);

        /// <summary>
        /// Writes a configuraton.
        /// </summary>
        /// <param name="type">The type of the configuration.</param>
        /// <param name="config">The configuration.</param>
        void WriteConfig(Type type, string config);
    }
}
