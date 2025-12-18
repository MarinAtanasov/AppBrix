// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace AppBrix.Configuration.Yaml.Converters;

internal sealed class VersionConverter : IYamlTypeConverter
{
	public bool Accepts(Type type) => type == typeof(System.Version);

	public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
	{
		var version = ((Scalar)parser.Current!).Value;
		parser.MoveNext();
		return string.IsNullOrEmpty(version) ? null : new System.Version(version);
	}

	public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer) =>
		emitter.Emit(new Scalar(((System.Version?)value)?.ToString() ?? string.Empty));
}
