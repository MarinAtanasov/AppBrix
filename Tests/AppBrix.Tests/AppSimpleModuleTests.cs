// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Testing;
using AppBrix.Testing.Modules;
using AppBrix.Testing.Xunit;
using AppBrix.Tests.Mocks;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace AppBrix.Tests;

public sealed class AppSimpleModuleTests : TestsBase<SimpleModuleMock>
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestSaveConfig()
    {
        var configService = this.App.ConfigService;
        var oldConfig = configService.Get<AppConfig>();
        oldConfig.Modules.Clear();
        var newConfig = new AppConfig();
        newConfig.Modules.Add(ModuleConfigElement.Create<SimpleModuleMock>());
        configService.Save(newConfig);
        var config = configService.GetAppConfig();
        config.Should().Be(newConfig, "Should return new config after saving it");
        oldConfig.Modules.Should().BeEmpty("Original config should not be modified.");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestStartStartedApp()
    {
        this.App.Start();
        var action = () => this.App.Start();
        action.Should().Throw<InvalidOperationException>("cannot start already started app");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestStopStoppedApp()
    {
        var action = () => this.App.Stop();
        action.Should().Throw<InvalidOperationException>("cannot stop already stopped app");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestInitializeInitializedApp()
    {
        this.App.Start();
        var action = () => this.App.Initialize();
        action.Should().NotThrow("initializing already initialized app should be safe");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestInitializeStoppedApp()
    {
        var action = () => this.App.Initialize();
        action.Should().Throw<InvalidOperationException>("cannot initialize stopped app");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUninitializeUninitializedApp()
    {
        this.App.Start();
        this.App.Uninitialize();
        var action = () => this.App.Uninitialize();
        action.Should().NotThrow("uninitializing already uninitialized app should be safe");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUninitializeStoppedApp()
    {
        var action = () => this.App.Uninitialize();
        action.Should().Throw<InvalidOperationException>("cannot uninitialize stopped app");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestInitializeModule()
    {
        this.App.Start();
        var module = this.GetModules().Select(x => x.Module).OfType<SimpleModuleMock>().Single();
        module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
        module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
        this.App.Uninitialize();
        module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
        this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(SimpleModuleMock).GetAssemblyQualifiedName())
            .Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestInitializeDisabledModule()
    {
        this.App.ConfigService.GetAppConfig().Modules.Single().Status = ModuleStatus.Disabled;
        this.App.Start();
        var module = this.GetModules().Select(x => x.Module).Cast<SimpleModuleMock>().SingleOrDefault();
        module.Should().BeNull("disabled modules should not be loaded in memory");
        this.App.Uninitialize();
        this.App.ConfigService.GetAppConfig().Modules.Single().Status.Should().Be(ModuleStatus.Disabled, "module status should not be changed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestInstallMainModule()
    {
        var configService = this.App.ConfigService;
        configService.GetAppConfig().Modules.Should().HaveCount(1, $"{nameof(AppBrix.App)}.{nameof(AppBrix.App.Create)} should add the main module");
        this.App.Start();
        var dependency = this.App.ConfigService.GetAppConfig().Modules[0];
        dependency.Type.Should().Be(typeof(SimpleModuleMock).GetAssemblyQualifiedName(), "The dependency should be placed before the main module");
        configService.GetAppConfig().Modules.Should().HaveCount(2, "Main module should add its dependencies");
        this.App.Stop();
        configService.GetAppConfig().Modules.Should().HaveCount(2, "Stopping the application shouldn't change the config");

        var newApp = AppBrix.App.Create<MainModule<SimpleModuleMock>>(configService);
        configService.GetAppConfig().Modules.Should().HaveCount(2, "Creating a new app shouldn't change the valid config");
        newApp.Start();
        configService.GetAppConfig().Modules.Should().HaveCount(2, "Starting the new app shouldn't change the valid config");
        newApp.Stop();
        configService.GetAppConfig().Modules.Should().HaveCount(2, "Stopping the new app shouldn't change the config");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceReinitialize()
    {
        this.App.Start();

        this.AssertPerformance(this.TestPerformanceReinitializeInternal);
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
    public void TestPerformanceRestart()
    {
        this.App.Start();

        this.AssertPerformance(this.TestPerformanceRestartInternal);
    }
    #endregion

    #region Private methods
    private IEnumerable<ModuleInfo> GetModules() => (IEnumerable<ModuleInfo>)this.modulesField.GetValue(this.App)!;

    private void TestPerformanceReinitializeInternal()
    {
        for (var i = 0; i < 4000; i++)
        {
            this.App.Reinitialize();
        }
    }

    private void TestPerformanceRestartInternal()
    {
        for (var i = 0; i < 750; i++)
        {
            this.App.Restart();
        }
    }
    #endregion

    #region Private fields and constants
    private readonly FieldInfo modulesField = typeof(DefaultApp).GetField("modules", BindingFlags.NonPublic | BindingFlags.Instance)!;
    #endregion
}
