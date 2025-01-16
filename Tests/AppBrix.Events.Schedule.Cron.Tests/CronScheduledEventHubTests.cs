// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Schedule.Contracts;
using AppBrix.Events.Schedule.Cron.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AppBrix.Events.Schedule.Cron.Tests;

public sealed class CronScheduledEventHubTests : TestsBase<CronScheduledEventsModule>
{
    #region Setup and cleanup
    public CronScheduledEventHubTests()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
        this.App.Start();

        this.timeService = new TimeServiceMock(this.App);
        this.App.Container.Register(this.timeService);
    }
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNullArgs()
    {
        var hub = this.App.GetCronScheduledEventHub();
        Action action = () => hub.Schedule<EventMock>(null, CronScheduledEventHubTests.EveryMinute);
        this.AssertThrows<ArgumentNullException>(action, "args is null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleNullExpression()
    {
        var hub = this.App.GetCronScheduledEventHub();
        Action action = () => hub.Schedule(new EventMock(0), null!);
        this.AssertThrows<ArgumentNullException>(action, "expression is null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestScheduleEmptyExpression()
    {
        var hub = this.App.GetCronScheduledEventHub();
        Action action = () => hub.Schedule(new EventMock(0), string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "expression is empty");;
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

        var hub = this.App.GetCronScheduledEventHub();
        hub.Schedule(new EventMock(0), CronScheduledEventHubTests.EveryMinute);
        hub.Schedule(new EventMock(1), CronScheduledEventHubTests.EveryMinute);
        //hub.Schedule(new EventMock(2), CronScheduledEventHubTests.EveryHour);

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
        var hub = this.App.GetCronScheduledEventHub();
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
        var hub = this.App.GetCronScheduledEventHub();
        var scheduledEvent = hub.Schedule(new EventMock(0), CronScheduledEventHubTests.EveryHour);
        hub.Unschedule(scheduledEvent);
        this.timeService.SetTime(this.timeService.GetTime().AddHours(1));
        Thread.Sleep(5);
        this.Assert(called == false, "event should be unscheduled");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceSchedule()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.App.Reinitialize();
        this.AssertPerformance(() => this.TestPerformanceScheduleInternal(new EventMock(0), 5000));
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceUnschedule()
    {
        this.App.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
        this.App.Reinitialize();
        this.AssertPerformance(() => this.TestPerformanceUnscheduleInternal(new EventMock(0), 5000));
    }
    #endregion

    #region Private methods
    private void TestPerformanceScheduleInternal(EventMock eventMock, int repeats)
    {
        var hub = this.App.GetCronScheduledEventHub();

        for (var i = 0; i < repeats; i++)
        {
            hub.Schedule(eventMock, CronScheduledEventHubTests.EveryHour);
        }

        this.App.Reinitialize();
    }

    private void TestPerformanceUnscheduleInternal(EventMock eventMock, int repeats)
    {
        var hub = this.App.GetCronScheduledEventHub();
        var scheduledEvents = new List<IScheduledEvent<EventMock>>(repeats);

        for (var i = 0; i < repeats; i++)
        {
            scheduledEvents.Add(hub.Schedule(eventMock, CronScheduledEventHubTests.EveryHour));
        }

        for (var i = scheduledEvents.Count - 1; i >= 0; i--)
        {
            hub.Unschedule(scheduledEvents[i]);
        }
    }
    #endregion

    #region Private fields and constants
    private const string EveryMinute = "* * * * *";
    private const string EveryHour = "* */1 * * *";
    private readonly TimeServiceMock timeService;
    #endregion
}
