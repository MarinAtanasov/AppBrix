// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Schedule.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
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
        this.app.GetEventHub().Subscribe<EventMock>(_ => called = true);
        this.app.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(0), TimeSpan.FromMilliseconds(2)));
        called.Should().BeFalse("event should not be called immediately");
        func.ShouldReturn(true, "event should have been raised");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleThreeArgs()
    {
        var called = new bool[3];
        this.app.GetEventHub().Subscribe<EventMock>(args =>
        {
            called[args.Value] = true;
            throw new InvalidOperationException();
        });
        this.app.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(2), TimeSpan.FromHours(1)));
        this.app.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(1), TimeSpan.Zero));
        this.app.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(0), TimeSpan.Zero));

        var func = () => called[0];
        func.ShouldReturn(true, "first event should have been raised");
        called[1].Should().BeTrue("second event should be raised");
        called[2].Should().BeFalse("third event should not be raised");
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
        var called = new bool[2];
        this.app.GetEventHub().Subscribe<EventMock>(args => called[args.Value] = true);
        var scheduledEvent = new ScheduledEventMock<EventMock>(new EventMock(0), TimeSpan.FromMilliseconds(2));
        this.app.GetScheduledEventHub().Schedule(scheduledEvent);
        this.app.GetScheduledEventHub().Schedule(new ScheduledEventMock<EventMock>(new EventMock(1), TimeSpan.FromMilliseconds(2)));
        this.app.GetScheduledEventHub().Unschedule(scheduledEvent);

        var func = () => called[1];
        func.ShouldReturn(true, "first event should have been raised");
        called[0].Should().BeFalse("first event should not be raised");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestMemoryRelease()
    {
        var called = false;
        var func = () => called;
        this.app.GetEventHub().Subscribe<EventMock>(_ => called = true);

        var weakReference = this.GetEventMockWeakReference(0);
        var schedule = (WeakReference<ScheduledEventMock<EventMock>> weakRef) =>
        {
            var hub = this.app.GetScheduledEventHub();
            weakRef.TryGetTarget(out var args);
            hub.Schedule(args);
        };
        schedule(weakReference);

        func.ShouldReturn(true, "event should have been raised");

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
            .Select(_ => new ScheduledEventMock<EventMock>(new EventMock(0), time))
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
            .Select(_ => new ScheduledEventMock<EventMock>(new EventMock(0), time))
            .ToList();
        TestUtils.TestPerformance(() => this.TestPerformanceUnscheduleInternal(scheduledEvents));
    }
    #endregion

    #region Private methods
    private WeakReference<ScheduledEventMock<EventMock>> GetEventMockWeakReference(int value) =>
        new WeakReference<ScheduledEventMock<EventMock>>(new ScheduledEventMock<EventMock>(new EventMock(value), TimeSpan.Zero));

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
