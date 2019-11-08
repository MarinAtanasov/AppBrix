// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Default implementation of the <see cref="IConfigService"/>.
    /// Uses <see cref="IConfigSerializer"/> to load and store the configuration and
    /// <see cref="IConfigSerializer"/> to serialize and deserialize the configurations.
    /// </summary>
    public sealed class ConfigService : IConfigService
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="ConfigService"/>
        /// </summary>
        /// <param name="provider">The provider which will be used to load and store the configurations.</param>
        /// <param name="serializer">The serializer which will be used to serialize and deserialize the configurations.</param>
        public ConfigService(IConfigProvider provider, IConfigSerializer serializer)
        {
            if (provider is null)
                throw new ArgumentNullException(nameof(provider));
            if (serializer is null)
                throw new ArgumentNullException(nameof(serializer));

            this.provider = provider;
            this.serializer = serializer;
        }
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Gets the currently loaded instance of the specified config.
        /// If the config is not loaded, tries to load from source.
        /// </summary>
        /// <param name="type">The type of the configuration.</param>
        /// <returns>The configuration</returns>
        public IConfig Get(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            if (!configs.TryGetValue(type, out var config))
            {
                config = this.ReadFromProvider(type) ?? (IConfig)type.CreateObject();
                configs[type] = config;
            }

            return config;
        }

        /// <summary>
        /// Saves one configuration.
        /// </summary>
        /// <param name="type">The type of the configuration.</param>
        public void Save(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            this.SaveInternal(this.Get(type));
        }

        /// <summary>
        /// Saves all modified configurations.
        /// </summary>
        public void SaveAll()
        {
            foreach (var config in this.configs)
            {
                this.SaveInternal(config.Value);
            }
        }
        #endregion

        #region Private methods
        private IConfig? ReadFromProvider(Type type)
        {
            var stringed = this.provider.ReadConfig(type);
            if (string.IsNullOrEmpty(stringed))
            {
                return null;
            }
            else
            {
                this.configStringed[type] = stringed;
                return this.serializer.Deserialize(stringed, type);
            }
        }

        private void SaveInternal(IConfig config)
        {
            var type = config.GetType();
            var stringed = this.serializer.Serialize(config);

            if (!this.configStringed.TryGetValue(type, out var cached) || cached != stringed)
            {
                this.provider.WriteConfig(stringed, type);
                this.configStringed[type] = stringed;
            }
        }
        #endregion

        #region Private fields and constants
        private readonly Dictionary<Type, IConfig> configs = new Dictionary<Type, IConfig>();
        private readonly Dictionary<Type, string> configStringed = new Dictionary<Type, string>();
        private readonly IConfigProvider provider;
        private readonly IConfigSerializer serializer;
        #endregion
    }
}
