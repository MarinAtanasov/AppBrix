// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Configuration;
using AppBrix.Configuration.Memory;
using System;
using System.Linq;

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
        #endregion
    }
}
