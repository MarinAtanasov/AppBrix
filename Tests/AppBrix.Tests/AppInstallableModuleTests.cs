// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Testing;
using AppBrix.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Tests;

[TestClass]
public sealed class AppInstallableModuleTests : TestsBase<InstallableModuleMock>
{
    #region Test lifecycle
    protected override void Initialize() { }
    #endregion

    #region Tests
    [Test, Functional]
    public void TestInstallModule()
    {
        this.App.Start();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        this.Assert(module.IsConfigured, "Configure should be called after starting the application");
        this.Assert(module.IsInstalled, "Install should be called after starting the application");
        this.Assert(module.IsInitialized, "Initialize should be called after starting the application");
        this.Assert(module.IsUninitialized == false, "Uninitialize should not be called before uninitializing the application");
        this.Assert(module.IsUninstalled == false, "Uninstall should not be called after starting the application");
        this.App.Uninitialize();
        this.Assert(module.IsUninitialized, "Uninitialize should be called after uninitializing the application");
        this.Assert(module.IsUninstalled == false, "Uninstall should not be called after uninitializing the application");
        var moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        this.Assert(moduleConfig.Status == ModuleStatus.Enabled, "module status should not be changed");
    }

    [Test, Functional]
    public void TestUpgradeModule()
    {
        this.App.Start();
        var moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Version = new Version(0, 0, 0, 0);
        ((IApp)this.App).Restart();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        this.Assert(module.IsInitialized, "Initialize should be called after starting the application");
        this.Assert(module.IsUninitialized == false, "Uninitialize should not be called before uninitializing the application");
        this.Assert(module.IsInstalled, "Install should not be called after starting the application");
        this.Assert(module.IsConfigured, "Configure should be called after starting the application");
        this.Assert(module.IsUninstalled == false, "Uninstall should not be called after starting the application");
        this.App.Uninitialize();
        this.Assert(module.IsUninitialized, "Uninitialize should be called after uninitializing the application");
        this.Assert(module.IsUninstalled == false, "Uninstall should not be called after uninitializing the application");
        moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        this.Assert(moduleConfig.Status == ModuleStatus.Enabled, "module status should not be changed");
    }

    [Test, Functional]
    public void TestInitializeInstallableModule()
    {
        this.App.Start();
        this.App.Restart();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        this.Assert(module.IsInitialized, "Initialize should be called after starting the application");
        this.Assert(module.IsUninitialized == false, "Uninitialize should not be called before uninitializing the application");
        this.Assert(module.IsInstalled == false, "Install should not be called after starting the application");
        this.Assert(module.IsConfigured == false, "Configure should be not called after starting the application");
        this.Assert(module.IsUninstalled == false, "Uninstall should not be called after starting the application");
        this.App.Uninitialize();
        this.Assert(module.IsUninitialized, "Uninitialize should be called after uninitializing the application");
        this.Assert(module.IsInstalled == false, "Install should not be called after uninitializing the application");
        this.Assert(module.IsConfigured == false, "Configure should not be called after uninitializing the application");
        this.Assert(module.IsUninstalled == false, "Uninstall should not be called after uninitializing the application");
        var moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        this.Assert(moduleConfig.Status == ModuleStatus.Enabled, "module status should not be changed");
    }

    [Test, Functional]
    public void TestUninstallEnabledModule()
    {
        this.App.Start();
        var moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Version = new Version(0, 0, 0, 0);
        this.App.Restart();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        this.Assert(module.IsInitialized, "Initialize should be called after starting the application");
        this.Assert(module.IsUninitialized == false, "Uninitialize should not be called before uninitializing the application");
        this.Assert(module.IsInstalled, "Install should be called after starting the application");
        this.Assert(module.IsConfigured, "Configure should be called after starting the application");
        this.Assert(module.IsUninstalled == false, "Uninstall should not be called after starting the application");
        moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status = ModuleStatus.Uninstalling;
        this.App.Uninitialize();
        this.Assert(module.IsUninitialized, "Uninitialize should be called after uninitializing the application");
        this.Assert(module.IsUninstalled, "Uninstall should be called after uninitializing the application");
        moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        this.Assert(moduleConfig.Status == ModuleStatus.Disabled, "module status be set to disabled after uninstall");
        this.Assert(moduleConfig.Version is null, "module version should be cleared after uninstall");
    }

    [Test, Functional]
    public void TestUninstallDisabledModule()
    {
        this.App.Start();
        var moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Version = new Version(0, 0, 0, 0);
        moduleConfig.Status = ModuleStatus.Disabled;
        this.App.Restart();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        this.Assert(module.IsUninitialized == false, "Uninitialize should not be called before uninitializing the application");
        this.Assert(module.IsInstalled == false, "Install should not be called after starting the application");
        this.Assert(module.IsConfigured == false, "Configure should not be called after starting the application");
        this.Assert(module.IsUninstalled == false, "Uninstall should not be called after starting the application");
        moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status = ModuleStatus.Uninstalling;
        this.App.Uninitialize();
        this.Assert(module.IsInitialized == false, "Initialize should not be called after starting the application");
        this.Assert(module.IsUninitialized == false, "Uninitialize should not be called after uninitializing the application");
        this.Assert(module.IsInstalled == false, "Install should not be called after uninitializing the application");
        this.Assert(module.IsUninstalled, "Uninstall should be called after uninitializing the application");
        moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        this.Assert(moduleConfig.Status == ModuleStatus.Disabled, "module status be set to disabled after uninstall");
        this.Assert(moduleConfig.Version is null, "module version should be cleared after uninstall");
    }

    [Test, Functional]
    public void TestUninstallUninstallingModule()
    {
        this.App.Start();
        this.App.Stop();
        var moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        moduleConfig.Status = ModuleStatus.Uninstalling;
        moduleConfig.Version = typeof(InstallableModuleMock).Assembly.GetName().Version;
        this.App.Start();
        var module = this.GetModules().Select(x => x.Module).OfType<InstallableModuleMock>().Single();
        this.Assert(module.IsInitialized == false, "Initialize should not be called after starting the application");
        this.Assert(module.IsUninitialized == false, "Uninitialize should not be called before uninitializing the application");
        this.Assert(module.IsInstalled == false, "Install should not be called after starting the application");
        this.Assert(module.IsConfigured == false, "Configure should not be called after starting the application");
        this.Assert(module.IsUninstalled == false, "Uninstall should not be called after starting the application");
        this.App.Uninitialize();
        this.Assert(module.IsUninitialized == false, "Uninitialize should not be called after uninitializing the application");
        this.Assert(module.IsInstalled == false, "Install should not be called after uninitializing the application");
        this.Assert(module.IsUninstalled, "Uninstall should be called after uninitializing the application");
        moduleConfig = this.App.ConfigService.GetAppConfig().Modules
            .Single(x => x.Type == typeof(InstallableModuleMock).GetAssemblyQualifiedName());
        this.Assert(moduleConfig.Status == ModuleStatus.Disabled, "module status be set to disabled after uninstall");
        this.Assert(moduleConfig.Version is null, "module version should be cleared after uninstall");
    }
    #endregion

    #region Private methods
    private IEnumerable<ModuleInfo> GetModules() => (IEnumerable<ModuleInfo>)this.modulesField.GetValue(this.App)!;
    #endregion

    #region Private fields and constants
    private readonly FieldInfo modulesField = typeof(DefaultApp).GetField("modules", BindingFlags.NonPublic | BindingFlags.Instance)!;
    #endregion
}
