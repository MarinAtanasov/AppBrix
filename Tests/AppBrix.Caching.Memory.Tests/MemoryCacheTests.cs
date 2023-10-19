// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Caching.Memory.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AppBrix.Caching.Memory.Tests;

public sealed class MemoryCacheTests : TestsBase<MemoryCachingModule>
{
    #region Setup and cleanup
    public MemoryCacheTests()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.App.ConfigService.GetMemoryCachingConfig().ExpirationCheck = TimeSpan.FromMilliseconds(1);
        this.App.Start();

        this.timeService = new TimeServiceMock(this.App);
        this.App.Container.Register(this.timeService);
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetMemoryCache()
    {
        var cache = this.App.GetMemoryCache();
        cache.Should().NotBeNull("cache must be registered and resolved");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNullKey()
    {
        var cache = this.App.GetMemoryCache();
        Action action = () => cache.Get(null!);
        action.Should().Throw<ArgumentNullException>("key should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetUnregisteredItem()
    {
        var cache = this.App.GetMemoryCache();
        var item = cache.Get(nameof(this.TestGetUnregisteredItem));
        item.Should().BeNull("asking for non-existing key should return null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetUnregisteredItemGenericExtension()
    {
        var cache = this.App.GetMemoryCache();
        var item = cache.Get<TimeSpan>(nameof(this.TestGetUnregisteredItem));
        item.Should().Be(default, "asking for non-existing struct should return its default value");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullKey()
    {
        var cache = this.App.GetMemoryCache();
        var action = () => cache.Set(null!, this);
        action.Should().Throw<ArgumentNullException>("key should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNullItem()
    {
        var cache = this.App.GetMemoryCache();
        var action = () => cache.Set(nameof(this.TestSetNullItem), null!);
        action.Should().Throw<ArgumentNullException>("item should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNegativeAbsoluteExpiration()
    {
        var cache = this.App.GetMemoryCache();
        var action = () => cache.Set(nameof(this.TestSetNegativeAbsoluteExpiration), this, absoluteExpiration: TimeSpan.FromSeconds(-1));
        action.Should().Throw<ArgumentException>("absolute expiration should not be negative");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSetNegativeSlidingExpiration()
    {
        var cache = this.App.GetMemoryCache();
        var action = () => cache.Set(nameof(this.TestSetNegativeSlidingExpiration), this, slidingExpiration: TimeSpan.FromSeconds(-1));
        action.Should().Throw<ArgumentException>("sliding expiration should not be negative");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveNullKey()
    {
        var cache = this.App.GetMemoryCache();
        Action action = () => cache.Remove(null!);
        action.Should().Throw<ArgumentNullException>("key should not be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetItem()
    {
        const string key = nameof(this.TestGetItem);

        var cache = this.App.GetMemoryCache();
        cache.Set(key, this);

        var item = cache.Get(key);
        item.Should().Be(this, "returned item should be the same as the original");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRemoveItem()
    {
        const string key = nameof(this.TestRemoveItem);

        var cache = this.App.GetMemoryCache();
        cache.Set(key, this);
        cache.Remove(key).Should().BeTrue("the item should have been removed successfully");

        var item = cache.Get(nameof(MemoryCacheTests.TestGetItem));
        item.Should().BeNull("the item should have been removed from the cache");
        cache.Remove(key).Should().BeFalse("the item should have already been removed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public Task TestAbsoluteExpiration()
    {
        const string key = nameof(this.TestAbsoluteExpiration);

        var cache = this.App.GetMemoryCache();
        cache.Set(key, this, absoluteExpiration: TimeSpan.FromMilliseconds(50));
        var item = cache.Get(key);
        item.Should().Be(this, "returned item should be the same as the original");

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(52));
        var func = () => cache.Get(key);
        return func.ShouldReturn(null, "the item should have been removed from the cache");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public Task TestMixedExpiration()
    {
        const string key = nameof(MemoryCacheTests.TestMixedExpiration);

        var cache = this.App.GetMemoryCache();
        cache.Set(key, this, absoluteExpiration: TimeSpan.FromMilliseconds(50), slidingExpiration: TimeSpan.FromMilliseconds(25));
        var item = cache.Get(key);
        item.Should().Be(this, "returned item should be the same as the original");

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(52));
        var func = () => cache.Get(key);
        return func.ShouldReturn(null, "the item should have been removed from the cache");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestDisposeOnAbsoluteExpiration()
    {
        const string key = nameof(MemoryCacheTests.TestDisposeOnAbsoluteExpiration);

        var disposed = false;
        var dispose = () =>
        {
            disposed = true;
            throw new InvalidOperationException("Failed to dispose.");
        };

        var cache = this.App.GetMemoryCache();
        this.timeService.SetTime(this.timeService.GetTime());
        cache.Set(key, this, dispose: dispose, absoluteExpiration: TimeSpan.FromMilliseconds(5));
        var item = cache.Get(key);
        item.Should().Be(this, "returned item should be the same as the original");
        disposed.Should().BeFalse("the item should not have expired yet");

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(10));
        var func = () => disposed;
        await func.ShouldReturn(true, "the item should have expired");
        item = cache.Get(key);
        item.Should().BeNull("the item should have been removed from the cache");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestDisposeOnSlidingExpiration()
    {
        const string key = nameof(MemoryCacheTests.TestDisposeOnSlidingExpiration);

        var disposed = false;

        var cache = this.App.GetMemoryCache();
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
        await func.ShouldReturn(true, "the item should have expired");
        item = cache.Get(key);
        item.Should().BeNull("the item should have been removed from the cache");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceMemoryCache() => this.AssertPerformance(this.TestPerformanceMemoryCacheInternal);
    #endregion

    #region Private methods
    private void TestPerformanceMemoryCacheInternal()
    {
        const int items = 800;
        var cache = this.App.GetMemoryCache();

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
