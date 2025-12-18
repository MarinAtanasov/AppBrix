// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration;
using AppBrix.Testing;
using AppBrix.Tests.Mocks;
using System;
using System.Linq;

namespace AppBrix.Tests;

[TestClass]
public sealed class AppTests : TestsBase
{
	#region Tests
	[Test, Functional]
	public void TestCreateAppNullConfigService()
	{
		var action = () => AppBrix.App.Create(null!);
		this.AssertThrows<ArgumentNullException>(action, "config service cannot be null");;
	}

	[Test, Functional]
	public void TestAppConfigCreateNullType()
	{
		var action = () => ModuleConfigElement.Create(null!);
		this.AssertThrows<ArgumentNullException>(action, "type cannot be null");;
	}

	[Test, Functional]
	public void TestAppConfigCreateInvalidType()
	{
		var action = () => ModuleConfigElement.Create(typeof(object));
		this.AssertThrows<ArgumentException>(action, "type must implement IModule");;
	}

	[Test, Functional]
	public void TestGetAllDependenciesExtensionNullModule()
	{
		var action = () => AppBrixExtensions.GetAllDependencies(null!);
		this.AssertThrows<ArgumentNullException>(action, "module cannot be null");;
	}

	[Test, Performance]
	public void TestPerformanceGetDependencies() => this.AssertPerformance(this.TestPerformanceGetDependenciesInternal);
	#endregion

	#region Private methods
	private void TestPerformanceGetDependenciesInternal()
	{
		var module = new EmptyModuleMock();
		for (var i = 0; i < 100; i++)
		{
			_ = module.Dependencies.Count();
		}
	}
	#endregion
}
