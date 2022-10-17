// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Services;
using AppBrix.Events.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AppBrix.Events.Tests;

public sealed class EventHubTests : TestsBase<EventsModule>
{
    #region Setup and cleanup
    public EventHubTests() => this.app.Start();
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
        var called = 0;
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
        var called = 0;
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
        var called = 0;
        Action<EventMock> handler = _ => called++;
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
        var called = 0;
        Action<IEvent> handler = _ => called++;
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
        hub.Subscribe<EventMock>(_ =>
        {
            childCalled.Should().BeTrue("child should be called before parent");
            parentCalled = true;
            interfaceCalled.Should().BeFalse("interface should be called after parent");
        });
        hub.Subscribe<EventMockChild>(_ =>
        {
            childCalled = true;
            parentCalled.Should().BeFalse("parent should be called after child");
            interfaceCalled.Should().BeFalse("interface should be called after child");
        });
        hub.Subscribe<IEvent>(_ =>
        {
            parentCalled.Should().BeTrue("parent should be called before interface");
            childCalled.Should().BeTrue("child should be called before interface");
            interfaceCalled = true;
        });
        hub.Raise(args);
        parentCalled.Should().BeTrue("parent should be called");
        childCalled.Should().BeTrue("child should be called");
        interfaceCalled.Should().BeTrue("interface should be called");
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
        called.Should().Be(1, "event handler should be called exactly once after the first raise");
        hub.Raise(args);
        called.Should().Be(2, "event handler should be called exactly twice after the second raise");
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
        var called = 0;
        Action<EventMock> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Raise(args);
        called.Should().Be(1, "event handler should be called exactly once after the first raise");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestNullArgumentSubscribe()
    {
        var hub = this.GetEventHub();
        Action action = () => hub.Subscribe<IEvent>(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestNullArgumentUnsubscribe()
    {
        var hub = this.GetEventHub();
        Action action = () => hub.Unsubscribe<IEvent>(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestNullArgumentRaise()
    {
        var hub = this.GetEventHub();
        Action action = () => hub.Raise(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHandlerThrowingException()
    {
        var hub = this.GetEventHub();
        var called = 0;
        hub.Subscribe<EventMock>(x => called++);
        hub.Subscribe<EventMock>(x => throw new InvalidOperationException());
        hub.Subscribe<EventMock>(x => called++);
        Action action = () => hub.Raise(new EventMock(5));
        action.Should().Throw<InvalidOperationException>("the exception should be propagated to the called");
        called.Should().Be(1, "the handler after the failing one shouldn't be called");
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
        beforeHandlerCalled.Should().Be(1, "before event handler should be called exactly once after the first raise");
        unsubscribingHandlerCalled.Should().Be(1, "unsubscribing event handler should be called exactly once after the first raise");
        afterHandlerCalled.Should().Be(1, "after event handler should be called exactly once after the first raise");

        hub.Raise(args);
        beforeHandlerCalled.Should().Be(2, "before event handler should be called exactly twice");
        unsubscribingHandlerCalled.Should().Be(1, "unsubscribing event handler should not be called after the second raise since it has unsubscribed itself during the first");
        afterHandlerCalled.Should().Be(2, "after event handler should be called exactly twice");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsSubscribe() => TestUtils.AssertPerformance(this.TestPerformanceEventsSubscribeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsUnsubscribe() => TestUtils.AssertPerformance(this.TestPerformanceEventsUnsubscribeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsRaise() => TestUtils.AssertPerformance(this.TestPerformanceEventsRaiseInternal);
    #endregion

    #region Private methods
    private IEventHub GetEventHub() => this.app.GetEventHub();

    private void TestPerformanceEventsSubscribeInternal()
    {
        const int calledCount = 50000;
        var hub = this.GetEventHub();
        var handlers = new List<Action<EventMockChild>>(calledCount);

        for (var i = 0; i < calledCount; i++)
        {
            var j = i;
            handlers.Add(_ => j++);
        }
        for (var i = 0; i < handlers.Count; i++)
        {
            hub.Subscribe(handlers[i]);
        }

        this.app.Reinitialize();
    }

    private void TestPerformanceEventsUnsubscribeInternal()
    {
        const int calledCount = 2500;
        var hub = this.GetEventHub();
        var handlers = new List<Action<EventMockChild>>(calledCount);

        for (var i = 0; i < calledCount; i++)
        {
            var j = i;
            handlers.Add(_ => j++);
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

        childCalled.Should().Be(calledCount * subscribers, "The child should be called exactly {0} times", calledCount * subscribers);
        interfaceCalled.Should().Be(calledCount * subscribers, "The interface should be called exactly {0} times", calledCount * subscribers);

        this.app.Reinitialize();
    }
    #endregion
}
