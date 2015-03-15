// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Base class for a generic configuration element collection.
    /// </summary>
    /// <typeparam name="T">The type of the configuration elements.</typeparam>
    public sealed class ConfigElementCollection<T> : ConfigurationElementCollection, IEnumerable<T>
        where T : ConfigElement
    {
        #region Public methods
        public T this[int index]
        {
            get { return (T)this.BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                    this.BaseRemoveAt(index);
                this.BaseAdd(index, value);
            }
        }

        public void Add(T serviceConfig)
        {
            this.BaseAdd(serviceConfig);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        public void Remove(T item)
        {
            this.BaseRemove(item.Key);
        }

        public void RemoveAt(int index)
        {
            this.BaseRemoveAt(index);
        }

        public void Remove(string key)
        {
            this.BaseRemove(key);
        }

        public new IEnumerator<T> GetEnumerator()
        {
            var enumerable = (System.Collections.IEnumerable)this;
            foreach (var item in enumerable)
            {
                yield return (T)item;
            }
        }
        #endregion

        #region Protected overriden methods
        protected override ConfigurationElement CreateNewElement()
        {
            return typeof(T).CreateObject<T>();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ConfigElement)element).Key;
        }
        #endregion
    }
}
