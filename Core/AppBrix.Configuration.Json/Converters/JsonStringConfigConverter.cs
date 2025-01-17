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
    public JsonStringConfigConverter(Lazy<Dictionary<string, Type>> configTypes)
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
            var type = this.configTypes.Value.GetValueOrDefault(typeName);

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
                if (!this.readMethods.TryGetValue(type, out var method))
                {
                    var methodInfo = converter.GetType().GetMethod(nameof(this.Read), BindingFlags.Public | BindingFlags.Instance)!;
                    this.readMethods[type] = method = methodInfo.CreateDelegate<ReadDelegate>(converter);
                }

                result[type] = method.Invoke(ref reader, type, options);
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
            if (!this.writeMethods.TryGetValue(config.Key, out var method))
            {
                var methodInfo = converter.GetType().GetMethod(nameof(this.Write), BindingFlags.Public | BindingFlags.Instance)!;
                this.writeMethods[config.Key] = method = MethodInvoker.Create(methodInfo);
            }

            method.Invoke(converter, writer, config.Value, options);
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
    private readonly Lazy<Dictionary<string, Type>> configTypes;
    private readonly Dictionary<Type, ReadDelegate> readMethods = new Dictionary<Type, ReadDelegate>();
    private readonly Dictionary<Type, MethodInvoker> writeMethods = new Dictionary<Type, MethodInvoker>();
    #endregion

    #region Private classes
    private delegate IConfig ReadDelegate(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);
    #endregion
}
