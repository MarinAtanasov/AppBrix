// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration.Memory;
using AppBrix.Resolver;
using AppBrix.Tests;
using AppBrix.Time.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace AppBrix.Time.Tests
{
    [TestClass]
    public class TimeServiceTests
    {
        #region Tests
        [TestMethod]
        public void TestUtcTimeService()
        {
            var app = this.CreateAppWithTimeModule();
            app.Start();
            app.GetConfig<TimeConfig>().Kind = DateTimeKind.Utc;
            app.Reinitialize();
            var timeBefore = DateTime.UtcNow;
            var time = app.GetTime();
            var timeAfter = DateTime.UtcNow;
            Assert.IsTrue(timeBefore <= time, "Before > call");
            Assert.IsTrue(time.Kind == DateTimeKind.Utc, "Kind is not Utc");
            Assert.IsTrue(timeAfter >= time, "After < call");
            app.Stop();
        }

        [TestMethod]
        public void TestLocalTimeService()
        {
            var app = this.CreateAppWithTimeModule();
            app.Start();
            app.GetConfig<TimeConfig>().Kind = DateTimeKind.Local;
            app.Reinitialize();
            var timeBefore = DateTime.Now;
            var time = app.GetTime();
            var timeAfter = DateTime.Now;
            Assert.IsTrue(timeBefore <= time, "Before > call");
            Assert.IsTrue(time.Kind == DateTimeKind.Local, "Kind is not Local");
            Assert.IsTrue(timeAfter >= time, "After < call");
            app.Stop();
        }
        #endregion

        #region Private methods
        private IApp CreateAppWithTimeModule()
        {
            return TestsUtils.CreateTestApp(
                typeof(ResolverModule),
                typeof(MemoryConfigModule),
                typeof(TimeModule));
        }
        #endregion
    }
}
