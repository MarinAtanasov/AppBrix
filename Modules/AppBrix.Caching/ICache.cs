// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Threading.Tasks;

namespace AppBrix.Caching
{
    /// <summary>
    /// Defines a remote cache which holds serializable data objects.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Refreshes the cache for a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        Task Refresh(string key);

        #nullable disable
        /// <summary>
        /// Gets a cached object by its key.
        /// </summary>
        /// <typeparam name="T">The type of the object to get.</typeparam>
        /// <param name="key">The key which is used to store the object in the cache.</param>
        /// <returns></returns>
        public async Task<T> Get<T>(string key) => (T)(await this.Get(key, typeof(T)).ConfigureAwait(false) ?? default(T));
        #nullable restore

        /// <summary>
        /// Gets a cached object by its key.
        /// </summary>
        /// <param name="key">The key which is used to store the object in the cache.</param>
        /// <param name="type">The type of the object to get.</param>
        /// <returns></returns>
        Task<object?> Get(string key, Type type);

        /// <summary>
        /// Sets a cached object.
        /// </summary>
        /// <param name="key">The key which will be used when storing the object.</param>
        /// <param name="item">The object to be cached.</param>
        /// <returns></returns>
        Task Set(string key, object item);

        /// <summary>
        /// Removes an object from the cache.
        /// </summary>
        /// <param name="key">The key to the object.</param>
        /// <returns></returns>
        Task Remove(string key);
    }
}
