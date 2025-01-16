// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Memory;
using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using Xunit;

namespace AppBrix.Configuration.Tests;

public sealed class MemoryConfigServiceTests : TestsBase
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNullType()
    {
        var service = new MemoryConfigService();
        var action = () => service.Get(null!);
        this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveNullConfig()
    {
        var service = new MemoryConfigService();
        var action = () => service.Save(null!);
        this.AssertThrows<ArgumentNullException>(action, "config cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveDoesNotThrow() => new MemoryConfigService().Save();

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNewConfig()
    {
        var service = new MemoryConfigService();
        var config = service.Get(typeof(ConfigMock));
        this.Assert(config is not null, "a new instance should be created and returned");
        this.Assert(service.Get(typeof(ConfigMock)) == config, "the same config should be returned");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetSavedConfig()
    {
        var service = new MemoryConfigService();
        var config = new ConfigMock();
        service.Save(config);
        this.Assert(service.Get(typeof(ConfigMock)) == config, "the saved config should be returned");
    }
    #endregion
}
