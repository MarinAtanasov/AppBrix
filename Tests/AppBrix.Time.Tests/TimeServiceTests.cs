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
    public void TestGetTime()
    {
        var timeBefore = DateTime.UtcNow;
        var time = this.app.GetTime();
        var timeAfter = DateTime.UtcNow;
        time.Should().BeOnOrAfter(timeBefore, "before time should be <= call time");
        time.Kind.Should().Be(DateTimeKind.Utc, "kind is not Utc");
        time.Should().BeOnOrBefore(timeAfter, "after time should be >= call time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetTimeLocal()
    {
        var timeBefore = DateTimeOffset.Now;
        var time = this.app.GetTimeLocal();
        var timeAfter = DateTimeOffset.Now;
        time.Should().BeOnOrAfter(timeBefore, "before time should be <= call time");
        time.Offset.Should().Be(timeBefore.Offset, "offset is not local");
        time.Should().BeOnOrBefore(timeAfter, "after time should be >= call time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetTimeUtc()
    {
        var timeBefore = DateTimeOffset.UtcNow;
        var time = this.app.GetTimeUtc();
        var timeAfter = DateTimeOffset.UtcNow;
        time.Should().BeOnOrAfter(timeBefore, "before time should be <= call time");
        time.Offset.Should().Be(timeBefore.Offset, "offset is not Utc");
        time.Should().BeOnOrBefore(timeAfter, "after time should be >= call time");
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

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDateTimeOffsetLocalSerialization()
    {
        var service = this.app.GetTimeService();
        var time = service.GetTimeLocal();
        time = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, time.Offset);
        var serialized = service.ToString(time);
        service.ToDateTimeOffset(serialized).Should().Be(time, "serialization and deserialization should return the same time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDateTimeOffsetUtcSerialization()
    {
        var service = this.app.GetTimeService();
        var time = service.GetTimeUtc();
        time = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, time.Offset);
        var serialized = service.ToString(time);
        service.ToDateTimeOffset(serialized).Should().Be(time, "serialization and deserialization should return the same time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetTime() => TestUtils.TestPerformance(this.TestPerformanceGetTimeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetTimeLocal() => TestUtils.TestPerformance(this.TestPerformanceGetTimeLocalInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetTimeUtc() => TestUtils.TestPerformance(this.TestPerformanceGetTimeUtcInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceConvertDateTime() => TestUtils.TestPerformance(this.TestPerformanceConvertDateTimeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceConvertDateTimeOffset() => TestUtils.TestPerformance(this.TestPerformanceConvertDateTimeOffsetInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetTimeInternal()
    {
        for (var i = 0; i < 250000; i++)
        {
            this.app.GetTime();
        }
    }

    private void TestPerformanceGetTimeLocalInternal()
    {
        for (var i = 0; i < 250000; i++)
        {
            this.app.GetTime();
        }
    }

    private void TestPerformanceGetTimeUtcInternal()
    {
        for (var i = 0; i < 250000; i++)
        {
            this.app.GetTime();
        }
    }

    private void TestPerformanceConvertDateTimeInternal()
    {
        var time = DateTime.UtcNow;
        var timeService = this.app.GetTimeService();
        for (var i = 0; i < 12500; i++)
        {
            timeService.ToDateTime(timeService.ToString(time));
        }
    }

    private void TestPerformanceConvertDateTimeOffsetInternal()
    {
        var time = DateTimeOffset.UtcNow;
        var timeService = this.app.GetTimeService();
        for (var i = 0; i < 15000; i++)
        {
            timeService.ToDateTimeOffset(timeService.ToString(time));
        }
    }
    #endregion
}
