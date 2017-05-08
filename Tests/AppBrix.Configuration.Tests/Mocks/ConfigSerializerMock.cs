// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Configuration.Tests.Mocks
{
    internal sealed class ConfigSerializerMock : IConfigSerializer
    {
        #region Properties
        public IList<KeyValuePair<Type, IConfig>> Serialized { get; } = new List<KeyValuePair<Type, IConfig>>();

        public IList<KeyValuePair<Type, string>> Deserialized { get; } = new List<KeyValuePair<Type, string>>();
        #endregion

        #region Public and overriden methods
        public string Serialize(IConfig config)
        {
            var type = config.GetType();
            this.Serialized.Add(new KeyValuePair<Type, IConfig>(type, config));
            return type.FullName + " Serialized";
        }

        public IConfig Deserialize(string config, Type type)
        {
            this.Deserialized.Add(new KeyValuePair<Type, string>(type, config));
            return null;
        }
        #endregion
    }
}
