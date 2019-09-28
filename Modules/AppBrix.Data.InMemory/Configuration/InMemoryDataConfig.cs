// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;

namespace AppBrix.Data.InMemory.Configuration
{
    /// <summary>
    /// Configures the InMemory data provider.
    /// </summary>
    public sealed class InMemoryDataConfig : IConfig
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="InMemoryDataConfig"/> with default property values.
        /// </summary>
        public InMemoryDataConfig()
        {
            this.ConnectionString = "AppBrix";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the connection string to the InMemory database instance.
        /// </summary>
        public string ConnectionString { get; set; }
        #endregion
    }
}
