// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Reads and writes configurations from an outside source (file, DB, etc.)
    /// </summary>
    public interface IConfigProvider
    {
        /// <summary>
        /// Reads a configuration by a given configuration type.
        /// Returns null if the configuration does not exist.
        /// </summary>
        /// <typeparam name="T">The type of the configuration to be read.</typeparam>
        /// <returns>The read configuration.</returns>
        string ReadConfig<T>() where T : class, IConfig => this.ReadConfig(typeof(T));

        /// <summary>
        /// Reads a configuration by a given configuration type.
        /// Returns null if the configuration does not exist.
        /// </summary>
        /// <param name="type">The type of the configuration to be read.</param>
        /// <returns>The read configuration.</returns>
        string ReadConfig(Type type);

        /// <summary>
        /// Writes a configuraton.
        /// </summary>
        /// <typeparam name="T">The type of the configuration.</typeparam>
        /// <param name="config">The configuration.</param>
        void WriteConfig<T>(string config) where T : class, IConfig => this.WriteConfig(config, typeof(T));

        /// <summary>
        /// Writes a configuraton.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="type">The type of the configuration.</param>
        void WriteConfig(string config, Type type);
    }
}
