// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using AppBrix.Events.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppBrix.Events.Tests
{
    public sealed class EventHubTests
    {
        #region Setup and cleanup
        public EventHubTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ContainerModule),
                typeof(EventsModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestNoSubscription()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            hub.Raise(args);
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestCallBaseSubscribeParent()
        {
            var hub = this.GetEventHub();
            var args = new EventMockChild(10);
            int called = 0;
            hub.Subscribe<EventMockChild>(e => called++);
            hub.Raise<EventMock>(args);
            called.Should().Be(0, "event handler should not be called if the arguments are passed as base class");
        }
        
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestNullArgumentSubscribe()
        {
            var hub = this.GetEventHub();
            Action action = () => hub.Subscribe<IEvent>(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestNullArgumentUnsubscribe()
        {
            var hub = this.GetEventHub();
            Action action = () => hub.Unsubscribe<IEvent>(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestNullArgumentRaise()
        {
            var hub = this.GetEventHub();
            Action action = () => hub.Raise<IEvent>(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsSubscribe()
        {
            Action action = this.TestPerformanceEventsSubscribeInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();
            this.app.Reinitialize();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsUnsubscribe()
        {
            Action action = this.TestPerformanceEventsUnsubscribeInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();
            this.app.Reinitialize();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsRaise()
        {
            Action action = this.TestPerformanceEventsRaiseInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();
            this.app.Reinitialize();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }
        #endregion

        #region Private methods
        private IEventHub GetEventHub()
        {
            return this.app.GetEventHub();
        }

        private void TestPerformanceEventsSubscribeInternal()
        {
            var hub = this.GetEventHub();
            var calledCount = 130000;
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
            var calledCount = 60000;
            var handlers = new List<Action<EventMockChild>>(calledCount);
            for (var i = 0; i < calledCount; i++)
            {
                var j = i;
                handlers.Add(e => j++);
            }
            for (int i = 0; i < handlers.Count; i++)
            {
                hub.Subscribe(handlers[i]);
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
            var calledCount = 50000;
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
