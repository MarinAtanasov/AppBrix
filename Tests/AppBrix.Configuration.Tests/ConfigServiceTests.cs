// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using Xunit;

namespace AppBrix.Configuration.Tests;

public sealed class ConfigServiceTests : TestsBase
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestConstructorNullProvider()
    {
        var action = () => new ConfigService(null!);
        this.AssertThrows<ArgumentNullException>(action, "provider cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetNullConfig()
    {
        var service = new ConfigService(new ConfigProviderMock());
        var action = () => service.Get(null!);
        this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveNullConfig()
    {
        var service = new ConfigService(new ConfigProviderMock());
        var action = () => service.Save(((IConfig)null)!);
        this.AssertThrows<ArgumentNullException>(action, "config cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveNullType()
    {
        IConfigService service = new ConfigService(new ConfigProviderMock());
        var action = () => service.Save(((Type)null)!);
        this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestGetConfig()
    {
        var provider = new ConfigProviderMock();
        IConfigService service = new ConfigService(provider);

        service.Get<ConfigMock>();
        this.Assert(provider.ReadConfigs.Count == 1, "the service should have tried to read the config");
        this.Assert(provider.ReadConfigs[0] == typeof(ConfigMock), "the read config should be of the requested type");
        this.Assert(provider.WrittenConfigs.Count == 0, "the service should not have tried to write the config");

        service.Get<ConfigMock>();
        this.Assert(provider.ReadConfigs.Count == 1, "the service should not have tried to reread the config");
        this.Assert(provider.WrittenConfigs.Count == 0, "the service should not have tried to write the config when returning it a second time");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveAllConfigs()
    {
        var provider = new ConfigProviderMock();
        IConfigService service = new ConfigService(provider);

        service.Save();
        this.Assert(provider.ReadConfigs.Count == 0, "the service should have not tried to read any configs");
        this.Assert(provider.WrittenConfigs.Count == 0, "the service should not have tried to write any configs");

        service.Get<ConfigMock>();
        service.Save();
        this.Assert(provider.WrittenConfigs.Count == 1, "the service should have tried to write the config");
        this.Assert(provider.WrittenConfigs[0].Key == typeof(ConfigMock), "the written config should be the same as the requested one");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveConfig()
    {
        var provider = new ConfigProviderMock();
        IConfigService service = new ConfigService(provider);
        var config = service.Get<ConfigMock>();

        service.Save<ConfigMock>();
        this.Assert(provider.WrittenConfigs.Count == 1, "the service should have tried to write the config");
        this.Assert(provider.WrittenConfigs[0].Key == config.GetType(), "the written config should be the same as the provided one one");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceConfigService() => this.AssertPerformance(this.TestPerformanceConfigServiceInternal);
    #endregion

    #region Private methods
    private void TestPerformanceConfigServiceInternal()
    {
        var provider = new ConfigProviderMock();
        var service = new ConfigService(provider);
        var type = typeof(ConfigMock);

        for (var i = 0; i < 800000; i++)
        {
            service.Get(type);
        }

        for (var i = 0; i < 800; i++)
        {
            service.Save();
        }
    }
    #endregion
}
