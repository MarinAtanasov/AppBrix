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
    #region Construction
    public JsonStringConfigConverter(Dictionary<string, Type> configTypes)
    {
        this.configTypes = configTypes;
    }
    #endregion

    #region Public and overriden methods
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

            var typeName = reader.GetString()!;
            var type = this.configTypes.TryGetValue(typeName, out var configType) ? configType : null;

            while (reader.TokenType != JsonTokenType.StartObject && reader.TokenType != JsonTokenType.Null)
                reader.Read();

            if (reader.TokenType == JsonTokenType.Null)
                throw new ArgumentNullException(typeName);

            if (type is null)
            {
                this.SkipObject(ref reader);
            }
            else
            {
                var converter = options.GetConverter(type);
                var method = converter.GetType().GetMethod(nameof(this.Read), BindingFlags.Public | BindingFlags.Instance)!;
                var config = method.CreateDelegate<ReadDelegate>(converter).Invoke(ref reader, type, options);
                result[type] = config;
            }

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
    #endregion

    #region Private methods
    private void SkipObject(ref Utf8JsonReader reader)
    {
        var depth = 1;
        while (depth > 0)
        {
            reader.Read();

            if (reader.TokenType == JsonTokenType.StartObject)
                depth++;
            else if (reader.TokenType == JsonTokenType.EndObject)
                depth--;
        }
    }
    #endregion

    #region Private fields and constants
    private readonly Dictionary<string, Type> configTypes;
    #endregion

    #region Private classes
    private delegate IConfig ReadDelegate(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);
    #endregion
}
