// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Configuration.Tests.Mocks
{
    internal class ConfigSerializerMock : IConfigSerializer
    {
        #region Properties
        public IList<KeyValuePair<Type, IConfig>> Serialized { get; } = new List<KeyValuePair<Type, IConfig>>();

        public IList<KeyValuePair<Type, string>> Deserialized { get; } = new List<KeyValuePair<Type, string>>();
        #endregion

        #region Public and overriden methods
        public string Serialize(Type type, IConfig config)
        {
            this.Serialized.Add(new KeyValuePair<Type, IConfig>(type, config));
            return type.FullName + " Serialized";
        }

        public IConfig Deserialize(Type type, string config)
        {
            this.Deserialized.Add(new KeyValuePair<Type, string>(type, config));
            return null;
        }
        #endregion
    }
}
