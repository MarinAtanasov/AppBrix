// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using System;
using System.Linq;

namespace AppBrix
{
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Saves one configuration.
        /// </summary>
        /// <typeparam name="T">The type of the configuraton.</typeparam>
        /// <param name="manager">The configuration manager.</param>
        /// <param name="config">The configuration.</param>
        public static void Save<T>(this IConfigManager manager, T config) where T : class, IConfig
        {
            manager.Save(typeof(T), config);
        }
        
        /// <summary>
        /// Reads a configuration by a given configuration type.
        /// </summary>
        /// <typeparam name="T">The type of the configuration to be read.</typeparam>
        /// <param name="provider">The configuration provider</param>
        /// <returns>The read configuration.</returns>
        public static string ReadConfig<T>(this IConfigProvider provider) where T : class, IConfig
        {
            return provider.ReadConfig(typeof(T));
        }
        
        /// <summary>
        /// Writes a configuraton.
        /// </summary>
        /// <typeparam name="T">The type of the configuration.</typeparam>
        /// <param name="provider">The configuration provider.</param>
        /// <param name="config">The configuration.</param>
        public static void WriteConfig<T>(this IConfigProvider provider, string config) where T : class, IConfig
        {
            provider.WriteConfig(typeof(T), config);
        }
        
        /// <summary>
        /// Serializes a config to a string.
        /// </summary>
        /// <typeparam name="T">The type of the configuration.</typeparam>
        /// <param name="serializer">The configuration serializer</param>
        /// <param name="config">The configuration.</param>
        /// <returns>The string representation of the configuration.</returns>
        public static string Serialize<T>(this IConfigSerializer serializer, T config) where T : class, IConfig
        {
            return serializer.Serialize(typeof(T), config);
        }
        
        /// <summary>
        /// Deserializes a string to a configuration.
        /// </summary>
        /// <typeparam name="T">The type of the configuration.</typeparam>
        /// <param name="serializer"></param>
        /// <param name="config">The string representation of the configuration.</param>
        /// <returns>The deserialized configuration.</returns>
        public static T Deserialize<T>(this IConfigSerializer serializer, string config) where T : class, IConfig
        {
            return (T)serializer.Deserialize(typeof(T), config);
        }
    }
}
