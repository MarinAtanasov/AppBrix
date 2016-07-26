// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using System;
using System.Linq;

namespace AppBrix.Caching.Memory
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
            this.ExpirationCheck = TimeSpan.FromSeconds(1);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets how often the local in-memory cache should check for rolling and absolute expiration.
        /// </summary>
        public TimeSpan ExpirationCheck { get; set; }
        #endregion
    }
}
