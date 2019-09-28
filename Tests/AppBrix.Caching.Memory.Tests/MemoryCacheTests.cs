// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Caching.Memory.Configuration;
using AppBrix.Caching.Memory.Tests.Mocks;
using AppBrix.Events.Schedule.Configuration;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Threading;
using Xunit;

namespace AppBrix.Caching.Memory.Tests
{
    public sealed class MemoryCacheTests : IDisposable
    {
        #region Setup and cleanup
        public MemoryCacheTests()
        {
            this.app = TestUtils.CreateTestApp(typeof(MemoryCachingModule));
            this.app.Start();
        }

        public void Dispose()
        {
            this.app.Stop();
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
            var item = cache.Get(nameof(TestGetUnregisteredItem));
            item.Should().BeNull("asking for non-existing key should return null");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGetUnregisteredItemGenericExtension()
        {
            var cache = this.app.GetMemoryCache();
            var item = cache.Get<TimeSpan>(nameof(TestGetUnregisteredItem));
            item.Should().Be(default(TimeSpan), "asking for non-existing struct should return its default value");
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
            Action action = () => cache.Set(nameof(TestSetNullItem), null);
            action.Should().Throw<ArgumentNullException>("item should not be null");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestSetNegativeAbsoluteExpiration()
        {
            var cache = this.app.GetMemoryCache();
            Action action = () => cache.Set(nameof(TestSetNegativeAbsoluteExpiration), this, absoluteExpiration: TimeSpan.FromSeconds(-1));
            action.Should().Throw<ArgumentException>("absolute expiration should not be negative");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestSetNegativeSlidingExpiration()
        {
            var cache = this.app.GetMemoryCache();
            Action action = () => cache.Set(nameof(TestSetNegativeSlidingExpiration), this, slidingExpiration: TimeSpan.FromSeconds(-1));
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
            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestGetItem), this);
            var item = cache.Get(nameof(TestGetItem));
            item.Should().Be(this, "returned item should be the same as the original");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRemoveItem()
        {
            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestRemoveItem), this);
            cache.Remove(nameof(TestRemoveItem));
            var item = cache.Get(nameof(TestGetItem));
            item.Should().BeNull("the item shold have been removed from the cache");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestAbsoluteExpiration()
        {
            this.app.GetConfig<ScheduledEventsConfig>().ExecutionCheck = TimeSpan.FromMilliseconds(1);
            this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck = TimeSpan.FromMilliseconds(1);
            this.app.Reinitialize();
            var timeService = new TimeServiceMock(this.app);
            this.app.Container.Register(timeService);

            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestAbsoluteExpiration), this, absoluteExpiration: TimeSpan.FromMilliseconds(50));
            var item = cache.Get(nameof(TestAbsoluteExpiration));
            item.Should().Be(this, "returned item should be the same as the original");
            
            timeService.SetTime(timeService.GetTime().AddMilliseconds(52));
            new Func<object>(() => cache.Get(nameof(TestAbsoluteExpiration)))
                .ShouldReturn(null, TimeSpan.FromMilliseconds(10000), "the item shold have been removed from the cache");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestMixedExpiration()
        {
            this.app.GetConfig<ScheduledEventsConfig>().ExecutionCheck = TimeSpan.FromMilliseconds(1);
            this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck = TimeSpan.FromMilliseconds(1);
            this.app.Reinitialize();
            var timeService = new TimeServiceMock(this.app);
            this.app.Container.Register(timeService);

            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestMixedExpiration), this, absoluteExpiration: TimeSpan.FromMilliseconds(50), slidingExpiration: TimeSpan.FromMilliseconds(25));
            var item = cache.Get(nameof(TestMixedExpiration));
            item.Should().Be(this, "returned item should be the same as the original");

            timeService.SetTime(timeService.GetTime().AddMilliseconds(52));
            new Func<object>(() => cache.Get(nameof(TestMixedExpiration)))
                .ShouldReturn(null, TimeSpan.FromMilliseconds(10000), "the item shold have been removed from the cache");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestDisposeOnAbsoluteExpiration()
        {
            this.app.GetConfig<ScheduledEventsConfig>().ExecutionCheck = TimeSpan.FromMilliseconds(1);
            this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck = TimeSpan.FromMilliseconds(1);
            this.app.Reinitialize();
            var timeService = new TimeServiceMock(this.app);
            this.app.Container.Register(timeService);

            var disposed = false;

            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestDisposeOnAbsoluteExpiration), this, dispose: () => disposed = true, absoluteExpiration: TimeSpan.FromMilliseconds(50));
            var item = cache.Get(nameof(TestDisposeOnAbsoluteExpiration));
            item.Should().Be(this, "returned item should be the same as the original");
            disposed.Should().BeFalse("the item should not have expired yet");

            timeService.SetTime(timeService.GetTime().AddMilliseconds(60));
            new Func<bool>(() => disposed).ShouldReturn(true, TimeSpan.FromMilliseconds(10000), "the item should have expired");
            item = cache.Get(nameof(TestDisposeOnAbsoluteExpiration));
            item.Should().BeNull("the item should have been removed from the cache");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestDisposeOnSlidingExpiration()
        {
            this.app.GetConfig<ScheduledEventsConfig>().ExecutionCheck = TimeSpan.FromMilliseconds(1);
            this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck = TimeSpan.FromMilliseconds(1);
            this.app.Reinitialize();
            var timeService = new TimeServiceMock(this.app);
            this.app.Container.Register(timeService);

            var disposed = false;

            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestDisposeOnSlidingExpiration), this, dispose: () => disposed = true, slidingExpiration: TimeSpan.FromMilliseconds(50));
            var item = cache.Get(nameof(TestDisposeOnSlidingExpiration));
            item.Should().Be(this, "returned item should be the same as the original");

            for (int i = 0; i < 75; i++)
            {
                item = cache.Get(nameof(TestDisposeOnSlidingExpiration));
                item.Should().Be(this, $"returned item should be the same as the original after {i} retries");
                disposed.Should().BeFalse($"the item should not have expired after {i} retries");
                Thread.Sleep(1);
            }

            timeService.SetTime(timeService.GetTime().AddMilliseconds(52));
            new Func<bool>(() => disposed).ShouldReturn(true, TimeSpan.FromMilliseconds(10000), "the item should have expired");
            item = cache.Get(nameof(TestDisposeOnSlidingExpiration));
            item.Should().BeNull("the item shold have been removed from the cache");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceMemoryCache() => TestUtils.TestPerformance(this.TestPerformanceMemoryCacheInternal);
        #endregion

        #region Private methods
        private void TestPerformanceMemoryCacheInternal()
        {
            var cache = this.app.GetMemoryCache();
            var items = 700;
            for (int i = 0; i < items; i++)
            {
                cache.Set(i.ToString(), i);
            }
            for (int i = 0; i < items * 100; i++)
            {
                cache.Get<int>((i % items).ToString());
            }
            for (int i = 0; i < items; i++)
            {
                cache.Remove(i.ToString());
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
