// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Schedule.Contracts;
using AppBrix.Events.Schedule.Timer.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AppBrix.Events.Schedule.Timer.Tests;

public sealed class TimerScheduledEventHubTests : TestsBase<TimerScheduledEventsModule>
{
    #region Setup and cleanup
    public TimerScheduledEventHubTests()
    {
        this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.app.Start();

        this.timeService = new TimeServiceMock(this.app);
        this.app.Container.Register(this.timeService);
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNullArgsTimer()
    {
        var hub = this.app.GetTimerScheduledEventHub();
        Action action = () => hub.Schedule<EventMock>(null, TimeSpan.FromMinutes(1));
        action.Should().Throw<ArgumentNullException>("args is null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNullArgsDateTime()
    {
        var hub = this.app.GetTimerScheduledEventHub();
        Action action = () => hub.Schedule<EventMock>(null, this.timeService.GetTime().AddMinutes(1));
        action.Should().Throw<ArgumentNullException>("args is null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNegativeDueTimeExpression()
    {
        var hub = this.app.GetTimerScheduledEventHub();
        Action action = () => hub.Schedule(new EventMock(0), TimeSpan.FromMilliseconds(-1));
        action.Should().Throw<ArgumentException>("dueTime must be non-negative");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNegativePeriodExpression()
    {
        var hub = this.app.GetTimerScheduledEventHub();
        Action action = () => hub.Schedule(new EventMock(0), this.timeService.GetTime().AddMinutes(1) , TimeSpan.FromMilliseconds(-1));
        action.Should().Throw<ArgumentException>("period must be non-negative");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestScheduleArgs()
    {
        var called = new bool[3];
        var funcs = Enumerable.Range(0, called.Length).Select<int, Func<bool>>(x => () => called[x]).ToList();
        this.app.GetEventHub().Subscribe<EventMock>(args =>
        {
            called[args.Value] = true;
            for (var i = 0; i < args.Value; i++)
            {
                called[i].Should().Be(true, "events should be called in order");
            }
        });

        var hub = this.app.GetTimerScheduledEventHub();
        hub.Schedule(new EventMock(0), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        hub.Schedule(new EventMock(1), TimeSpan.FromMinutes(1), TimeSpan.FromHours(1));
        hub.Schedule(new EventMock(2), this.timeService.GetTime().AddHours(1));

        funcs[0]().Should().BeFalse("first event should not be called immediately");
        funcs[1]().Should().BeFalse("second event should not be called immediately");
        funcs[2]().Should().BeFalse("third event should not be called immediately");

        this.timeService.SetTime(this.timeService.GetTime().AddMinutes(30));
        await funcs[0].ShouldReturn(true, "first event should have been raised");
        await funcs[1].ShouldReturn(true, "second event should have been raised");
        funcs[2]().Should().BeFalse("third event shouldn't be called yet");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnscheduleNullArgs()
    {
        var hub = this.app.GetTimerScheduledEventHub();
        var action = () => hub.Unschedule<EventMock>(null!);
        action.Should().Throw<ArgumentNullException>("args is null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnscheduleArgs()
    {
        this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.app.Reinitialize();
        var called = false;
        this.app.GetEventHub().Subscribe<EventMock>(_ => called = true);
        var hub = this.app.GetTimerScheduledEventHub();
        var scheduledEvent = hub.Schedule(new EventMock(0), TimeSpan.FromHours(1));
        hub.Unschedule(scheduledEvent);
        this.timeService.SetTime(this.timeService.GetTime().AddHours(1));
        Thread.Sleep(5);
        called.Should().BeFalse("event should be unscheduled");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestMemoryRelease()
    {
        var called = new bool[2];
        var func = Enumerable.Range(0, called.Length).Select(i => (Func<bool>)(() => called[i])).ToArray();
        this.app.GetEventHub().Subscribe<EventMock>(args =>
        {
            called[args.Value] = true;
            if (args.Value == 0)
                this.app.GetTimerScheduledEventHub().Schedule(new EventMock(1), TimeSpan.Zero);
        });

        var weakReference = this.GetEventMockWeakReference(0);
        var schedule = (WeakReference<EventMock> weakRef) =>
        {
            if (weakRef.TryGetTarget(out var args))
                this.app.GetTimerScheduledEventHub().Schedule(args, TimeSpan.Zero);
        };
        schedule(weakReference);

        await func[0].ShouldReturn(true, "the first event should have been raised");
        await func[1].ShouldReturn(true, "the second event should have been raised");

        GC.Collect();
        weakReference.TryGetTarget(out _).Should().BeFalse("the event hub shouldn't hold references to completed non-recurring events");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceSchedule()
    {
        this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.app.Reinitialize();
        this.AssertPerformance(() => this.TestPerformanceScheduleInternal(new EventMock(0), 50000));
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceUnschedule()
    {
        this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.app.Reinitialize();
        this.AssertPerformance(() => this.TestPerformanceUnscheduleInternal(new EventMock(0), 30000));
    }
    #endregion

    #region Private methods
    private WeakReference<EventMock> GetEventMockWeakReference(int value) => new WeakReference<EventMock>(new EventMock(value));

    private void TestPerformanceScheduleInternal(EventMock eventMock, int repeats)
    {
        var hub = this.app.GetTimerScheduledEventHub();
        for (var i = 0; i < repeats; i++)
        {
            hub.Schedule(eventMock, TimeSpan.FromHours(1));
        }
        this.app.Reinitialize();
    }

    private void TestPerformanceUnscheduleInternal(EventMock eventMock, int repeats)
    {
        var hub = this.app.GetTimerScheduledEventHub();
        var scheduledEvents = new List<IScheduledEvent<EventMock>>(repeats);
        for (var i = 0; i < repeats; i++)
        {
            scheduledEvents.Add(hub.Schedule(eventMock, TimeSpan.FromHours(1)));
        }
        for (var i = scheduledEvents.Count - 1; i >= 0; i--)
        {
            hub.Unschedule(scheduledEvents[i]);
        }
    }
    #endregion

    #region Private fields and constants
    private readonly TimeServiceMock timeService;
    #endregion
}
