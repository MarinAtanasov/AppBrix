// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Container.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Container.Tests
{
    public class ContainerTests
    {
        #region Setup and cleanup
        public ContainerTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ContainerModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestGetContainer()
        {
            var container = this.GetContainer();
            container.Should().NotBeNull("unable to get the container");
            var container2 = this.GetContainer();
            container2.Should().NotBeNull("second call did not return a container");
            container2.Should().BeSameAs(container, "returned a different instance of the container");
        }

        [Fact]
        public void TestResolveByInterface()
        {
            var container = this.GetContainer();
            var iContainer = container.Get<IContainer>();
            iContainer.Should().NotBeNull("unable to resolve the IContainer interface");
            iContainer.Should().BeSameAs(container, "returned IContainer is a different instance");
        }

        [Fact]
        public void TestResolveByClass()
        {
            var container = this.GetContainer();
            var registered = new ChildMock();
            container.Register(registered);
            var resolved = container.Get<ChildMock>();
            resolved.Should().NotBeNull("unable to resolve the item by class");
            resolved.Should().BeSameAs(registered, "returned item is a different instance than the registered");
        }

        [Fact]
        public void TestResolveByBaseClass()
        {
            var container = this.GetContainer();
            var original = new ChildMock();
            container.Register(original);
            var resolved = container.Get<ParentMock>();
            resolved.Should().NotBeNull("unable to resolve the Parent class");
            resolved.Should().BeSameAs(original, "returned Child is a different instance");
        }

        [Fact]
        public void TestResolveAllNoElements()
        {
            var container = this.GetContainer();
            var resolved = container.GetAll().OfType<ContainerTests>().ToList();
            resolved.Should().NotBeNull("resolved collection should not be null");
            resolved.Count.Should().Be(0, "resolved collection should be empty");
        }

        [Fact]
        public void TestResolveAllOneElement()
        {
            var container = this.GetContainer();
            var resolved = container.GetAll().OfType<IContainer>().ToList();
            resolved.Should().NotBeNull("resolved collection should not be null");
            resolved.Count.Should().Be(1, "resolved collection should have 1 element");
            resolved.Single().Should().Be(container, "resolved element should be the original container");
        }

        [Fact]
        public void TestResolveAllTwoElements()
        {
            var container = this.GetContainer();
            var first = new ParentMock();
            container.Register(first);
            var second = new ChildMock();
            container.Register(second, second.GetType());
            var resolved = container.GetAll().OfType<ParentMock>().ToList();
            resolved.Should().NotBeNull("resolved collection should not be null");
            resolved.Count.Should().Be(2, "resolved collection should have 2 elements");
            resolved.First().Should().BeSameAs(first, "resolved element 1 should be the original item");
            resolved.Last().Should().BeSameAs(second, "resolved element 2 should be the new item");
        }

        [Fact]
        public void TestRegisterNull()
        {
            var container = this.GetContainer();
            Action action = () => container.Register<IContainer>(null);
            action.ShouldThrow<ArgumentNullException>("passing a null object is not allowed");
        }

        [Fact]
        public void TestRegisterNullObjectExcplicitType()
        {
            var container = this.GetContainer();
            Action action = () => container.Register(null, typeof(IContainer));
            action.ShouldThrow<ArgumentNullException>("passing a null object is not allowed");
        }

        [Fact]
        public void TestRegisterNullTypeExcplicitType()
        {
            var container = this.GetContainer();
            Action action = () => container.Register(container, null);
            action.ShouldThrow<ArgumentNullException>("passing a null object is not allowed");
        }

        [Fact]
        public void TestObjectBaseTypeNotRegistered()
        {
            var container = this.GetContainer();
            container.Register(new ChildMock());
            Action action = () => container.Get<object>();
            action.ShouldThrow<InvalidOperationException>("items should not be registered as type of object");
        }

        [Fact]
        public void TestDoubleRegistration()
        {
            var container = this.GetContainer();
            var resolved = new ChildMock();
            var resolved2 = new ChildMock();
            container.Register(resolved);
            container.Register(resolved2);
            container.Get<ChildMock>().Should().BeSameAs(resolved2, "object not replaced with second");
            container.Register(resolved);
            container.Get<ChildMock>().Should().BeSameAs(resolved, "object not replaced with original");
            container.GetAll().OfType<ChildMock>().Count().Should().Be(2, "first object has been removed from history");
        }

        [Fact]
        public void TestRegisterGenericObjectError()
        {
            var container = this.GetContainer();
            Action action = () => container.Register<object>(container);
            action.ShouldThrow<ArgumentException>("registering an item as System.Object should not be allowed.");
        }

        [Fact]
        public void TestRegisterExplicitObjectTypeError()
        {
            var container = this.GetContainer();
            Action action = () => container.Register(container, typeof(object));
            action.ShouldThrow<ArgumentException>("registering an item as System.Object should not be allowed.");
        }

        [Fact]
        public void TestPerformanceContainer()
        {
            Action action = this.TestPerformanceContainerInternal;

            // Invoke the action once to make sure that the assemblies are loaded.
            action.Invoke();
            this.app.Reinitialize();

            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }
        #endregion

        #region Private methods
        private IContainer GetContainer()
        {
            return this.app.GetContainer();
        }

        private void TestPerformanceContainerInternal()
        {
            var container = this.GetContainer();
            for (var i = 0; i < 12000; i++)
            {
                container.Register(new ChildMock());
            }
            for (var i = 0; i < 120000; i++)
            {
                container.Get<ChildMock>();
                container.Get<ParentMock>();
                container.Get<IContainer>();
                container.GetAll();
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
