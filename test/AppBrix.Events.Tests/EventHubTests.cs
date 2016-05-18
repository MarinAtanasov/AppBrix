// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Events.Tests.Mocks;
using AppBrix.Resolver;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppBrix.Events.Tests
{
    public class EventHubTests : IDisposable
    {
        #region Setup and cleanup
        public EventHubTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ResolverModule),
                typeof(EventsModule));
            this.app.Start();
        }

        public void Dispose()
        {
            this.app.Stop();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestEvent()
        {
            var hub = this.GetEventHub();
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

        [Fact]
        public void TestEventChild()
        {
            var hub = this.GetEventHub();
            var args = new EventMockChild(10);
            int called = 0;
            hub.Subscribe<EventMock>(e =>
            {
                e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
                called++;
            });
            hub.Raise(args);
            called.Should().Be(1, "event handler should be called exactly once");
        }

        [Fact]
        public void TestEventInterface()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            hub.Subscribe<IEvent>(e =>
            {
                e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
                called++;
            });
            hub.Raise(args);
            called.Should().Be(1, "event handler should be called exactly once");
        }

        [Fact]
        public void TestNoSubscription()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            hub.Raise(args);
        }

        [Fact]
        public void TestParentAndChildSubscription()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<EventMock> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Subscribe<EventMockChild>(handler);
            hub.Raise(args);
            called.Should().Be(1, "event handler should be called exactly once");
        }

        [Fact]
        public void TestDoubleSubscription()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<IEvent> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Subscribe(handler);
            hub.Raise(args);
            called.Should().Be(2, "event handler should be called exactly twice");
        }

        [Fact]
        public void TestHierarchyCallingOrder()
        {
            var hub = this.GetEventHub();
            var args = new EventMockChild(10);
            var parentCalled = false;
            var childCalled = false;
            var interfaceCalled = false;
            hub.Subscribe<EventMock>(e =>
            {
                childCalled.Should().BeTrue("child should be called before parent");
                parentCalled = true;
                interfaceCalled.Should().BeFalse("interface should be called after parent");
            });
            hub.Subscribe<EventMockChild>(e =>
            {
                childCalled = true;
                parentCalled.Should().BeFalse("parent should be called after child");
                interfaceCalled.Should().BeFalse("interface should be called after child");
            });
            hub.Subscribe<IEvent>(e =>
            {
                parentCalled.Should().BeTrue("parent should be called before interface");
                childCalled.Should().BeTrue("child should be called before interface");
                interfaceCalled = true;
            });
            hub.Raise(args);
            parentCalled.Should().BeTrue("parent should be called");
            parentCalled.Should().BeTrue("child should be called");
            parentCalled.Should().BeTrue("interface should be called");
        }

        [Fact]
        public void TestCallBaseSubscribeParent()
        {
            var hub = this.GetEventHub();
            var args = new EventMockChild(10);
            int called = 0;
            hub.Subscribe<EventMockChild>(e => called++);
            hub.Raise<EventMock>(args);
            called.Should().Be(0, "event handler should not be called if the arguments are passed as base class");
        }
        
        [Fact]
        public void TestDoubleRaise()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<IEvent> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Raise(args);
            called.Should().Be(1, "event handler should be called exactly once after the first raise");
            hub.Raise(args);
            called.Should().Be(2, "event handler should be called exactly twice after the second raise");
        }

        [Fact]
        public void TestUnsubscribe()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<EventMock> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Raise(args);
            called.Should().Be(1, "event handler should be called exactly once after the first raise");
            hub.Unsubscribe(handler);
            hub.Raise(args);
            called.Should().Be(1, "event handler should be called exactly once after the unsubscription");
        }

        [Fact]
        public void TestUninitialize()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<EventMock> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Raise(args);
            called.Should().Be(1, "event handler should be called exactly once after the first raise");
        }

        [Fact]
        public void TestNullArgumentSubscribe()
        {
            var hub = this.GetEventHub();
            Action action = () => hub.Subscribe<IEvent>(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void TestNullArgumentUnsubscribe()
        {
            var hub = this.GetEventHub();
            Action action = () => hub.Unsubscribe<IEvent>(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void TestNullArgumentRaise()
        {
            var hub = this.GetEventHub();
            Action action = () => hub.Raise<IEvent>(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void TestHandlerUnsubscribingItself()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);

            int beforeHandlerCalled = 0;
            Action<IEvent> beforeHandler = (e => beforeHandlerCalled++);
            hub.Subscribe(beforeHandler);

            int unsubscribingHandlerCalled = 0;
            Action<IEvent> unsubscribingHandler = null;
            unsubscribingHandler = (e => { unsubscribingHandlerCalled++; hub.Unsubscribe(unsubscribingHandler); });
            hub.Subscribe(unsubscribingHandler);

            int afterHandlerCalled = 0;
            Action<IEvent> afterHandler = (e => afterHandlerCalled++);
            hub.Subscribe(afterHandler);

            hub.Raise(args);
            beforeHandlerCalled.Should().Be(1, "before event handler should be called exactly once after the first raise");
            unsubscribingHandlerCalled.Should().Be(1, "unsubscribing event handler should be called exactly once after the first raise");
            afterHandlerCalled.Should().Be(1, "after event handler should be called exactly once after the first raise");

            hub.Raise(args);
            beforeHandlerCalled.Should().Be(2, "before event handler should be called exactly twice");
            unsubscribingHandlerCalled.Should().Be(1, "unsubscribing event handler should not be called after the second raise since it has unsubscribed itself during the first");
            afterHandlerCalled.Should().Be(2, "after event handler should be called exactly twice");
        }

        [Fact]
        public void TestPerformanceEventsSubscribe()
        {
            Action action = this.TestPerformanceEventsSubscribeInternal;
            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(30), "this is a performance test");
        }

        [Fact]
        public void TestPerformanceEventsUnsubscribe()
        {
            Action action = this.TestPerformanceEventsUnsubscribeInternal;
            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(30), "this is a performance test");
        }

        [Fact]
        public void TestPerformanceEventsRaise()
        {
            Action action = this.TestPerformanceEventsRaiseInternal;
            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(30), "this is a performance test");
        }
        #endregion

        #region Private methods
        private IEventHub GetEventHub()
        {
            return this.app.Get<IEventHub>();
        }

        private void TestPerformanceEventsSubscribeInternal()
        {
            var hub = this.GetEventHub();
            var calledCount = 40000;
            var handlers = new List<Action<EventMockChild>>(calledCount);
            for (var i = 0; i < calledCount; i++)
            {
                var j = i;
                handlers.Add(e => j++);
            }
            foreach (var handler in handlers)
            {
                hub.Subscribe(handler);
            }
        }

        private void TestPerformanceEventsUnsubscribeInternal()
        {
            var hub = this.GetEventHub();
            var calledCount = 20000;
            var handlers = new List<Action<EventMockChild>>(calledCount);
            for (var i = 0; i < calledCount; i++)
            {
                var j = i;
                handlers.Add(e => j++);
            }
            foreach (var handler in handlers)
            {
                hub.Subscribe(handler);
            }
            for (var i = handlers.Count - 1; i >= 0; i--)
            {
                hub.Unsubscribe(handlers[i]);
            }
        }

        private void TestPerformanceEventsRaiseInternal()
        {
            var hub = this.GetEventHub();
            var args = new EventMockChild(10);
            var parentCalled = 0;
            var childCalled = 0;
            var interfaceCalled = 0;
            hub.Subscribe<EventMock>(e => parentCalled++);
            hub.Subscribe<EventMockChild>(e => childCalled++);
            hub.Subscribe<IEvent>(e => interfaceCalled++);
            var calledCount = 5000;
            for (int i = 0; i < calledCount; i++)
            {
                hub.Raise(args);
            }
            parentCalled.Should().Be(calledCount, "The parent should be called exactly {0} times", calledCount);
            childCalled.Should().Be(calledCount, "The child should be called exactly {0} times", calledCount);
            interfaceCalled.Should().Be(calledCount, "The interface should be called exactly {0} times", calledCount);
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
