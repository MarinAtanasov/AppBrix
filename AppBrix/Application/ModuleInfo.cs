// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;
using AppBrix.Modules;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AppBrix.Application
{
    /// <summary>
    /// Used for storing a part of module and its configuration element.
    /// </summary>
    internal class ModuleInfo
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
        public IModule Module { get; }

        public ModuleConfigElement Config { get; }

        public ModuleStatus Status { get; }
        #endregion

        #region Public methods
        public static IEnumerable<ModuleInfo> SortByPriority(IEnumerable<ModuleInfo> modules)
        {
            var moduleToAssembly = modules.Select(x => Tuple.Create(x, x.Module.GetType().GetTypeInfo().Assembly.GetName().Name));
            var assemblyReferences = ModuleInfo.GetAssemblyReferences(new HashSet<string>(moduleToAssembly.Select(x => x.Item2)));
            var sortedModuleInfos = new List<ModuleInfo>();
            var loaded = new HashSet<string>();
            var remaining = new LinkedList<Tuple<ModuleInfo, string>>(moduleToAssembly);

            var item = remaining.First; ;
            while (item != null)
            {
                if (assemblyReferences[item.Value.Item2].All(x => loaded.Contains(x)))
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
        private static IDictionary<string, HashSet<string>> GetAssemblyReferences(HashSet<string> assemblies)
        {
            var libraryManager = PlatformServices.Default.LibraryManager;
            var assembliesToReferencing = assemblies
                .Select(x => Tuple.Create(
                    x, libraryManager.GetReferencingLibraries(x).Select(l => l.Name).Where(l => assemblies.Contains(l))))
                .ToList();

            var assemblyReferences = new Dictionary<string, HashSet<string>>();
            foreach (var assemblyToReferencing in assembliesToReferencing)
            {
                assemblyReferences.Add(assemblyToReferencing.Item1, new HashSet<string>());
            }
            foreach (var assemblyToReferencing in assembliesToReferencing)
            {
                foreach (var referencing in assemblyToReferencing.Item2)
                {
                    assemblyReferences[referencing].Add(assemblyToReferencing.Item1);
                }
            }

            return assemblyReferences;
        }
        #endregion
    }
}
