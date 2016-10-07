// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Caching.Json;
using AppBrix.Container;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Caching.Tests
{
    public class CacheTests
    {
        #region Setup and cleanup
        public CacheTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(CachingModule),
                typeof(JsonCachingModule),
                typeof(ContainerModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestGetCache()
        {
            var cache = this.app.GetCache();
            cache.Should().NotBeNull("cache must be registered and resolved");
        }

        [Fact]
        public void TestCacheItem()
        {
            var cache = this.app.GetCache();
            var key = "key";
            var value = "Test Value";
            cache.Set(key, value);
            var cached = cache.Get<object>(key).Result;
            cached.Should().NotBeNull("the item should be in the cache");
            cached.Should().Be(value, "the returned object should be equal to the original");
            cache.Remove(key).Wait();
            cache.Get<object>(key).Result.Should().BeNull("item should have been removed");
        }

        [Fact]
        public void TestReplaceItem()
        {
            var cache = this.app.GetCache();
            var key = "key";
            var value = "Test Value";
            cache.Set(key, value);
            var cached = cache.Get<object>(key).Result;
            cached.Should().NotBeNull("the item should be in the cache");
            cached.Should().Be(value, "the returned object should be equal to the original");
            value = "Test Replaced Value";
            cache.Set(key, value).Wait();
            cached = cache.Get<object>(key).Result;
            cached.Should().NotBeNull("the item should be in the cache after being replaced");
            cached.Should().Be(value, "the returned object should be equal to the replaced");
            cache.Remove(key).Wait();
            cache.Get<object>(key).Result.Should().BeNull("item should have been removed");
        }

        [Fact]
        public void TestPerformanceCache()
        {
            Action action = this.TestPerformanceCacheInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }
        #endregion

        #region Private methods
        private void TestPerformanceCacheInternal()
        {
            var cache = this.app.GetCache();
            var items = 4000;
            for (int i = 0; i < items; i++)
            {
                cache.Set(i.ToString(), i);
            }
            for (int i = 0; i < items; i++)
            {
                cache.Get<int>(i.ToString());
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
