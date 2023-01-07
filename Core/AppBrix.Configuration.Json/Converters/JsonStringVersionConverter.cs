// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppBrix.Configuration.Json.Converters;

internal sealed class JsonStringVersionConverter : JsonConverter<Version?>
{
    public override Version? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringed = reader.GetString();
        return string.IsNullOrEmpty(stringed) ? null : Version.Parse(stringed);
    }

    public override void Write(Utf8JsonWriter writer, Version? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(value.ToString());
    }
}
