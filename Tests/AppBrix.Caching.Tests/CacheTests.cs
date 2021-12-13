// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Caching.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;

namespace AppBrix.Caching.Tests;

public sealed class CacheTests : TestsBase
{
    #region Setup and cleanup
    public CacheTests() : base(TestUtils.CreateTestApp<CachingModule>())
    {
        this.app.Start();
        this.app.Container.Register(new JsonCacheSerializer());
        this.app.Container.Register(new MemoryDistributedCache(new CustomMemoryDistributedCacheOptions()));
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetCache()
    {
        var cache = this.app.GetCache();
        cache.Should().NotBeNull("cache must be registered and resolved");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestCacheItem()
    {
        var cache = this.app.GetCache();
        var key = "key";
        var value = "Test Value";
        cache.Set(key, value);
        var cached = cache.Get<string>(key).GetAwaiter().GetResult();
        cached.Should().NotBeNull("the item should be in the cache");
        cached.Should().Be(value, "the returned object should be equal to the original");
        cache.Remove(key).GetAwaiter().GetResult();
        cache.Refresh(key).GetAwaiter().GetResult();
        cache.Get<object>(key).GetAwaiter().GetResult().Should().BeNull("item should have been removed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestReplaceItem()
    {
        var cache = this.app.GetCache();
        var key = "key";
        var value = "Test Value";
        cache.Set(key, value);
        var cached = cache.Get<string>(key).GetAwaiter().GetResult();
        cached.Should().NotBeNull("the item should be in the cache");
        cached.Should().Be(value, "the returned object should be equal to the original");
        value = "Test Replaced Value";
        cache.Set(key, value).GetAwaiter().GetResult();
        cached = cache.Get<string>(key).GetAwaiter().GetResult();
        cached.Should().NotBeNull("the item should be in the cache after being replaced");
        cached.Should().Be(value, "the returned object should be equal to the replaced");
        cache.Remove(key).GetAwaiter().GetResult();
        cache.Get<object>(key).GetAwaiter().GetResult().Should().BeNull("item should have been removed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceCache() => TestUtils.TestPerformance(this.TestPerformanceCacheInternal);
    #endregion

    #region Private methods
    private void TestPerformanceCacheInternal()
    {
        var cache = this.app.GetCache();
        var items = 2000;
        for (var i = 0; i < items; i++)
        {
            cache.Set(i.ToString(), i).GetAwaiter().GetResult();
        }
        var gets = items * 10;
        for (var i = 0; i < gets; i++)
        {
            var itemId = i % items;
            cache.Get(itemId.ToString(), typeof(int)).GetAwaiter().GetResult();
        }
        for (var i = 0; i < items; i++)
        {
            cache.Remove(i.ToString()).GetAwaiter().GetResult();
        }
    }
    #endregion
}
