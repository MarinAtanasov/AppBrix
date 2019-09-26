// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Async.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace AppBrix.Events.Async.Tests
{
    public sealed class AsyncEventHubTests
    {
        #region Setup and cleanup
        public AsyncEventHubTests()
        {
            this.app = TestUtils.CreateTestApp(typeof(AsyncEventsModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestNoSubscription()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMock(10);
            hub.Raise(args);
            this.app.Stop();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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
        
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestNullArgumentSubscribe()
        {
            var hub = this.GetAsyncEventHub();
            Action action = () => hub.Subscribe<IEvent>(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestNullArgumentUnsubscribe()
        {
            var hub = this.GetAsyncEventHub();
            Action action = () => hub.Unsubscribe<IEvent>(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestNullArgumentRaise()
        {
            var hub = this.GetAsyncEventHub();
            Action action = () => hub.Raise(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestThreadManagement()
        {
            var initialThreads = Process.GetCurrentProcess().Threads.Count;
            var hub = this.GetAsyncEventHub();
            Process.GetCurrentProcess().Threads.Count.Should().Be(initialThreads, "no threads should be created when getting the async event hub");
            hub.Subscribe<IEvent>(e => { });
            Process.GetCurrentProcess().Threads.Count.Should().Be(initialThreads, "no thread should be created when subscribing to a new event");
            hub.Subscribe<IEvent>(e => { });
            Process.GetCurrentProcess().Threads.Count.Should().Be(initialThreads, "no threads should be created when subscribing to an event with subscribers");
            hub.Subscribe<EventMock>(e => { });
            Process.GetCurrentProcess().Threads.Count.Should().Be(initialThreads, "no thread should be created when subscribing to a second new event");
            this.app.Stop();
            Process.GetCurrentProcess().Threads.Count.Should().Be(initialThreads, "threads should be disposed of on uninitialization");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsSubscribe() => TestUtils.TestPerformance(this.TestPerformanceEventsSubscribeInternal);

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsUnsubscribe() => TestUtils.TestPerformance(this.TestPerformanceEventsUnsubscribeInternal);

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceEventsRaise() => TestUtils.TestPerformance(this.TestPerformanceEventsRaiseInternal);
        #endregion

        #region Private methods
        private IAsyncEventHub GetAsyncEventHub() => this.app.GetAsyncEventHub();

        private void TestPerformanceEventsSubscribeInternal()
        {
            var hub = this.GetAsyncEventHub();
            var calledCount = 100000;
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
            this.app.Reinitialize();
        }

        private void TestPerformanceEventsUnsubscribeInternal()
        {
            var hub = this.GetAsyncEventHub();
            var calledCount = 60000;
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
            this.app.Reinitialize();
        }

        private void TestPerformanceEventsRaiseInternal()
        {
            var hub = this.GetAsyncEventHub();
            var args = new EventMockChild(10);
            var childCalled = 0;
            var interfaceCalled = 0;
            hub.Subscribe<EventMockChild>(e => childCalled++);
            hub.Subscribe<IEvent>(e => interfaceCalled++);
            var calledCount = 15000;
            for (int i = 0; i < calledCount; i++)
            {
                hub.Raise(args);
            }
            this.app.Stop();
            childCalled.Should().Be(calledCount, "The child should be called exactly {0} times", calledCount);
            interfaceCalled.Should().Be(calledCount, "The interface should be called exactly {0} times", calledCount);
            this.app.Start();
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
