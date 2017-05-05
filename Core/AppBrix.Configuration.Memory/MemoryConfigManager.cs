// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Configuration.Memory
{
    /// <summary>
    /// In-memory implementation of the <see cref="IConfigManager"/>.
    /// </summary>
    public sealed class MemoryConfigManager : IConfigManager
    {
        #region Public and overriden methods
        public T Get<T>() where T : class, IConfig
        {
            var type = typeof(T);

            IConfig config;
            if (!configs.TryGetValue(type, out config))
            {
                config = type.CreateObject<T>();
                configs[type] = config;
            }

            return (T)config;
        }

        public void Save(IConfig config)
        {
        }

        public void SaveAll()
        {
        }
        #endregion
        
        #region Private fields and constants
        private readonly IDictionary<Type, IConfig> configs = new Dictionary<Type, IConfig>();
        #endregion
    }
}
