// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Container.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using System.Collections.Generic;
using Xunit;

namespace AppBrix.Container.Tests;

public sealed class ContainerTests : TestsBase<ContainerModule>
{
    #region Setup and cleanup
    public ContainerTests() => this.App.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetContainer()
    {
        var container = this.App.Container;
        this.Assert(container is not null, "the container should be registered");
        this.Assert(this.App.Container == container, "should return the same instance of the container");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestResolveByInterface()
    {
        this.Assert(this.App.Get<IContainer>() == this.App.Container, "IContainer must be the same instance");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestResolveByClass()
    {
        var container = this.App.Container;
        var registered = new ChildMock();
        container.Register(registered);
        this.Assert(this.App.Get(typeof(ChildMock)) == registered, "returned item must be the same instance than as registered");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestResolveByBaseClass()
    {
        var container = this.App.Container;
        var original = new ChildMock();
        container.Register(original);
        this.Assert(container.Get<ParentMock>() == original, "returned Child must be the same instance");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRegisterNull()
    {
        var container = this.App.Container;
        var action = () => container.Register(null!);
        this.AssertThrows<ArgumentNullException>(action, "passing a null object is not allowed");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestObjectBaseTypeNotRegistered()
    {
        var container = this.App.Container;
        container.Register(new ChildMock());
        Action action = () => container.Get<object>();
        this.AssertThrows<KeyNotFoundException>(action, "items should not be registered as type of object");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDoubleRegistration()
    {
        var container = this.App.Container;
        var resolved = new ChildMock();
        var resolved2 = new ChildMock();
        container.Register(resolved);

        container.Register(resolved2);
        this.Assert(container.Get<ChildMock>() == resolved2, "object should be replaced with second");

        container.Register(resolved);
        this.Assert(container.Get<ChildMock>() == resolved, "object should be replaced with original");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRegisterGenericObjectError()
    {
        var container = this.App.Container;
        var action = () => container.Register(new object());
        this.AssertThrows<ArgumentException>(action, "registering a System.Object should not be allowed.");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRegisterString()
    {
        var container = this.App.Container;
        var action = () => container.Register("AppBrix");
        this.AssertThrows<ArgumentException>(action, "registering a string should not be allowed");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRegisterInt()
    {
        var container = this.App.Container;
        var action = () => container.Register(42);
        this.AssertThrows<ArgumentException>(action, "registering a value type should not be allowed");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceContainer() => this.AssertPerformance(this.TestPerformanceContainerInternal);
    #endregion

    #region Private methods
    private void TestPerformanceContainerInternal()
    {
        var container = this.App.Container;

        for (var i = 0; i < 1000; i++)
        {
            container.Register(new ChildMock());
        }
        for (var i = 0; i < 200000; i++)
        {
            container.Get(typeof(ChildMock));
            container.Get(typeof(ParentMock));
            container.Get(typeof(IContainer));
        }

        this.App.Reinitialize();
    }
    #endregion
}
