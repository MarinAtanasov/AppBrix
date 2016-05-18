// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace AppBrix.Configuration.Yaml
{
    internal class VersionConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type == typeof(System.Version);
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var version = ((Scalar)parser.Current).Value;
            parser.MoveNext();
            return string.IsNullOrEmpty(version) ? null : new System.Version(version);
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            var version = (System.Version)value;
            emitter.Emit(new Scalar(version?.ToString(4) ?? string.Empty));
        }
    }
}
