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
        public T GetConfig<T>() where T : class, IConfig
        {
            var type = typeof(T);

            if (!configs.ContainsKey(type))
                configs[type] = type.CreateObject<T>();

            return (T)configs[type];
        }

        public void SaveAll()
        {
        }

        public void Save(IConfig config)
        {
        }
        #endregion
        
        #region Private fields and constants
        private readonly IDictionary<Type, IConfig> configs = new Dictionary<Type, IConfig>();
        #endregion
    }
}
