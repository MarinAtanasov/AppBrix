// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Time;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppBrix.Web.Client.Impl
{
    internal sealed class JsonStringDateTimeConverter : JsonConverter<DateTime>
    {
        public JsonStringDateTimeConverter(ITimeService timeService)
        {
            this.timeService = timeService;
        }

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => this.timeService.ToDateTime(reader.GetString());

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(this.timeService.ToString(value));

        private ITimeService timeService;
    }
}
