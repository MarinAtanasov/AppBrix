// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            this.settings = new JsonSerializerOptions();
            this.settings.Converters.Add(new JsonStringEnumConverter());
            this.settings.Converters.Add(new JsonStringTimeSpanConverter());
            this.settings.Converters.Add(new JsonStringVersionConverter());
            this.settings.WriteIndented = true;
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
            if (config is null)
                throw new ArgumentNullException(nameof(config));

            return JsonSerializer.Serialize(config, config.GetType(), this.settings);
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
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            return (IConfig)JsonSerializer.Deserialize(config, type, this.settings);
        }
        #endregion

        #region Private fields and constants
        private JsonSerializerOptions settings;
        #endregion
    }
}
