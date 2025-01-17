// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Async.Services;
using AppBrix.Events.Async.Tests.Mocks;
using AppBrix.Events.Contracts;
using AppBrix.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace AppBrix.Events.Async.Tests;

[TestClass]
public sealed class AsyncEventHubSyncEventsTests : TestsBase<AsyncEventsModule>
{
    #region Test lifecycle
    protected override void Initialize() => this.App.Start();
    #endregion

    #region Tests
    [Test, Functional]
    public Task TestEvent()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        hub.Subscribe<EventMock>(e =>
        {
            this.Assert(e == args, "the passed arguments should be the same as provided");
            called++;
        });

        hub.Raise(args);

        var func = () => called;
        return this.AssertReturns(func, 1, "event handler should be called exactly once");
    }

    [Test, Functional]
    public Task TestEventChild()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMockChild(10);
        var called = 0;
        hub.Subscribe<EventMock>(e =>
        {
            this.Assert(e == args, "the passed arguments should be the same as provided");
            called++;
        });

        hub.Raise(args);

        var func = () => called;
        return this.AssertReturns(func, 1, "event handler should be called exactly once");
    }

    [Test, Functional]
    public Task TestEventInterface()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        hub.Subscribe<IEvent>(e =>
        {
            this.Assert(e == args, "the passed arguments should be the same as provided");
            called++;
        });

        hub.Raise(args);

        var func = () => called;
        return this.AssertReturns(func, 1, "event handler should be called exactly once");
    }

    [Test, Functional]
    public void TestNoSubscription()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        hub.Raise(args);
    }

    [Test, Functional]
    public Task TestParentAndChildSubscription()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Action<EventMock> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Subscribe<EventMockChild>(handler);

        hub.Raise(args);

        var func = () => called;
        return this.AssertReturns(func, 1, "event handler should be called exactly once");
    }

    [Test, Functional]
    public Task TestDoubleSubscription()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Action<IEvent> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Subscribe(handler);

        hub.Raise(args);

        var func = () => called;
        return this.AssertReturns(func, 2, "event handler should be called exactly twice");
    }

    [Test, Functional]
    public Task TestDoubleRaise()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Action<IEvent> handler = _ => called++;
        hub.Subscribe(handler);

        hub.Raise(args);
        hub.Raise(args);

        var func = () => called;
        return this.AssertReturns(func, 2, "event handler should be called exactly twice after the second raise");
    }

    [Test, Functional]
    public Task TestUnsubscribe()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Action<EventMock> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Unsubscribe(handler);

        hub.Raise(args);

        var func = () => called;
        return this.AssertReturns(func, 0, "event handler should not be called after the unsubscription");
    }

    [Test, Functional]
    public Task TestUninitialize()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);
        var called = 0;
        Action<EventMock> handler = _ => called++;
        hub.Subscribe(handler);

        hub.Raise(args);

        var func = () => called;
        return this.AssertReturns(func, 1, "event handler should be called exactly once after the first raise");
    }

    [Test, Functional]
    public void TestNullArgumentSubscribe()
    {
        var hub = this.GetAsyncEventHub();
        var action = () => hub.Subscribe(((Action<IEvent>)null)!);
        this.AssertThrows<ArgumentNullException>(action);
    }

    [Test, Functional]
    public void TestNullArgumentUnsubscribe()
    {
        var hub = this.GetAsyncEventHub();
        var action = () => hub.Unsubscribe(((Action<IEvent>)null)!);
        this.AssertThrows<ArgumentNullException>(action);
    }

    [Test, Functional]
    public void TestNullArgumentRaise()
    {
        var hub = this.GetAsyncEventHub();
        var action = () => hub.Raise(null!);
        this.AssertThrows<ArgumentNullException>(action);
    }

    [Test, Functional]
    public async Task TestHandlerUnsubscribingItself()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);

        var beforeHandlerCalled = 0;
        var unsubscribingHandlerCalled = 0;
        var afterHandlerCalled = 0;

        hub.Subscribe<IEvent>(_ => beforeHandlerCalled++);
        Action<IEvent> unsubscribingHandler = null;
        unsubscribingHandler = _ =>
        {
            unsubscribingHandlerCalled++;
            hub.Unsubscribe(unsubscribingHandler!);
            throw new Exception();
        };
        hub.Subscribe(unsubscribingHandler);
        hub.Subscribe<IEvent>(_ => afterHandlerCalled++);

        hub.Raise(args);
        hub.Raise(args);

        var beforeHandlerCalledFunc = () => beforeHandlerCalled;
        await this.AssertReturns(beforeHandlerCalledFunc, 2, "before event handler should be called exactly twice");

        var unsubscribingHandlerCalledFunc = () => unsubscribingHandlerCalled;
        await this.AssertReturns(unsubscribingHandlerCalledFunc, 1, "unsubscribing event handler should not be called after the second raise since it has unsubscribed itself during the first");

        var afterHandlerCalledFunc = () => afterHandlerCalled;
        await this.AssertReturns(afterHandlerCalledFunc, 2, "after event handler should be called exactly twice");
    }

    [Test, Functional]
    public async Task TestHandlerThrowingException()
    {
        var hub = this.GetAsyncEventHub();
        var args = new EventMock(10);

        var beforeHandlerCalled = 0;
        var throwingHandlerCalled = 0;
        var afterHandlerCalled = 0;

        hub.Subscribe<IEvent>(_ => beforeHandlerCalled++);
        Action<IEvent> handler = _ =>
        {
            throwingHandlerCalled++;
            throw new InvalidOperationException("Exception during handler");
        };
        hub.Subscribe(handler);
        hub.Subscribe<IEvent>(_ => afterHandlerCalled++);

        hub.Raise(args);
        hub.Raise(args);

        var beforeHandlerCalledFunc = () => beforeHandlerCalled;
        await this.AssertReturns(beforeHandlerCalledFunc, 2, "before event handler should be called exactly twice");

        var throwingHandlerCalledFunc = () => throwingHandlerCalled;
        await this.AssertReturns(throwingHandlerCalledFunc, 2, "throwing event handler should be called exactly twice");

        var afterHandlerCalledFunc = () => afterHandlerCalled;
        await this.AssertReturns(afterHandlerCalledFunc, 2, "after event handler should be called exactly twice");
    }

    [Test, Functional]
    public async Task TestThreadManagement()
    {
        var getThreads = () => Process.GetCurrentProcess().Threads
            .Cast<ProcessThread>()
            .Count(x => x.ThreadState == ThreadState.Running);
        
        var threadsAdded = (int current) => () => getThreads() > current;

        var threads = getThreads();
        var hub = this.GetAsyncEventHub();
        await this.AssertReturns(threadsAdded(threads), false, "no threads should be created when getting the async event hub");

        threads = getThreads();
        hub.Subscribe<IEvent>(_ => { });
        await this.AssertReturns(threadsAdded(threads), false, "no thread should be created when subscribing to a new event");
        
        threads = getThreads();
        hub.Subscribe<IEvent>(_ => { });
        await this.AssertReturns(threadsAdded(threads), false, "no threads should be created when subscribing to an event with subscribers");
        
        threads = getThreads();
        hub.Subscribe<EventMock>(_ => { });
        await this.AssertReturns(threadsAdded(threads), false, "no thread should be created when subscribing to a second new event");
        
        threads = getThreads();
        this.App.Reinitialize();
        await this.AssertReturns(threadsAdded(threads), false, "threads should be disposed of on uninitialization");
    }

    [Test, Performance]
    public void TestPerformanceEventsSubscribe() => this.AssertPerformance(this.TestPerformanceEventsSubscribeInternal);

    [Test, Performance]
    public void TestPerformanceEventsUnsubscribe() => this.AssertPerformance(this.TestPerformanceEventsUnsubscribeInternal);

    [Test, Performance]
    public void TestPerformanceEventsRaise() => this.AssertPerformance(this.TestPerformanceEventsRaiseInternal);
    #endregion

    #region Private methods
    private IAsyncEventHub GetAsyncEventHub() => this.App.GetAsyncEventHub();

    private void TestPerformanceEventsSubscribeInternal()
    {
        const int calledCount = 80000;
        var hub = this.GetAsyncEventHub();
        var handlers = new List<Action<EventMockChild>>(calledCount);

        for (var i = 0; i < calledCount; i++)
        {
            var j = i;
            handlers.Add(_ => j++);
        }

        foreach (var handler in handlers)
        {
            hub.Subscribe(handler);
        }

        this.App.Reinitialize();
    }

    private void TestPerformanceEventsUnsubscribeInternal()
    {
        const int calledCount = 60000;
        var hub = this.GetAsyncEventHub();
        var handlers = new List<Action<EventMockChild>>(calledCount);

        for (var i = 0; i < calledCount; i++)
        {
            var j = i;
            handlers.Add(_ => j++);
        }

        foreach (var handler in handlers)
        {
            hub.Subscribe(handler);
        }

        for (var i = handlers.Count - 1; i >= 0; i--)
        {
            hub.Unsubscribe(handlers[i]);
        }

        this.App.Reinitialize();
    }

    private async Task TestPerformanceEventsRaiseInternal()
    {
        const int calledCount = 15000;
        var hub = this.GetAsyncEventHub();
        var args = new EventMockChild(10);
        var childCalled = 0;
        var interfaceCalled = 0;

        hub.Subscribe<EventMockChild>(_ => childCalled++);
        hub.Subscribe<IEvent>(_ => interfaceCalled++);
        for (var i = 0; i < calledCount; i++)
        {
            hub.Raise(args);
        }

        var childCalledFunc = () => childCalled;
        await this.AssertReturns(childCalledFunc, calledCount, $"The child should be called exactly {calledCount} times");
        var interfaceCalledFunc = () => interfaceCalled;
        await this.AssertReturns(interfaceCalledFunc, calledCount, $"The interface should be called exactly {calledCount} times");

        this.App.Reinitialize();
    }
    #endregion
}
