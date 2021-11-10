// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Schedule.Cron.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace AppBrix.Events.Schedule.Cron.Tests
{
    public sealed class CronScheduledEventHubTests : TestsBase
    {
        #region Setup and cleanup
        public CronScheduledEventHubTests() : base(TestUtils.CreateTestApp<CronScheduledEventsModule>())
        {
            this.app.Start();
            this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
            this.app.Reinitialize();
            this.timeService = new TimeServiceMock(this.app);
            this.app.Container.Register(this.timeService);
        }
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestScheduleNullArgs()
        {
            var hub = this.app.GetCronScheduledEventHub();
            Action action = () => hub.Schedule<EventMock>(null, CronScheduledEventHubTests.EveryMinute);
            action.Should().Throw<ArgumentNullException>("args is null");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestScheduleNullExpression()
        {
            var hub = this.app.GetCronScheduledEventHub();
            Action action = () => hub.Schedule(new EventMock(0), null);
            action.Should().Throw<ArgumentNullException>("expression is null");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestScheduleEmptyExpression()
        {
            var hub = this.app.GetCronScheduledEventHub();
            Action action = () => hub.Schedule(new EventMock(0), string.Empty);
            action.Should().Throw<ArgumentNullException>("expression is empty");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestScheduleArgs()
        {
            var called = new bool[3];
            this.app.GetEventHub().Subscribe<EventMock>(args =>
            {
                called[args.Value] = true;
                for (var i = 0; i < args.Value; i++)
                {
                    called[i].Should().Be(true, "events should be called in order");
                }
            });
            var hub = this.app.GetCronScheduledEventHub();
            hub.Schedule(new EventMock(0), CronScheduledEventHubTests.EveryMinute);
            hub.Schedule(new EventMock(1), CronScheduledEventHubTests.EveryMinute);
            called[0].Should().BeFalse("first event should not be called immediately");
            called[1].Should().BeFalse("second event should not be called immediately");
            this.timeService.SetTime(this.timeService.GetTime().AddMinutes(30));
            var funcs = Enumerable.Range(0, called.Length)
                .Select(x => new Func<bool>(() => called[x]))
                .ToList();
            funcs[0].ShouldReturn(true, TimeSpan.FromMilliseconds(10000), "first event should have been raised");
            funcs[1].ShouldReturn(true, TimeSpan.FromMilliseconds(10000), "second event should have been raised");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUnscheduleNullArgs()
        {
            var hub = this.app.GetCronScheduledEventHub();
            Action action = () => hub.Unschedule<EventMock>(null);
            action.Should().Throw<ArgumentNullException>("args is null");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUnscheduleArgs()
        {
            this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromMilliseconds(1);
            this.app.Reinitialize();
            var called = false;
            this.app.GetEventHub().Subscribe<EventMock>((args) => called = true);
            var hub = this.app.GetCronScheduledEventHub();
            var scheduledEvent = hub.Schedule(new EventMock(0), CronScheduledEventHubTests.EveryHour);
            hub.Unschedule(scheduledEvent);
            this.timeService.SetTime(this.timeService.GetTime().AddHours(1));
            Thread.Sleep(5);
            called.Should().BeFalse("event should be unscheduled");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceSchedule()
        {
            this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
            this.app.Reinitialize();
            TestUtils.TestPerformance(() => this.TestPerformanceScheduleInternal(new EventMock(0), 3000));
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceUnschedule()
        {
            this.app.ConfigService.GetScheduledEventsConfig().ExecutionCheck = TimeSpan.FromHours(1);
            this.app.Reinitialize();
            TestUtils.TestPerformance(() => this.TestPerformanceUnscheduleInternal(new EventMock(0), 5000));
        }
        #endregion

        #region Private methods
        private void TestPerformanceScheduleInternal(EventMock eventMock, int repeats)
        {
            var hub = this.app.GetCronScheduledEventHub();
            for (var i = 0; i < repeats; i++)
            {
                hub.Schedule(eventMock, CronScheduledEventHubTests.EveryHour);
            }
            this.app.Reinitialize();
        }

        private void TestPerformanceUnscheduleInternal(EventMock eventMock, int repeats)
        {
            var hub = this.app.GetCronScheduledEventHub();
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
}
