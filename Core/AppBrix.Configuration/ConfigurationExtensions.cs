// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for AppBrix configurations.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Gets the currently loaded instance of the specified config.
        /// If the config is not loaded, tries to load from source.
        /// </summary>
        /// <typeparam name="T">Type of the configuration.</typeparam>
        /// <returns>The configuration</returns>
        public static T Get<T>(this IConfigService service) where T : class, IConfig => (T)service.Get(typeof(T));

        /// <summary>
        /// Saves one cached configuration.
        /// </summary>
        /// <typeparam name="T">The type of the configuraton.</typeparam>
        /// <param name="service">The configuration service.</param>
        public static void Save<T>(this IConfigService service) where T : class, IConfig => service.Save(typeof(T));
        
        /// <summary>
        /// Reads a configuration by a given configuration type.
        /// </summary>
        /// <typeparam name="T">The type of the configuration to be read.</typeparam>
        /// <param name="provider">The configuration provider</param>
        /// <returns>The read configuration.</returns>
        public static string ReadConfig<T>(this IConfigProvider provider) where T : class, IConfig => provider.ReadConfig(typeof(T));
        
        /// <summary>
        /// Writes a configuraton.
        /// </summary>
        /// <typeparam name="T">The type of the configuration.</typeparam>
        /// <param name="provider">The configuration provider.</param>
        /// <param name="config">The configuration.</param>
        public static void WriteConfig<T>(this IConfigProvider provider, string config) where T : class, IConfig => provider.WriteConfig(config, typeof(T));

        /// <summary>
        /// Deserializes a string to a configuration.
        /// </summary>
        /// <typeparam name="T">The type of the configuration.</typeparam>
        /// <param name="serializer"></param>
        /// <param name="config">The string representation of the configuration.</param>
        /// <returns>The deserialized configuration.</returns>
        public static T Deserialize<T>(this IConfigSerializer serializer, string config) where T : class, IConfig => (T)serializer.Deserialize(config, typeof(T));
    }
}
