// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration.Tests.Mocks;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AppBrix.Configuration.Tests
{
    public class ConfigManagerTests
    {
        #region Tests
        [Fact]
        public void TestGetConfig()
        {
            var provider = new ConfigProviderMock();
            var serializer = new ConfigSerializerMock();
            var manager = new ConfigManager(provider, serializer);

            manager.Get<ConfigMock>();
            provider.ReadConfigs.Should().ContainSingle("the manager should have tried to read the config");
            provider.ReadConfigs[0].Should().Be(typeof(ConfigMock), "the read config should be of the requested type");
            provider.WrittenConfigs.Should().BeEmpty("the manager should not have tried to write the config");
            serializer.Serialized.Should().BeEmpty("the manager should not have tried to serialize the config");
            serializer.Deserialized.Should().ContainSingle("the manager should have tried to deserialize the read config");
            serializer.Deserialized[0].Key.Should().Be(typeof(ConfigMock), "the deserialized config should be the same as the read one");

            manager.Get<ConfigMock>();
            provider.ReadConfigs.Should().ContainSingle("the manager should not have tried to reread the config");
            provider.WrittenConfigs.Should().BeEmpty("the manager should not have tried to write the config when returning it a second time");
            serializer.Serialized.Should().BeEmpty("the manager should not have tried to serialize the config when returning it a second time");
            serializer.Deserialized.Should().ContainSingle("the manager should not have tried to re-deserialize the config");
        }

        [Fact]
        public void TestSaveAllConfigs()
        {
            var provider = new ConfigProviderMock();
            var serializer = new ConfigSerializerMock();
            var manager = new ConfigManager(provider, serializer);

            manager.SaveAll();
            provider.ReadConfigs.Should().BeEmpty("the manager should have not tried to read any configs");
            provider.WrittenConfigs.Should().BeEmpty("the manager should not have tried to write any configs");
            serializer.Serialized.Should().BeEmpty("the manager should not have tried to serialize any configs");
            serializer.Deserialized.Should().BeEmpty("the manager should have tried to deserialize any configs");

            manager.Get<ConfigMock>();
            manager.SaveAll();
            provider.WrittenConfigs.Should().ContainSingle("the manager should have tried to write the config");
            provider.WrittenConfigs[0].Key.Should().Be(typeof(ConfigMock), "the written config should be the same as the requested one");
            serializer.Serialized.Should().ContainSingle("the manager should have tried to serialize the config");
            serializer.Serialized[0].Key.Should().Be(typeof(ConfigMock), "the serialized config should be the same as the requested one");
            
            manager.SaveAll();
            provider.WrittenConfigs.Should().ContainSingle("the manager should not have tried to re-write an unmodified config");
        }

        [Fact]
        public void TestSaveConfig()
        {
            var provider = new ConfigProviderMock();
            var serializer = new ConfigSerializerMock();
            var config = new ConfigMock();
            var manager = new ConfigManager(provider, serializer);

            manager.Save(config);
            provider.WrittenConfigs.Should().ContainSingle("the manager should have tried to write the config");
            provider.WrittenConfigs[0].Key.Should().Be(config.GetType(), "the written config should be the same as the provided one one");
            serializer.Serialized.Should().ContainSingle("the manager should have tried to serialize the config");
            serializer.Serialized[0].Key.Should().Be(config.GetType(), "the serialized config type should be the same as the original");
            serializer.Serialized[0].Value.Should().BeSameAs(config, "the serialized config should be the same as the provided one");

            manager.Save(config);
            provider.WrittenConfigs.Should().ContainSingle("the manager should not have tried to re-write an unmodified config");
        }
        #endregion
    }
}
