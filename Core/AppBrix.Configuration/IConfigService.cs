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
        /// <typeparam name="T">Type of the configuration.</typeparam>
        /// <returns>The configuration</returns>
        public T Get<T>() where T : class, IConfig => (T)this.Get(typeof(T));

        /// <summary>
        /// Gets the currently loaded instance of the specified config.
        /// If the config is not loaded, tries to load from source.
        /// </summary>
        /// <param name="type">The type of the configuration.</param>
        /// <returns>The configuration</returns>
        IConfig Get(Type type);

        /// <summary>
        /// Saves one cached configuration.
        /// </summary>
        /// <typeparam name="T">The type of the configuration.</typeparam>
        public void Save<T>() where T : class, IConfig => this.Save(typeof(T));

        /// <summary>
        /// Saves one configuration.
        /// </summary>
        /// <param name="type">The type of the configuration.</param>
        public void Save(Type type) => this.Save(this.Get(type));

        /// <summary>
        /// Saves one configuration.
        /// </summary>
        /// <param name="config">The configuration to save.</param>
        void Save(IConfig config);

        /// <summary>
        /// Saves all modified configurations.
        /// </summary>
        void SaveAll();
    }
}
