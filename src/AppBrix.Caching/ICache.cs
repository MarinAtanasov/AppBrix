// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppBrix.Caching
{
    public interface ICache
    {
        /// <summary>
        /// Refreshes the cache for a key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        Task Refresh(string key);

        /// <summary>
        /// Gets a cached object by its key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> Get<T>(string key);

        /// <summary>
        /// Sets a cached object.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="key">The key which will be used when storing the object.</param>
        /// <param name="item">The object to be cached.</param>
        /// <returns></returns>
        Task Set<T>(string key, T item);

        /// <summary>
        /// Removes an object from the cache.
        /// </summary>
        /// <param name="key">The key to the object.</param>
        /// <returns></returns>
        Task Remove(string key);
    }
}
