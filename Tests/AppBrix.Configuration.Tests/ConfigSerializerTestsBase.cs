// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using System;
using Xunit;

namespace AppBrix.Configuration.Tests;

public abstract class ConfigSerializerTestsBase : TestsBase
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSerializeNullConfig()
    {
        var serializer = this.GetSerializer();
        var action = () => serializer.Serialize(null!);
        this.AssertThrows<ArgumentNullException>(action, "config cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeserializeNullConfig()
    {
        var serializer = this.GetSerializer();
        var action = () => serializer.Deserialize(null!, typeof(ConfigMock));
        this.AssertThrows<ArgumentNullException>(action, "config cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeserializeEmptyConfig()
    {
        var serializer = this.GetSerializer();
        var action = () => serializer.Deserialize(string.Empty, typeof(ConfigMock));
        this.AssertThrows<ArgumentNullException>(action, "config cannot be empty");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeserializeNullType()
    {
        var serializer = this.GetSerializer();
        var action = () => serializer.Deserialize(nameof(ConfigMock), null!);
        this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestReserializeDefaultConfig()
    {
        var serializer = this.GetSerializer();
        var config = new ConfigMock();

        var serialized = serializer.Serialize(config);
        this.Assert(string.IsNullOrEmpty(serialized) == false, "the config should be successfully serialized");

        var deserialized = serializer.Deserialize<ConfigMock>(serialized);
        this.Assert(deserialized is not null, "the config should be successfully deserialized");
        this.Assert(deserialized != config, "a new instance should be reserialized");
        this.Assert(deserialized!.Enum == config.Enum, "the enum should be successfully reserialized");
        this.Assert(deserialized!.TimeSpan == config.TimeSpan, "the timespan should be successfully reserialized");
        this.Assert(deserialized!.Version == config.Version, "the version should be successfully reserialized");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestReserializeModifiedConfig()
    {
        var serializer = this.GetSerializer();
        var config = new ConfigMock()
        {
            Enum = DateTimeKind.Utc,
            TimeSpan = TimeSpan.FromDays(1),
            Version = new Version(1, 2, 3)
        };

        var serialized = serializer.Serialize(config);
        this.Assert(string.IsNullOrEmpty(serialized) == false, "the config should be successfully serialized");

        var deserialized = (ConfigMock)serializer.Deserialize(serialized, config.GetType());
        this.Assert(deserialized is not null, "the config should be successfully deserialized");
        this.Assert(deserialized != config, "a new instance should be reserialized");
        this.Assert(deserialized.Enum == config.Enum, "the enum should be successfully reserialized");
        this.Assert(deserialized.TimeSpan == config.TimeSpan, "the timespan should be successfully reserialized");
        this.Assert(deserialized.Version == config.Version, "the version should be successfully reserialized");
    }
    #endregion

    #region Protected methods
    protected abstract IConfigSerializer GetSerializer();
    #endregion
}
