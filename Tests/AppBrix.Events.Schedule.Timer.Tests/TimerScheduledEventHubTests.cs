// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Schedule.Contracts;
using AppBrix.Events.Schedule.Timer.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
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
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.App.Start();

        this.timeService = new TimeServiceMock(this.App);
        this.App.Container.Register(this.timeService);
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNullArgsTimer()
    {
        var hub = this.App.GetTimerScheduledEventHub();
        Action action = () => hub.Schedule<EventMock>(null, TimeSpan.FromMinutes(1));
        this.AssertThrows<ArgumentNullException>(action, "args is null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNullArgsDateTime()
    {
        var hub = this.App.GetTimerScheduledEventHub();
        Action action = () => hub.Schedule<EventMock>(null, this.timeService.GetTime().AddMinutes(1));
        this.AssertThrows<ArgumentNullException>(action, "args is null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNegativeDueTimeExpression()
    {
        var hub = this.App.GetTimerScheduledEventHub();
        Action action = () => hub.Schedule(new EventMock(0), TimeSpan.FromMilliseconds(-1));
        this.AssertThrows<ArgumentException>(action, "dueTime must be non-negative");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNegativePeriodExpression()
    {
        var hub = this.App.GetTimerScheduledEventHub();
        Action action = () => hub.Schedule(new EventMock(0), this.timeService.GetTime().AddMinutes(1) , TimeSpan.FromMilliseconds(-1));
        this.AssertThrows<ArgumentException>(action, "period must be non-negative");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestScheduleArgs()
    {
        var called = new bool[3];
        var funcs = Enumerable.Range(0, called.Length).Select<int, Func<bool>>(x => () => called[x]).ToList();
        this.App.GetEventHub().Subscribe<EventMock>(args =>
        {
            called[args.Value] = true;
            for (var i = 0; i < args.Value; i++)
            {
                this.Assert(called[i], "events should be called in order");
            }
        });

        var hub = this.App.GetTimerScheduledEventHub();
        hub.Schedule(new EventMock(0), TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        hub.Schedule(new EventMock(1), TimeSpan.FromMinutes(1), TimeSpan.FromHours(1));
        hub.Schedule(new EventMock(2), this.timeService.GetTime().AddHours(1));

        this.Assert(funcs[0]() == false, "first event should not be called immediately");
        this.Assert(funcs[1]() == false, "second event should not be called immediately");
        this.Assert(funcs[2]() == false, "third event should not be called immediately");

        this.timeService.SetTime(this.timeService.GetTime().AddMinutes(30));
        await this.AssertReturns(funcs[0], true, "first event should have been raised");
        await this.AssertReturns(funcs[1], true, "second event should have been raised");
        this.Assert(funcs[2]() == false, "third event shouldn't be called yet");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnscheduleNullArgs()
    {
        var hub = this.App.GetTimerScheduledEventHub();
        var action = () => hub.Unschedule<EventMock>(null!);
        this.AssertThrows<ArgumentNullException>(action, "args is null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnscheduleArgs()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.App.Reinitialize();
        var called = false;
        this.App.GetEventHub().Subscribe<EventMock>(_ => called = true);
        var hub = this.App.GetTimerScheduledEventHub();
        var scheduledEvent = hub.Schedule(new EventMock(0), TimeSpan.FromHours(1));
        hub.Unschedule(scheduledEvent);
        this.timeService.SetTime(this.timeService.GetTime().AddHours(1));
        Thread.Sleep(5);
        this.Assert(called == false, "event should be unscheduled");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public async Task TestMemoryRelease()
    {
        var called = new bool[2];
        var func = Enumerable.Range(0, called.Length).Select(i => (Func<bool>)(() => called[i])).ToArray();
        this.App.GetEventHub().Subscribe<EventMock>(args =>
        {
            called[args.Value] = true;
            if (args.Value == 0)
                this.App.GetTimerScheduledEventHub().Schedule(new EventMock(1), TimeSpan.Zero);
        });

        var weakReference = this.GetEventMockWeakReference(0);
        var schedule = (WeakReference<EventMock> weakRef) =>
        {
            if (weakRef.TryGetTarget(out var args))
                this.App.GetTimerScheduledEventHub().Schedule(args, TimeSpan.Zero);
        };
        schedule(weakReference);

        await this.AssertReturns(func[0], true, "the first event should have been raised");
        await this.AssertReturns(func[1], true, "the second event should have been raised");

        GC.Collect();
        this.Assert(weakReference.TryGetTarget(out _) == false, "the event hub shouldn't hold references to completed non-recurring events");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceSchedule()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.App.Reinitialize();
        this.AssertPerformance(() => this.TestPerformanceScheduleInternal(new EventMock(0), 50000));
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceUnschedule()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.App.Reinitialize();
        this.AssertPerformance(() => this.TestPerformanceUnscheduleInternal(new EventMock(0), 30000));
    }
    #endregion

    #region Private methods
    private WeakReference<EventMock> GetEventMockWeakReference(int value) => new WeakReference<EventMock>(new EventMock(value));

    private void TestPerformanceScheduleInternal(EventMock eventMock, int repeats)
    {
        var hub = this.App.GetTimerScheduledEventHub();
        for (var i = 0; i < repeats; i++)
        {
            hub.Schedule(eventMock, TimeSpan.FromHours(1));
        }
        this.App.Reinitialize();
    }

    private void TestPerformanceUnscheduleInternal(EventMock eventMock, int repeats)
    {
        var hub = this.App.GetTimerScheduledEventHub();
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
