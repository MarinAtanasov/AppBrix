// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Caching.Memory;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix memory caches.
    /// </summary>
    public static class MemoryCacheExtensions
    {
        /// <summary>
        /// Gets the currently registered local in-memory cache.
        /// This can be used for objects which are long running and should
        /// be disposed after absolute or sliding expiration time.
        /// </summary>
        /// <param name="app">The currently running application.</param>
        /// <returns>The local in-memory cache.</returns>
        public static IMemoryCache GetMemoryCache(this IApp app) => (IMemoryCache)app.Get(typeof(IMemoryCache));
    }
}
