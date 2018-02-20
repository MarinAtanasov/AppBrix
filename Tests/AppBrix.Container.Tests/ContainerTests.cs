// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Container.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AppBrix.Container.Tests
{
    public sealed class ContainerTests
    {
        #region Setup and cleanup
        public ContainerTests()
        {
            this.app = TestUtils.CreateTestApp(typeof(ContainerModule));
            this.app.Start();
        }
        #endregion

        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestGetContainer()
        {
            var container = this.GetContainer();
            container.Should().NotBeNull("unable to get the container");
            var container2 = this.GetContainer();
            container2.Should().NotBeNull("second call did not return a container");
            container2.Should().BeSameAs(container, "returned a different instance of the container");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestResolveByInterface()
        {
            var container = this.GetContainer();
            var iContainer = container.Get<IContainer>();
            iContainer.Should().NotBeNull("unable to resolve the IContainer interface");
            iContainer.Should().BeSameAs(container, "returned IContainer is a different instance");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestResolveByClass()
        {
            var container = this.GetContainer();
            var registered = new ChildMock();
            container.Register(registered);
            var resolved = container.Get<ChildMock>();
            resolved.Should().NotBeNull("unable to resolve the item by class");
            resolved.Should().BeSameAs(registered, "returned item is a different instance than the registered");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestResolveByBaseClass()
        {
            var container = this.GetContainer();
            var original = new ChildMock();
            container.Register(original);
            var resolved = container.Get<ParentMock>();
            resolved.Should().NotBeNull("unable to resolve the Parent class");
            resolved.Should().BeSameAs(original, "returned Child is a different instance");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestResolveAllNoElements()
        {
            var container = this.GetContainer();
            var resolved = container.GetAll().OfType<ContainerTests>().ToList();
            resolved.Should().NotBeNull("resolved collection should not be null");
            resolved.Count.Should().Be(0, "resolved collection should be empty");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestResolveAllOneElement()
        {
            var container = this.GetContainer();
            var resolved = container.GetAll().OfType<IContainer>().ToList();
            resolved.Should().NotBeNull("resolved collection should not be null");
            resolved.Count.Should().Be(1, "resolved collection should have 1 element");
            resolved.Single().Should().Be(container, "resolved element should be the original container");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRegisterNull()
        {
            var container = this.GetContainer();
            Action action = () => container.Register<IContainer>(null);
            action.ShouldThrow<ArgumentNullException>("passing a null object is not allowed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRegisterNullObjectExcplicitType()
        {
            var container = this.GetContainer();
            Action action = () => container.Register(null, typeof(IContainer));
            action.ShouldThrow<ArgumentNullException>("passing a null object is not allowed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRegisterNullTypeExcplicitType()
        {
            var container = this.GetContainer();
            Action action = () => container.Register(container, null);
            action.ShouldThrow<ArgumentNullException>("passing a null object is not allowed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestObjectBaseTypeNotRegistered()
        {
            var container = this.GetContainer();
            container.Register(new ChildMock());
            Action action = () => container.Get<object>();
            action.ShouldThrow<KeyNotFoundException>("items should not be registered as type of object");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
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
            container.GetAll().OfType<ChildMock>().Count().Should().Be(3, "first object has been removed from history");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRegisterGenericObjectError()
        {
            var container = this.GetContainer();
            Action action = () => container.Register<object>(container);
            action.ShouldThrow<ArgumentException>("registering an item as System.Object should not be allowed.");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestRegisterExplicitObjectTypeError()
        {
            var container = this.GetContainer();
            Action action = () => container.Register(container, typeof(object));
            action.ShouldThrow<ArgumentException>("registering an item as System.Object should not be allowed.");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceContainer()
        {
            TestUtils.TestPerformance(this.TestPerformanceContainerInternal);
        }
        #endregion

        #region Private methods
        private IContainer GetContainer()
        {
            return this.app.Container;
        }

        private void TestPerformanceContainerInternal()
        {
            var container = this.GetContainer();
            for (var i = 0; i < 1000; i++)
            {
                this.app.Container.Register(new ChildMock());
            }
            for (var i = 0; i < 300000; i++)
            {
                this.app.Get(typeof(ChildMock));
                this.app.Get(typeof(ParentMock));
                this.app.Get(typeof(IContainer));
                this.app.Container.GetAll();
            }
            this.app.Reinitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
