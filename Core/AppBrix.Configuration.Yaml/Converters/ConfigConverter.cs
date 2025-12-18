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
	#region Construction
	public ConfigConverter(Lazy<Dictionary<string, Type>> configTypes, IValueSerializer serializer)
	{
		this.configTypes = configTypes;
		this.serializer = serializer;
	}

	public ConfigConverter(Lazy<Dictionary<string, Type>> configTypes, IValueDeserializer deserializer)
	{
		this.configTypes = configTypes;
		this.deserializer = deserializer;
	}
	#endregion

	#region Public and overriden methods
	public bool Accepts(Type type) => type == typeof(Dictionary<Type, IConfig>);

	public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
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
			var configType = this.configTypes.Value.GetValueOrDefault(typeName);

			parser.MoveNext();
			while (parser.Current is not MappingStart && parser.Current is not Scalar)
				parser.MoveNext();

			if (parser.Current is Scalar)
				throw new ArgumentNullException(typeName);

			if (configType is null)
			{
				this.SkipObject(parser);
			}
			else
			{
				var config = this.deserializer!.DeserializeValue(parser, configType, new SerializerState(), this.deserializer)!;
				configs[configType] = (IConfig)config;
			}
		}

		parser.MoveNext();

		return configs;
	}

	public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
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

			this.serializer!.SerializeValue(emitter, config.Value, config.Key);
		}

		emitter.Emit(new MappingEnd());
	}
	#endregion

	#region Private methods
	private void SkipObject(IParser parser)
	{
		var depth = 0;
		do {
			if (parser.Current is MappingStart)
				depth++;
			else if (parser.Current is MappingEnd)
				depth--;

			parser.MoveNext();
		} while (depth > 0);
	}
	#endregion

	#region Private fields and constants
	private readonly Lazy<Dictionary<string, Type>> configTypes;
	private readonly IValueSerializer? serializer;
	private readonly IValueDeserializer? deserializer;
	#endregion
}
