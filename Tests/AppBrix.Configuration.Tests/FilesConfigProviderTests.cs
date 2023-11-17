// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Files;
using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace AppBrix.Configuration.Tests;

public sealed class FilesConfigProviderTests : TestsBase
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullSerializer()
    {
        var action = () => new FilesConfigProvider(null!, "test_dir");
        action.Should().Throw<ArgumentNullException>("serializer cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullDirectory()
    {
        var action = () => new FilesConfigProvider(new ConfigSerializerMock(), null!);
        action.Should().Throw<ArgumentNullException>("directory cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorEmptyDirectory()
    {
        var action = () => new FilesConfigProvider(new ConfigSerializerMock(), string.Empty);
        action.Should().Throw<ArgumentNullException>("directory cannot be empty");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullFileExtension()
    {
        var action = () => new FilesConfigProvider(new ConfigSerializerMock(), "test_dir", null!);
        action.Should().Throw<ArgumentNullException>("fileExtension cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorEmptyFileExtension()
    {
        var action = () => new FilesConfigProvider(new ConfigSerializerMock(), "test_dir", string.Empty);
        action.Should().Throw<ArgumentNullException>("fileExtension cannot be empty");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetConfigNullType()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var provider = new FilesConfigProvider(new ConfigSerializerMock(), directory);
        var action = () => provider.Get(null!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveConfigNullConfig()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var provider = new FilesConfigProvider(new ConfigSerializerMock(), directory);
        var action = () => provider.Save(((IConfig)null)!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }
    #endregion
}
