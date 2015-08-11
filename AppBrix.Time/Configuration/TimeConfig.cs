// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Configuration;
using System.Linq;

namespace AppBrix.Time.Configuration
{
    public class TimeConfig : ConfigurationSection
    {
        /// <summary>
        /// Gets or sets the time kind to be used inside the application.
        /// Supported values: Utc, Local
        /// Changing this property requires module reinitialization.
        /// </summary>
        [ConfigurationProperty("kind", DefaultValue = DateTimeKind.Utc)]
        public DateTimeKind Kind
        {
            get { return (DateTimeKind)this["kind"]; }
            set
            {
                if (value != DateTimeKind.Local && value != DateTimeKind.Utc)
                    throw new ArgumentException("Unspecified is not a supported DateTimeKind.");

                this["kind"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the time string format to be used inside the application.
        /// Changing this property requires module reinitialization.
        /// </summary>
        [ConfigurationProperty("format", DefaultValue = @"yyyy-MM-ddTHH:mm:ss.fffK")]
        public string Format
        {
            get { return (string)this["format"]; }
            set { this["format"] = value; }
        }
    }
}
