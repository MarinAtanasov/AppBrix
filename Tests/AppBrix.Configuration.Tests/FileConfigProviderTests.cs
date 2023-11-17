// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Files;
using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using System.Reflection;
using Xunit;

namespace AppBrix.Configuration.Tests;

public sealed class FileConfigProviderTests : TestsBase
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullSerializer()
    {
        var action = () => new FileConfigProvider(null!, "test_dir");
        action.Should().Throw<ArgumentNullException>("serializer cannot be null");
    }
    
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullPath()
    {
        var action = () => new FileConfigProvider(new ConfigSerializerMock(), null!);
        action.Should().Throw<ArgumentNullException>("directory cannot be null");
    }
    
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorEmptyPath()
    {
        var action = () => new FileConfigProvider(new ConfigSerializerMock(), string.Empty);
        action.Should().Throw<ArgumentNullException>("directory cannot be empty");
    }
    
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetConfigNullType()
    {
        var path = Assembly.GetExecutingAssembly().Location;
        var provider = new FileConfigProvider(new ConfigSerializerMock(), path);
        var action = () => provider.Get(null!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }
    
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveConfigNullConfig()
    {
        var path = Assembly.GetExecutingAssembly().Location;
        var provider = new FileConfigProvider(new ConfigSerializerMock(), path);
        var action = () => provider.Save(((IConfig)null)!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }
    #endregion
}
