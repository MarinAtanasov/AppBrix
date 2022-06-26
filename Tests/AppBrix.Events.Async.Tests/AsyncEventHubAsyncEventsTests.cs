// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Async.Services;
using AppBrix.Events.Async.Tests.Mocks;
using AppBrix.Events.Contracts;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace AppBrix.Events.Async.Tests;

public sealed class AsyncEventHubAsyncEventsTests : TestsBase
{
    #region Setup and cleanup
    public AsyncEventHubAsyncEventsTests() : base(TestUtils.CreateTestApp<AsyncEventsModule>()) => this.app.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestEvent()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        hub.Subscribe<EventMock>(e =>
        {
            e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
            called++;
            return Task.CompletedTask;
        });

        hub.Raise(args);

        var func = () => called;
        func.ShouldReturn(1, "event handler should be called exactly once");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestEventChild()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMockChild(10);
        var called = 0;
        hub.Subscribe<EventMock>(e =>
        {
            e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
            called++;
            return Task.CompletedTask;
        });

        hub.Raise(args);

        var func = () => called;
        func.ShouldReturn(1, "event handler should be called exactly once");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestEventInterface()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        hub.Subscribe<IEvent>(e =>
        {
            e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
            called++;
            return Task.CompletedTask;
        });

        hub.Raise(args);

        var func = () => called;
        func.ShouldReturn(1, "event handler should be called exactly once");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestNoSubscription()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        hub.Raise(args);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestParentAndChildSubscription()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Func<EventMock, Task> handler = _ => { called++; return Task.CompletedTask; };
        hub.Subscribe(handler);
        hub.Subscribe<EventMockChild>(handler);

        hub.Raise(args);

        var func = () => called;
        func.ShouldReturn(1, "event handler should be called exactly once");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDoubleSubscription()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Func<IEvent, Task> handler = _ => { called++; return Task.CompletedTask; };
        hub.Subscribe(handler);
        hub.Subscribe(handler);

        hub.Raise(args);

        var func = () => called;
        func.ShouldReturn(2, "event handler should be called exactly twice");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDoubleRaise()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Func<IEvent, Task> handler = _ => { called++; return Task.CompletedTask; };
        hub.Subscribe(handler);

        hub.Raise(args);
        hub.Raise(args);

        var func = () => called;
        func.ShouldReturn(2, "event handler should be called exactly twice after the second raise");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsubscribe()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Func<EventMock, Task> handler = _ => { called++; return Task.CompletedTask; };
        hub.Subscribe(handler);
        hub.Unsubscribe(handler);

        hub.Raise(args);

        var func = () => called;
        func.ShouldReturn(0, "event handler should not be called after the unsubscription");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUninitialize()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Func<EventMock, Task> handler = _ => { called++; return Task.CompletedTask; };
        hub.Subscribe(handler);

        hub.Raise(args);

        var func = () => called;
        func.ShouldReturn(1, "event handler should be called exactly once after the first raise");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestNullArgumentSubscribe()
    {
        var hub = this.GetAsyncEventHub();
        Action action = () => hub.Subscribe((Func<IEvent, Task>)null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestNullArgumentUnsubscribe()
    {
        var hub = this.GetAsyncEventHub();
        Action action = () => hub.Unsubscribe((Func<IEvent, Task>)null);
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

        var beforeHandlerCalled = 0;
        var unsubscribingHandlerCalled = 0;
        var afterHandlerCalled = 0;

        hub.Subscribe<IEvent>(_ => { beforeHandlerCalled++; return Task.CompletedTask; });
        Func<IEvent, Task> unsubscribingHandler = null;
        unsubscribingHandler = _ =>
        {
            unsubscribingHandlerCalled++;
            hub.Unsubscribe(unsubscribingHandler!);
            throw new Exception();
        };
        hub.Subscribe(unsubscribingHandler);
        hub.Subscribe<IEvent>(_ => { afterHandlerCalled++; return Task.CompletedTask; });

        hub.Raise(args);
        hub.Raise(args);

        var beforeHandlerCalledFunc = () => beforeHandlerCalled;
        beforeHandlerCalledFunc.ShouldReturn(2, "before event handler should be called exactly twice");

        var unsubscribingHandlerCalledFunc = () => unsubscribingHandlerCalled;
        unsubscribingHandlerCalledFunc.ShouldReturn(1, "unsubscribing event handler should not be called after the second raise since it has unsubscribed itself during the first");

        var afterHandlerCalledFunc = () => afterHandlerCalled;
        afterHandlerCalledFunc.ShouldReturn(2, "after event handler should be called exactly twice");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHandlerThrowingException()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);

        var beforeHandlerCalled = 0;
        var throwingHandlerCalled = 0;
        var afterHandlerCalled = 0;

        hub.Subscribe<IEvent>(_ => { beforeHandlerCalled++; return Task.CompletedTask; });
        Func<IEvent, Task> handler = _ =>
        {
            throwingHandlerCalled++;
            throw new InvalidOperationException("Exception during handler");
        };
        hub.Subscribe(handler);
        hub.Subscribe<IEvent>(_ => { afterHandlerCalled++; return Task.CompletedTask; });

        hub.Raise(args);
        hub.Raise(args);

        var beforeHandlerCalledFunc = () => beforeHandlerCalled;
        beforeHandlerCalledFunc.ShouldReturn(2, "before event handler should be called exactly twice");

        var throwingHandlerCalledFunc = () => throwingHandlerCalled;
        throwingHandlerCalledFunc.ShouldReturn(2, "throwing event handler should be called exactly twice");

        var afterHandlerCalledFunc = () => afterHandlerCalled;
        afterHandlerCalledFunc.ShouldReturn(2, "after event handler should be called exactly twice");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestThreadManagement()
    {
        var getThreads = () => Process.GetCurrentProcess().Threads.Count;
        var initialThreads = getThreads();
        var hub = this.GetAsyncEventHub();
        getThreads.ShouldReturn(initialThreads, "no threads should be created when getting the async event hub");
        hub.Subscribe<IEvent>(_ => Task.CompletedTask);
        getThreads.ShouldReturn(initialThreads, "no thread should be created when subscribing to a new event");
        hub.Subscribe<IEvent>(_ => Task.CompletedTask);
        getThreads.ShouldReturn(initialThreads, "no threads should be created when subscribing to an event with subscribers");
        hub.Subscribe<EventMock>(_ => Task.CompletedTask);
        getThreads.ShouldReturn(initialThreads, "no thread should be created when subscribing to a second new event");
        this.app.Reinitialize();
        getThreads.ShouldReturn(initialThreads, "threads should be disposed of on uninitialization");
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
        var handlers = new List<Func<EventMockChild, Task>>(calledCount);
        for (var i = 0; i < calledCount; i++)
        {
            var j = i;
            handlers.Add(_ => { j++; return Task.CompletedTask; });
        }
        for (var i = 0; i < handlers.Count; i++)
        {
            hub.Subscribe(handlers[i]);
        }
        this.app.Reinitialize();
    }

    private void TestPerformanceEventsUnsubscribeInternal()
    {
        var hub = this.GetAsyncEventHub();
        var calledCount = 60000;
        var handlers = new List<Func<EventMockChild, Task>>(calledCount);
        for (var i = 0; i < calledCount; i++)
        {
            var j = i;
            handlers.Add(_ => { j++; return Task.CompletedTask; });
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

    private void TestPerformanceEventsRaiseInternal()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMockChild(10);
        var childCalled = 0;
        var interfaceCalled = 0;
        hub.Subscribe<EventMockChild>(_ => { childCalled++; return Task.CompletedTask; });
        hub.Subscribe<IEvent>(_ => { interfaceCalled++; return Task.CompletedTask; });
        var calledCount = 15000;
        for (var i = 0; i < calledCount; i++)
        {
            hub.Raise(args);
        }

        var childCalledFunc = () => childCalled;
        childCalledFunc.ShouldReturn(calledCount, $"The child should be called exactly {calledCount} times");

        var interfaceCalledFunc = () => interfaceCalled;
        interfaceCalledFunc.ShouldReturn(calledCount, $"The interface should be called exactly {calledCount} times");

        this.app.Reinitialize();
    }
    #endregion
}
