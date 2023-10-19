// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Events.Contracts;
using AppBrix.Events.Delayed.Configuration;
using AppBrix.Events.Delayed.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace AppBrix.Events.Delayed.Tests;

public sealed class DelayedEventHubTests : TestsBase<DelayedEventsModule>
{
    #region Setup and cleanup
    public DelayedEventHubTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSubscribeNullHandler()
    {
        var hub = this.App.GetDelayedEventHub();
        var action = () => hub.Subscribe<EventMock>(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsubscribeNullHandler()
    {
        var hub = this.App.GetDelayedEventHub();
        var action = () => hub.Unsubscribe<EventMock>(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsubscribeImmediate()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Immediate;
        var hub = this.App.GetDelayedEventHub();
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
    public void TestRaiseImmediateNullArgument()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Delayed;
        var hub = this.App.GetDelayedEventHub();
        var action = () => hub.RaiseImmediate(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRaiseImmediateDefaultEvent()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Immediate;
        var hub = this.App.GetDelayedEventHub();
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
    public void TestRaiseImmediate()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Delayed;
        var hub = this.App.GetDelayedEventHub();
        var args = new EventMock(10);
        var called = 0;
        hub.Subscribe<EventMock>(e =>
        {
            e.Should().BeSameAs(args, "the passed arguments should be the same as provided");
            called++;
        });
        hub.RaiseImmediate(args);
        called.Should().Be(1, "event handler should be called exactly once");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHandlerThrowingExceptionImmediate()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Delayed;
        var hub = this.App.GetDelayedEventHub();
        var called = 0;
        hub.Subscribe<EventMock>(_ => called++);
        hub.Subscribe<EventMock>(_ => throw new InvalidOperationException());
        hub.Subscribe<EventMock>(_ => called++);
        var action = () => hub.RaiseImmediate(new EventMock(5));
        action.Should().Throw<InvalidOperationException>("the exception should be propagated to the called");
        called.Should().Be(1, "the handler after the failing one shouldn't be called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUnsubscribeDelayed()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Delayed;
        var hub = this.App.GetDelayedEventHub();
        var args = new EventMock(10);
        var called = 0;

        Action<EventMock> handler = _ => called++;
        hub.Subscribe(handler);
        hub.Raise(args);
        hub.Flush();
        called.Should().Be(1, "event handler should be called exactly once after the first raise");

        hub.Unsubscribe(handler);
        hub.Raise(args);
        hub.Flush();
        called.Should().Be(1, "event handler should be called exactly once after the unsubscription");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRaiseDelayedNullArgument()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Immediate;
        var hub = this.App.GetDelayedEventHub();
        var action = () => hub.RaiseDelayed(null!);
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRaiseDelayedDefaultEvent()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Delayed;
        var hub = this.App.GetDelayedEventHub();
        var args = new EventMock(10);
        var called = 0;
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
    public void TestRaiseDelayed()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Immediate;
        var hub = this.App.GetDelayedEventHub();
        var args = new EventMock(10);
        var called = 0;
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

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestHandlerThrowingExceptionDelayed()
    {
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Immediate;
        var hub = this.App.GetDelayedEventHub();
        var called = 0;
        hub.Subscribe<EventMock>(_ => called++);
        hub.Subscribe<EventMock>(_ => throw new InvalidOperationException());
        hub.Subscribe<EventMock>(_ => called++);
        hub.RaiseDelayed(new EventMock(5));
        var action = () => hub.Flush();
        action.Should().Throw<InvalidOperationException>("the exception should be propagated to the called");
        called.Should().Be(1, "the handler after the failing one shouldn't be called");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsSubscribe() => this.AssertPerformance(this.TestPerformanceEventsSubscribeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsUnsubscribe() => this.AssertPerformance(this.TestPerformanceEventsUnsubscribeInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsRaiseImmediate() => this.AssertPerformance(this.TestPerformanceEventsRaiseImmediateInternal);

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceEventsRaiseDelayed() => this.AssertPerformance(this.TestPerformanceEventsRaiseDelayedInternal);
    #endregion

    #region Private methods
    private void TestPerformanceEventsSubscribeInternal()
    {
        const int calledCount = 60000;
        var hub = this.App.GetDelayedEventHub();
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

        this.App.Reinitialize();
    }

    private void TestPerformanceEventsUnsubscribeInternal()
    {
        const int calledCount = 3000;
        var hub = this.App.GetDelayedEventHub();
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

        this.App.Reinitialize();
    }

    private void TestPerformanceEventsRaiseImmediateInternal()
    {
        const int calledCount = 100000;
        var hub = this.App.GetDelayedEventHub();
        var args = new EventMockChild(10);
        var childCalled = 0;
        var interfaceCalled = 0;

        hub.Subscribe<EventMockChild>(_ => childCalled++);
        hub.Subscribe<IEvent>(_ => interfaceCalled++);
        for (var i = 0; i < calledCount; i++)
        {
            hub.Raise(args);
        }

        childCalled.Should().Be(calledCount, "The child should be called exactly {0} times", calledCount);
        interfaceCalled.Should().Be(calledCount, "The interface should be called exactly {0} times", calledCount);

        this.App.Reinitialize();
    }

    private void TestPerformanceEventsRaiseDelayedInternal()
    {
        const int calledCount = 80000;
        this.App.ConfigService.GetDelayedEventsConfig().DefaultBehavior = EventBehavior.Delayed;
        var hub = this.App.GetDelayedEventHub();
        var args = new EventMockChild(10);
        var childCalled = 0;
        var interfaceCalled = 0;

        hub.Subscribe<EventMockChild>(_ => childCalled++);
        hub.Subscribe<IEvent>(_ => interfaceCalled++);
        for (var i = 0; i < calledCount; i++)
        {
            hub.Raise(args);
        }

        childCalled.Should().Be(0, "The child should not be called before flush");
        interfaceCalled.Should().Be(0, "The interface should not be called before flush");
        hub.Flush();
        childCalled.Should().Be(calledCount, "The child should be called exactly {0} times", calledCount);
        interfaceCalled.Should().Be(calledCount, "The interface should be called exactly {0} times", calledCount);

        this.App.Reinitialize();
    }
    #endregion
}
