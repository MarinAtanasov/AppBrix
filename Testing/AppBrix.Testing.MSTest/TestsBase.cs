// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Memory;
using AppBrix.Modules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace AppBrix.Testing;

/// <summary>
/// A base testing class that uses MSTest.
/// Used for testing an application without preloaded modules.
/// </summary>
public abstract class TestsBase : TestingBase
{
    #region Test lifecycle
    /// <summary>
    /// MSTest initialize method.
    /// </summary>
    [TestInitialize]
    public virtual void Start()
    {
        this.Start(AppBrix.App.Create(new MemoryConfigService()));
        this.Initialize();
    }

    /// <summary>
    /// MSTest uninitialize method.
    /// </summary>
    [TestCleanup]
    public override void Stop() => base.Stop();
    #endregion

    #region Public and overriden methods
    /// <summary>
    /// Gets a test runner specific assertion exception.
    /// </summary>
    /// <param name="message">The exception message.</param>
    /// <returns>The assert exception.</returns>
    protected override Exception GetAssertException(string message) => new AssertFailedException(message);
    #endregion
}

/// <summary>
/// A base testing class that uses MSTest.
/// Used for testing an application with one module and its dependencies.
/// </summary>
public abstract class TestsBase<T> : TestsBase
    where T : class, IModule
{
    #region Test lifecycle
    /// <summary>
    /// MSTest initialize method.
    /// </summary>
    [TestInitialize]
    public override void Start()
    {
        this.Start(TestApp.Create<T>());
        this.Initialize();
    }
    #endregion
}

/// <summary>
/// A base testing class that uses MSTest.
/// Used for testing an application with two modules and their dependencies.
/// </summary>
public abstract class TestsBase<T1, T2> : TestsBase
    where T1 : class, IModule
    where T2 : class, IModule
{
    #region Test lifecycle
    /// <summary>
    /// MSTest initialize method.
    /// </summary>
    [TestInitialize]
    public override void Start()
    {
        this.Start(TestApp.Create<T1, T2>());
        this.Initialize();
    }
    #endregion
}
