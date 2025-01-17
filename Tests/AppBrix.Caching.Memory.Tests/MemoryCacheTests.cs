// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Caching.Memory.Tests.Mocks;
using AppBrix.Testing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppBrix.Caching.Memory.Tests;

[TestClass]
public sealed class MemoryCacheTests : TestsBase<MemoryCachingModule>
{
    #region Setup and cleanup
    protected override void Initialize()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.App.ConfigService.GetMemoryCachingConfig().ExpirationCheck = TimeSpan.FromMilliseconds(1);
        this.App.Start();

        this.timeService = new TimeServiceMock(this.App);
        this.App.Container.Register(this.timeService);
    }
    #endregion

    #region Tests
    [Test, Functional]
    public void TestGetMemoryCache()
    {
        this.Assert(this.App.GetMemoryCache() is not null, "cache must be registered and resolved");
    }

    [Test, Functional]
    public void TestGetNullKey()
    {
        var cache = this.App.GetMemoryCache();
        Action action = () => cache.Get(null!);
        this.AssertThrows<ArgumentNullException>(action, "key should not be null");
    }

    [Test, Functional]
    public void TestGetUnregisteredItem()
    {
        var cache = this.App.GetMemoryCache();
        var item = cache.Get(nameof(this.TestGetUnregisteredItem));
        this.Assert(item is null, "asking for non-existing key should return null");
    }

    [Test, Functional]
    public void TestGetUnregisteredItemGenericExtension()
    {
        var cache = this.App.GetMemoryCache();
        var item = cache.Get<TimeSpan>(nameof(this.TestGetUnregisteredItemGenericExtension));
        this.Assert(item == TimeSpan.Zero,"asking for non-existing struct should return its default value");
    }

    [Test, Functional]
    public void TestSetNullKey()
    {
        var cache = this.App.GetMemoryCache();
        var action = () => cache.Set(null!, this);
        this.AssertThrows<ArgumentNullException>(action, "key should not be null");
    }

    [Test, Functional]
    public void TestSetNullItem()
    {
        var cache = this.App.GetMemoryCache();
        var action = () => cache.Set(nameof(this.TestSetNullItem), null!);
        this.AssertThrows<ArgumentNullException>(action, "item should not be null");
    }

    [Test, Functional]
    public void TestSetNegativeAbsoluteExpiration()
    {
        var cache = this.App.GetMemoryCache();
        var action = () => cache.Set(nameof(this.TestSetNegativeAbsoluteExpiration), this, absoluteExpiration: TimeSpan.FromSeconds(-1));
        this.AssertThrows<ArgumentException>(action, "absolute expiration should not be negative");
    }

    [Test, Functional]
    public void TestSetNegativeSlidingExpiration()
    {
        var cache = this.App.GetMemoryCache();
        var action = () => cache.Set(nameof(this.TestSetNegativeSlidingExpiration), this, slidingExpiration: TimeSpan.FromSeconds(-1));
        this.AssertThrows<ArgumentException>(action, "sliding expiration should not be negative");
    }

    [Test, Functional]
    public void TestRemoveNullKey()
    {
        var cache = this.App.GetMemoryCache();
        Action action = () => cache.Remove(null!);
        this.AssertThrows<ArgumentNullException>(action, "key should not be null");
    }

    [Test, Functional]
    public void TestGetItem()
    {
        const string key = nameof(this.TestGetItem);

        var cache = this.App.GetMemoryCache();
        cache.Set(key, this);

        this.Assert(cache.Get(key) == this, "returned item should be the same as the original");
    }

    [Test, Functional]
    public void TestRemoveItem()
    {
        const string key = nameof(this.TestRemoveItem);

        var cache = this.App.GetMemoryCache();
        cache.Set(key, this);

        this.Assert(cache.Remove(key), "the item should have been removed successfully");
        this.Assert(cache.Get(key) is null, "the item should have been removed from the cache");
        this.Assert(cache.Remove(key) == false, "the item should have already been removed");
    }

    [Test, Functional]
    public Task TestAbsoluteExpiration()
    {
        const string key = nameof(this.TestAbsoluteExpiration);

        var cache = this.App.GetMemoryCache();
        cache.Set(key, this, absoluteExpiration: TimeSpan.FromMilliseconds(50));
        this.Assert(cache.Get(key) == this, "returned item should be the same as the original");

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(52));
        var func = () => cache.Get(key);
        return this.AssertReturns(func, null, "the item should have been removed from the cache");
    }

    [Test, Functional]
    public Task TestMixedExpiration()
    {
        const string key = nameof(MemoryCacheTests.TestMixedExpiration);

        var cache = this.App.GetMemoryCache();
        cache.Set(key, this, absoluteExpiration: TimeSpan.FromMilliseconds(50), slidingExpiration: TimeSpan.FromMilliseconds(25));
        this.Assert(cache.Get(key) == this, "returned item should be the same as the original");

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(52));
        var func = () => cache.Get(key);
        return this.AssertReturns(func, null, "the item should have been removed from the cache");
    }

    [Test, Functional]
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
        cache.Set(key, this, dispose: dispose, absoluteExpiration: TimeSpan.FromMilliseconds(5));
        this.Assert(cache.Get(key) == this, "returned item should be the same as the original");
        this.Assert(disposed == false, "the item should not have expired yet");

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(10));
        var func = () => disposed;
        await this.AssertReturns(func, true, "the item should have expired");
        this.Assert(cache.Get(key) is null, "the item should have been removed from the cache");
    }

    [Test, Functional]
    public async Task TestDisposeOnSlidingExpiration()
    {
        const string key = nameof(MemoryCacheTests.TestDisposeOnSlidingExpiration);

        var disposed = false;

        var cache = this.App.GetMemoryCache();
        cache.Set(key, this, dispose: () => disposed = true, slidingExpiration: TimeSpan.FromMilliseconds(2));

        this.Assert(cache.Get(key) == this, "returned item should be the same as the original");

        for (var i = 0; i < 5; i++)
        {
            this.Assert(cache.Get(key) == this, $"returned item should be the same as the original after {i} retries");
            this.Assert(disposed == false, $"the item should not have expired after {i} retries");
            this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(1));
            Thread.Sleep(1);
        }

        this.timeService.SetTime(this.timeService.GetTime().AddMilliseconds(5));
        var func = () => disposed;
        await this.AssertReturns(func, true, "the item should have expired");
        this.Assert(cache.Get(key) is null, "the item should have been removed from the cache");
    }

    [Test, Performance]
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
    private TimeServiceMock timeService;
    #endregion
}
