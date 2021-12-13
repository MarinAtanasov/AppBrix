// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Tests;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Time.Tests;

public sealed class TimeServiceTests : TestsBase
{
    #region Setup and cleanup
    public TimeServiceTests() : base(TestUtils.CreateTestApp<TimeModule>()) => this.app.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUtcTimeService()
    {
        this.app.ConfigService.GetTimeConfig().Kind = DateTimeKind.Utc;
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
        this.app.ConfigService.GetTimeConfig().Kind = DateTimeKind.Local;
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
        this.app.ConfigService.GetTimeConfig().Kind = DateTimeKind.Utc;
        this.app.Reinitialize();
        var time = DateTime.UtcNow;
        var appTime = this.app.GetTimeService().ToAppTime(time);
        appTime.Kind.Should().Be(DateTimeKind.Utc, "kind should be converted");
        appTime.Should().Be(time, "returned time should be the same as passed in");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestLocalToUtcTime()
    {
        this.app.ConfigService.GetTimeConfig().Kind = DateTimeKind.Utc;
        this.app.Reinitialize();
        var time = DateTime.Now;
        var appTime = this.app.GetTimeService().ToAppTime(time);
        appTime.Kind.Should().Be(DateTimeKind.Utc, "kind should be converted");
        appTime.Should().Be(time.ToUniversalTime(), "returned time should be equal to the passed in");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUtcToLocalTime()
    {
        this.app.ConfigService.GetTimeConfig().Kind = DateTimeKind.Local;
        this.app.Reinitialize();
        var time = DateTime.UtcNow;
        var appTime = this.app.GetTimeService().ToAppTime(time);
        appTime.Kind.Should().Be(DateTimeKind.Local, "kind should be converted");
        appTime.Should().Be(time.ToLocalTime(), "returned time should be equal to the passed in");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestLocalToLocalTime()
    {
        this.app.ConfigService.GetTimeConfig().Kind = DateTimeKind.Local;
        this.app.Reinitialize();
        var time = DateTime.Now;
        var appTime = app.GetTimeService().ToAppTime(time);
        appTime.Kind.Should().Be(DateTimeKind.Local, "kind should be converted");
        appTime.Should().Be(time, "returned time should be the same as passed in");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDateTimeSerialization()
    {
        var service = this.app.GetTimeService();
        var time = service.GetTime();
        time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Utc);
        var serialized = service.ToString(time);
        service.ToDateTime(serialized).Should().Be(time, "serialization and deserialization should return the same time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetTime() => TestUtils.TestPerformance(this.TestPerformanceGetTimeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceToAppTime() => TestUtils.TestPerformance(this.TestPerformanceToAppTimeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceConvertTime() => TestUtils.TestPerformance(this.TestPerformanceConvertTimeInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetTimeInternal()
    {
        for (var i = 0; i < 200000; i++)
        {
            this.app.GetTime();
        }
    }

    private void TestPerformanceToAppTimeInternal()
    {
        var utcTime = DateTime.UtcNow;
        var localTime = utcTime.ToLocalTime();
        var timeService = this.app.GetTimeService();

        for (var i = 0; i < 100000; i++)
        {
            timeService.ToAppTime(utcTime);
            timeService.ToAppTime(localTime);
        }
    }

    private void TestPerformanceConvertTimeInternal()
    {
        var time = DateTime.UtcNow;
        var timeService = this.app.GetTimeService();

        for (var i = 0; i < 15000; i++)
        {
            timeService.ToDateTime(timeService.ToString(time));
        }
    }
    #endregion
}
