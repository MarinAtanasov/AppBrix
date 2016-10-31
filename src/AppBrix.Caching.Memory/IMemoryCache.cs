// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Linq;

namespace AppBrix.Caching.Memory
{
    /// <summary>
    /// Defines an in-memory local cache.
    /// This can be used for objects which are long running and should
    /// be disposed after absolute or rolling expiration time.
    /// </summary>
    public interface IMemoryCache
    {
        /// <summary>
        /// Gets a cached object by its key.
        /// </summary>
        /// <param name="key">The key which is used to store the object in the cache.</param>
        /// <returns>The cached object. Returns null if no object is found.</returns>
        object Get(object key);

        /// <summary>
        /// Stores an object in the cache.
        /// </summary>
        /// <param name="key">The key which will be used to store the item.</param>
        /// <param name="item">The item to be stored.</param>
        /// <param name="dispose">Optional action to be executed when the absolute or rolling expirations are reached.</param>
        /// <param name="absoluteExpiration">Absolute expiration time.</param>
        /// <param name="rollingExpiration">Rolling expiration time.</param>
        void Set(object key, object item, Action dispose = null, TimeSpan absoluteExpiration = default(TimeSpan), TimeSpan rollingExpiration = default(TimeSpan));

        /// <summary>
        /// Removes a cached item by its key.
        /// </summary>
        /// <param name="key">The key of the stored item.</param>
        void Remove(object key);
    }
}
