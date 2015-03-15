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
    }
}
