// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Default implementation of the <see cref="IConfigManager"/>.
    /// Uses <see cref="IConfigSerializer"/> to load and store the configuration and
    /// <see cref="IConfigSerializer"/> to serialize and deserialize the configurations.
    /// </summary>
    public sealed class ConfigManager : IConfigManager
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="ConfigManager"/>
        /// </summary>
        /// <param name="provider">The provider which will be used to load and store the configurations.</param>
        /// <param name="serializer">The serializer which will be used to serialize and deserialize the configurations.</param>
        public ConfigManager(IConfigProvider provider, IConfigSerializer serializer)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (serializer == null)
                throw new ArgumentNullException(nameof(serializer));

            this.provider = provider;
            this.serializer = serializer;
        }
        #endregion

        #region Public and overriden methods
        public T GetConfig<T>() where T : class, IConfig
        {
            var type = typeof(T);

            if (!configs.ContainsKey(type))
                configs[type] = this.ReadFromProvider<T>() ?? type.CreateObject<T>();

            return (T)configs[type];
        }

        public void SaveAll()
        {
            foreach (var config in this.configs)
            {
                this.SaveInternal(config.Value);
            }
        }

        public void Save(IConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            this.SaveInternal(config);
        }
        #endregion

        #region Private methods
        private T ReadFromProvider<T>() where T : class, IConfig
        {
            var type = typeof(T);
            var stringed = this.provider.ReadConfig(type);
            this.configStringed[type] = stringed;
            return stringed != null ? (T)this.serializer.Deserialize(type, stringed) : null;
        }

        private void SaveInternal(IConfig config)
        {
            var type = config.GetType();
            var stringed = this.serializer.Serialize(type, config);
            if (!this.configStringed.ContainsKey(type) || stringed != this.configStringed[type])
            {
                this.provider.WriteConfig(type, stringed);
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IDictionary<Type, IConfig> configs = new Dictionary<Type, IConfig>();
        private readonly IDictionary<Type, string> configStringed = new Dictionary<Type, string>();
        private readonly IConfigProvider provider;
        private readonly IConfigSerializer serializer;
        #endregion
    }
}
