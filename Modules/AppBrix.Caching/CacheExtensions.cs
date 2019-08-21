// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Caching;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix caches.
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Gets the currently registered cache.
        /// </summary>
        /// <param name="app">The currently running application.</param>
        /// <returns>The cache.</returns>
        public static ICache GetCache(this IApp app) => (ICache)app.Get(typeof(ICache));

        /// <summary>
        /// Gets a cached object by its key.
        /// </summary>
        /// <typeparam name="T">The type of the object to get.</typeparam>
        /// <param name="cache">The object cache.</param>
        /// <param name="key">The key which is used to store the object in the cache.</param>
        /// <returns></returns>
        public static async Task<T> Get<T>(this ICache cache, string key) =>
            (T)(await cache.Get(key, typeof(T)).ConfigureAwait(false) ?? default(T));
    }
}
