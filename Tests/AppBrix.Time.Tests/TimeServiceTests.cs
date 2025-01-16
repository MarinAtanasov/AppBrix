// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using Xunit;

namespace AppBrix.Time.Tests;

public sealed class TimeServiceTests : TestsBase<TimeModule>
{
    #region Setup and cleanup
    public TimeServiceTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetTime()
    {
        var timeBefore = DateTime.UtcNow;
        var time = this.App.GetTime();
        var timeAfter = DateTime.UtcNow;
        this.Assert(time >= timeBefore, "before time should be <= call time");
        this.Assert(time.Kind == DateTimeKind.Utc, "kind is not Utc");
        this.Assert(time <= timeAfter, "after time should be >= call time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetTimeLocal()
    {
        var timeBefore = DateTimeOffset.Now;
        var time = this.App.GetTimeLocal();
        var timeAfter = DateTimeOffset.Now;
        this.Assert(time >= timeBefore, "before time should be <= call time");
        this.Assert(time.Offset == timeBefore.Offset, "offset is not local");
        this.Assert(time <= timeAfter, "after time should be >= call time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetTimeUtc()
    {
        var timeBefore = DateTimeOffset.UtcNow;
        var time = this.App.GetTimeUtc();
        var timeAfter = DateTimeOffset.UtcNow;
        this.Assert(time >= timeBefore, "before time should be <= call time");
        this.Assert(time.Offset == timeBefore.Offset, "offset is not Utc");
        this.Assert(time <= timeAfter, "after time should be >= call time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDateTimeSerialization()
    {
        var service = this.App.GetTimeService();
        var time = service.GetTime();
        time = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, DateTimeKind.Utc);
        var serialized = service.ToString(time);
        this.Assert(service.ToDateTime(serialized) == time, "serialization and deserialization should return the same time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDateTimeOffsetLocalSerialization()
    {
        var service = this.App.GetTimeService();
        var time = service.GetTimeLocal();
        time = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, time.Offset);
        var serialized = service.ToString(time);
        this.Assert(service.ToDateTimeOffset(serialized) == time, "serialization and deserialization should return the same time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDateTimeOffsetUtcSerialization()
    {
        var service = this.App.GetTimeService();
        var time = service.GetTimeUtc();
        time = new DateTimeOffset(time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second, time.Millisecond, time.Offset);
        var serialized = service.ToString(time);
        this.Assert(service.ToDateTimeOffset(serialized) == time, "serialization and deserialization should return the same time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetTime() => this.AssertPerformance(this.TestPerformanceGetTimeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetTimeLocal() => this.AssertPerformance(this.TestPerformanceGetTimeLocalInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetTimeUtc() => this.AssertPerformance(this.TestPerformanceGetTimeUtcInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceConvertDateTime() => this.AssertPerformance(this.TestPerformanceConvertDateTimeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceConvertDateTimeOffset() => this.AssertPerformance(this.TestPerformanceConvertDateTimeOffsetInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetTimeInternal()
    {
        for (var i = 0; i < 250000; i++)
        {
            this.App.GetTime();
        }
    }

    private void TestPerformanceGetTimeLocalInternal()
    {
        for (var i = 0; i < 250000; i++)
        {
            this.App.GetTime();
        }
    }

    private void TestPerformanceGetTimeUtcInternal()
    {
        for (var i = 0; i < 250000; i++)
        {
            this.App.GetTime();
        }
    }

    private void TestPerformanceConvertDateTimeInternal()
    {
        var time = DateTime.UtcNow;
        var timeService = this.App.GetTimeService();
        for (var i = 0; i < 12500; i++)
        {
            timeService.ToDateTime(timeService.ToString(time));
        }
    }

    private void TestPerformanceConvertDateTimeOffsetInternal()
    {
        var time = DateTimeOffset.UtcNow;
        var timeService = this.App.GetTimeService();
        for (var i = 0; i < 15000; i++)
        {
            timeService.ToDateTimeOffset(timeService.ToString(time));
        }
    }
    #endregion
}
