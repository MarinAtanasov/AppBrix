// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;

namespace AppBrix.Configuration;

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
    string Serialize(object config);

    /// <summary>
    /// Deserializes a string to a configuration.
    /// </summary>
    /// <typeparam name="T">The type of the configuration.</typeparam>
    /// <param name="config">The string representation of the configuration.</param>
    /// <returns>The deserialized configuration.</returns>
    T Deserialize<T>(string config) => (T)this.Deserialize(config, typeof(T));

    /// <summary>
    /// Deserializes a string to a configuration.
    /// </summary>
    /// <param name="config">The string representation of the configuration.</param>
    /// <param name="type">The type of the configuration.</param>
    /// <returns>The deserialized configuration.</returns>
    object Deserialize(string config, Type type);
}
