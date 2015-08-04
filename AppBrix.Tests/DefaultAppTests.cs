// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Tests
{
    [TestClass]
    public class DefaultAppTests
    {
        #region Tests
        [TestMethod]
        public void TestInitializeModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleModuleMock));
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleModuleMock>().Single();
            Assert.IsTrue(module.IsInitialized, "Initialize should be called after starting the application.");
            Assert.IsFalse(module.IsUninitialized, "Uninitialize should not be called before uninitializing the application.");
            app.Uninitialize();
            Assert.IsTrue(module.IsUninitialized, "Uninitialize should be called after uninitializing the application.");
            Assert.AreEqual(ModuleStatus.Enabled, app.AppConfig.Modules.Single().Status, "Module status should not be chaged.");
        }

        [TestMethod]
        public void TestInitializeDisabledModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleModuleMock));
            app.AppConfig.Modules.Single().Status = ModuleStatus.Disabled;
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleModuleMock>().SingleOrDefault();
            Assert.IsNull(module, "Default modules should not be loaded in memory.");
            app.Uninitialize();
            Assert.AreEqual(ModuleStatus.Disabled, app.AppConfig.Modules.Single().Status, "Module status should not be chaged.");
        }

        [TestMethod]
        public void TestInstallModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            Assert.IsTrue(module.IsInitialized, "Initialize should be called after starting the application.");
            Assert.IsFalse(module.IsUninitialized, "Uninitialize should not be called before uninitializing the application.");
            Assert.IsTrue(module.IsInstalled, "Install should be called after starting the application.");
            Assert.IsFalse(module.IsUpgraded, "Upgrade should not be called after starting the application.");
            Assert.IsFalse(module.IsUninstalled, "Uninstall should not be called after starting the application.");
            app.Uninitialize();
            Assert.IsTrue(module.IsUninitialized, "Uninitialize should be called after uninitializing the application.");
            Assert.IsFalse(module.IsUpgraded, "Upgrade should not be called after uninitializing the application.");
            Assert.IsFalse(module.IsUninstalled, "Uninstall should not be called after uninitializing the application.");
            Assert.AreEqual(ModuleStatus.Enabled, app.AppConfig.Modules.Single().Status, "Module status should not be chaged.");
        }

        [TestMethod]
        public void TestUpgradeModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.AppConfig.Modules.Single().Version = new Version(0, 0, 0, 0);
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            Assert.IsTrue(module.IsInitialized, "Initialize should be called after starting the application.");
            Assert.IsFalse(module.IsUninitialized, "Uninitialize should not be called before uninitializing the application.");
            Assert.IsFalse(module.IsInstalled, "Install should not be called after starting the application.");
            Assert.IsTrue(module.IsUpgraded, "Upgrade should be called after starting the application.");
            Assert.IsFalse(module.IsUninstalled, "Uninstall should not be called after starting the application.");
            app.Uninitialize();
            Assert.IsTrue(module.IsUninitialized, "Uninitialize should be called after uninitializing the application.");
            Assert.IsFalse(module.IsInstalled, "Install should not be called after uninitializing the application.");
            Assert.IsFalse(module.IsUninstalled, "Uninstall should not be called after uninitializing the application.");
            Assert.AreEqual(ModuleStatus.Enabled, app.AppConfig.Modules.Single().Status, "Module status should not be chaged.");
        }

        [TestMethod]
        public void TestInitializeInstallableModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.AppConfig.Modules.Single().Version = typeof(SimpleInstallableModuleMock).Assembly.GetName().Version;
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            Assert.IsTrue(module.IsInitialized, "Initialize should be called after starting the application.");
            Assert.IsFalse(module.IsUninitialized, "Uninitialize should not be called before uninitializing the application.");
            Assert.IsFalse(module.IsInstalled, "Install should not be called after starting the application.");
            Assert.IsFalse(module.IsUpgraded, "Upgrade should be not called after starting the application.");
            Assert.IsFalse(module.IsUninstalled, "Uninstall should not be called after starting the application.");
            app.Uninitialize();
            Assert.IsTrue(module.IsUninitialized, "Uninitialize should be called after uninitializing the application.");
            Assert.IsFalse(module.IsInstalled, "Install should not be called after uninitializing the application.");
            Assert.IsFalse(module.IsUpgraded, "Upgrade should not be called after uninitializing the application.");
            Assert.IsFalse(module.IsUninstalled, "Uninstall should not be called after uninitializing the application.");
            Assert.AreEqual(ModuleStatus.Enabled, app.AppConfig.Modules.Single().Status, "Module status should not be chaged.");
        }

        [TestMethod]
        public void TestUninstallEnabledModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.AppConfig.Modules.Single().Version = new Version(0, 0, 0, 0);
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            Assert.IsTrue(module.IsInitialized, "Initialize should be called after starting the application.");
            Assert.IsFalse(module.IsUninitialized, "Uninitialize should not be called before uninitializing the application.");
            Assert.IsFalse(module.IsInstalled, "Install should not be called after starting the application.");
            Assert.IsTrue(module.IsUpgraded, "Upgrade should be called after starting the application.");
            Assert.IsFalse(module.IsUninstalled, "Uninstall should not be called after starting the application.");
            app.AppConfig.Modules.Single().Status = ModuleStatus.Uninstalling;
            app.Uninitialize();
            Assert.IsTrue(module.IsUninitialized, "Uninitialize should be called after uninitializing the application.");
            Assert.IsFalse(module.IsInstalled, "Install should not be called after uninitializing the application.");
            Assert.IsTrue(module.IsUninstalled, "Uninstall should be called after uninitializing the application.");
            Assert.AreEqual(ModuleStatus.Disabled, app.AppConfig.Modules.Single().Status, "Module status be set to disabled.");
            Assert.IsNull(app.AppConfig.Modules.Single().Version, "Module version should be cleared after uninstall.");
        }

        [TestMethod]
        public void TestUninstallDisabledModule()
        {
            var app = this.CreateDefaultApp(typeof(SimpleInstallableModuleMock));
            app.AppConfig.Modules.Single().Status = ModuleStatus.Uninstalling;
            app.AppConfig.Modules.Single().Version = typeof(SimpleInstallableModuleMock).Assembly.GetName().Version;
            app.Start();
            var module = this.GetModules(app).Select(x => x.Module).Cast<SimpleInstallableModuleMock>().Single();
            Assert.IsFalse(module.IsInitialized, "Initialize should not be called after starting the application.");
            Assert.IsFalse(module.IsUninitialized, "Uninitialize should not be called before uninitializing the application.");
            Assert.IsFalse(module.IsInstalled, "Install should not be called after starting the application.");
            Assert.IsFalse(module.IsUpgraded, "Upgrade should not be called after starting the application.");
            Assert.IsFalse(module.IsUninstalled, "Uninstall should not be called after starting the application.");
            app.Uninitialize();
            Assert.IsFalse(module.IsUninitialized, "Uninitialize should not be called after uninitializing the application.");
            Assert.IsFalse(module.IsInstalled, "Install should not be called after uninitializing the application.");
            Assert.IsTrue(module.IsUninstalled, "Uninstall should be called after uninitializing the application.");
            Assert.AreEqual(ModuleStatus.Disabled, app.AppConfig.Modules.Single().Status, "Module status be set to disabled.");
            Assert.IsNull(app.AppConfig.Modules.Single().Version, "Module version should be cleared after uninstall.");
        }
        #endregion

        #region Private methods
        private DefaultApp CreateDefaultApp(params Type[] modules)
        {
            return (DefaultApp)TestUtils.CreateTestApp(modules);
        }

        private IEnumerable<ModuleInfo> GetModules(DefaultApp app)
        {
            return (IEnumerable<ModuleInfo>)this.modulesField.GetValue(app);
        }
        #endregion

        #region Private fields and constants
        private readonly FieldInfo modulesField = typeof(DefaultApp).GetField("modules", BindingFlags.NonPublic | BindingFlags.Instance);
        #endregion
    }
}
