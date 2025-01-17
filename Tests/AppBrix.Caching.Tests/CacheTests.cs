// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Caching.Tests.Mocks;
using AppBrix.Testing;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace AppBrix.Caching.Tests;

[TestClass]
public sealed class CacheTests : TestsBase<CachingModule>
{
    #region Test lifecycle
    protected override void Initialize()
    {
        this.App.Start();
        this.App.Container.Register(new JsonCacheSerializer());
        this.App.Container.Register(new MemoryDistributedCache(new CustomMemoryDistributedCacheOptions()));
    }
    #endregion

    #region Tests
    [Test, Functional]
    public void TestGetCache()
    {
        this.Assert(this.App.GetCache() is not null, "cache must be registered and resolved");
    }

    [Test, Functional]
    public async Task TestCacheItem()
    {
        const string key = "key";
        const string value = "Test Value";

        var cache = this.App.GetCache();
        await cache.Set(key, value);
        this.Assert(await cache.Get<string>(key) == value, "the returned object should be equal to the original");

        await cache.Remove(key);
        await cache.Refresh(key);
        this.Assert(await cache.Get<object>(key) is null, "item should have been removed");
    }

    [Test, Functional]
    public async Task TestReplaceItem()
    {
        const string key = "key";
        const string value = "Test Value";
        const string newValue = "Test Replaced Value";

        var cache = this.App.GetCache();
        await cache.Set(key, value);
        this.Assert(await cache.Get<string>(key) == value, "the returned object should be equal to the original");

        await cache.Set(key, newValue);
        this.Assert(await cache.Get<string>(key) == newValue, "the returned object should be equal to the replaced");

        await cache.Remove(key);
        this.Assert(await cache.Get<object>(key) is null, "item should have been removed");
    }

    [Test, Performance]
    public void TestPerformanceCache() => this.AssertPerformance(this.TestPerformanceCacheInternal);
    #endregion

    #region Private methods
    private async Task TestPerformanceCacheInternal()
    {
        const int items = 2000;
        const int gets = items * 10;
        var cache = this.App.GetCache();

        for (var i = 0; i < items; i++)
        {
            await cache.Set(i.ToString(), i);
        }
        for (var i = 0; i < gets; i++)
        {
            var itemId = i % items;
            await cache.Get(itemId.ToString(), typeof(int));
        }
        for (var i = 0; i < items; i++)
        {
            await cache.Remove(i.ToString());
        }
    }
    #endregion
}
