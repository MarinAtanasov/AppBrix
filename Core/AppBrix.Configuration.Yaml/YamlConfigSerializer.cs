// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Yaml.Converters;
using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AppBrix.Configuration.Yaml;

/// <summary>
/// A configuration serializer that serializes to/from YAML.
/// </summary>
public sealed class YamlConfigSerializer : IConfigSerializer
{
    #region Public and overriden methods
    /// <summary>
    /// Serializes a config to YAML.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <returns>The YAML representation of the configuration.</returns>
    public string Serialize(IConfig config)
    {
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        using var writer = new StringWriter();
        var serializer = new SerializerBuilder()
            .EmitDefaults()
            .WithNamingConvention(new NullNamingConvention())
            .WithTypeConverter(new VersionConverter())
            .Build();
        serializer.Serialize(writer, config);
        return writer.ToString();
    }

    /// <summary>
    /// Deserializes a YAML string to a configuration.
    /// </summary>
    /// <param name="config">The YAML representation of the configuration.</param>
    /// <param name="type">The type of the configuration.</param>
    /// <returns>The deserialized configuration.</returns>
    public IConfig Deserialize(string config, Type type)
    {
        if (string.IsNullOrEmpty(config))
            throw new ArgumentNullException(nameof(config));
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        using var reader = new StringReader(config);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(new NullNamingConvention())
            .IgnoreUnmatchedProperties()
            .WithTypeConverter(new VersionConverter())
            .Build();
        return (IConfig)deserializer.Deserialize(reader, type);
    }
    #endregion
}
