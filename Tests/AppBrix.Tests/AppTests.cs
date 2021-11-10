// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Configuration.Memory;
using AppBrix.Modules;
using AppBrix.Tests.Mocks;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace AppBrix.Tests
{
    public sealed class AppTests
    {
        #region Tests
        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestSaveConfig()
        {
            var app = this.CreateDefaultApp<SimpleModuleMock>();
            var configService = app.ConfigService;
            var oldConfig = configService.GetAppConfig();
            oldConfig.Modules.Clear();
            var newConfig = new AppConfig();
            newConfig.Modules.Add(ModuleConfigElement.Create<SimpleModuleMock>());
            configService.Save(newConfig);
            var config = configService.GetAppConfig();
            config.Should().Be(newConfig, "Should return new config after saving it");
            oldConfig.Modules.Should().BeEmpty("Original config should not be modified.");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInitializeModule()
        {
            var app = this.CreateDefaultApp<SimpleModuleMock>();
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).OfType<SimpleModuleMock>().Single();
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleModuleMock).GetAssemblyQualifiedName())
                .Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInitializeDisabledModule()
        {
            var app = this.CreateDefaultApp<SimpleModuleMock>();
            app.ConfigService.GetAppConfig().Modules.Single().Status = ModuleStatus.Disabled;
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleModuleMock>().SingleOrDefault();
            module.Should().BeNull("disabled modules should not be loaded in memory");
            app.Uninitialize();
            app.ConfigService.GetAppConfig().Modules.Single().Status.Should().Be(ModuleStatus.Disabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInstallMainModule()
        {
            var configService = new MemoryConfigService();
            var app = App.Create<MainModuleMock<SimpleModuleMock>>(configService);
            configService.GetAppConfig().Modules.Should().HaveCount(1, $"{nameof(App)}.{nameof(App.Create)} should add the main module");
            app.Start();
            var dependency = app.ConfigService.GetAppConfig().Modules[0];
            dependency.Type.Should().Be(typeof(SimpleModuleMock).GetAssemblyQualifiedName(), "The dependency should be placed before the main module");
            configService.GetAppConfig().Modules.Should().HaveCount(2, $"Main module should add its dependencies");
            app.Stop();
            configService.GetAppConfig().Modules.Should().HaveCount(2, $"Stopping the application shouldn't change the config");

            app = App.Create<MainModuleMock<SimpleModuleMock>>(configService);
            configService.GetAppConfig().Modules.Should().HaveCount(2, $"Creating a new app shouldn't change the valid config");
            app.Start();
            configService.GetAppConfig().Modules.Should().HaveCount(2, $"Starting the new app shouldn't change the valid config");
            app.Stop();
            configService.GetAppConfig().Modules.Should().HaveCount(2, $"Stopping the new app shouldn't change the config");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInstallModule()
        {
            var app = this.CreateDefaultApp<SimpleInstallableModuleMock>();
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).OfType<SimpleInstallableModuleMock>().Single();
            module.IsConfigured.Should().BeTrue("Configure should be called after starting the application");
            module.IsInstalled.Should().BeTrue("Install should be called after starting the application");
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after uninitializing the application");
            app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName())
                .Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUpgradeModule()
        {
            var app = this.CreateDefaultApp<SimpleInstallableModuleMock>();
            app.Start();
            var moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Version = new Version(0, 0, 0, 0);
            ((IApp)app).Restart();
            var module = this.GetModules(app).Select(x => x.Module).OfType<SimpleInstallableModuleMock>().Single();
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeTrue("Install should not be called after starting the application");
            module.IsConfigured.Should().BeTrue("Configure should be called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after uninitializing the application");
            moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestInitializeInstallableModule()
        {
            var app = this.CreateDefaultApp<SimpleInstallableModuleMock>();
            app.Start();
            ((IApp)app).Restart();
            var module = this.GetModules(app).Select(x => x.Module).OfType<SimpleInstallableModuleMock>().Single();
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
            module.IsConfigured.Should().BeFalse("Configure should be not called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
            module.IsConfigured.Should().BeFalse("Configure should not be called after uninitializing the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after uninitializing the application");
            app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName())
                .Status.Should().Be(ModuleStatus.Enabled, "module status should not be changed");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUninstallEnabledModule()
        {
            var app = this.CreateDefaultApp<SimpleInstallableModuleMock>();
            app.Start();
            var moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Version = new Version(0, 0, 0, 0);
            ((IApp)app).Restart();
            var module = this.GetModules(app).Select(x => x.Module).OfType<SimpleInstallableModuleMock>().Single();
            module.IsInitialized.Should().BeTrue("Initialize should be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeTrue("Install should be called after starting the application");
            module.IsConfigured.Should().BeTrue("Configure should be called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Status = ModuleStatus.Uninstalling;
            app.Uninitialize();
            module.IsUninitialized.Should().BeTrue("Uninitialize should be called after uninitializing the application");
            module.IsUninstalled.Should().BeTrue("Uninstall should be called after uninitializing the application");
            moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Status.Should().Be(ModuleStatus.Disabled, "module status be set to disabled after uninstall");
            moduleConfig.Version.Should().BeNull("module version should be cleared after uninstall");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUninstallDisabledModule()
        {
            var app = this.CreateDefaultApp<SimpleInstallableModuleMock>();
            app.Start();
            var moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Version = new Version(0, 0, 0, 0);
            moduleConfig.Status = ModuleStatus.Disabled;
            ((IApp)app).Restart();
            var module = this.GetModules(app).Select(x => x.Module).OfType<SimpleInstallableModuleMock>().Single();
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
            module.IsConfigured.Should().BeFalse("Configure should not be called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Status = ModuleStatus.Uninstalling;
            app.Uninitialize();
            module.IsInitialized.Should().BeFalse("Initialize should not be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called after uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
            module.IsUninstalled.Should().BeTrue("Uninstall should be called after uninitializing the application");
            moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Status.Should().Be(ModuleStatus.Disabled, "module status be set to disabled after uninstall");
            moduleConfig.Version.Should().BeNull("module version should be cleared after uninstall");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Functional)]
        public void TestUninstallUninstallingModule()
        {
            var app = this.CreateDefaultApp<SimpleInstallableModuleMock>();
            app.Start();
            app.Stop();
            var moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Status = ModuleStatus.Uninstalling;
            moduleConfig.Version = typeof(SimpleInstallableModuleMock).Assembly.GetName().Version;
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).OfType<SimpleInstallableModuleMock>().Single();
            module.IsInitialized.Should().BeFalse("Initialize should not be called after starting the application");
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called before uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after starting the application");
            module.IsConfigured.Should().BeFalse("Configure should not be called after starting the application");
            module.IsUninstalled.Should().BeFalse("Uninstall should not be called after starting the application");
            app.Uninitialize();
            module.IsUninitialized.Should().BeFalse("Uninitialize should not be called after uninitializing the application");
            module.IsInstalled.Should().BeFalse("Install should not be called after uninitializing the application");
            module.IsUninstalled.Should().BeTrue("Uninstall should be called after uninitializing the application");
            moduleConfig = app.ConfigService.GetAppConfig().Modules
                .Single(x => x.Type == typeof(SimpleInstallableModuleMock).GetAssemblyQualifiedName());
            moduleConfig.Status.Should().Be(ModuleStatus.Disabled, "module status be set to disabled after uninstall");
            moduleConfig.Version.Should().BeNull("module version should be cleared after uninstall");
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceReinitialize()
        {
            var app = this.CreateDefaultApp<SimpleModuleMock>();
            app.Start();

            TestUtils.TestPerformance(() => this.TestPerformanceReinitializeInternal(app));
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceRestart()
        {
            var app = this.CreateDefaultApp<SimpleModuleMock>();
            app.Start();

            TestUtils.TestPerformance(() => this.TestPerformanceRestartInternal(app));
        }

        [Fact, Trait(TestCategories.Category, TestCategories.Performance)]
        public void TestPerformanceGetDependencies() => TestUtils.TestPerformance(this.TestPerformanceGetDependenciesInternal);
        #endregion

        #region Private methods
        private DefaultApp CreateDefaultApp<T>() where T : IModule => (DefaultApp)TestUtils.CreateTestApp<T>();

        private IEnumerable<ModuleInfo> GetModules(DefaultApp app) => (IEnumerable<ModuleInfo>)this.modulesField.GetValue(app)!;

        private void TestPerformanceReinitializeInternal(IApp app)
        {
            for (var i = 0; i < 4000; i++)
            {
                app.Reinitialize();
            }
        }

        private void TestPerformanceRestartInternal(IApp app)
        {
            for (var i = 0; i < 750; i++)
            {
                app.Restart();
            }
        }

        private void TestPerformanceGetDependenciesInternal()
        {
            var module = new SimpleEmptyModuleMock();
            for (var i = 0; i < 70; i++)
            {
                var _ = module.Dependencies.ToList();
            }
        }
        #endregion

        #region Private fields and constants
        private readonly FieldInfo modulesField = typeof(DefaultApp).GetField("modules", BindingFlags.NonPublic | BindingFlags.Instance)!;
        #endregion
    }
}
