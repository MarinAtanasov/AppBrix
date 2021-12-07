// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Time.Services;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppBrix.Web.Client.Impl;

internal sealed class JsonStringDateTimeConverter : JsonConverter<DateTime?>
{
    public JsonStringDateTimeConverter(ITimeService timeService)
    {
        this.timeService = timeService;
    }

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var stringed = reader.GetString();
        if (string.IsNullOrEmpty(stringed))
            return null;
        else
            return this.timeService.ToDateTime(stringed);
    }

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
            writer.WriteNullValue();
        else
            writer.WriteStringValue(this.timeService.ToString(value.Value));
    }

    private readonly ITimeService timeService;
}
