// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;

namespace AppBrix.Configuration.Memory;

/// <summary>
/// In-memory implementation of the <see cref="IConfigService"/>.
/// </summary>
public sealed class MemoryConfigService : IConfigService
{
    #region Public and overriden methods
    /// <summary>
    /// Gets the currently loaded instance of the specified config.
    /// If the config is not loaded, creates a new instance.
    /// </summary>
    /// <param name="type">The type of the configuration.</param>
    /// <returns>The configuration.</returns>
    public IConfig Get(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        if (!configs.TryGetValue(type, out var config))
        {
            config = (IConfig)type.CreateObject();
            configs[type] = config;
        }

        return config;
    }

    /// <summary>
    /// Saves one configuration. Does not do anything.
    /// </summary>
    /// <param name="config">The configuration to save.</param>
    public void Save(IConfig config)
    {
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        this.configs[config.GetType()] = config;
    }

    /// <summary>
    /// Saves all modified configurations. Does not do anything.
    /// </summary>
    public void Save()
    {
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<Type, IConfig> configs = new Dictionary<Type, IConfig>();
    #endregion
}
