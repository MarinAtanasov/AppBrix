// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using AppBrix.Tests;
using AppBrix.Time;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading;
using Xunit;

namespace AppBrix.Caching.Memory.Tests
{
    public class MemoryCacheTests : IDisposable
    {
        #region Setup and cleanup
        public MemoryCacheTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(MemoryCachingModule),
                typeof(TimeModule),
                typeof(ContainerModule));
            this.app.Start();
        }

        public void Dispose()
        {
            this.app.Stop();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestGetMemoryCache()
        {
            var cache = this.app.GetMemoryCache();
            cache.Should().NotBeNull("cache must be registered and resolved");
        }

        [Fact]
        public void TestGetNullKey()
        {
            var cache = this.app.GetMemoryCache();
            Action action = () => cache.Get(null);
            action.ShouldThrow<ArgumentNullException>("key should not be null");
        }

        [Fact]
        public void TestGetUnregisteredItem()
        {
            var cache = this.app.GetMemoryCache();
            var item = cache.Get(nameof(TestGetUnregisteredItem));
            item.Should().BeNull("asking for non-existing key should return null");
        }

        [Fact]
        public void TestGetUnregisteredItemGenericExtension()
        {
            var cache = this.app.GetMemoryCache();
            var item = cache.Get<TimeSpan>(nameof(TestGetUnregisteredItem));
            item.Should().Be(default(TimeSpan), "asking for non-existing struct should return its default value");
        }

        [Fact]
        public void TestSetNullKey()
        {
            var cache = this.app.GetMemoryCache();
            Action action = () => cache.Set(null, this);
            action.ShouldThrow<ArgumentNullException>("key should not be null");
        }

        [Fact]
        public void TestSetNullItem()
        {
            var cache = this.app.GetMemoryCache();
            Action action = () => cache.Set(nameof(TestSetNullItem), null);
            action.ShouldThrow<ArgumentNullException>("item should not be null");
        }

        [Fact]
        public void TestSetNegativeAbsoluteExpiration()
        {
            var cache = this.app.GetMemoryCache();
            Action action = () => cache.Set(nameof(TestSetNegativeAbsoluteExpiration), this, absoluteExpiration: TimeSpan.FromSeconds(-1));
            action.ShouldThrow<ArgumentException>("absolute expiration should not be negative");
        }

        [Fact]
        public void TestSetNegativeRollingExpiration()
        {
            var cache = this.app.GetMemoryCache();
            Action action = () => cache.Set(nameof(TestSetNegativeRollingExpiration), this, rollingExpiration: TimeSpan.FromSeconds(-1));
            action.ShouldThrow<ArgumentException>("rolling expiration should not be negative");
        }

        [Fact]
        public void TestRemoveNullKey()
        {
            var cache = this.app.GetMemoryCache();
            Action action = () => cache.Remove(null);
            action.ShouldThrow<ArgumentNullException>("key should not be null");
        }

        [Fact]
        public void TestGetItem()
        {
            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestGetItem), this);
            var item = cache.Get(nameof(TestGetItem));
            item.Should().Be(this, "returned item should be the same as the original");
        }

        [Fact]
        public void TestRemoveItem()
        {
            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestRemoveItem), this);
            cache.Remove(nameof(TestRemoveItem));
            var item = cache.Get(nameof(TestGetItem));
            item.Should().BeNull("the item shold have been removed from the cache");
        }

        [Fact]
        public void TestAbsoluteExpiration()
        {
            this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck = TimeSpan.FromMilliseconds(5);
            this.app.Reinitialize();

            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestAbsoluteExpiration), this, absoluteExpiration: TimeSpan.FromMilliseconds(20));
            var item = cache.Get(nameof(TestAbsoluteExpiration));
            item.Should().Be(this, "returned item should be the same as the original");

            Thread.Sleep(200);
            item = cache.Get(nameof(TestAbsoluteExpiration));
            item.Should().BeNull("the item shold have been removed from the cache");
        }

        [Fact]
        public void TestRollingExpiration()
        {
            this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck = TimeSpan.FromMilliseconds(5);
            this.app.Reinitialize();

            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestRollingExpiration), this, rollingExpiration: TimeSpan.FromMilliseconds(20));
            var item = cache.Get(nameof(TestRollingExpiration));
            item.Should().Be(this, "returned item should be the same as the original");

            for (int i = 0; i < 10; i++)
            {
                item = cache.Get(nameof(TestRollingExpiration));
                item.Should().Be(this, $"returned item should be the same as the original after {i} retries");
                Thread.Sleep(1);
            }

            Thread.Sleep(200);
            item = cache.Get(nameof(TestRollingExpiration));
            item.Should().BeNull("the item shold have been removed from the cache");
        }

        [Fact]
        public void TestDisposeOnAbsoluteExpiration()
        {
            this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck = TimeSpan.FromMilliseconds(5);
            this.app.Reinitialize();

            var disposed = false;

            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestDisposeOnAbsoluteExpiration), this, dispose: () => disposed = true, absoluteExpiration: TimeSpan.FromMilliseconds(20));
            var item = cache.Get(nameof(TestDisposeOnAbsoluteExpiration));
            item.Should().Be(this, "returned item should be the same as the original");
            disposed.Should().BeFalse("the item should not have expired yet");

            Thread.Sleep(200);
            item = cache.Get(nameof(TestDisposeOnAbsoluteExpiration));
            item.Should().BeNull("the item shold have been removed from the cache");
            disposed.Should().BeTrue("the item should have expired");
        }

        [Fact]
        public void TestDisposeOnRollingExpiration()
        {
            this.app.GetConfig<MemoryCachingConfig>().ExpirationCheck = TimeSpan.FromMilliseconds(5);
            this.app.Reinitialize();

            var disposed = false;

            var cache = this.app.GetMemoryCache();
            cache.Set(nameof(TestRollingExpiration), this, dispose: () => disposed = true, rollingExpiration: TimeSpan.FromMilliseconds(20));
            var item = cache.Get(nameof(TestRollingExpiration));
            item.Should().Be(this, "returned item should be the same as the original");

            for (int i = 0; i < 10; i++)
            {
                item = cache.Get(nameof(TestRollingExpiration));
                item.Should().Be(this, $"returned item should be the same as the original after {i} retries");
                disposed.Should().BeFalse($"the item should not have expired after {i} retries");
                Thread.Sleep(1);
            }

            Thread.Sleep(200);
            item = cache.Get(nameof(TestRollingExpiration));
            item.Should().BeNull("the item shold have been removed from the cache");
            disposed.Should().BeTrue("the item should have expired");
        }

        [Fact]
        public void TestPerformanceMemoryCache()
        {
            Action action = this.TestPerformanceMemoryCacheInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }
        #endregion

        #region Private methods
        private void TestPerformanceMemoryCacheInternal()
        {
            var cache = this.app.GetMemoryCache();
            var items = 800;
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
