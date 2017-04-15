// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace AppBrix.Configuration.Json
{
    /// <summary>
    /// A configuration serializer that serializes to/from JSON.
    /// </summary>
    public sealed class JsonConfigSerializer : IConfigSerializer
    {
        #region Public and overriden methods
        public string Serialize(IConfig config, Type type)
        {
            return JsonConvert.SerializeObject(config, Formatting.Indented, this.GetSettings());
        }

        public IConfig Deserialize(string config, Type type)
        {
            return (IConfig)JsonConvert.DeserializeObject(config, type, this.GetSettings());
        }
        #endregion

        #region Private methods
        private JsonSerializerSettings GetSettings()
        {
            if (this.settings == null)
            {
                this.settings = new JsonSerializerSettings()
                {
                    Culture = CultureInfo.InvariantCulture,
                    DefaultValueHandling = DefaultValueHandling.Include,
                    NullValueHandling = NullValueHandling.Ignore,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                };

                this.settings.Converters.Add(new IsoDateTimeConverter());
                this.settings.Converters.Add(new StringEnumConverter());
                this.settings.Converters.Add(new VersionConverter());
            }
            return this.settings;
        }
        #endregion

        #region Private fields and constants
        private JsonSerializerSettings settings;
        #endregion
    }
}
