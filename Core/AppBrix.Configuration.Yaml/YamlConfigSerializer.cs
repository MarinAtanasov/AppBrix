// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;

namespace AppBrix.Configuration.Yaml
{
    /// <summary>
    /// A configuration serializer that serializes to/from YAML.
    /// </summary>
    public sealed class YamlConfigSerializer : IConfigSerializer
    {
        #region Public and overriden methods
        public string Serialize(IConfig config)
        {
            using (var writer = new StringWriter())
            {
                var serializer = new SerializerBuilder()
                    .EmitDefaults()
                    .WithNamingConvention(new NullNamingConvention())
                    .WithTypeConverter(new VersionConverter())
                    .Build();
                serializer.Serialize(writer, config);
                return writer.ToString();
            }
        }

        public IConfig Deserialize(string config, Type type)
        {
            using (var reader = new StringReader(config))
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(new NullNamingConvention())
                    .IgnoreUnmatchedProperties()
                    .WithTypeConverter(new VersionConverter())
                    .Build();
                return (IConfig)deserializer.Deserialize(reader, type);
            }
        }
        #endregion
    }
}
