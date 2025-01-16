// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Services;
using AppBrix.Events.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using System.Collections.Generic;
using Xunit;

namespace AppBrix.Events.Tests;

public sealed class EventHubTests : TestsBase<EventsModule>
{
    #region Setup and cleanup
    public EventHubTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestEvent()
    {
        var hub = this.GetEventHub();
        var args = new EventMock(10);
        var called = 0;
        hub.Subscribe<EventMock>(e =>
        {
            this.Assert(e == args, "the passed arguments should be the same as provided");
            called++;
        });
        hub.Raise(args);
        this.Assert(called == 1, "event handler should be called exactly once");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestEventChild()
    {
        var hub = this.GetEventHub();
        var args = new EventMockChild(10);
        var called = 0;
        hub.Subscribe<EventMock>(e =>
        {
            this.Assert(e == args, "the passed arguments should be the same as provided");
            called++;
        });
        hub.Raise(args);
        this.Assert(called == 1, "event handler should be called exactly once");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestEventInterface()
    {
        var hub = this.GetEventHub();
        var args = new EventMock(10);
        var called = 0;
        hub.Subscribe<IEvent>(e =>
        {
            this.Assert(e == args, "the passed arguments should be the same as provided");
            called++;
        });
        hub.Raise(args);
        this.Assert(called == 1, "event handler should be called exactly once");
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
        var called = 0;
        Action<EventMock> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Subscribe<EventMockChild>(handler);
        hub.Raise(args);
        this.Assert(called == 1, "event handler should be called exactly once");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDoubleSubscription()
    {
        var hub = this.GetEventHub();
        var args = new EventMock(10);
        var called = 0;
        Action<IEvent> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Subscribe(handler);
        hub.Raise(args);
        this.Assert(called == 2, "event handler should be called exactly twice");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHierarchyCallingOrder()
    {
        var hub = this.GetEventHub();
        var args = new EventMockChild(10);
        var parentCalled = false;
        var childCalled = false;
        var interfaceCalled = false;
        hub.Subscribe<EventMock>(_ =>
        {
            this.Assert(childCalled, "child should be called before parent");
            parentCalled = true;
            this.Assert(interfaceCalled == false, "interface should be called after parent");
        });
        hub.Subscribe<EventMockChild>(_ =>
        {
            childCalled = true;
            this.Assert(parentCalled == false, "parent should be called after child");
            this.Assert(interfaceCalled == false, "interface should be called after child");
        });
        hub.Subscribe<IEvent>(_ =>
        {
            this.Assert(parentCalled, "parent should be called before interface");
            this.Assert(childCalled, "child should be called before interface");
            interfaceCalled = true;
        });
        hub.Raise(args);
        this.Assert(parentCalled, "parent should be called");
        this.Assert(childCalled, "child should be called");
        this.Assert(interfaceCalled, "interface should be called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDoubleRaise()
    {
        var hub = this.GetEventHub();
        var args = new EventMock(10);
        var called = 0;
        Action<IEvent> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Raise(args);
        this.Assert(called == 1, "event handler should be called exactly once after the first raise");
        hub.Raise(args);
        this.Assert(called == 2, "event handler should be called exactly twice after the second raise");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsubscribe()
    {
        var hub = this.GetEventHub();
        var args = new EventMock(10);
        var called = 0;
        Action<EventMock> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Raise(args);
        this.Assert(called == 1, "event handler should be called exactly once after the first raise");
        hub.Unsubscribe(handler);
        hub.Raise(args);
        this.Assert(called == 1, "event handler should be called exactly once after the unsubscription");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUninitialize()
    {
        var hub = this.GetEventHub();
        var args = new EventMock(10);
        var called = 0;
        Action<EventMock> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Raise(args);
        this.Assert(called == 1, "event handler should be called exactly once after the first raise");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestNullArgumentSubscribe()
    {
        var hub = this.GetEventHub();
        var action = () => hub.Subscribe<IEvent>(null!);
        this.AssertThrows<ArgumentNullException>(action);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestNullArgumentUnsubscribe()
    {
        var hub = this.GetEventHub();
        var action = () => hub.Unsubscribe<IEvent>(null!);
        this.AssertThrows<ArgumentNullException>(action);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestNullArgumentRaise()
    {
        var hub = this.GetEventHub();
        var action = () => hub.Raise(null!);
        this.AssertThrows<ArgumentNullException>(action);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHandlerThrowingException()
    {
        var hub = this.GetEventHub();
        var called = 0;
        hub.Subscribe<EventMock>(_ => called++);
        hub.Subscribe<EventMock>(_ => throw new InvalidOperationException());
        hub.Subscribe<EventMock>(_ => called++);
        var action = () => hub.Raise(new EventMock(5));
        this.AssertThrows<InvalidOperationException>(action, "the exception should be propagated to the called");;
        this.Assert(called == 1, "the handler after the failing one shouldn't be called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHandlerUnsubscribingItself()
    {
        var hub = this.GetEventHub();
        var args = new EventMock(10);

        var beforeHandlerCalled = 0;
        Action<IEvent> beforeHandler = _ => beforeHandlerCalled++;
        hub.Subscribe(beforeHandler);

        var unsubscribingHandlerCalled = 0;
        Action<IEvent> unsubscribingHandler = null;
        unsubscribingHandler = _ =>
        {
            unsubscribingHandlerCalled++;
            hub.Unsubscribe(unsubscribingHandler!);
        };
        hub.Subscribe(unsubscribingHandler);

        var afterHandlerCalled = 0;
        Action<IEvent> afterHandler = _ => afterHandlerCalled++;
        hub.Subscribe(afterHandler);

        hub.Raise(args);
        this.Assert(beforeHandlerCalled == 1, "before event handler should be called exactly once after the first raise");
        this.Assert(unsubscribingHandlerCalled == 1, "unsubscribing event handler should be called exactly once after the first raise");
        this.Assert(afterHandlerCalled == 1, "after event handler should be called exactly once after the first raise");

        hub.Raise(args);
        this.Assert(beforeHandlerCalled == 2, "before event handler should be called exactly twice");
        this.Assert(unsubscribingHandlerCalled == 1, "unsubscribing event handler should not be called after the second raise since it has unsubscribed itself during the first");
        this.Assert(afterHandlerCalled == 2, "after event handler should be called exactly twice");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsSubscribe() => this.AssertPerformance(this.TestPerformanceEventsSubscribeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsUnsubscribe() => this.AssertPerformance(this.TestPerformanceEventsUnsubscribeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsRaise() => this.AssertPerformance(this.TestPerformanceEventsRaiseInternal);
    #endregion

    #region Private methods
    private IEventHub GetEventHub() => this.App.GetEventHub();

    private void TestPerformanceEventsSubscribeInternal()
    {
        const int calledCount = 60000;
        var hub = this.GetEventHub();
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
        const int calledCount = 3000;
        var hub = this.GetEventHub();
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

    private void TestPerformanceEventsRaiseInternal()
    {
        const int subscribers = 10;
        const int calledCount = 70000;
        var hub = this.GetEventHub();
        var args = new EventMockChild(10);
        var childCalled = 0;
        var interfaceCalled = 0;

        for (var i = 0; i < subscribers; i++)
        {
            hub.Subscribe<EventMockChild>(_ => childCalled++);
            hub.Subscribe<IEvent>(_ => interfaceCalled++);
        }

        for (var i = 0; i < calledCount; i++)
        {
            hub.Raise(args);
        }

        this.Assert(childCalled == calledCount * subscribers, $"The child should be called exactly {calledCount * subscribers} times");
        this.Assert(interfaceCalled == calledCount * subscribers, $"The interface should be called exactly {calledCount * subscribers} times");

        this.App.Reinitialize();
    }
    #endregion
}
