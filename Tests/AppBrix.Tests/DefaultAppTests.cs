// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Tests.Mocks;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace AppBrix.Tests
{
    public sealed class DefaultAppTests
    {
        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInitializeModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleModuleMock));
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleModuleMock>().Single();
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            app.GetConfig<AppConfig>().Modules.Single().Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInitializeDisabledModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleModuleMock));
            app.GetConfig<AppConfig>().Modules.Single().Status = ModuleStatus.Disabled;
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleModuleMock>().SingleOrDefault();
            module.Should().BeNull("disabled modules should not be loaded in memory");
            app.Uninitialize();
            app.GetConfig<AppConfig>().Modules.Single().Status.Should().Be(ModuleStatus.Disabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInstallModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeTrue("Install should be called after starting the application");
            module.IsUpgraded.Should().BeFalse("Upgrade should not be called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            module.IsUpgraded.Should().BeFalse("Upgrade should not be called after uninitializing the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after uninitializing the application");
            app.GetConfig<AppConfig>().Modules.Single().Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUpgradeModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.GetConfig<AppConfig>().Modules.Single().Version = new Version(0, 0, 0, 0);
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
            module.IsUpgraded.Should().BeTrue("Upgrade should be called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after uninitializing the application");
            app.GetConfig<AppConfig>().Modules.Single().Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInitializeInstallableModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.GetConfig<AppConfig>().Modules.Single().Version = typeof(SimpleInstallableModuleMock).Assembly.GetName().Version;
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
            module.IsUpgraded.Should().BeFalse("Upgrade should be not called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
            module.IsUpgraded.Should().BeFalse("Upgrade should not be called after uninitializing the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after uninitializing the application");
            app.GetConfig<AppConfig>().Modules.Single().Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUninstallEnabledModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.GetConfig<AppConfig>().Modules.Single().Version = new Version(0, 0, 0, 0);
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
            module.IsUpgraded.Should().BeTrue("Upgrade should be called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.GetConfig<AppConfig>().Modules.Single().Status = ModuleStatus.Uninstalling;
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
            module.IsUninstalled.Should().BeTrue("Uninstall should be called after uninitializing the application");
            app.GetConfig<AppConfig>().Modules.Single().Status.Should().Be(ModuleStatus.Disabled, "module status be set to disabled after uninstall");
            app.GetConfig<AppConfig>().Modules.Single().Version.Should().BeNull("module version should be cleared after uninstall");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUninstallDisabledModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.GetConfig<AppConfig>().Modules.Single().Version = new Version(0, 0, 0, 0);
            app.GetConfig<AppConfig>().Modules.Single().Status = ModuleStatus.Disabled;
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
            module.IsUpgraded.Should().BeFalse("Upgrade should not be called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.GetConfig<AppConfig>().Modules.Single().Status = ModuleStatus.Uninstalling;
            app.Uninitialize();
            module.IsInitialized.Should().BeFalse("Initialize should not be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called after uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
            module.IsUninstalled.Should().BeTrue("Uninstall should be called after uninitializing the application");
            app.GetConfig<AppConfig>().Modules.Single().Status.Should().Be(ModuleStatus.Disabled, "module status be set to disabled after uninstall");
            app.GetConfig<AppConfig>().Modules.Single().Version.Should().BeNull("module version should be cleared after uninstall");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUninstallUninstallingModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.GetConfig<AppConfig>().Modules.Single().Status = ModuleStatus.Uninstalling;
            app.GetConfig<AppConfig>().Modules.Single().Version = typeof(SimpleInstallableModuleMock).Assembly.GetName().Version;
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            module.IsInitialized.Should().BeFalse("Initialize should not be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
            module.IsUpgraded.Should().BeFalse("Upgrade should not be called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called after uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
            module.IsUninstalled.Should().BeTrue("Uninstall should be called after uninitializing the application");
            app.GetConfig<AppConfig>().Modules.Single().Status.Should().Be(ModuleStatus.Disabled, "module status be set to disabled after uninstall");
            app.GetConfig<AppConfig>().Modules.Single().Version.Should().BeNull("module version should be cleared after uninstall");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceReinitialize()
        {
            var app = this.CreateDefaultApp(typeof(SimpleModuleMock));
            app.Start();

            TestUtils.TestPerformance(() => this.TestPerformanceReinitializeInternal(app));
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceRestart()
        {
            var app = this.CreateDefaultApp(typeof(SimpleModuleMock));
            app.Start();
            
            TestUtils.TestPerformance(() => this.TestPerformanceRestartInternal(app));
        }
        #endregion

        #region Private methods
        private DefaultApp CreateDefaultApp(Type module)
        {
            return (DefaultApp)TestUtils.CreateTestApp(module);
        }

        private IEnumerable<ModuleInfo> GetModules(DefaultApp app)
        {
            return (IEnumerable<ModuleInfo>)this.modulesField.GetValue(app);
        }

        private void TestPerformanceReinitializeInternal(IApp app)
        {
            for (int i = 0; i < 15000; i++)
            {
                app.Reinitialize();
            }
        }

        private void TestPerformanceRestartInternal(IApp app)
        {
            for (int i = 0; i < 2000; i++)
            {
                app.Restart();
            }
        }
        #endregion

        #region Private fields and constants
        private readonly FieldInfo modulesField = typeof(DefaultApp).GetField("modules", BindingFlags.NonPublic | BindingFlags.Instance);
        #endregion
    }
}
