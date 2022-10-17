// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Memory;
using AppBrix.Modules;
using AppBrix.Tests.Mocks;
using FluentAssertions;
using System;

namespace AppBrix.Tests;

/// <summary>
/// Contains commonly used testing utilities.
/// </summary>
public static class TestUtils
{
    #region Public and overriden methods
    /// <summary>
    /// Checks that the action is executed under a specified time.
    /// </summary>
    /// <param name="action">The action to be invoked.</param>
    public static void AssertPerformance(Action action)
    {
        // Invoke the action once to make sure that the assemblies are loaded.
        action.ExecutionTime().Should().BeLessThan(TimeSpan.FromMilliseconds(5000), "this is a performance test");

        GC.Collect();

        action.ExecutionTime().Should().BeLessThan(TimeSpan.FromMilliseconds(100), "this is a performance test");
    }

    /// <summary>
    /// Creates an app with an in-memory configuration using the provided module and its dependencies.
    /// </summary>
    /// <typeparam name="T">The module to load inside the application.</typeparam>
    /// <returns>The created application.</returns>
    public static IApp CreateTestApp<T>() where T : class, IModule =>
        App.Create<MainModule<T>>(new MemoryConfigService());

    /// <summary>
    /// Creates an app with an in-memory configuration using the provided module and its dependencies.
    /// </summary>
    /// <typeparam name="T1">The first module to load inside the application.</typeparam>
    /// <typeparam name="T2">The second module to load inside the application.</typeparam>
    /// <returns>The created application.</returns>
    public static IApp CreateTestApp<T1, T2>() where T1 : class, IModule where T2 : class, IModule =>
        App.Create<MainModule<T1, T2>>(new MemoryConfigService());
    #endregion
}
