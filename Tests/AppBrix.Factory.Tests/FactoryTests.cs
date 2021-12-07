// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Factory.Services;
using AppBrix.Factory.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Factory.Tests;

public sealed class FactoryTests : TestsBase
{
    #region Setup and cleanup
    public FactoryTests() : base(TestUtils.CreateTestApp<FactoryModule>()) => this.app.Start();
    #endregion

    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestFactoryNonRegisteredObject()
    {
        var factory = this.GetFactory();

        var method = factory.GetFactory<DefaultConstructorClass>();
        method.Should().BeNull("no factory has been registered");

        Action action = () => factory.Get<DefaultConstructorClass>();
        action.Should().Throw<InvalidOperationException>("no factory has been registered");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestFactoryDefaultConstructorCall()
    {
        var factory = this.GetFactory();
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
        var factory = this.GetFactory();
        factory.Register(() => new NonDefaultConstructorClass(true));
        var returned1 = factory.Get<NonDefaultConstructorClass>();
        var returned2 = factory.Get<NonDefaultConstructorClass>();

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
        var factory = this.GetFactory();
        var original = new NonDefaultConstructorClass(true);
        factory.Register(() => original);

        var returnedChild = factory.Get<NonDefaultConstructorClass>();
        returnedChild.Should().NotBeNull("the factory should return child object");
        returnedChild.Should().BeSameAs(original, "child object should be the same as the original object");

        var returnedParent = factory.Get<DefaultConstructorClass>();
        returnedParent.Should().NotBeNull("the factory should return parent object");
        returnedParent.Should().BeSameAs(original, "parent object should be the same as the original object");

        var returnedInterface = factory.Get<ITestInterface>();
        returnedInterface.Should().NotBeNull("the factory should return object from interface");
        returnedInterface.Should().BeSameAs(original, "interface object should be the same as the original object");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestFactoryCanBeWrapped()
    {
        var factory = this.GetFactory();
        factory.Register(() => new NonDefaultConstructorClass(true));
        var method = factory.GetFactory<NonDefaultConstructorClass>();
        factory.Register(() =>
        {
            var obj = method.Get();
            obj.Modified = true;
            return obj;
        });

        var returned = factory.Get<NonDefaultConstructorClass>();
        returned.Should().NotBeNull("the factory should return child object");
        returned.Value.Should().BeTrue($"object value should be {true}");
        returned.Modified.Should().BeTrue($"object modified should be {true}");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceFactory() => TestUtils.TestPerformance(this.TestPerformanceFactoryInternal);
    #endregion

    #region Private methods
    private IFactoryService GetFactory() => this.app.GetFactoryService();

    private void TestPerformanceFactoryInternal()
    {
        var factory = this.GetFactory();
        FactoryTests method() => this;
        var type = typeof(FactoryTests);

        for (var i = 0; i < 1000; i++)
        {
            factory.Register(method, type);
        }

        for (var i = 0; i < 400000; i++)
        {
            factory.Get(type);
        }

        this.app.Reinitialize();
    }
    #endregion
}
