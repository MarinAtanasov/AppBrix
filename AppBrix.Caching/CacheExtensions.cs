// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using System;
using System.Linq;
using System.Runtime.Caching;

namespace AppBrix
{
    public static class CacheExtensions
    {
        /// <summary>
        /// Gets the currently registered cache.
        /// </summary>
        /// <param name="app">The currently running application.</param>
        /// <returns>The cache.</returns>
        public static ObjectCache GetCache(this IApp app)
        {
            return app.Get<ObjectCache>();
        }

        /// <summary>
        /// Gets the item from the cache and casts it to the desired type.
        /// Return null if the item is not found.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="cache">The cache.</param>
        /// <param name="key">The key of the cached item.</param>
        /// <param name="regionName">The region of the item. Optional.</param>
        /// <returns>The cached item. Null if not found.</returns>
        public static T Get<T>(this ObjectCache cache, string key, string regionName = null)
            where T : class
        {
            return (T)cache.Get(key, regionName);
        }
    }
}
