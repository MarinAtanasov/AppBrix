// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Memory;
using AppBrix.Modules;
using NUnit.Framework;
using System;

namespace AppBrix.Testing;

/// <summary>
/// A base testing class that uses NUnit.
/// Used for testing an application without preloaded modules.
/// </summary>
public abstract class TestsBase : TestingBase
{
    #region Test lifecycle
    /// <summary>
    /// NUnit initialize method.
    /// </summary>
    [SetUp]
    public virtual void Start()
    {
        this.Start(AppBrix.App.Create(new MemoryConfigService()));
        this.Initialize();
    }

    /// <summary>
    /// NUnit uninitialize method.
    /// </summary>
    [TearDown]
    public override void Stop() => base.Stop();
    #endregion

    #region Public and overriden methods
    /// <summary>
    /// Gets a test runner specific assertion exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <returns>The assert exception.</returns>
    protected override Exception GetAssertException(string message) => new AssertionException(message);
    #endregion
}

/// <summary>
/// A base testing class that uses NUnit.
/// Used for testing an application with one module and its dependencies.
/// </summary>
public abstract class TestsBase<T> : TestsBase
    where T : class, IModule
{
    #region Test lifecycle
    /// <summary>
    /// NUnit initialize method.
    /// </summary>
    [SetUp]
    public override void Start()
    {
        this.Start(TestApp.Create<T>());
        this.Initialize();
    }
    #endregion
}

/// <summary>
/// A base testing class that uses NUnit.
/// Used for testing an application with two modules and their dependencies.
/// </summary>
public abstract class TestsBase<T1, T2> : TestsBase
    where T1 : class, IModule
    where T2 : class, IModule
{
    #region Test lifecycle
    /// <summary>
    /// NUnit initialize method.
    /// </summary>
    [SetUp]
    public override void Start()
    {
        this.Start(TestApp.Create<T1, T2>());
        this.Initialize();
    }
    #endregion
}
