// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Events.Tests
{
    [TestClass]
    public class EventHubTests
    {
        #region Tests
        [TestMethod]
        public void TestEvent()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            hub.Subscribe<EventMock>((e) =>
            {
                Assert.AreSame(args, e, "The passed arguments are not the same.");
                called++;
            });
            hub.Raise(args);
            Assert.AreEqual(1, called, "Event handler should be called exactly once.");
        }

        [TestMethod]
        public void TestEventChild()
        {
            var hub = this.GetEventHub();
            var args = new EventMockChild(10);
            int called = 0;
            hub.Subscribe<EventMock>((e) =>
            {
                Assert.AreSame(args, e, "The passed arguments are not the same.");
                called++;
            });
            hub.Raise(args);
            Assert.AreEqual(1, called, "Event handler should be called exactly once.");
        }

        [TestMethod]
        public void TestEventInterface()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            hub.Subscribe<IEvent>((e) =>
            {
                Assert.AreSame(args, e, "The passed arguments are not the same.");
                called++;
            });
            hub.Raise(args);
            Assert.AreEqual(1, called, "Event handler should be called exactly once.");
        }

        [TestMethod]
        public void TestNoSubscription()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            hub.Raise(args);
        }

        [TestMethod]
        public void TestParentAndChildSubscription()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<EventMock> handler = (e => { called++; });
            hub.Subscribe<EventMock>(handler);
            hub.Subscribe<EventMockChild>(handler);
            hub.Raise(args);
            Assert.AreEqual(1, called, "Event handler should be called exactly once.");
        }

        [TestMethod]
        public void TestDoubleSubscription()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<IEvent> handler = (e => { called++; });
            hub.Subscribe<IEvent>(handler);
            hub.Subscribe<IEvent>(handler);
            hub.Raise(args);
            Assert.AreEqual(2, called, "Event handler should be called exactly twice.");
        }

        [TestMethod]
        public void TestHierarchyCallingOrder()
        {
            var hub = this.GetEventHub();
            var args = new EventMockChild(10);
            var parentCalled = false;
            var childCalled = false;
            var interfaceCalled = false;
            hub.Subscribe<EventMock>((e) =>
            {
                Assert.IsTrue(childCalled, "Child should be called before parent.");
                parentCalled = true;
                Assert.IsFalse(interfaceCalled, "Interface should be called after parent.");
            });
            hub.Subscribe<EventMockChild>((e) =>
            {
                childCalled = true;
                Assert.IsFalse(parentCalled, "Parent should be called after child.");
                Assert.IsFalse(interfaceCalled, "Interface should be called after child.");
            });
            hub.Subscribe<IEvent>((e) =>
            {
                Assert.IsTrue(parentCalled, "Parent should be called before interface.");
                Assert.IsTrue(childCalled, "Child should be called before interface.");
                interfaceCalled = true;
            });
            hub.Raise(args);
            Assert.IsTrue(parentCalled, "Parent should be called.");
            Assert.IsTrue(parentCalled, "Child should be called.");
            Assert.IsTrue(parentCalled, "Interface should be called.");
        }

        [TestMethod]
        public void TestCallBaseSubscribeParent()
        {
            var hub = this.GetEventHub();
            var args = new EventMockChild(10);
            int called = 0;
            hub.Subscribe<EventMockChild>((e => { called++; }));
            hub.Raise<EventMock>(args);
            Assert.AreEqual(0, called, "Event handler should not be called if the arguments are passed as base class.");
        }

        [TestMethod]
        public void TestDoubleRaise()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<IEvent> handler = (e => { called++; });
            hub.Subscribe<IEvent>(handler);
            hub.Raise(args);
            Assert.AreEqual(1, called, "Event handler should be called exactly once after the first raise.");
            hub.Raise(args);
            Assert.AreEqual(2, called, "Event handler should be called exactly twice after the second raise.");
        }

        [TestMethod]
        public void TestUnsubscribe()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<EventMock> handler = (e => { called++; });
            hub.Subscribe<EventMock>(handler);
            hub.Raise(args);
            Assert.AreEqual(1, called, "Event handler should be called exactly once after the first raise.");
            hub.Unsubscribe(handler);
            hub.Raise(args);
            Assert.AreEqual(1, called, "Event handler should be called exactly once after the unsubscription.");
        }

        [TestMethod]
        public void TestUninitialize()
        {
            var hub = this.GetEventHub();
            var args = new EventMock(10);
            int called = 0;
            Action<EventMock> handler = (e => { called++; });
            hub.Subscribe<EventMock>(handler);
            hub.Raise(args);
            Assert.AreEqual(1, called, "Event handler should be called exactly once after the first raise.");
            hub.Uninitialize();
            hub.Raise(args);
            Assert.AreEqual(1, called, "Event handler should be called exactly once after the uninitialization.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullArgumentSubscribe()
        {
            var hub = this.GetEventHub();
            hub.Subscribe<IEvent>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullArgumentUnsubscribe()
        {
            var hub = this.GetEventHub();
            hub.Unsubscribe<IEvent>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullArgumentRaise()
        {
            var hub = this.GetEventHub();
            hub.Raise<IEvent>(null);
        }

        [TestMethod]
        [Timeout(20)]
        public void TestPerformanceEventsSubscribe()
        {
            var hub = this.GetEventHub();
            var calledCount = 40000;
            var handlers = new List<Action<EventMockChild>>(calledCount);
            for (int i = 0; i < calledCount; i++)
            {
                var j = i;
                handlers.Add((e) => j++);
            }
            foreach (var handler in handlers)
            {
                hub.Subscribe<EventMockChild>(handler);
            }
            hub.Uninitialize();
        }

        [TestMethod]
        [Timeout(20)]
        public void TestPerformanceEventsUnsubscribe()
        {
            var hub = this.GetEventHub();
            var calledCount = 20000;
            var handlers = new List<Action<EventMockChild>>(calledCount);
            for (int i = 0; i < calledCount; i++)
            {
                var j = i;
                handlers.Add((e) => j++);
            }
            foreach (var handler in handlers)
            {
                hub.Subscribe<EventMockChild>(handler);
            }
            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                hub.Unsubscribe<EventMockChild>(handlers[i]);
            }
            hub.Uninitialize();
        }

        [TestMethod]
        [Timeout(20)]
        public void TestPerformanceEventsRaise()
        {
            var hub = this.GetEventHub();
            var args = new EventMockChild(10);
            var parentCalled = 0;
            var childCalled = 0;
            var interfaceCalled = 0;
            hub.Subscribe<EventMock>((e) => parentCalled++);
            hub.Subscribe<EventMockChild>((e) => childCalled++);
            hub.Subscribe<IEvent>((e) => interfaceCalled++);
            var calledCount = 5000;
            for (int i = 0; i < calledCount; i++)
            {
                hub.Raise(args);
            }
            Assert.AreEqual(calledCount, parentCalled, "The parent should be called exactly {0} times", calledCount);
            Assert.AreEqual(calledCount, childCalled, "The child should be called exactly {0} times", calledCount);
            Assert.AreEqual(calledCount, interfaceCalled, "The interface should be called exactly {0} times", calledCount);
            hub.Uninitialize();
        }
        #endregion

        #region Private methods
        private DefaultEventHub GetEventHub()
        {
            return new DefaultEventHub();
        }
        #endregion
    }
}
