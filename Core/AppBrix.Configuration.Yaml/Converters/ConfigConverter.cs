// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Utilities;

namespace AppBrix.Configuration.Yaml.Converters;

internal sealed class ConfigConverter : IYamlTypeConverter
{
    public ConfigConverter(IValueSerializer serializer)
    {
        this.Serializer = serializer;
    }
    
    public ConfigConverter(IValueDeserializer deserializer)
    {
        this.Deserializer = deserializer;
    }

    public IValueSerializer? Serializer { get; }

    public IValueDeserializer? Deserializer { get; }

    public bool Accepts(Type type) => type == typeof(Dictionary<Type, IConfig>);

    public object? ReadYaml(IParser parser, Type type)
    {
        if (parser.Current is not MappingStart)
        {
            parser.MoveNext();
            return null;
        }

        parser.MoveNext();

        var configs = new Dictionary<Type, IConfig>();
        while (parser.Current is not MappingEnd)
        {
            while (parser.Current is not Scalar)
                parser.MoveNext();

            var typeName = ((Scalar)parser.Current).Value;
            var configType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.ExportedTypes)
                .Where(x => x.IsClass && !x.IsAbstract && x.Name == typeName)
                .First(x => typeof(IConfig).IsAssignableFrom(x));

            while (parser.Current is not MappingStart)
                parser.MoveNext();

            var config = this.Deserializer!.DeserializeValue(parser, configType, new SerializerState(), this.Deserializer);
            configs[configType] = (IConfig)config;
        }

        parser.MoveNext();

        return configs;
    }

    public void WriteYaml(IEmitter emitter, object value, Type type)
    {
        if (value is null)
        {
            emitter.Emit(new Scalar(string.Empty));
            return;
        }

        emitter.Emit(new MappingStart());
        
        var configs = (Dictionary<Type, IConfig>)value;
        foreach (var config in configs.OrderBy(x => x.Key.Name))
        {
            emitter.Emit(new Scalar(config.Key.Name));
            if (config.Value is null)
            {
                emitter.Emit(new Scalar(string.Empty));
                continue;
            }

            this.Serializer!.SerializeValue(emitter, config.Value, config.Key);
        }

        emitter.Emit(new MappingEnd());
    }
}
