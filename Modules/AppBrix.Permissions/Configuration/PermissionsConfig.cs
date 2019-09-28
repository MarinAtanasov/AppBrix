// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;

namespace AppBrix.Permissions.Configuration
{
    /// <summary>
    /// Configuration which sets how the permissions should behave.
    /// </summary>
    public sealed class PermissionsConfig : IConfig
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="PermissionsConfig"/>.
        /// </summary>
        public PermissionsConfig()
        {
            this.EnableCaching = true;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether parent permissions should be cached or resolved at runtime.
        /// </summary>
        public bool EnableCaching { get; set; }
        #endregion
    }
}
