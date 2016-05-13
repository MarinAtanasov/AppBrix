// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.IO;
using System.Linq;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AppBrix.Configuration.Yaml
{
    /// <summary>
    /// A configuration serializer that serializes to/from YAML.
    /// </summary>
    public class YamlConfigSerializer : IConfigSerializer
    {
        #region Public and overriden methods
        public string Serialize(Type type, IConfig config)
        {
            using (var writer = new StringWriter())
            {
                var serializer = new Serializer(SerializationOptions.EmitDefaults, new NullNamingConvention());
                serializer.Serialize(writer, config);
                return writer.ToString();
            }
        }

        public IConfig Deserialize(Type type, string config)
        {
            using (var reader = new StringReader(config))
            {
                var deserializer = new Deserializer(namingConvention: new NullNamingConvention(), ignoreUnmatched: true);
                return (IConfig)deserializer.Deserialize(reader, type);
            }
        }
        #endregion
    }
}
