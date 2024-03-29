﻿// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Memory;
using AppBrix.Modules;
using System;
using System.Threading.Tasks;

namespace AppBrix.Testing.Xunit;

/// <summary>
/// A base testing class that holds an application.
/// The application is stopped on <see cref="Dispose"/>.
/// </summary>
public abstract class TestsBase : IDisposable
{
    #region Setup and cleanup
    /// <summary>
    /// Creates a new instance of <see cref="TestsBase"/> with a blank app.
    /// </summary>
    protected TestsBase() => this.App = AppBrix.App.Create(new MemoryConfigService());

    /// <summary>
    /// Creates a new instance of <see cref="TestsBase"/> with the provided app.
    /// </summary>
    /// <param name="app">The app to be tested.</param>
    protected TestsBase(IApp app) => this.App = app;

    /// <summary>
    /// Stops the application.
    /// </summary>
    public virtual void Dispose()
    {
        try { this.App.Stop(); } catch (InvalidOperationException) { }
        GC.SuppressFinalize(this);
    }
    #endregion

    #region Properties
    /// <summary>
    /// Gets the app that is being tested.
    /// </summary>
    protected IApp App { get; }
    #endregion

    #region Public and protected methods
    /// <summary>
    /// Checks that the action is executed under a specified time.
    /// It does one initial pass to make sure that the code path is loaded and hot.
    /// </summary>
    /// <param name="action">The action to be invoked.</param>
    /// <param name="duration">The maximum allowed duration.</param>
    /// <param name="firstPass">The maximum allowed duration on the first pass.</param>
    protected void AssertPerformance(Action action, TimeSpan duration = default, TimeSpan firstPass = default) =>
        action.AssertPerformance(duration, firstPass);

    /// <summary>
    /// Checks that the function is executed under a specified time.
    /// </summary>
    /// <param name="func">The function to be invoked.</param>
    /// <param name="duration">The maximum allowed duration.</param>
    /// <param name="firstPass">The maximum allowed duration on the first pass.</param>
    protected void AssertPerformance(Func<Task> func, TimeSpan duration = default, TimeSpan firstPass = default) =>
        func.AssertPerformance(duration, firstPass);
    #endregion
}

/// <summary>
/// A base testing class that holds an application.
/// The application will be created with one module and its dependencies.
/// The application is stopped on <see cref="IDisposable.Dispose"/>.
/// </summary>
public abstract class TestsBase<T> : TestsBase
    where T : class, IModule
{
    #region Setup and cleanup
    /// <summary>
    /// Creates a new instance of <see cref="TestsBase{T}"/>
    /// </summary>
    protected TestsBase() : base(TestApp.Create<T>())
    {
    }
    #endregion
}

/// <summary>
/// A base testing class that holds an application.
/// The application will be created with two modules and their dependencies.
/// The application is stopped on <see cref="IDisposable.Dispose"/>.
/// </summary>
public abstract class TestsBase<T1, T2> : TestsBase
    where T1 : class, IModule
    where T2 : class, IModule
{
    #region Setup and cleanup
    /// <summary>
    /// Creates a new instance of <see cref="TestsBase{T1,T2}"/>.
    /// </summary>
    protected TestsBase() : base(TestApp.Create<T1, T2>())
    {
    }
    #endregion
}
