// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Schedule.Tests.Mocks;
using AppBrix.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppBrix.Events.Schedule.Tests;

[TestClass]
public sealed class ScheduledEventHubTests : TestsBase<ScheduledEventsModule>
{
    #region Setup and cleanup
    protected override void Initialize()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.App.Start();
    }
    #endregion

    #region Tests
    [Test, Functional]
    public void TestScheduleNullArgs()
    {
        var hub = this.App.GetScheduledEventHub();
        var action = () => hub.Schedule<EventMock>(null!);
        this.AssertThrows<ArgumentNullException>(action, "args is null");;
    }

    [Test, Functional]
    public void TestScheduleDelayedRaise()
    {
        var called = false;
        this.App.GetEventHub().Subscribe<EventMock>(_ => called = true);
        this.App.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(0), TimeSpan.FromMinutes(1)));
        this.Assert(called == false, "event should not be called immediately");
    }

    [Test, Functional]
    public Task TestScheduleArgs()
    {
        var called = false;
        var func = () => called;
        this.App.GetEventHub().Subscribe<EventMock>(_ => called = true);
        this.App.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(0), TimeSpan.FromMilliseconds(2)));
        return this.AssertReturns(func, true, "event should have been raised");
    }

    [Test, Functional]
    public async Task TestScheduleThreeArgs()
    {
        var called = new bool[3];
        this.App.GetEventHub().Subscribe<EventMock>(args =>
        {
            called[args.Value] = true;
            throw new InvalidOperationException();
        });
        this.App.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(2), TimeSpan.FromHours(1)));
        this.App.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(1), TimeSpan.Zero));
        this.App.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(0), TimeSpan.Zero));

        var func = () => called[0];
        await this.AssertReturns(func, true, "first event should have been raised");
        this.Assert(called[1], "second event should be raised");
        this.Assert(called[2] == false, "third event should not be raised");
    }

    [Test, Functional]
    public void TestUnscheduleNullArgs()
    {
        var hub = this.App.GetScheduledEventHub();
        var action = () => hub.Unschedule<EventMock>(null!);
        this.AssertThrows<ArgumentNullException>(action, "args is null");;
    }

    [Test, Functional]
    public async Task TestUnscheduleArgs()
    {
        var called = new bool[2];
        this.App.GetEventHub().Subscribe<EventMock>(args => called[args.Value] = true);
        var scheduledEvent = new ScheduledEventMock<EventMock>(new EventMock(0), TimeSpan.FromMilliseconds(5));
        this.App.GetScheduledEventHub().Schedule(scheduledEvent);
        this.App.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(1), TimeSpan.FromMilliseconds(5)));
        this.App.GetScheduledEventHub().Unschedule(scheduledEvent);

        var func = () => called[1];
        await this.AssertReturns(func, true, "second event should have been raised");
        this.Assert(called[0] == false, "first event should not be raised");
    }

    [Test, Functional]
    public async Task TestMemoryRelease()
    {
        var called = new bool[2];
        var func = Enumerable.Range(0, called.Length).Select(i => (Func<bool>)(() => called[i])).ToArray();
        this.App.GetEventHub().Subscribe<EventMock>(args =>
        {
            called[args.Value] = true;
            if (args.Value == 0)
                this.App.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(1), TimeSpan.Zero));
        });

        var weakReference = this.GetEventMockWeakReference(0);
        var schedule = (WeakReference<ScheduledEventMock<EventMock>> weakRef) =>
        {
            if (weakRef.TryGetTarget(out var args))
                this.App.GetScheduledEventHub().Schedule(args);
        };
        schedule(weakReference);

        await this.AssertReturns(func[0], true, "the first event should have been raised");
        await this.AssertReturns(func[1], true, "the second event should have been raised");

        GC.Collect();
        this.Assert(weakReference.TryGetTarget(out var _) == false, "the event hub shouldn't hold references to completed non-recurring events");
    }

    [Test, Performance]
    public void TestPerformanceSchedule()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.App.Reinitialize();
        var time = this.App.GetTime().AddHours(1);
        var scheduledEvents = Enumerable.Range(0, 80000)
            .Select(_ => new ScheduledEventMock<EventMock>(new EventMock(0), time))
            .ToList();
        this.AssertPerformance(() => this.TestPerformanceScheduleInternal(scheduledEvents));
    }

    [Test, Performance]
    public void TestPerformanceUnschedule()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.App.Reinitialize();
        var time = this.App.GetTime().AddHours(1);
        var scheduledEvents = Enumerable.Range(0, 80000)
            .Select(_ => new ScheduledEventMock<EventMock>(new EventMock(0), time))
            .ToList();
        this.AssertPerformance(() => this.TestPerformanceUnscheduleInternal(scheduledEvents));
    }
    #endregion

    #region Private methods
    private WeakReference<ScheduledEventMock<EventMock>> GetEventMockWeakReference(int value) =>
        new WeakReference<ScheduledEventMock<EventMock>>(new ScheduledEventMock<EventMock>(new EventMock(value), TimeSpan.Zero));

    private void TestPerformanceScheduleInternal(List<ScheduledEventMock<EventMock>> scheduledEvents)
    {
        var hub = this.App.GetScheduledEventHub();
        foreach (var scheduledEvent in scheduledEvents)
        {
            hub.Schedule(scheduledEvent);
        }
        this.App.Reinitialize();
    }

    private void TestPerformanceUnscheduleInternal(List<ScheduledEventMock<EventMock>> scheduledEvents)
    {
        var hub = this.App.GetScheduledEventHub();
        for (var i = 0; i < scheduledEvents.Count; i++)
        {
            hub.Schedule(scheduledEvents[i]);
        }
        for (var i = scheduledEvents.Count - 1; i >= 0; i--)
        {
            hub.Unschedule(scheduledEvents[i]);
        }
    }
    #endregion
}
