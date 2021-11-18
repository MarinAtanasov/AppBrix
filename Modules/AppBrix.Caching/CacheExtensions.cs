// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Caching;

namespace AppBrix;

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
}
