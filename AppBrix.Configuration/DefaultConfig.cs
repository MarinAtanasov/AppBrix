// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace AppBrix.Configuration
{
    internal sealed class DefaultConfig : IConfig, IApplicationLifecycle
    {
        #region Construction
        /// <summary>
        /// Creates a default config using the entry assembly's configuration.
        /// </summary>
        public DefaultConfig() : this(null)
        {
        }

        /// <summary>
        /// Creates a default config using the specified configuration.
        /// </summary>
        /// <param name="path">The path to the configuration.</param>
        public DefaultConfig(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                this.config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            }
            else if (path.EndsWith(DefaultConfig.ConfigExtension))
            {
                this.config = ConfigurationManager.OpenMappedExeConfiguration(
                    new ExeConfigurationFileMap() { ExeConfigFilename = path, },
                    ConfigurationUserLevel.None);
            }
            else
            {
                this.config = ConfigurationManager.OpenExeConfiguration(path);
            }
        }
        #endregion

        #region IApplicationLifecycle implementation
        public void Initialize(IInitializeContext context)
        {
            this.app = context.App;
        }

        public void Uninitialize()
        {
            this.ClearCache();
            this.Save();
            this.app = null;
        }
        #endregion

        #region IConfig implementation
        public T GetSection<T>() where T : ConfigurationSection
        {
            var section = this.GetFromCache<T>();
            if (section == null)
            {
                section = this.GetFromConfig<T>();
                this.SaveInCache<T>(section);
            }
            return section;
        }
        #endregion

        #region Private methods
        private T GetFromConfig<T>() where T : ConfigurationSection
        {
            var section = (T)this.config.GetSection(typeof(T).Name);
            if (section == null)
            {
                section = typeof(T).CreateObject<T>();
                this.config.Sections.Add(typeof(T).Name, section);
            }
            return section;
        }

        private T GetFromCache<T>() where T : ConfigurationSection
        {
            return (T)(this.cache.ContainsKey(typeof(T)) ? this.cache[typeof(T)] : null);
        }

        private void SaveInCache<T>(T section) where T : ConfigurationSection
        {
            this.cache[typeof(T)] = section;
        }

        private void ClearCache()
        {
            this.cache.Clear();
        }
        #endregion

        #region Private fields and constants
        private void Save()
        {
            var mode = this.app.AppConfig.Properties[ConfigModule.ConfigSaveModeProperty];
            this.config.Save(mode != null ? mode.Value.ToEnum<ConfigurationSaveMode>() : ConfigurationSaveMode.Minimal);
        }
        #endregion

        #region Private fields and constants
        private const string ConfigExtension = ".config";
        private readonly System.Configuration.Configuration config;
        private IDictionary<Type, object> cache = new Dictionary<Type, object>();
        private IApp app;
        #endregion
    }
}
