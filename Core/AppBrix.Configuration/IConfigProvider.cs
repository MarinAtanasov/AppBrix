// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace AppBrix.Configuration;

/// <summary>
/// Reads and writes configurations from an outside source (file, DB, etc.)
/// </summary>
public interface IConfigProvider
{
    /// <summary>
    /// Gets a configuration by a given configuration type.
    /// Returns null if the configuration does not exist.
    /// </summary>
    /// <param name="type">The type of the configuration to be read.</param>
    /// <returns>The read configuration.</returns>
    IConfig? Get(Type type);

    /// <summary>
    /// Saves a configuration.
    /// </summary>
    /// <param name="config">The configuration.</param>
    void Save(IConfig config);

    /// <summary>
    /// Saves the configurations.
    /// <param name="configs">The configurations.</param>
    /// </summary>
    void Save(IEnumerable<IConfig> configs);
}
