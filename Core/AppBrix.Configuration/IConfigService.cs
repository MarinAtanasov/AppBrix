// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;

namespace AppBrix.Configuration
{
    /// <summary>
    /// A configuration service used for getting and storing configurations.
    /// </summary>
    public interface IConfigService
    {
        /// <summary>
        /// Gets the currently loaded instance of the specified config.
        /// If the config is not loaded, tries to load from source.
        /// </summary>
        /// <param name="type">The type of the configuration.</param>
        /// <returns>The configuration</returns>
        IConfig Get(Type type);

        /// <summary>
        /// Saves one configuration.
        /// </summary>
        /// <param name="type">The type of the configuration.</param>
        void Save(Type type);

        /// <summary>
        /// Saves all modified configurations.
        /// </summary>
        void SaveAll();
    }
}
