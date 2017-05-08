// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Serializes and deserializes configurations.
    /// </summary>
    public interface IConfigSerializer
    {
        /// <summary>
        /// Serializes a config to a string.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The string representation of the configuration.</returns>
        string Serialize(IConfig config);

        /// <summary>
        /// Deserializes a string to a configuration.
        /// </summary>
        /// <param name="config">The string representation of the configuration.</param>
        /// <param name="type">The type of the configuration.</param>
        /// <returns>The deserialized configuration.</returns>
        IConfig Deserialize(string config, Type type);
    }
}
