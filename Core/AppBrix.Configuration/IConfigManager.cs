// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// A configuration manager used for getting and storing configurations.
    /// </summary>
    public interface IConfigManager
    {
        /// <summary>
        /// Gets the currently loaded instance of the specified config.
        /// If the config is not loaded, tries to load from source.
        /// </summary>
        /// <typeparam name="T">Type of the configuration.</typeparam>
        /// <returns>The configuration</returns>
        T Get<T>() where T : class, IConfig;

        /// <summary>
        /// Saves one configuration.
        /// </summary>
        /// <param name="config">The configuration.</param>
        void Save(IConfig config);

        /// <summary>
        /// Saves all modified configurations.
        /// </summary>
        void SaveAll();
    }
}
