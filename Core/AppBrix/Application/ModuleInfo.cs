// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Application
{
    /// <summary>
    /// Used for storing a part of module and its configuration element.
    /// </summary>
    internal sealed class ModuleInfo
    {
        #region Construction
        public ModuleInfo(IModule module, ModuleConfigElement config)
        {
            this.Module = module;
            this.Config = config;
            this.Status = config.Status;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the instance of the module which is used by the application.
        /// </summary>
        public IModule Module { get; }

        /// <summary>
        /// Gets the configuration element for the module.
        /// </summary>
        public ModuleConfigElement Config { get; }

        /// <summary>
        /// Gets the initial status of the module when this instance of <see cref="ModuleInfo"/> was created.
        /// </summary>
        public ModuleStatus Status { get; }
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Sorts the modules by assembly load priority based on assembly references.
        /// </summary>
        /// <param name="modules">The modules to be sorted.</param>
        /// <returns>The sorted modules.</returns>
        public static IEnumerable<ModuleInfo> SortByPriority(IEnumerable<ModuleInfo> modules)
        {
            var moduleToAssembly = modules.Select(x => Tuple.Create(x, x.Module.GetType().Assembly)).ToList();
            var assemblyReferences = ModuleInfo.GetAssemblyReferences(moduleToAssembly.Select(x => x.Item2).ToList());
            var sortedModuleInfos = new List<ModuleInfo>();
            var loaded = new HashSet<string>();
            var remaining = new LinkedList<Tuple<ModuleInfo, string>>(
                moduleToAssembly.Select(x => Tuple.Create(x.Item1, x.Item2.GetName().Name)));

            var item = remaining.First;
            while (item != null)
            {
                if (assemblyReferences[item.Value.Item2].All(loaded.Contains))
                {
                    sortedModuleInfos.Add(item.Value.Item1);
                    loaded.Add(item.Value.Item2);
                    remaining.Remove(item);
                    item = remaining.First;
                }
                else
                {
                    item = item.Next;
                }
            }

            return sortedModuleInfos;
        }
        #endregion

        #region Private methods
        private static Dictionary<string, HashSet<string>> GetAssemblyReferences(List<Assembly> assemblies)
        {
            var assemblyNames = new HashSet<string>(assemblies.Select(x => x.GetName().Name));
            return assemblies
                .Distinct(new AssemblyNameComparer())
                .ToDictionary(
                    x => x.GetName().Name,
                    x => new HashSet<string>(x.GetReferencedAssemblies().Select(a => a.Name).Where(assemblyNames.Contains)));
        }
        #endregion

        #region Private classes
        private class AssemblyNameComparer : IEqualityComparer<Assembly>
        {
            public bool Equals(Assembly x, Assembly y)
            {
                return x.GetName().Name.Equals(y.GetName().Name);
            }

            public int GetHashCode(Assembly obj)
            {
                return obj.GetName().Name.GetHashCode();
            }
        }
        #endregion
    }
}
