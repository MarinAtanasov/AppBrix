// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Yaml.Converters;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AppBrix.Configuration.Yaml;

/// <summary>
/// A configuration serializer that serializes to/from YAML.
/// </summary>
public sealed class YamlConfigSerializer : IConfigSerializer
{
    #region Construction
    /// <summary>
    /// Creates a new instance of <see cref="YamlConfigSerializer"/>.
    /// </summary>
    public YamlConfigSerializer()
    {
        var configTypes = Assembly.GetCallingAssembly()
            .GetReferencedAssemblies(true)
            .SelectMany(x => x.ExportedTypes)
            .Where(x => x.IsClass && !x.IsAbstract)
            .Where(typeof(IConfig).IsAssignableFrom)
            .ToDictionary(x => x.Name);

        var serializerBuilder = new SerializerBuilder()
            .EmitDefaults()
            .WithNamingConvention(new NullNamingConvention())
            .WithTypeConverter(new VersionConverter());
        this.serializer = serializerBuilder
            .WithTypeConverter(new ConfigConverter(configTypes, serializerBuilder.BuildValueSerializer()))
            .Build();

        var deserializerBuilder = new DeserializerBuilder()
            .WithNamingConvention(new NullNamingConvention())
            .IgnoreUnmatchedProperties()
            .WithTypeConverter(new VersionConverter());
        this.deserializer = deserializerBuilder
            .WithTypeConverter(new ConfigConverter(configTypes, deserializerBuilder.BuildValueDeserializer()))
            .Build();
    }
    #endregion

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
        this.serializer.Serialize(writer, config);
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
        return this.deserializer.Deserialize(reader, type);
    }
    #endregion

    #region Private fields and constants
    private readonly Serializer serializer;
    private readonly Deserializer deserializer;
    #endregion
}
