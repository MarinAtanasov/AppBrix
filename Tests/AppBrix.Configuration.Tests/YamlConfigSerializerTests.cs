// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Tests.Mocks;
using AppBrix.Configuration.Yaml;
using AppBrix.Testing;
using AppBrix.Testing.Xunit;
using FluentAssertions;
using System;
using Xunit;

namespace AppBrix.Configuration.Tests;

public sealed class YamlConfigSerializerTests : TestsBase
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSerializeNullConfig()
    {
        var serializer = new YamlConfigSerializer();
        var action = () => serializer.Serialize(null!);
        action.Should().Throw<ArgumentNullException>("config cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeserializeNullConfig()
    {
        var serializer = new YamlConfigSerializer();
        var action = () => serializer.Deserialize(null!, typeof(ConfigMock));
        action.Should().Throw<ArgumentNullException>("config cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeserializeEmptyConfig()
    {
        var serializer = new YamlConfigSerializer();
        var action = () => serializer.Deserialize(string.Empty, typeof(ConfigMock));
        action.Should().Throw<ArgumentNullException>("config cannot be empty");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestDeserializeNullType()
    {
        var serializer = new YamlConfigSerializer();
        var action = () => serializer.Deserialize(nameof(ConfigMock), null!);
        action.Should().Throw<ArgumentNullException>("type cannot be null");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestReserializeDefaultConfig()
    {
        var serializer = new YamlConfigSerializer();
        var config = new ConfigMock();

        var serialized = serializer.Serialize(config);
        serialized.Should().NotBeNullOrEmpty("the config should be successfully serialized");

        var deserialized = (ConfigMock)serializer.Deserialize(serialized, config.GetType());
        deserialized.Should().NotBeNull("the config should be successfully deserialized");
        deserialized.Should().NotBeSameAs(config, "a new instance should be reserialized");
        deserialized.Enum.Should().Be(config.Enum, "the enum should be successfully reserialized");
        deserialized.TimeSpan.Should().Be(config.TimeSpan, "the timespan should be successfully reserialized");
        deserialized.Version.Should().Be(config.Version, "the version should be successfully reserialized");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestReserializeModifiedConfig()
    {
        var serializer = new YamlConfigSerializer();
        var config = new ConfigMock()
        {
            Enum = DateTimeKind.Utc,
            TimeSpan = TimeSpan.FromDays(1),
            Version = new Version(1, 2, 3)
        };

        var serialized = serializer.Serialize(config);
        serialized.Should().NotBeNullOrEmpty("the config should be successfully serialized");

        var deserialized = ((IConfigSerializer)serializer).Deserialize<ConfigMock>(serialized);
        deserialized.Should().NotBeNull("the config should be successfully deserialized");
        deserialized.Should().NotBeSameAs(config, "a new instance should be reserialized");
        deserialized.Enum.Should().Be(config.Enum, "the enum should be successfully reserialized");
        deserialized.TimeSpan.Should().Be(config.TimeSpan, "the timespan should be successfully reserialized");
        deserialized.Version.Should().Be(config.Version, "the version should be successfully reserialized");
    }
    #endregion
}
