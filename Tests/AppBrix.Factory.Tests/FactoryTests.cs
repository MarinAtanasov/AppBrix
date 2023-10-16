// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Factory.Contracts;
using AppBrix.Factory.Services;
using AppBrix.Factory.Tests.Mocks;
using AppBrix.Testing;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Factory.Tests;

public sealed class FactoryTests : TestsBase<FactoryModule>
{
    #region Setup and cleanup
    public FactoryTests() => this.app.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRegisterNullFactory()
    {
        var service = this.GetFactoryService();
        var action = () => service.Register(((IFactory<FactoryTests>)null)!);
        action.Should().Throw<ArgumentNullException>("factory cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRegisterNullFactoryType()
    {
        var service = this.GetFactoryService();
        var factory = new FactoryMock<FactoryTests>(this);
        var action = () => service.Register(factory, null!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRegisterNullFactoryMethod()
    {
        var service = this.GetFactoryService();
        var action = () => service.Register(((Func<FactoryTests>)null)!, typeof(FactoryTests));
        action.Should().Throw<ArgumentNullException>("factory method cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestRegisterNullFactoryMethodType()
    {
        var service = this.GetFactoryService();
        var factory = () => this;
        var action = () => service.Register(factory, null!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestFactoryNonRegisteredObject()
    {
        var service = this.GetFactoryService();

        var method = service.GetFactory<DefaultConstructorClass>();
        method.Should().BeNull("no factory has been registered");

        Action action = () => service.Get<DefaultConstructorClass>();
        action.Should().Throw<InvalidOperationException>("no factory has been registered");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestFactoryDefaultConstructorCall()
    {
        var factory = this.GetFactoryService();
        factory.Register(() => new DefaultConstructorClass());
        var returned1 = factory.Get<DefaultConstructorClass>();
        var returned2 = factory.Get<DefaultConstructorClass>();

        returned1.Should().NotBeNull("the factory should return first object");
        returned2.Should().NotBeNull("the factory should return second object");
        returned1.Should().NotBeSameAs(returned2, "the factory should always return a new object");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestFactoryNonDefaultConstructorCall()
    {
        var service = this.GetFactoryService();
        service.Register(() => new NonDefaultConstructorClass(true));
        var returned1 = service.Get<NonDefaultConstructorClass>();
        var returned2 = service.Get<NonDefaultConstructorClass>();

        returned1.Should().NotBeNull("the factory should return first object");
        returned1.Value.Should().BeTrue($"first object value should be {true}");
        returned1.Modified.Should().BeFalse($"first object modified should be {false}");
        returned2.Should().NotBeNull("the factory should return second object");
        returned2.Value.Should().BeTrue($"second object value should be {true}");
        returned2.Modified.Should().BeFalse($"first object modified should be {false}");
        returned1.Should().NotBeSameAs(returned2, "the factory should always return a new object");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestFactoryRegistersHierarchically()
    {
        var service = this.GetFactoryService();
        var original = new NonDefaultConstructorClass(true);
        service.Register(() => original);

        var returnedChild = service.Get<NonDefaultConstructorClass>();
        returnedChild.Should().NotBeNull("the factory should return child object");
        returnedChild.Should().BeSameAs(original, "child object should be the same as the original object");

        var returnedParent = service.Get<DefaultConstructorClass>();
        returnedParent.Should().NotBeNull("the factory should return parent object");
        returnedParent.Should().BeSameAs(original, "parent object should be the same as the original object");

        var returnedInterface = service.Get<ITestInterface>();
        returnedInterface.Should().NotBeNull("the factory should return object from interface");
        returnedInterface.Should().BeSameAs(original, "interface object should be the same as the original object");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestFactoryCanBeWrapped()
    {
        var service = this.GetFactoryService();
        service.Register(() => new NonDefaultConstructorClass(true));
        var method = service.GetFactory<NonDefaultConstructorClass>()!;
        service.Register(() =>
        {
            var obj = method.Get();
            obj.Modified = true;
            return obj;
        });

        var returned = service.Get<NonDefaultConstructorClass>();
        returned.Should().NotBeNull("the factory should return child object");
        returned.Value.Should().BeTrue($"object value should be {true}");
        returned.Modified.Should().BeTrue($"object modified should be {true}");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceFactory() => this.AssertPerformance(this.TestPerformanceFactoryInternal);
    #endregion

    #region Private methods
    private IFactoryService GetFactoryService() => this.app.GetFactoryService();

    private void TestPerformanceFactoryInternal()
    {
        var service = this.GetFactoryService();
        FactoryTests Factory() => this;
        var type = typeof(FactoryTests);

        for (var i = 0; i < 1000; i++)
        {
            service.Register(Factory, type);
        }

        for (var i = 0; i < 400000; i++)
        {
            service.Get(type);
        }

        this.app.Reinitialize();
    }
    #endregion
}
