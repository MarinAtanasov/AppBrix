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
        /// Creates an app with an in-memory configuration using the provided module and its dependencies.
        /// </summary>
        /// <param name="module">The module to load inside the application.</param>
        /// <returns>The created application.</returns>
        public static IApp CreateTestApp(Type module)
        {
            var service = new MemoryConfigService();
            var config = service.Get<AppConfig>();
            module.GetReferencedModules()
                .Concat(new[] { module })
                .Select(ModuleConfigElement.Create)
                .ToList()
                .ForEach(config.Modules.Add);
            return App.Create(service);
        }
        #endregion
    }
}
