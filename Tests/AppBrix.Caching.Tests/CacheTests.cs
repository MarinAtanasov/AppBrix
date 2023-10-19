// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Caching.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using Xunit;

namespace AppBrix.Caching.Tests;

public sealed class CacheTests : TestsBase<CachingModule>
{
    #region Setup and cleanup
    public CacheTests()
    {
        this.App.Start();
        this.App.Container.Register(new JsonCacheSerializer());
        this.App.Container.Register(new MemoryDistributedCache(new CustomMemoryDistributedCacheOptions()));
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetCache()
    {
        var cache = this.App.GetCache();
        cache.Should().NotBeNull("cache must be registered and resolved");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestCacheItem()
    {
        var cache = this.App.GetCache();
        var key = "key";
        var value = "Test Value";
        await cache.Set(key, value);
        var cached = await cache.Get<string>(key);
        cached.Should().NotBeNull("the item should be in the cache");
        cached.Should().Be(value, "the returned object should be equal to the original");
        await cache.Remove(key);
        await cache.Refresh(key);
        var removed = await cache.Get<object>(key);
        removed.Should().BeNull("item should have been removed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestReplaceItem()
    {
        var cache = this.App.GetCache();
        var key = "key";
        var value = "Test Value";
        await cache.Set(key, value);
        var cached = await cache.Get<string>(key);
        cached.Should().NotBeNull("the item should be in the cache");
        cached.Should().Be(value, "the returned object should be equal to the original");
        value = "Test Replaced Value";
        await cache.Set(key, value);
        cached = await cache.Get<string>(key);
        cached.Should().NotBeNull("the item should be in the cache after being replaced");
        cached.Should().Be(value, "the returned object should be equal to the replaced");
        await cache.Remove(key);
        var removed = await cache.Get<object>(key);
        removed.Should().BeNull("item should have been removed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
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
