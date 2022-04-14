// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Memory;
using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Tests;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Configuration.Tests;

public sealed class MemoryConfigServiceTests
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNullType()
    {
        var service = new MemoryConfigService();
        var action = () => service.Get(null);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveNullConfig()
    {
        var service = new MemoryConfigService();
        var action = () => service.Save(null);
        action.Should().Throw<ArgumentNullException>("config cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveDoesNotThrow() => new MemoryConfigService().Save();

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNewConfig()
    {
        var service = new MemoryConfigService();
        var config = service.Get(typeof(ConfigMock));
        config.Should().NotBeNull("a new instance should be created and returned");
        service.Get(typeof(ConfigMock)).Should().BeSameAs(config, "the same config should be returned");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetSavedConfig()
    {
        var service = new MemoryConfigService();
        var config = new ConfigMock();
        service.Save(config);
        service.Get(typeof(ConfigMock)).Should().BeSameAs(config, "the saved config should be returned");
    }
    #endregion
}
