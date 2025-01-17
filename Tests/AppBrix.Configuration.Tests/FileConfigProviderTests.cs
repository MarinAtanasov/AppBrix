// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Files;
using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Testing;
using System;
using System.Reflection;

namespace AppBrix.Configuration.Tests;

[TestClass]
public sealed class FileConfigProviderTests : TestsBase
{
    #region Tests
    [Test, Functional]
    public void TestConstructorNullSerializer()
    {
        var action = () => new FileConfigProvider(null!, "test_dir");
        this.AssertThrows<ArgumentNullException>(action, "serializer cannot be null");;
    }
    
    [Test, Functional]
    public void TestConstructorNullPath()
    {
        var action = () => new FileConfigProvider(new ConfigSerializerMock(), null!);
        this.AssertThrows<ArgumentNullException>(action, "directory cannot be null");;
    }
    
    [Test, Functional]
    public void TestConstructorEmptyPath()
    {
        var action = () => new FileConfigProvider(new ConfigSerializerMock(), string.Empty);
        this.AssertThrows<ArgumentNullException>(action, "directory cannot be empty");;
    }
    
    [Test, Functional]
    public void TestGetConfigNullType()
    {
        var path = Assembly.GetExecutingAssembly().Location;
        var provider = new FileConfigProvider(new ConfigSerializerMock(), path);
        var action = () => provider.Get(null!);
        this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
    }
    
    [Test, Functional]
    public void TestSaveConfigNullConfig()
    {
        var path = Assembly.GetExecutingAssembly().Location;
        var provider = new FileConfigProvider(new ConfigSerializerMock(), path);
        var action = () => provider.Save(((IConfig)null)!);
        this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
    }
    #endregion
}
