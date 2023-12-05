// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace AppBrix.Configuration;

/// <summary>
/// Default implementation of the <see cref="IConfigService"/>.
/// Uses <see cref="IConfigSerializer"/> to load and store the configuration and
/// <see cref="IConfigSerializer"/> to serialize and deserialize the configurations.
/// </summary>
public class ConfigService : IConfigService
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="ConfigService"/>
    /// </summary>
    /// <param name="provider">The provider which will be used to load and store the configurations.</param>
    public ConfigService(IConfigProvider provider)
    {
        if (provider is null)
            throw new ArgumentNullException(nameof(provider));

        this.provider = provider;
    }
    #endregion

    #region Public and overriden methods
    /// <summary>
    /// Gets the currently loaded instance of the specified config.
    /// If the config is not loaded, tries to load from source.
    /// </summary>
    /// <param name="type">The type of the configuration.</param>
    /// <returns>The configuration</returns>
    public IConfig Get(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        if (!this.configs.TryGetValue(type, out var config))
            this.configs[type] = config = this.provider.Get(type) ?? (IConfig)type.CreateObject();

        return config;
    }

    /// <summary>
    /// Saves all modified configurations.
    /// </summary>
    public void Save() => this.provider.Save(this.configs.Values);

    /// <summary>
    /// Saves a configuration.
    /// </summary>
    /// <param name="config">The configuration to save.</param>
    public void Save(IConfig config)
    {
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        this.configs[config.GetType()] = config;
        this.provider.Save(config);
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<Type, IConfig> configs = new Dictionary<Type, IConfig>();
    private readonly IConfigProvider provider;
    #endregion
}
