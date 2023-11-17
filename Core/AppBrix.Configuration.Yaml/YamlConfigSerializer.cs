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
    public string Serialize(object config)
    {
        if (config is null)
            throw new ArgumentNullException(nameof(config));

        using var writer = new StringWriter();
        var builder = new SerializerBuilder()
            .EmitDefaults()
            .WithNamingConvention(new NullNamingConvention())
            .WithTypeConverter(new VersionConverter());
        var serializer = builder
            .WithTypeConverter(new ConfigConverter(builder.BuildValueSerializer()))
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
    public object Deserialize(string config, Type type)
    {
        if (string.IsNullOrEmpty(config))
            throw new ArgumentNullException(nameof(config));
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        using var reader = new StringReader(config);
        var builder = new DeserializerBuilder()
            .WithNamingConvention(new NullNamingConvention())
            .IgnoreUnmatchedProperties()
            .WithTypeConverter(new VersionConverter());
        var deserializer = builder
            .WithTypeConverter(new ConfigConverter(builder.BuildValueDeserializer()))
            .Build();
        return deserializer.Deserialize(reader, type);
    }
    #endregion
}
