// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Configuration;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Configuration section which holds the modules collection.
    /// </summary>
    internal sealed class AppConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("modules")]
        [ConfigurationCollection(typeof(ConfigElementCollection<ModuleConfigElement>))]
        public ConfigElementCollection<ModuleConfigElement> Modules
        {
            get { return (ConfigElementCollection<ModuleConfigElement>)this["modules"]; }
        }
    }
}
