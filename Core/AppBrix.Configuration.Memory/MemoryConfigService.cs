// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Configuration.Memory
{
    /// <summary>
    /// In-memory implementation of the <see cref="IConfigService"/>.
    /// </summary>
    public sealed class MemoryConfigService : IConfigService
    {
        #region Public and overriden methods
        public IConfig Get(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!configs.TryGetValue(type, out var config))
            {
                config = (IConfig)type.CreateObject();
                configs[type] = config;
            }

            return config;
        }

        public void Save(Type type)
        {
        }

        public void SaveAll()
        {
        }
        #endregion

        #region Private fields and constants
        private readonly Dictionary<Type, IConfig> configs = new Dictionary<Type, IConfig>();
        #endregion
    }
}
