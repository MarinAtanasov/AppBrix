// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Testing;
using AppBrix.Testing.Modules;
using AppBrix.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Tests;

[TestClass]
public sealed class AppSimpleModuleTests : TestsBase<SimpleModuleMock>
{
	#region Test lifecycle
	protected override void Initialize() { }
	#endregion

	#region Tests
	[Test, Functional]
	public void TestSaveConfig()
	{
		var configService = this.App.ConfigService;
		var oldConfig = configService.Get<AppConfig>();
		oldConfig.Modules.Clear();
		var newConfig = new AppConfig();
		newConfig.Modules.Add(ModuleConfigElement.Create<SimpleModuleMock>());
		configService.Save(newConfig);
		this.Assert(configService.GetAppConfig() == newConfig,"Should return new config after saving it");
		this.Assert(oldConfig.Modules.Count == 0,"Original config should not be modified.");
	}

	[Test, Functional]
	public void TestStartStartedApp()
	{
		this.App.Start();
		var action = () => this.App.Start();
		this.AssertThrows<InvalidOperationException>(action, "cannot start already started app");;
	}

	[Test, Functional]
	public void TestStopStoppedApp()
	{
		var action = () => this.App.Stop();
		this.AssertThrows<InvalidOperationException>(action, "cannot stop already stopped app");;
	}

	[Test, Functional]
	public void TestInitializeInitializedApp()
	{
		this.App.Start();
		// Initializing already initialized app should be safe
		this.App.Initialize();
	}

	[Test, Functional]
	public void TestInitializeStoppedApp()
	{
		var action = () => this.App.Initialize();
		this.AssertThrows<InvalidOperationException>(action, "cannot initialize stopped app");;
	}

	[Test, Functional]
	public void TestUninitializeUninitializedApp()
	{
		this.App.Start();
		this.App.Uninitialize();
		// Uninitializing already uninitialized app should be safe
		this.App.Uninitialize();
	}

	[Test, Functional]
	public void TestUninitializeStoppedApp()
	{
		var action = () => this.App.Uninitialize();
		this.AssertThrows<InvalidOperationException>(action, "cannot uninitialize stopped app");;
	}

	[Test, Functional]
	public void TestInitializeModule()
	{
		this.App.Start();
		var module = this.GetModules().Select(x => x.Module).OfType<SimpleModuleMock>().Single();
		this.Assert(module.IsInitialized, "Initialize should be called after starting the application");
		this.Assert(module.IsUninitialized == false, "Uninitialize should not be called before uninitializing the application");
		this.App.Uninitialize();
		this.Assert(module.IsUninitialized, "Uninitialize should be called after uninitializing the application");
		var moduleConfig = this.App.ConfigService.GetAppConfig().Modules
			.Single(x => x.Type == typeof(SimpleModuleMock).GetAssemblyQualifiedName());
		this.Assert(moduleConfig.Status == ModuleStatus.Enabled, "module status should not be changed");
	}

	[Test, Functional]
	public void TestInitializeDisabledModule()
	{
		this.App.ConfigService.GetAppConfig().Modules.Single().Status = ModuleStatus.Disabled;
		this.App.Start();
		var module = this.GetModules().Select(x => x.Module).Cast<SimpleModuleMock>().SingleOrDefault();
		this.Assert(this.GetModules().Select(x => x.Module).Any() == false, "disabled modules should not be loaded in memory");
		this.App.Uninitialize();
		this.Assert(this.App.ConfigService.GetAppConfig().Modules.Single().Status == ModuleStatus.Disabled, "module status should not be changed");
	}

	[Test, Functional]
	public void TestInstallMainModule()
	{
		var configService = this.App.ConfigService;
		this.Assert(configService.GetAppConfig().Modules.Count == 1, $"{nameof(AppBrix.App)}.{nameof(AppBrix.App.Create)} should add the main module");
		this.App.Start();
		this.Assert(this.App.ConfigService.GetAppConfig().Modules[0].Type == typeof(SimpleModuleMock).GetAssemblyQualifiedName(), "The dependency should be placed before the main module");
		this.Assert(configService.GetAppConfig().Modules.Count == 2, "Main module should add its dependencies");
		this.App.Stop();
		this.Assert(configService.GetAppConfig().Modules.Count == 2, "Stopping the application shouldn't change the config");

		var newApp = AppBrix.App.Create<MainModule<SimpleModuleMock>>(configService);
		this.Assert(configService.GetAppConfig().Modules.Count == 2, "Creating a new app shouldn't change the valid config");
		newApp.Start();
		this.Assert(configService.GetAppConfig().Modules.Count == 2, "Starting the new app shouldn't change the valid config");
		newApp.Stop();
		this.Assert(configService.GetAppConfig().Modules.Count == 2, "Stopping the new app shouldn't change the config");
	}

	[Test, Performance]
	public void TestPerformanceReinitialize()
	{
		this.App.Start();

		this.AssertPerformance(this.TestPerformanceReinitializeInternal);
	}

	[Test, Performance]
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
