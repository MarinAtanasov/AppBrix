// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using AppBrix.Tests.Mocks;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Tests;

public sealed class AppTests : TestsBase
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestCreateAppNullConfigService()
    {
        var action = () => AppBrix.App.Create(null!);
        action.Should().Throw<ArgumentNullException>("config service cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAppConfigCreateNullType()
    {
        var action = () => ModuleConfigElement.Create(null!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestAppConfigCreateInvalidType()
    {
        var action = () => ModuleConfigElement.Create(typeof(object));
        action.Should().Throw<ArgumentException>("type must implement IModule");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetAllDependenciesExtensionNullModule()
    {
        var action = () => AppBrixExtensions.GetAllDependencies(null!);
        action.Should().Throw<ArgumentNullException>("module cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceGetDependencies() => this.AssertPerformance(this.TestPerformanceGetDependenciesInternal);
    #endregion

    #region Private methods
    private void TestPerformanceGetDependenciesInternal()
    {
        var module = new EmptyModuleMock();
        for (var i = 0; i < 100; i++)
        {
            _ = module.Dependencies.Count();
        }
    }
    #endregion
}
