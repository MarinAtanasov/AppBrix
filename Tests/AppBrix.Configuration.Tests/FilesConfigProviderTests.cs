// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Files;
using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace AppBrix.Configuration.Tests;

public sealed class FilesConfigProviderTests
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullDirectory()
    {
        var action = () => new FilesConfigProvider(null!);
        action.Should().Throw<ArgumentNullException>("directory cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorEmptyDirectory()
    {
        var action = () => new FilesConfigProvider(string.Empty);
        action.Should().Throw<ArgumentNullException>("directory cannot be empty");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullFileExtension()
    {
        var action = () => new FilesConfigProvider("test_dir", null!);
        action.Should().Throw<ArgumentNullException>("fileExtension cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorEmptyFileExtension()
    {
        var action = () => new FilesConfigProvider("test_dir", string.Empty);
        action.Should().Throw<ArgumentNullException>("fileExtension cannot be empty");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestReadConfigNullType()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var provider = new FilesConfigProvider(directory);
        var action = () => provider.ReadConfig(null!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestWriteConfigNullConfig()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var provider = new FilesConfigProvider(directory);
        var action = () => provider.WriteConfig(null!, typeof(ConfigMock));
        action.Should().Throw<ArgumentNullException>("config cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestWriteConfigEmptyConfig()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var provider = new FilesConfigProvider(directory);
        var action = () => ((IConfigProvider)provider).WriteConfig<ConfigMock>(string.Empty);
        action.Should().Throw<ArgumentNullException>("config cannot be empty");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestWriteConfigNullType()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var provider = new FilesConfigProvider(directory);
        var action = () => provider.WriteConfig(nameof(ConfigMock), null!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }
    #endregion
}
