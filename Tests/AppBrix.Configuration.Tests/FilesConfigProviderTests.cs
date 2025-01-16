// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Files;
using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
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
        this.AssertThrows<ArgumentNullException>(action, "serializer cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullDirectory()
    {
        var action = () => new FilesConfigProvider(new ConfigSerializerMock(), null!);
        this.AssertThrows<ArgumentNullException>(action, "directory cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorEmptyDirectory()
    {
        var action = () => new FilesConfigProvider(new ConfigSerializerMock(), string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "directory cannot be empty");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullFileExtension()
    {
        var action = () => new FilesConfigProvider(new ConfigSerializerMock(), "test_dir", null!);
        this.AssertThrows<ArgumentNullException>(action, "fileExtension cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorEmptyFileExtension()
    {
        var action = () => new FilesConfigProvider(new ConfigSerializerMock(), "test_dir", string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "fileExtension cannot be empty");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetConfigNullType()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var provider = new FilesConfigProvider(new ConfigSerializerMock(), directory);
        var action = () => provider.Get(null!);
        this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveConfigNullConfig()
    {
        var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var provider = new FilesConfigProvider(new ConfigSerializerMock(), directory);
        var action = () => provider.Save(((IConfig)null)!);
        this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
    }
    #endregion
}
