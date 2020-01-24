// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Delayed.Configuration;
using AppBrix.Events.Delayed.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AppBrix.Events.Delayed.Tests
{
    public sealed class DelayedEventHubTests
    {
        #region Setup and cleanup
        public DelayedEventHubTests()
        {
            this.app = TestUtils.CreateTestApp(typeof(DelayedEventsModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestEventDefaultImmediate()
        {
            this.app.GetConfig<DelayedEventsConfig>().DefaultBehavior = EventBehavior.Immediate;
            var hub = this.app.GetDelayedEventHub();
            var args = new EventMock(10);
            int called = 0;
            hub.Subscribe<EventMock>(e =>
            {
                e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
                called++;
            });
            hub.Raise(args);
            called.Should().Be(1, "event handler should be called exactly once");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestEventImmediate()
        {
            this.app.GetConfig<DelayedEventsConfig>().DefaultBehavior = EventBehavior.Delayed;
            var hub = this.app.GetDelayedEventHub();
            var args = new EventMock(10);
            int called = 0;
            hub.Subscribe<EventMock>(e =>
            {
                e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
                called++;
            });
            hub.RaiseImmediate(args);
            called.Should().Be(1, "event handler should be called exactly once");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestEventDefaultDelayed()
        {
            this.app.GetConfig<DelayedEventsConfig>().DefaultBehavior = EventBehavior.Delayed;
            var hub = this.app.GetDelayedEventHub();
            var args = new EventMock(10);
            int called = 0;
            hub.Subscribe<EventMock>(e =>
            {
                e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
                called++;
            });
            hub.Raise(args);
            called.Should().Be(0, "event handler should not be called before flush");
            hub.Flush();
            called.Should().Be(1, "event handler should be called exactly once after flush");
            hub.Flush();
            called.Should().Be(1, "event handler should be called exactly once after second flush");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestEventDelayed()
        {
            this.app.GetConfig<DelayedEventsConfig>().DefaultBehavior = EventBehavior.Immediate;
            var hub = this.app.GetDelayedEventHub();
            var args = new EventMock(10);
            int called = 0;
            hub.Subscribe<EventMock>(e =>
            {
                e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
                called++;
            });
            hub.RaiseDelayed(args);
            called.Should().Be(0, "event handler should not be called before flush");
            hub.Flush();
            called.Should().Be(1, "event handler should be called exactly once after flush");
            hub.Flush();
            called.Should().Be(1, "event handler should be called exactly once after second flush");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsSubscribe() => TestUtils.TestPerformance(this.TestPerformanceEventsSubscribeInternal);

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsUnsubscribe() => TestUtils.TestPerformance(this.TestPerformanceEventsUnsubscribeInternal);

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsRaiseImmediate() => TestUtils.TestPerformance(this.TestPerformanceEventsRaiseImmediateInternal);

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsRaiseDelayed() => TestUtils.TestPerformance(this.TestPerformanceEventsRaiseDelayedInternal);
        #endregion

        #region Private methods
        private void TestPerformanceEventsSubscribeInternal()
        {
            var hub = this.app.GetDelayedEventHub();
            var calledCount = 100000;
            var handlers = new List<Action<EventMockChild>>(calledCount);
            for (var i = 0; i < calledCount; i++)
            {
                var j = i;
                handlers.Add(e => j++);
            }
            for (var i = 0; i < handlers.Count; i++)
            {
                hub.Subscribe(handlers[i]);
            }
            this.app.Reinitialize();
        }

        private void TestPerformanceEventsUnsubscribeInternal()
        {
            var hub = this.app.GetDelayedEventHub();
            var calledCount = 75000;
            var handlers = new List<Action<EventMockChild>>(calledCount);
            for (var i = 0; i < calledCount; i++)
            {
                var j = i;
                handlers.Add(e => j++);
            }
            for (var i = 0; i < handlers.Count; i++)
            {
                hub.Subscribe(handlers[i]);
            }
            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                hub.Unsubscribe(handlers[i]);
            }
            this.app.Reinitialize();
        }

        private void TestPerformanceEventsRaiseImmediateInternal()
        {
            var hub = this.app.GetDelayedEventHub();
            var args = new EventMockChild(10);
            var childCalled = 0;
            var interfaceCalled = 0;
            hub.Subscribe<EventMockChild>(e => childCalled++);
            hub.Subscribe<IEvent>(e => interfaceCalled++);
            var calledCount = 50000;
            for (var i = 0; i < calledCount; i++)
            {
                hub.Raise(args);
            }
            childCalled.Should().Be(calledCount, "The child should be called exactly {0} times", calledCount);
            interfaceCalled.Should().Be(calledCount, "The interface should be called exactly {0} times", calledCount);
            this.app.Reinitialize();
        }

        private void TestPerformanceEventsRaiseDelayedInternal()
        {
            this.app.GetConfig<DelayedEventsConfig>().DefaultBehavior = EventBehavior.Delayed;
            var hub = this.app.GetDelayedEventHub();
            var args = new EventMockChild(10);
            var childCalled = 0;
            var interfaceCalled = 0;
            hub.Subscribe<EventMockChild>(e => childCalled++);
            hub.Subscribe<IEvent>(e => interfaceCalled++);
            var calledCount = 50000;
            for (var i = 0; i < calledCount; i++)
            {
                hub.Raise(args);
            }
            childCalled.Should().Be(0, "The child should not be called before flush");
            interfaceCalled.Should().Be(0, "The interface should not be called before flush");
            hub.Flush();
            childCalled.Should().Be(calledCount, "The child should be called exactly {0} times", calledCount);
            interfaceCalled.Should().Be(calledCount, "The interface should be called exactly {0} times", calledCount);
            this.app.Reinitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
