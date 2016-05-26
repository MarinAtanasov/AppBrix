// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using AppBrix.Tests;
using AppBrix.Time.Configuration;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Time.Tests
{
    public class TimeServiceTests : IDisposable
    {
        #region Setup and cleanup
        public TimeServiceTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ContainerModule),
                typeof(TimeModule));
            this.app.Start();
        }

        public void Dispose()
        {
            this.app.Stop();
        }
        #endregion

        #region Tests
        [Fact]
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
            app.Stop();
        }

        [Fact]
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
            app.Stop();
        }

        [Fact]
        public void TestUtcToUtcTime()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Utc;
            this.app.Reinitialize();
            var time = DateTime.UtcNow;
            var appTime = app.GetTimeService().ToAppTime(time);
            appTime.Kind.Should().Be(DateTimeKind.Utc, "kind should be converted");
            appTime.Should().Be(time, "returned time should be the same as passed in");
            app.Stop();
        }

        [Fact]
        public void TestLocalToUtcTime()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Utc;
            this.app.Reinitialize();
            var time = DateTime.Now;
            var appTime = app.GetTimeService().ToAppTime(time);
            appTime.Kind.Should().Be(DateTimeKind.Utc, "kind should be converted");
            appTime.Should().Be(time.ToUniversalTime(), "returned time should be equal to the passed in");
            app.Stop();
        }

        [Fact]
        public void TestUtcToLocalTime()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Local;
            this.app.Reinitialize();
            var time = DateTime.UtcNow;
            var appTime = app.GetTimeService().ToAppTime(time);
            appTime.Kind.Should().Be(DateTimeKind.Local, "kind should be converted");
            appTime.Should().Be(time.ToLocalTime(), "returned time should be equal to the passed in");
            app.Stop();
        }

        [Fact]
        public void TestLocalToLocalTime()
        {
            this.app.GetConfig<TimeConfig>().Kind = DateTimeKind.Local;
            this.app.Reinitialize();
            var time = DateTime.Now;
            var appTime = app.GetTimeService().ToAppTime(time);
            appTime.Kind.Should().Be(DateTimeKind.Local, "kind should be converted");
            appTime.Should().Be(time, "returned time should be the same as passed in");
            app.Stop();
        }
        #endregion
        
        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
