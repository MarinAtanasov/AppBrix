// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using System;
using System.Linq;

namespace AppBrix.Caching.Memory.Config
{
    /// <summary>
    /// Configuration which sets how the local in-memory cache should behave.
    /// </summary>
    public sealed class MemoryCachingConfig : IConfig
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="MemoryCachingConfig"/>.
        /// </summary>
        public MemoryCachingConfig()
        {
            this.DefaultAbsoluteExpiration = TimeSpan.FromHours(6);
            this.DefaultRollingExpiration = TimeSpan.FromMinutes(30);
            this.ExpirationCheck = TimeSpan.FromSeconds(15);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the default absolute expiration to be used when the user does not specify a value explicitly.
        /// </summary>
        public TimeSpan DefaultAbsoluteExpiration { get; set; }

        /// <summary>
        /// Gets or sets the default rolling expiration to be used when the user does not specify a value explicitly.
        /// </summary>
        public TimeSpan DefaultRollingExpiration { get; set; }

        /// <summary>
        /// Gets or sets how often the local in-memory cache should check for rolling and absolute expiration.
        /// </summary>
        public TimeSpan ExpirationCheck { get; set; }
        #endregion
    }
}
