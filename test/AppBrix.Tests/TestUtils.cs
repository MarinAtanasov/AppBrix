// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Configuration.Memory;
using FluentAssertions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace AppBrix.Tests
{
    /// <summary>
    /// Contains commonly used testing utilities.
    /// </summary>
    public static class TestUtils
    {
        #region Public methods
        /// <summary>
        /// Creates an app with an in-memory configuration using the list of provided modules.
        /// </summary>
        /// <param name="modules">The modules to load inside the application.</param>
        /// <returns>The created application.</returns>
        public static IApp CreateTestApp(params Type[] modules)
        {
            var manager = new MemoryConfigManager();
            var config = manager.Get<AppConfig>();
            foreach (var module in modules)
            {
                config.Modules.Add(ModuleConfigElement.Create(module));
            }
            return App.Create(manager);
        }
        
        /// <summary>
        /// Asserts that a function will return a predefined value by a certain timeout.
        /// </summary>
        /// <typeparam name="T">The type of the result.</typeparam>
        /// <param name="func">The function to be called until it returns the predefined value or the timeout expires.</param>
        /// <param name="result">The result which should be returned by the function before the timeout expires.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="because">A formatted phrase explaining why the assertion should be satisfied. If the phrase does not start with the word because, it is prepended to the message.</param>
        /// <param name="becauseArgs">Zero or more values to use for filling in any <see cref="string.Format(string, object[])"/> compatible placeholders.</param>
        public static void ShouldReturn<T>(this Func<T> func, T result, TimeSpan timeout, string because = "", params object[] becauseArgs)
        {
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var sw = Stopwatch.StartNew();
            while (sw.Elapsed < timeout && !object.Equals(func(), result))
            {
                Thread.Sleep(1);
            }

            func().Should().Be(result, because, becauseArgs);
        }
        #endregion
    }
}
