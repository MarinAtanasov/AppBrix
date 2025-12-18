// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Memory;
using AppBrix.Modules;
using System;
using Xunit.Sdk;

namespace AppBrix.Testing;

/// <summary>
/// A base testing class that uses Xunit.
/// Used for testing an application without preloaded modules.
/// </summary>
public abstract class TestsBase : TestingBase, IDisposable
{
	#region Test lifecycle
	/// <summary>
	/// Creates a new instance of <see cref="TestsBase"/> with a blank app.
	/// NUnit initialize method.
	/// </summary>
	protected TestsBase() : this(AppBrix.App.Create(new MemoryConfigService())) { }

	/// <summary>
	/// Creates a new instance of <see cref="TestsBase"/> with the provided app.
	/// NUnit initialize method.
	/// </summary>
	/// <param name="app">The app to be tested.</param>
	protected TestsBase(IApp app)
	{
		this.Start(app);
		this.Initialize();
	}

	/// <summary>
	/// Xunit uninitialize method.
	/// </summary>
	public void Dispose()
	{
		this.Stop();
		GC.SuppressFinalize(this);
	}
	#endregion

	#region Public and overriden methods
	/// <summary>
	/// Gets a test runner specific assertion exception.
	/// </summary>
	/// <param name="message">The exception message.</param>
	/// <returns>The assert exception.</returns>
	protected override Exception GetAssertException(string message) => FailException.ForFailure(message);
	#endregion
}

/// <summary>
/// A base testing class that uses Xunit.
/// Used for testing an application with one module and its dependencies.
/// </summary>
public abstract class TestsBase<T> : TestsBase
	where T : class, IModule
{
	#region Test lifecycle
	/// <summary>
	/// Creates a new instance of <see cref="TestsBase{T}"/>
	/// NUnit initialize method.
	/// </summary>
	protected TestsBase() : base(TestApp.Create<T>())
	{
	}
	#endregion
}

/// <summary>
/// A base testing class that uses Xunit.
/// Used for testing an application with two modules and their dependencies.
/// </summary>
public abstract class TestsBase<T1, T2> : TestsBase
	where T1 : class, IModule
	where T2 : class, IModule
{
	#region Test lifecycle
	/// <summary>
	/// Creates a new instance of <see cref="TestsBase{T1,T2}"/>.
	/// NUnit initialize method.
	/// </summary>
	protected TestsBase() : base(TestApp.Create<T1, T2>())
	{
	}
	#endregion
}
