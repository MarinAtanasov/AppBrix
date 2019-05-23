// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix
{
    /// <summary>
    /// Extension methods for easier manipulation of AppBrix applications.
    /// </summary>
    public static class AppBrixExtensions
    {
        /// <summary>
        /// Unloads and reloads the application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void Restart(this IApp app)
        {
            app.Stop();
            app.Start();
        }

        /// <summary>
        /// Uninitializes and reinitializes the application.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void Reinitialize(this IApp app)
        {
            app.Uninitialize();
            app.Initialize();
        }

        /// <summary>
        /// Shorthand for getting the config from the currently defined <see cref="IConfigService"/>.
        /// </summary>
        /// <typeparam name="T">The type of the config.</typeparam>
        /// <param name="app">The current application.</param>
        /// <returns>The config.</returns>
        public static T GetConfig<T>(this IApp app) where T : class, IConfig
        {
            return app.ConfigService.Get<T>();
        }

        /// <summary>
        /// Shorthand for getting the config from the currently defined <see cref="IConfigService"/>.
        /// </summary>
        /// <param name="app">The current application.</param>
        /// <param name="type">The type of the config.</param>
        /// <returns>The config.</returns>
        public static IConfig GetConfig(this IApp app, Type type)
        {
            return app.ConfigService.Get(type);
        }

        /// <summary>
        /// Gets the types of all modules which the current module depends on.
        /// This method goes through the <see cref="IModule.Dependencies"/> recursively.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <returns>All modules inside the referenced assemblies.</returns>
        public static IEnumerable<Type> GetAllDependencies(this IModule module)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            var dependencies = new List<Type> { module.GetType() };
            var unique = new HashSet<Type> { module.GetType() };
            for (int i = 0; i < dependencies.Count; i++)
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
