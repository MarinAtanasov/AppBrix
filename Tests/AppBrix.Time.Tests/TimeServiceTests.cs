// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Tests;
using AppBrix.Time.Configuration;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Time.Tests
{
    public sealed class TimeServiceTests
    {
        #region Setup and cleanup
        public TimeServiceTests()
        {
            this.app = TestUtils.CreateTestApp(typeof(TimeModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUtcTimeService()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Utc;
            this.app.Reinitialize();
            var timeBefore = DateTime.UtcNow;
            var time = this.app.GetTime();
            var timeAfter = DateTime.UtcNow;
            time.Should().BeOnOrAfter(timeBefore, "before time should be <= call time");
            time.Kind.Should().Be(DateTimeKind.Utc, "kind is not Utc");
            time.Should().BeOnOrBefore(timeAfter, "after time should be >= call time");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestLocalTimeService()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Local;
            this.app.Reinitialize();
            var timeBefore = DateTime.Now;
            var time = this.app.GetTime();
            var timeAfter = DateTime.Now;
            time.Should().BeOnOrAfter(timeBefore, "before time should be <= call time");
            time.Kind.Should().Be(DateTimeKind.Local, "kind is not Local");
            time.Should().BeOnOrBefore(timeAfter, "after time should be >= call time");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUtcToUtcTime()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Utc;
            this.app.Reinitialize();
            var time = DateTime.UtcNow;
            var appTime = app.GetTimeService().ToAppTime(time);
            appTime.Kind.Should().Be(DateTimeKind.Utc, "kind should be converted");
            appTime.Should().Be(time, "returned time should be the same as passed in");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestLocalToUtcTime()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Utc;
            this.app.Reinitialize();
            var time = DateTime.Now;
            var appTime = app.GetTimeService().ToAppTime(time);
            appTime.Kind.Should().Be(DateTimeKind.Utc, "kind should be converted");
            appTime.Should().Be(time.ToUniversalTime(), "returned time should be equal to the passed in");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUtcToLocalTime()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Local;
            this.app.Reinitialize();
            var time = DateTime.UtcNow;
            var appTime = app.GetTimeService().ToAppTime(time);
            appTime.Kind.Should().Be(DateTimeKind.Local, "kind should be converted");
            appTime.Should().Be(time.ToLocalTime(), "returned time should be equal to the passed in");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestLocalToLocalTime()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Local;
            this.app.Reinitialize();
            var time = DateTime.Now;
            var appTime = app.GetTimeService().ToAppTime(time);
            appTime.Kind.Should().Be(DateTimeKind.Local, "kind should be converted");
            appTime.Should().Be(time, "returned time should be the same as passed in");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceGetTime()
        {
            TestUtils.TestPerformance(this.TestPerformanceGetTimeInternal);
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceToAppTime()
        {
            TestUtils.TestPerformance(this.TestPerformanceToAppTimeInternal);
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceConvertTime()
        {
            TestUtils.TestPerformance(this.TestPerformanceConvertTimeInternal);
        }
        #endregion

        #region Private methods
        private void TestPerformanceGetTimeInternal()
        {
            for (int i = 0; i < 250000; i++)
            {
                app.GetTime();
            }
        }

        private void TestPerformanceToAppTimeInternal()
        {
            var utcTime = DateTime.UtcNow;
            var localTime = utcTime.ToLocalTime();
            var timeService = app.GetTimeService();

            for (int i = 0; i < 75000; i++)
            {
                timeService.ToAppTime(utcTime);
                timeService.ToAppTime(localTime);
            }
        }

        private void TestPerformanceConvertTimeInternal()
        {
            var time = DateTime.UtcNow;
            var timeService = app.GetTimeService();

            for (int i = 0; i < 12000; i++)
            {
                timeService.ToDateTime(timeService.ToString(time));
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
