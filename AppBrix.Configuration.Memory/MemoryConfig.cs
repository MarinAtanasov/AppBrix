// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AppBrix.Configuration.Memory
{
    /// <summary>
    /// A configuration manager which stores the configuration section within memory.
    /// Can be used for testing purposes.
    /// </summary>
    internal sealed class MemoryConfig : IConfig
    {
        #region IConfig implementation
        public T GetSection<T>() where T : ConfigurationSection
        {
            if (!this.sections.ContainsKey(typeof(T)))
                this.sections[typeof(T)] = typeof(T).CreateObject<T>();
            return (T)this.sections[typeof(T)];
        }
        #endregion

        #region Private fields and constants
        private IDictionary<Type, object> sections = new Dictionary<Type, object>();
        #endregion
    }
}
