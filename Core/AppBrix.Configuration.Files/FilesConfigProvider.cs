// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace AppBrix.Configuration.Files;

/// <summary>
/// A file provider which stores each config inside its own file.
/// </summary>
public sealed class FilesConfigProvider : IConfigProvider
{
	#region Construction
	/// <summary>
	/// Creates a new instance of <see cref="FilesConfigProvider"/>.
	/// </summary>
	/// <param name="serializer">The serializer which will be used to serialize and deserialize the configurations.</param>
	/// <param name="directory">The directory where the configuration files are stored.</param>
	/// <param name="fileExtension">The extension to be used when reading/writing config files.</param>
	public FilesConfigProvider(IConfigSerializer serializer, string directory, string fileExtension = FilesConfigProvider.DefaultExtension)
	{
		if (serializer is null)
			throw new ArgumentNullException(nameof(serializer));
		if (string.IsNullOrEmpty(directory))
			throw new ArgumentNullException(nameof(directory));
		if (string.IsNullOrEmpty(fileExtension))
			throw new ArgumentNullException(nameof(fileExtension));

		this.serializer = serializer;
		this.directory = directory;
		this.extension = fileExtension.TrimStart(FilesConfigProvider.ExtensionDot);
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

		var path = this.BuildFilePath(type);
		var content = File.Exists(path) ? File.ReadAllText(path) : string.Empty;
		return string.IsNullOrEmpty(content) ? null : (IConfig)this.serializer.Deserialize(content, type);
	}

	/// <summary>
	/// Saves a configuration.
	/// </summary>
	/// <param name="config">The configuration.</param>
	public void Save(IConfig config)
	{
		if (config is null)
			throw new ArgumentNullException(nameof(config));

		var type = config.GetType();
		this.SaveInternal(type, config);
	}

	/// <summary>
	/// Saves the configurations.
	/// <param name="configs">The configurations.</param>
	/// </summary>
	public void Save(IEnumerable<IConfig> configs)
	{
		if (configs is null)
			throw new ArgumentNullException(nameof(configs));

		foreach (var config in configs)
		{
			this.SaveInternal(config.GetType(), config);
		}
	}
	#endregion

	#region Private methods
	private string BuildFilePath(Type type)
	{
		var typeName = type.Name;
		var fileName = typeName.EndsWith(FilesConfigProvider.ToRemove, StringComparison.OrdinalIgnoreCase) && typeName.Length > FilesConfigProvider.ToRemove.Length ?
			typeName[..^FilesConfigProvider.ToRemove.Length] : typeName;
		return Path.Combine(this.directory, string.Concat(fileName, FilesConfigProvider.ExtensionDot, this.extension));
	}

	private void SaveInternal(Type type, IConfig config)
	{
		if (!Directory.Exists(this.directory))
			Directory.CreateDirectory(this.directory);

		var content = this.serializer.Serialize(config);
		if (!this.configs.TryGetValue(type, out var saved) || saved != content)
		{
			File.WriteAllText(this.BuildFilePath(type), content);
			this.configs[type] = content;
		}
	}
	#endregion

	#region Private fields and constants
	private const string DefaultExtension = "config";
	private const string ToRemove = "config";
	private const char ExtensionDot = '.';
	private readonly Dictionary<Type, string> configs = new Dictionary<Type, string>();
	private readonly IConfigSerializer serializer;
	private readonly string directory;
	private readonly string extension;
	#endregion
}
