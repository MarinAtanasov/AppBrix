// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppBrix.Caching.Tests.Mocks
{
    internal sealed class JsonCacheSerializer : ICacheSerializer
    {
        #region Construction
        public JsonCacheSerializer()
        {
            this.settings = new JsonSerializerOptions();
            this.settings.Converters.Add(new JsonStringEnumConverter());
        }
        #endregion

        #region Public and overriden methods
        public byte[] Serialize(object item) => JsonSerializer.SerializeToUtf8Bytes(item, item.GetType(), this.settings);

        public object Deserialize(byte[] serialized, Type type) => JsonSerializer.Deserialize(serialized, type, this.settings);
        #endregion

        #region Private fields and constants
        private readonly JsonSerializerOptions settings;
        #endregion
    }
}
