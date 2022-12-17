// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Caching.Memory.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Threading;
using Xunit;

namespace AppBrix.Caching.Memory.Tests;

public sealed class MemoryCacheTests : TestsBase<MemoryCachingModule>
{
    #region Setup and cleanup
    public MemoryCacheTests()
    {
        this.app.Start();
        this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.app.ConfigService.GetMemoryCachingConfig().ExpirationCheck = TimeSpan.FromMilliseconds(1);
        this.app.Reinitialize();
        this.timeService = new TimeServiceMock(this.app);
        this.app.Container.Register(this.timeService);
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetMemoryCache()
    {
        var cache = this.app.GetMemoryCache();
        cache.Should().NotBeNull("cache must be registered and resolved");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNullKey()
    {
        var cache = this.app.GetMemoryCache();
        Action action = () => cache.Get(null);
        action.Should().Throw<ArgumentNullException>("key should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetUnregisteredItem()
    {
        var cache = this.app.GetMemoryCache();
        var item = cache.Get(nameof(this.TestGetUnregisteredItem));
        item.Should().BeNull("asking for non-existing key should return null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetUnregisteredItemGenericExtension()
    {
        var cache = this.app.GetMemoryCache();
        var item = cache.Get<TimeSpan>(nameof(this.TestGetUnregisteredItem));
        item.Should().Be(default, "asking for non-existing struct should return its default value");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullKey()
    {
        var cache = this.app.GetMemoryCache();
        Action action = () => cache.Set(null, this);
        action.Should().Throw<ArgumentNullException>("key should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullItem()
    {
        var cache = this.app.GetMemoryCache();
        Action action = () => cache.Set(nameof(this.TestSetNullItem), null);
        action.Should().Throw<ArgumentNullException>("item should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNegativeAbsoluteExpiration()
    {
        var cache = this.app.GetMemoryCache();
        Action action = () => cache.Set(nameof(this.TestSetNegativeAbsoluteExpiration), this, absoluteExpiration: TimeSpan.FromSeconds(-1));
        action.Should().Throw<ArgumentException>("absolute expiration should not be negative");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNegativeSlidingExpiration()
    {
        var cache = this.app.GetMemoryCache();
        Action action = () => cache.Set(nameof(this.TestSetNegativeSlidingExpiration), this, slidingExpiration: TimeSpan.FromSeconds(-1));
        action.Should().Throw<ArgumentException>("sliding expiration should not be negative");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveNullKey()
    {
        var cache = this.app.GetMemoryCache();
        Action action = () => cache.Remove(null);
        action.Should().Throw<ArgumentNullException>("key should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetItem()
    {
        const string key = nameof(this.TestGetItem);

        var cache = this.app.GetMemoryCache();
        cache.Set(key, this);

        var item = cache.Get(key);
        item.Should().Be(this, "returned item should be the same as the original");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveItem()
    {
        const string key = nameof(this.TestRemoveItem);

        var cache = this.app.GetMemoryCache();
        cache.Set(key, this);
        cache.Remove(key).Should().BeTrue("the item should have been removed successfully");

        var item = cache.Get(nameof(MemoryCacheTests.TestGetItem));
        item.Should().BeNull("the item should have been removed from the cache");
        cache.Remove(key).Should().BeFalse("the item should have already been removed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAbsoluteExpiration()
    {
        const string key = nameof(this.TestAbsoluteExpiration);

        var cache = this.app.GetMemoryCache();
        cache.Set(key, this, absoluteExpiration: TimeSpan.FromMilliseconds(50));
        var item = cache.Get(key);
        item.Should().Be(this, "returned item should be the same as the original");

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(52));
        var func = () => cache.Get(key);
        func.ShouldReturn(null, "the item should have been removed from the cache");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestMixedExpiration()
    {
        const string key = nameof(MemoryCacheTests.TestMixedExpiration);

        var cache = this.app.GetMemoryCache();
        cache.Set(key, this, absoluteExpiration: TimeSpan.FromMilliseconds(50), slidingExpiration: TimeSpan.FromMilliseconds(25));
        var item = cache.Get(key);
        item.Should().Be(this, "returned item should be the same as the original");

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(52));
        var func = () => cache.Get(key);
        func.ShouldReturn(null, "the item should have been removed from the cache");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDisposeOnAbsoluteExpiration()
    {
        const string key = nameof(MemoryCacheTests.TestDisposeOnAbsoluteExpiration);

        var disposed = false;

        void Dispose()
        {
            disposed = true;
            throw new InvalidOperationException("Failed to dispose.");
        }

        var cache = this.app.GetMemoryCache();
        this.timeService.SetTime(this.timeService.GetTime());
        cache.Set(key, this, dispose: Dispose, absoluteExpiration: TimeSpan.FromMilliseconds(5));
        var item = cache.Get(key);
        item.Should().Be(this, "returned item should be the same as the original");
        disposed.Should().BeFalse("the item should not have expired yet");

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(10));
        var func = () => disposed;
        func.ShouldReturn(true, "the item should have expired");
        item = cache.Get(key);
        item.Should().BeNull("the item should have been removed from the cache");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDisposeOnSlidingExpiration()
    {
        const string key = nameof(MemoryCacheTests.TestDisposeOnSlidingExpiration);

        var disposed = false;

        var cache = this.app.GetMemoryCache();
        this.timeService.SetTime(this.timeService.GetTime());
        cache.Set(key, this, dispose: () => disposed = true, slidingExpiration: TimeSpan.FromMilliseconds(2));

        var item = cache.Get(key);
        item.Should().Be(this, "returned item should be the same as the original");

        for (var i = 0; i < 5; i++)
        {
            item = cache.Get(key);
            item.Should().Be(this, $"returned item should be the same as the original after {i} retries");
            disposed.Should().BeFalse($"the item should not have expired after {i} retries");
            this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(1));
            Thread.Sleep(1);
        }

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(5));
        var func = () => disposed;
        func.ShouldReturn(true, "the item should have expired");
        item = cache.Get(key);
        item.Should().BeNull("the item should have been removed from the cache");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceMemoryCache() => TestUtils.AssertPerformance(this.TestPerformanceMemoryCacheInternal);
    #endregion

    #region Private methods
    private void TestPerformanceMemoryCacheInternal()
    {
        const int items = 800;
        var cache = this.app.GetMemoryCache();

        for (var i = 0; i < items; i++)
        {
            cache.Set(i.ToString(), i);
        }
        for (var i = 0; i < items * 100; i++)
        {
            cache.Get<int>((i % items).ToString());
        }
        for (var i = 0; i < items; i++)
        {
            cache.Remove(i.ToString());
        }
    }
    #endregion

    #region Private fields and constants
    private readonly TimeServiceMock timeService;
    #endregion
}
