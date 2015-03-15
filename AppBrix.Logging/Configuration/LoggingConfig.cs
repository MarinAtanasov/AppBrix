// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Logging.Entries;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AppBrix.Logging.Configuration
{
    public sealed class LoggingConfig : ConfigurationSection
    {
        /// <summary>
        /// Gets or sets whether to use an asychronous logger.
        /// </summary>
        [ConfigurationProperty("async", DefaultValue = true)]
        public bool Async
        {
            get { return (bool)this["async"]; }
            set { this["async"] = value; }
        }

        /// <summary>
        /// Gets or sets the minimal level in which the log entry events should be raised.
        /// </summary>
        [ConfigurationProperty("level")]
        public LogLevel LogLevel
        {
            get { return (LogLevel)this["level"]; }
            set { this["level"] = value; }
        }
    }
}
