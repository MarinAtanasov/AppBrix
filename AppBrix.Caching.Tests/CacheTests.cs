// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Resolver;
using AppBrix.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Caching.Tests
{
    [TestClass]
    public class CacheTests
    {
        #region Setup and cleanup
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            CacheTests.app = TestUtils.CreateTestApp(
                typeof(ResolverModule),
                typeof(CacheModule));
            CacheTests.app.Start();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CacheTests.app.Stop();
            CacheTests.app = null;
        }

        [TestCleanup]
        public void Cleanup()
        {
            CacheTests.app.Reinitialize();
        }
        #endregion

        #region Tests
        [TestMethod]
        public void TestGetCache()
        {
            var cache = CacheTests.app.GetCache();
            Assert.IsNotNull(cache, "No cache found.");
        }

        [TestMethod]
        public void TestCacheItem()
        {
            var cache = CacheTests.app.GetCache();
            var key = "key";
            var value = new object();
            cache[key] = value;
            Assert.IsTrue(cache.Contains(key), "Item not found.");
            Assert.AreSame(value, cache[key], "Different object returned.");
            cache.Remove(key);
            Assert.IsFalse(cache.Contains(key), "Item not removed.");
        }

        [TestMethod]
        public void TestReplaceItem()
        {
            var cache = CacheTests.app.GetCache();
            var key = "key";
            var value = new object();
            cache[key] = value;
            Assert.IsTrue(cache.Contains(key), "Item not found.");
            Assert.AreSame(value, cache[key], "Different object returned.");
            value = new object();
            cache[key] = value;
            Assert.IsTrue(cache.Contains(key), "Item not found after replace.");
            Assert.AreSame(value, cache[key], "Different object returned after replace.");
            cache.Remove(key);
            Assert.IsFalse(cache.Contains(key), "Item not removed.");
        }
        #endregion

        #region Private fields and constants
        private static IApp app;
        #endregion
    }
}
