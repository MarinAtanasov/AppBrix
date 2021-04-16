// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
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

        /// <summary>
        /// Sorts the modules by assembly load priority based on assembly references.
        /// </summary>
        /// <param name="modules">The modules to be sorted.</param>
        /// <returns>The sorted modules.</returns>
        internal static IEnumerable<ModuleInfo> SortByPriority(this IEnumerable<ModuleInfo> modules)
        {
            var sortedModuleInfos = new List<ModuleInfo>();
            var remainingList = new LinkedList<(ModuleInfo info, Type type, List<Type> dependencies)>(
                modules.Select(x => (x, x.Module.GetType(), x.Module.Dependencies.ToList()))
            );
            var remainingHash = new HashSet<Type>(remainingList.Select(x => x.type));

            var current = remainingList.First;
            while (current != null)
            {
                if (current.Value.dependencies.Any(remainingHash.Contains))
                {
                    current = current.Next;
                }
                else
                {
                    sortedModuleInfos.Add(current.Value.info);
                    remainingHash.Remove(current.Value.type);
                    remainingList.Remove(current);
                    current = remainingList.First;
                }
            }

            return sortedModuleInfos;
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
