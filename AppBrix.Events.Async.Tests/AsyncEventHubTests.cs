// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container;
using AppBrix.Events.Async.Tests.Mocks;
using AppBrix.Lifecycle;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppBrix.Events.Async.Tests
{
    public class AsyncEventHubTests
    {
        #region Setup and cleanup
        public AsyncEventHubTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(AsyncEventsModule),
                typeof(ContainerModule),
                typeof(EventsModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestEvent()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMock(10);
            int called = 0;
            hub.Subscribe<EventMock>(e =>
            {
                e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
                called++;
            });
            hub.Raise(args);
            this.app.Stop();
            called.Should().Be(1, "event handler should be called exactly once");
        }

        [Fact]
        public void TestEventChild()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMockChild(10);
            int called = 0;
            hub.Subscribe<EventMock>(e =>
            {
                e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
                called++;
            });
            hub.Raise(args);
            this.app.Stop();
            called.Should().Be(1, "event handler should be called exactly once");
        }

        [Fact]
        public void TestEventInterface()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMock(10);
            int called = 0;
            hub.Subscribe<IEvent>(e =>
            {
                e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
                called++;
            });
            hub.Raise(args);
            this.app.Stop();
            called.Should().Be(1, "event handler should be called exactly once");
        }

        [Fact]
        public void TestNoSubscription()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMock(10);
            hub.Raise(args);
            this.app.Stop();
        }

        [Fact]
        public void TestParentAndChildSubscription()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<EventMock> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Subscribe<EventMockChild>(handler);
            hub.Raise(args);
            this.app.Stop();
            called.Should().Be(1, "event handler should be called exactly once");
        }

        [Fact]
        public void TestDoubleSubscription()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<IEvent> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Subscribe(handler);
            hub.Raise(args);
            this.app.Stop();
            called.Should().Be(2, "event handler should be called exactly twice");
        }
        
        [Fact]
        public void TestCallBaseSubscribeParent()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMockChild(10);
            int called = 0;
            hub.Subscribe<EventMockChild>(e => called++);
            hub.Raise<EventMock>(args);
            called.Should().Be(0, "event handler should not be called if the arguments are passed as base class");
        }
        
        [Fact]
        public void TestDoubleRaise()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<IEvent> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Raise(args);
            hub.Raise(args);
            this.app.Stop();
            called.Should().Be(2, "event handler should be called exactly twice after the second raise");
        }

        [Fact]
        public void TestUnsubscribe()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<EventMock> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Unsubscribe(handler);
            hub.Raise(args);
            this.app.Stop();
            called.Should().Be(0, "event handler should not be called after the unsubscription");
        }

        [Fact]
        public void TestUninitialize()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<EventMock> handler = (e => called++);
            hub.Subscribe(handler);
            hub.Raise(args);
            this.app.Stop();
            called.Should().Be(1, "event handler should be called exactly once after the first raise");
        }

        [Fact]
        public void TestNullArgumentSubscribe()
        {
            var hub = this.GetAsyncEventHub();
            Action action = () => hub.Subscribe<IEvent>(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void TestNullArgumentUnsubscribe()
        {
            var hub = this.GetAsyncEventHub();
            Action action = () => hub.Unsubscribe<IEvent>(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void TestNullArgumentRaise()
        {
            var hub = this.GetAsyncEventHub();
            Action action = () => hub.Raise<IEvent>(null);
            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void TestHandlerUnsubscribingItself()
        {
            var hub = this.GetAsyncEventHub();
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
            hub.Raise(args);
            this.app.Stop();
            beforeHandlerCalled.Should().Be(2, "before event handler should be called exactly twice");
            unsubscribingHandlerCalled.Should().Be(1, "unsubscribing event handler should not be called after the second raise since it has unsubscribed itself during the first");
            afterHandlerCalled.Should().Be(2, "after event handler should be called exactly twice");
        }
        
        //[Fact] // Skip automatic execution. Test is flaky during multithreaded execution
        public void TestPerformanceEventsSubscribe()
        {
            Action action = this.TestPerformanceEventsSubscribeInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();
            this.app.Reinitialize();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }

        //[Fact] // Skip automatic execution. Test is flaky during multithreaded execution
        public void TestPerformanceEventsUnsubscribe()
        {
            Action action = this.TestPerformanceEventsUnsubscribeInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();
            this.app.Reinitialize();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }

        //[Fact] // Skip automatic execution. Test is flaky during multithreaded execution
        public void TestPerformanceEventsRaise()
        {
            Action action = this.TestPerformanceEventsRaiseInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();
            this.app.Start();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }
        #endregion

        #region Private methods
        private IAsyncEventHub GetAsyncEventHub()
        {
            return this.app.GetAsyncEventHub();
        }

        private void TestPerformanceEventsSubscribeInternal()
        {
            var hub = this.GetAsyncEventHub();
            var calledCount = 115000;
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
            var hub = this.GetAsyncEventHub();
            var calledCount = 75000;
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
            var hub = this.GetAsyncEventHub();
            var args = new EventMockChild(10);
            var parentCalled = 0;
            var childCalled = 0;
            var interfaceCalled = 0;
            hub.Subscribe<EventMock>(e => parentCalled++);
            hub.Subscribe<EventMockChild>(e => childCalled++);
            hub.Subscribe<IEvent>(e => interfaceCalled++);
            var calledCount = 15000;
            for (int i = 0; i < calledCount; i++)
            {
                hub.Raise(args);
            }
            this.app.Stop();
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
