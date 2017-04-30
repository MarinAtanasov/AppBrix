// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;

namespace AppBrix.Caching.Json
{
    internal sealed class JsonCacheSerializer : ICacheSerializer
    {
        #region Public and overriden methods
        public byte[] Serialize(object item)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item, Formatting.None, this.GetSettings()));
        }

        public object Deserialize(byte[] serialized, Type type)
        {
            return JsonConvert.DeserializeObject(Encoding.UTF8.GetString(serialized), type, this.GetSettings());
        }
        #endregion

        #region Private methods
        private JsonSerializerSettings GetSettings()
        {
            if (this.settings == null)
            {
                this.settings = new JsonSerializerSettings
                {
                    Culture = CultureInfo.InvariantCulture,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                };

                this.settings.Converters.Add(new StringEnumConverter());
            }
            return this.settings;
        }
        #endregion

        #region Private fields and constants
        private JsonSerializerSettings settings;
        #endregion
    }
}
