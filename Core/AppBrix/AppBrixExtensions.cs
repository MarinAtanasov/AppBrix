// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Modules;
using System;
using System.Collections.Generic;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix applications.
    /// </summary>
    public static class AppBrixExtensions
    {
        /// <summary>
        /// Gets the <see cref="AppConfig"/> from <see cref="IConfigService"/>.
        /// </summary>
        /// <param name="service">The configuration service.</param>
        /// <returns>The <see cref="AppConfig"/>.</returns>
        public static AppConfig GetAppConfig(this IConfigService service) => (AppConfig)service.Get(typeof(AppConfig));

        /// <summary>
        /// Gets the types of all modules which the current module depends on.
        /// This method goes through the <see cref="IModule.Dependencies"/> recursively.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>All modules inside the referenced assemblies.</returns>
        public static IEnumerable<Type> GetAllDependencies(this IModule module)
        {
            if (module is null)
                throw new ArgumentNullException(nameof(module));

            var dependencies = new List<Type>();
            AppBrixExtensions.GetAllDependencies(module.GetType(), dependencies, new HashSet<Type>());
            return dependencies;
        }

        private static void GetAllDependencies(Type type, List<Type> dependencies, HashSet<Type> unique)
        {
            if (!unique.Add(type))
                return;

            foreach (var dependency in type.CreateObject<IModule>().Dependencies)
            {
                AppBrixExtensions.GetAllDependencies(dependency, dependencies, unique);
            }

            dependencies.Add(type);
        }
    }
}
