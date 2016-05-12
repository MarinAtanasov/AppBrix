// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Resolver.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Resolver.Tests
{
    public class ResolverTests : IDisposable
    {
        #region Setup and cleanup
        public ResolverTests()
        {
            this.app = TestUtils.CreateTestApp(
                typeof(ResolverModule));
            this.app.Start();
        }

        public void Dispose()
        {
            this.app.Stop();
        }
        #endregion

        #region Tests
        [Fact]
        public void TestGetResolver()
        {
            var resolver = this.GetResolver();
            resolver.Should().NotBeNull("unable to get the resolver");
            var resolver2 = this.GetResolver();
            resolver2.Should().NotBeNull("second call did not return a resolver");
            resolver2.Should().BeSameAs(resolver, "returned a different instance of the resolver");
        }

        [Fact]
        public void TestResolveByInterface()
        {
            var resolver = this.GetResolver();
            var iResolver = resolver.Get<IResolver>();
            iResolver.Should().NotBeNull("unable to resolve the IResolver interface");
            iResolver.Should().BeSameAs(resolver, "returned IResolver is a different instance");
        }

        [Fact]
        public void TestResolveByClass()
        {
            var resolver = this.GetResolver();
            var registered = new ChildMock();
            resolver.Register(registered);
            var resolved = resolver.Get<ChildMock>();
            resolved.Should().NotBeNull("unable to resolve the item by class");
            resolved.Should().BeSameAs(registered, "returned item is a different instance than the registered");
        }

        [Fact]
        public void TestResolveByBaseClass()
        {
            var resolver = this.GetResolver();
            var original = new ChildMock();
            resolver.Register(original);
            var resolved = resolver.Get<ParentMock>();
            resolved.Should().NotBeNull("unable to resolve the Parent class");
            resolved.Should().BeSameAs(original, "returned Child is a different instance");
        }

        [Fact]
        public void TestResolveAllNoElements()
        {
            var resolver = this.GetResolver();
            var resolved = resolver.GetAll().OfType<ResolverTests>();
            resolved.Should().NotBeNull("resolved collection should not be null");
            resolved.Count().Should().Be(0, "resolved collection should be empty");
        }

        [Fact]
        public void TestResolveAllOneElement()
        {
            var resolver = this.GetResolver();
            var resolved = resolver.GetAll().OfType<IResolver>();
            resolved.Should().NotBeNull("resolved collection should not be null");
            resolved.Count().Should().Be(1, "resolved collection should have 1 element");
            resolved.Single().Should().Be(resolver, "resolved element should be the original resolver");
        }

        [Fact]
        public void TestResolveAllTwoElements()
        {
            var resolver = this.GetResolver();
            var first = new ParentMock();
            resolver.Register(first);
            var second = new ChildMock();
            resolver.Register(second, second.GetType());
            var resolved = resolver.GetAll().OfType<ParentMock>();
            resolved.Should().NotBeNull("resolved collection should not be null");
            resolved.Count().Should().Be(2, "resolved collection should have 2 elements");
            resolved.First().Should().BeSameAs(first, "resolved element 1 should be the original item");
            resolved.Last().Should().BeSameAs(second, "resolved element 2 should be the new item");
        }

        [Fact]
        public void TestRegisterNull()
        {
            var resolver = this.GetResolver();
            Action action = () => resolver.Register<IResolver>(null);
            action.ShouldThrow<ArgumentNullException>("passing a null object is not allowed");
        }

        [Fact]
        public void TestRegisterNullObjectExcplicitType()
        {
            var resolver = this.GetResolver();
            Action action = () => resolver.Register(null, typeof(IResolver));
            action.ShouldThrow<ArgumentNullException>("passing a null object is not allowed");
        }

        [Fact]
        public void TestRegisterNullTypeExcplicitType()
        {
            var resolver = this.GetResolver();
            Action action = () => resolver.Register(resolver, null);
            action.ShouldThrow<ArgumentNullException>("passing a null object is not allowed");
        }

        [Fact]
        public void TestObjectBaseTypeNotRegistered()
        {
            var resolver = this.GetResolver();
            resolver.Register(new ChildMock());
            resolver.Get<object>().Should().BeNull("items should not be registered as type of object");
        }

        [Fact]
        public void TestDoubleRegistration()
        {
            var resolver = this.GetResolver();
            var resolved = new ChildMock();
            var resolved2 = new ChildMock();
            resolver.Register(resolved);
            resolver.Register(resolved2);
            resolver.Get<ChildMock>().Should().BeSameAs(resolved2, "object not replaced with second");
            resolver.Register(resolved);
            resolver.Get<ChildMock>().Should().BeSameAs(resolved, "object not replaced with original");
            resolver.GetAll().OfType<ChildMock>().Count().Should().Be(2, "first object has been removed from history");
        }

        [Fact]
        public void TestRegisterGenericObjectError()
        {
            var resolver = this.GetResolver();
            Action action = () => resolver.Register<object>(resolver);
            action.ShouldThrow<ArgumentException>("registering an item as System.Object should not be allowed.");
        }

        [Fact]
        public void TestRegisterExplicitObjectTypeError()
        {
            var resolver = this.GetResolver();
            Action action = () => resolver.Register(resolver, typeof(object));
            action.ShouldThrow<ArgumentException>("registering an item as System.Object should not be allowed.");
        }

        [Fact]
        public void TestPerformanceResolver()
        {
            Action action = this.TestPerformanceResolverInternal;
            action.ExecutionTime().ShouldNotExceed(TimeSpan.FromMilliseconds(30), "this is a performance test");
        }
        #endregion

        #region Private methods
        private IResolver GetResolver()
        {
            return this.app.GetResolver();
        }

        private void TestPerformanceResolverInternal()
        {
            var resolver = this.GetResolver();
            for (int i = 0; i < 10000; i++)
            {
                resolver.Register(new ChildMock());
            }
            for (int i = 0; i < 10000; i++)
            {
                resolver.Get<ChildMock>();
                resolver.Get<ParentMock>();
                resolver.Get<IResolver>();
                resolver.GetAll();
            }
        }
        #endregion

        #region Private fields and constants
        private readonly IApp app;
        #endregion
    }
}
