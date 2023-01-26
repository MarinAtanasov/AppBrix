// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Caching.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace AppBrix;

/// <summary>
/// Extension methods for easier manipulation of AppBrix caches.
/// </summary>
public static class CachingExtensions
{
    /// <summary>
    /// Gets the currently registered <see cref="ICache"/>.
    /// </summary>
    /// <param name="app">The currently running application.</param>
    /// <returns>The <see cref="ICache"/>.</returns>
    public static ICache GetCache(this IApp app) => (ICache)app.Get(typeof(ICache));
    
    /// <summary>
    /// Gets the currently registered <see cref="IDistributedCache"/>.
    /// </summary>
    /// <param name="app">The currently running application.</param>
    /// <returns>The <see cref="IDistributedCache"/>.</returns>
    internal static IDistributedCache GetDistributedCache(this IApp app) => (IDistributedCache)app.Get(typeof(IDistributedCache));

    /// <summary>
    /// Gets the currently registered <see cref="ICacheSerializer"/>..
    /// </summary>
    /// <param name="app">The currently running application.</param>
    /// <returns>The <see cref="ICacheSerializer"/>.</returns>
    internal static ICacheSerializer GetCacheSerializer(this IApp app) => (ICacheSerializer)app.Get(typeof(ICacheSerializer));
}
