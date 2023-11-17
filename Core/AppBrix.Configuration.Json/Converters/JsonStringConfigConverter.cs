// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppBrix.Configuration.Json.Converters;

internal sealed class JsonStringConfigConverter : JsonConverter<Dictionary<Type, IConfig>?>
{
    public override Dictionary<Type, IConfig>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        var result = new Dictionary<Type, IConfig>();

        reader.Read();  // Start object

        while (reader.TokenType != JsonTokenType.EndObject)
        {
            while (reader.TokenType != JsonTokenType.PropertyName)
                reader.Read();

            var typeName = reader.GetString();
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.ExportedTypes)
                .Where(x => x.IsClass && !x.IsAbstract && x.Name == typeName)
                .First(x => typeof(IConfig).IsAssignableFrom(x));

            while (reader.TokenType != JsonTokenType.StartObject && reader.TokenType != JsonTokenType.Null)
                reader.Read();

            if (reader.TokenType == JsonTokenType.Null)
                throw new ArgumentNullException(typeName);

            var converter = options.GetConverter(type);
            var method = converter.GetType().GetMethod(nameof(this.Read), BindingFlags.Public | BindingFlags.Instance)!;
            var config = method.CreateDelegate<ReadDelegate>(converter).Invoke(ref reader, type, options);

            result[type] = config;

            reader.Read();
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<Type, IConfig>? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartObject();

        foreach (var config in value.OrderBy(x => x.Key.Name))
        {
            writer.WritePropertyName(config.Key.Name);

            var converter = options.GetConverter(config.Key);
            var method = converter.GetType().GetMethod(nameof(this.Write), BindingFlags.Public | BindingFlags.Instance)!;

            method.Invoke(converter, new object[] { writer, config.Value, options });
        }

        writer.WriteEndObject();
    }

    private delegate IConfig ReadDelegate(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);
}
