// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Configuration;
using System.Linq;

namespace AppBrix.Logging.File.Configuration
{
    public sealed class FileLoggerConfig : ConfigurationSection
    {
        /// <summary>
        /// Gets or sets the path to the file where to keep the log.
        /// </summary>
        [ConfigurationProperty("path", DefaultValue = "Log.log")]
        public string Path
        {
            get { return (string)this["path"]; }
            set { this["path"] = value; }
        }
    }
}
