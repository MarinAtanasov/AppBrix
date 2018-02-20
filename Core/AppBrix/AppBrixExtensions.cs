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
        /// Gets an enumeration of the classes which implement <see cref="IModule"/>
        /// inside the assemblies referenced by the module type's assembly.
        /// </summary>
        /// <param name="type">The type of the module.</param>
        /// <returns>All modules inside the referenced assemblies.</returns>
        public static IEnumerable<Type> GetReferencedModules(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (!typeof(IModule).IsAssignableFrom(type))
                throw new ArgumentException($"Type {type} is not of type {nameof(IModule)}.");

            return type.Assembly.GetAllReferencedAssemblies()
                .SelectMany(assembly => assembly.ExportedTypes)
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(typeof(IModule).IsAssignableFrom)
                .OrderBy(t => t.Namespace)
                .ThenBy(t => t.Name);
        }
    }
}
