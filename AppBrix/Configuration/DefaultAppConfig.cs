// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace AppBrix.Configuration
{
    internal sealed class DefaultAppConfig : IAppConfig
    {
        #region Construction
        /// <summary>
        /// Creates a default app config using the entry assembly's configuration.
        /// </summary>
        public DefaultAppConfig() : this(null)
        {
        }

        /// <summary>
        /// Creates a default app config using the specified configuration.
        /// </summary>
        /// <param name="path">The path to the configuration.</param>
        public DefaultAppConfig(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                this.config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            }
            else if (path.EndsWith(DefaultAppConfig.ConfigExtension))
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

        #region IAppConfig implementation
        public KeyValueConfigurationCollection Properties
        {
            get { return config.AppSettings.Settings; }
        }

        public ConfigElementCollection<ModuleConfigElement> Modules
        {
            get
            {
                var section = (AppConfigSection)this.config.GetSection(DefaultAppConfig.AppConfigElementName);
                if (section == null)
                {
                    section = typeof(AppConfigSection).CreateObject<AppConfigSection>();
                    this.config.Sections.Add(DefaultAppConfig.AppConfigElementName, section);
                }
                return section.Modules;
            }
        }

        public void Save()
        {
            var saveMode = this.Properties[DefaultAppConfig.SaveModePropertyName];
            this.config.Save(saveMode != null ? saveMode.Value.ToEnum<ConfigurationSaveMode>() : ConfigurationSaveMode.Minimal);
        }
        #endregion

        #region Private fields and constants
        private const string ConfigExtension = ".config";
        private const string AppConfigElementName = "AppConfig";
        private const string SaveModePropertyName = "ConfigSaveMode";
        private readonly System.Configuration.Configuration config;
        #endregion
    }
}
