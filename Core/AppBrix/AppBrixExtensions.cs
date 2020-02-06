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
        public static AppConfig GetAppConfig(this IConfigService service) => (AppConfig) service.Get(typeof(AppConfig));
        
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

            var dependencies = new List<Type> { module.GetType() };
            var unique = new HashSet<Type> { module.GetType() };
            for (var i = 0; i < dependencies.Count; i++)
            {
                foreach (var dependency in dependencies[i].CreateObject<IModule>().Dependencies)
                {
                    if (unique.Add(dependency))
                    {
                        dependencies.Add(dependency);
                    }
                }
            }

            return dependencies;
        }
    }
}
