// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.

using AppBrix.Configuration.Memory;
using AppBrix.Modules;
using AppBrix.Testing.Modules;

namespace AppBrix.Testing;

/// <summary>
/// Contains utilities for managing test apps.
/// </summary>
public static class TestApp
{
    #region Public and overriden methods
    /// <summary>
    /// Creates an app with an in-memory configuration using the provided module and its dependencies.
    /// </summary>
    /// <typeparam name="T">The module to load inside the application.</typeparam>
    /// <returns>The created application.</returns>
    public static IApp Create<T>() where T : class, IModule =>
        App.Create<MainModule<T>>(new MemoryConfigService());

    /// <summary>
    /// Creates an app with an in-memory configuration using the provided module and its dependencies.
    /// </summary>
    /// <typeparam name="T1">The first module to load inside the application.</typeparam>
    /// <typeparam name="T2">The second module to load inside the application.</typeparam>
    /// <returns>The created application.</returns>
    public static IApp Create<T1, T2>() where T1 : class, IModule where T2 : class, IModule =>
        App.Create<MainModule<T1, T2>>(new MemoryConfigService());
    #endregion
}
