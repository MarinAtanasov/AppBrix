// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Schedule.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace AppBrix.Events.Schedule.Tests;

public sealed class ScheduledEventHubTests : TestsBase
{
    #region Setup and cleanup
    public ScheduledEventHubTests() : base(TestUtils.CreateTestApp<ScheduledEventsModule>())
    {
        this.app.Start();
        this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.app.Reinitialize();
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNullArgs()
    {
        var hub = this.app.GetScheduledEventHub();
        Action action = () => hub.Schedule<EventMock>(null);
        action.Should().Throw<ArgumentNullException>("args is null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleArgs()
    {
        var called = false;
        var func = () => called;
        this.app.GetEventHub().Subscribe<EventMock>((args) => called = true);
        var hub = this.app.GetScheduledEventHub();
        hub.Schedule(new ScheduledEventMock<EventMock>(new EventMock(0), TimeSpan.FromMilliseconds(2)));
        called.Should().BeFalse("event should not be called immediately");
        func.ShouldReturn(true, TimeSpan.FromMilliseconds(10000), "event should have been raised");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnscheduleNullArgs()
    {
        var hub = this.app.GetScheduledEventHub();
        Action action = () => hub.Unschedule<EventMock>(null);
        action.Should().Throw<ArgumentNullException>("args is null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnscheduleArgs()
    {
        var called = false;
        this.app.GetEventHub().Subscribe<EventMock>((args) => called = true);
        var hub = this.app.GetScheduledEventHub();
        var scheduledEvent = new ScheduledEventMock<EventMock>(new EventMock(0), TimeSpan.FromMilliseconds(2));
        hub.Schedule(scheduledEvent);
        hub.Unschedule(scheduledEvent);
        Thread.Sleep(5);
        called.Should().BeFalse("event should be unscheduled");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestMemoryRelease()
    {
        var called = false;
        var func = () => called;
        this.app.GetEventHub().Subscribe<EventMock>(args => called = true);

        var weakReference = this.GetEventMockWeakReference(0);
        var schedule = (WeakReference<ScheduledEventMock<EventMock>> weakRef) =>
        {
            var hub = this.app.GetScheduledEventHub();
            weakRef.TryGetTarget(out var args);
            hub.Schedule(args);
        };
        schedule(weakReference);

        func.ShouldReturn(true, TimeSpan.FromMilliseconds(10000), "event should have been raised");

        GC.Collect();
        weakReference.TryGetTarget(out _).Should().BeFalse("the event hub shouldn't hold references to completed non-reccuring events");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceSchedule()
    {
        this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.app.Reinitialize();
        var time = this.app.GetTime().AddHours(1);
        var scheduledEvents = Enumerable.Range(0, 50000)
            .Select(x => new ScheduledEventMock<EventMock>(new EventMock(0), time))
            .ToList();
        TestUtils.TestPerformance(() => this.TestPerformanceScheduleInternal(scheduledEvents));
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceUnschedule()
    {
        this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.app.Reinitialize();
        var time = this.app.GetTime().AddHours(1);
        var scheduledEvents = Enumerable.Range(0, 50000)
            .Select(x => new ScheduledEventMock<EventMock>(new EventMock(0), time))
            .ToList();
        TestUtils.TestPerformance(() => this.TestPerformanceUnscheduleInternal(scheduledEvents));
    }
    #endregion

    #region Private methods
    private WeakReference<ScheduledEventMock<EventMock>> GetEventMockWeakReference(int value) =>
        new WeakReference<ScheduledEventMock<EventMock>>(new ScheduledEventMock<EventMock>(new EventMock(value), TimeSpan.FromMilliseconds(1)));
    
    private void TestPerformanceScheduleInternal(List<ScheduledEventMock<EventMock>> scheduledEvents)
    {
        var hub = this.app.GetScheduledEventHub();
        for (var i = 0; i < scheduledEvents.Count; i++)
        {
            hub.Schedule(scheduledEvents[i]);
        }
        this.app.Reinitialize();
    }

    private void TestPerformanceUnscheduleInternal(List<ScheduledEventMock<EventMock>> scheduledEvents)
    {
        var hub = this.app.GetScheduledEventHub();
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
