// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Configuration;
using System.Linq;
using System.Xml.Serialization;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Base element for the configuration elements.
    /// </summary>
    public abstract class ConfigElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the unique key of the config element.
        /// Can be overriden in order to change serialization order.
        /// </summary>
        [ConfigurationProperty("key", IsKey = true, IsRequired = true)]
        public virtual string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }
    }
}
