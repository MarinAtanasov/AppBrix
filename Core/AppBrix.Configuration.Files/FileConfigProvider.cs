// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace AppBrix.Configuration.Files;

/// <summary>
/// A file provider which stores all configs inside one file.
/// </summary>
public sealed class FileConfigProvider : IConfigProvider
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="FilesConfigProvider"/>.
    /// </summary>
    /// <param name="serializer">The serializer which will be used to serialize and deserialize the configurations.</param>
    /// <param name="path">The path to the configuration file.</param>
    public FileConfigProvider(IConfigSerializer serializer, string path)
    {
        if (serializer is null)
            throw new ArgumentNullException(nameof(serializer));
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));

        var directory = new FileInfo(path).Directory!.FullName;
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        this.serializer = serializer;
        this.path = path;
    }
    #endregion

    #region Public and overriden methods
    /// <summary>
    /// Gets a configuration by a given configuration type.
    /// Returns null if the configuration does not exist.
    /// </summary>
    /// <param name="type">The type of the configuration to be read.</param>
    /// <returns>The read configuration.</returns>
    public IConfig? Get(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        this.LoadConfigs();
        if (!this.configs.TryGetValue(type, out var config))
            this.configs[type] = null;

        return config;
    }

    /// <summary>
    /// Saves a configuration.
    /// </summary>
    /// <param name="config">The configuration.</param>
    public void Save(IConfig config)
    {
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        this.LoadConfigs();
        this.configs[config.GetType()] = config;
        this.savedConfigs![config.GetType()] = config;
        this.SaveInternal();
    }

    /// <summary>
    /// Saves the configurations.
    /// <param name="configs">The configurations.</param>
    /// </summary>
    public void Save(IEnumerable<IConfig> configs)
    {
        if (configs is null)
            throw new ArgumentNullException(nameof(configs));

        this.LoadConfigs();
        foreach (var config in configs)
        {
            this.configs[config.GetType()] = config;
            this.savedConfigs![config.GetType()] = config;
        }
        this.SaveInternal();
    }
    #endregion

    #region Private methods
    private Dictionary<Type, IConfig> GetConfigs(string content) =>
        this.serializer.Deserialize<Dictionary<Type, IConfig>>(content);

    private void LoadConfigs()
    {
        if (this.savedConfigs is not null)
            return;

        var file = new FileInfo(this.path);
        if (!file.Exists)
        {
            this.savedConfigs = new Dictionary<Type, IConfig>();
            return;
        }

        var content = File.ReadAllText(file.FullName);
        if (string.IsNullOrEmpty(content))
        {
            this.savedConfigs = new Dictionary<Type, IConfig>();
            return;
        }

        this.configs = this.GetConfigs(content)!;
        this.savedConfigs = this.GetConfigs(content);
        this.savedContent = content;
    }

    private void SaveInternal()
    {
        var content = this.serializer.Serialize(this.savedConfigs!);
        if (this.savedContent != content)
        {
            File.WriteAllText(this.path, content);
            this.savedConfigs = this.GetConfigs(content);
            this.savedContent = content;
        }
    }
    #endregion

    #region Private fields and constants
    private readonly IConfigSerializer serializer;
    private readonly string path;
    private Dictionary<Type, IConfig?> configs = new Dictionary<Type, IConfig?>();
    private Dictionary<Type, IConfig>? savedConfigs;
    private string savedContent = string.Empty;
    #endregion
}
