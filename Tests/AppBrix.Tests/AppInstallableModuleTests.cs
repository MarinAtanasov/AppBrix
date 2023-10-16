﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Testing;
using AppBrix.Tests.Mocks;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace AppBrix.Tests;

public sealed class AppInstallableModuleTests : TestsBase<InstallableModuleMock>
{
    #region Tests
    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestInstallModule()
    {
        this.app.Start();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        module.IsConfigured.Should().BeTrue("Configure should be called after starting the application");
        module.IsInstalled.Should().BeTrue("Install should be called after starting the application");
        module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
        module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
        module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
        this.app.Uninitialize();
        module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
        module.IsUninstalled.Should().BeFalse("Uninstall should not be called after uninitializing the application");
        this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName())
            .Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUpgradeModule()
    {
        this.app.Start();
        var moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Version = new Version(0, 0, 0, 0);
        ((IApp)this.app).Restart();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
        module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
        module.IsInstalled.Should().BeTrue("Install should not be called after starting the application");
        module.IsConfigured.Should().BeTrue("Configure should be called after starting the application");
        module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
        this.app.Uninitialize();
        module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
        module.IsUninstalled.Should().BeFalse("Uninstall should not be called after uninitializing the application");
        moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestInitializeInstallableModule()
    {
        this.app.Start();
        this.app.Restart();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
        module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
        module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
        module.IsConfigured.Should().BeFalse("Configure should be not called after starting the application");
        module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
        this.app.Uninitialize();
        module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
        module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
        module.IsConfigured.Should().BeFalse("Configure should not be called after uninitializing the application");
        module.IsUninstalled.Should().BeFalse("Uninstall should not be called after uninitializing the application");
        this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName())
            .Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUninstallEnabledModule()
    {
        this.app.Start();
        var moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Version = new Version(0, 0, 0, 0);
        this.app.Restart();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
        module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
        module.IsInstalled.Should().BeTrue("Install should be called after starting the application");
        module.IsConfigured.Should().BeTrue("Configure should be called after starting the application");
        module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
        moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status = ModuleStatus.Uninstalling;
        this.app.Uninitialize();
        module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
        module.IsUninstalled.Should().BeTrue("Uninstall should be called after uninitializing the application");
        moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status.Should().Be(ModuleStatus.Disabled, "module status be set to disabled after uninstall");
        moduleConfig.Version.Should().BeNull("module version should be cleared after uninstall");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUninstallDisabledModule()
    {
        this.app.Start();
        var moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Version = new Version(0, 0, 0, 0);
        moduleConfig.Status = ModuleStatus.Disabled;
        this.app.Restart();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
        module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
        module.IsConfigured.Should().BeFalse("Configure should not be called after starting the application");
        module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
        moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status = ModuleStatus.Uninstalling;
        this.app.Uninitialize();
        module.IsInitialized.Should().BeFalse("Initialize should not be called after starting the application");
        module.IsUninitialized.Should().BeFalse("Uninitialize should not be called after uninitializing the application");
        module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
        module.IsUninstalled.Should().BeTrue("Uninstall should be called after uninitializing the application");
        moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status.Should().Be(ModuleStatus.Disabled, "module status be set to disabled after uninstall");
        moduleConfig.Version.Should().BeNull("module version should be cleared after uninstall");
    }

    [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
    public void TestUninstallUninstallingModule()
    {
        this.app.Start();
        this.app.Stop();
        var moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status = ModuleStatus.Uninstalling;
        moduleConfig.Version = typeof(InstallableModuleMock).Assembly.GetName().Version;
        this.app.Start();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        module.IsInitialized.Should().BeFalse("Initialize should not be called after starting the application");
        module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
        module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
        module.IsConfigured.Should().BeFalse("Configure should not be called after starting the application");
        module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
        this.app.Uninitialize();
        module.IsUninitialized.Should().BeFalse("Uninitialize should not be called after uninitializing the application");
        module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
        module.IsUninstalled.Should().BeTrue("Uninstall should be called after uninitializing the application");
        moduleConfig = this.app.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status.Should().Be(ModuleStatus.Disabled, "module status be set to disabled after uninstall");
        moduleConfig.Version.Should().BeNull("module version should be cleared after uninstall");
    }
    #endregion

    #region Private methods
    private IEnumerable<ModuleInfo> GetModules() => (IEnumerable<ModuleInfo>)this.modulesField.GetValue(this.app)!;
    #endregion

    #region Private fields and constants
    private readonly FieldInfo modulesField = typeof(DefaultApp).GetField("modules", BindingFlags.NonPublic | BindingFlags.Instance)!;
    #endregion
}
