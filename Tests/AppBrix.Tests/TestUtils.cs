// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Configuration.Memory;
using AppBrix.Modules;
using FluentAssertions;
using System;
using System.Linq;
using AppBrix.Tests.Mocks;

namespace AppBrix.Tests
{
    /// <summary>
    /// Contains commonly used testing utilities.
    /// </summary>
    public static class TestUtils
    {
        #region Public and overriden methods

        /// <summary>
        /// Creates an app with an in-memory configuration using the provided module and its dependencies.
        /// </summary>
        /// <typeparam name="T">The module to load inside the application.</typeparam>
        /// <returns>The created application.</returns>
        public static IApp CreateTestApp<T>() where T : IModule => App.Create<MainModuleMock<T>>(new MemoryConfigService());

        /// <summary>
        /// Creates an app with an in-memory configuration using the provided module and its dependencies.
        /// </summary>
        /// <typeparam name="T">The module to load inside the application.</typeparam>
        /// <returns>The created application.</returns>
        public static IApp CreateTestApp<T1, T2>() where T1 : IModule where T2 : IModule => App.Create<MainModuleMock<T1, T2>>(new MemoryConfigService());

        public static void TestPerformance(Action action)
        {
            // Invoke the action once to make sure that the assemblies are loaded.
            action.ExecutionTime().Should().BeLessThan(TimeSpan.FromMilliseconds(5000), "this is a performance test");

            GC.Collect();

            action.ExecutionTime().Should().BeLessThan(TimeSpan.FromMilliseconds(100), "this is a performance test");
        }
        #endregion
    }
}
