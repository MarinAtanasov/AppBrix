// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Globalization;

namespace AppBrix.Configuration.Json
{
    /// <summary>
    /// A configuration serializer that serializes to/from JSON.
    /// </summary>
    public sealed class JsonConfigSerializer : IConfigSerializer
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="JsonConfigSerializer"/>.
        /// </summary>
        public JsonConfigSerializer()
        {
            this.settings = new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture,
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                DateFormatString = @"yyyy-MM-ddTHH:mm:ss.fffK"
            };

            this.settings.Converters.Add(new IsoDateTimeConverter());
            this.settings.Converters.Add(new StringEnumConverter());
            this.settings.Converters.Add(new VersionConverter());
        }
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Serializes a config to JSON.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>The JSON representation of the configuration.</returns>
        public string Serialize(IConfig config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            return JsonConvert.SerializeObject(config, Formatting.Indented, this.settings);
        }

        /// <summary>
        /// Deserializes a JSON string to a configuration.
        /// </summary>
        /// <param name="config">The JSON representation of the configuration.</param>
        /// <param name="type">The type of the configuration.</param>
        /// <returns>The deserialized configuration.</returns>
        public IConfig Deserialize(string config, Type type)
        {
            if (string.IsNullOrEmpty(config))
                throw new ArgumentNullException(nameof(config));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return (IConfig)JsonConvert.DeserializeObject(config, type, this.settings);
        }
        #endregion

        #region Private fields and constants
        private JsonSerializerSettings settings;
        #endregion
    }
}
