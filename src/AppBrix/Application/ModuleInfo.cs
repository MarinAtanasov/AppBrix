﻿// Copyright (c) MarinAtanasov. All rights reserved.
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
            var moduleToAssembly = modules.Select(x => Tuple.Create(x, x.Module.GetType().GetTypeInfo().Assembly)).ToList();
            var assemblyReferences = ModuleInfo.GetAssemblyReferences(moduleToAssembly.Select(x => x.Item2).ToList());
            var sortedModuleInfos = new List<ModuleInfo>();
            var loaded = new HashSet<string>();
            var remaining = new LinkedList<Tuple<ModuleInfo, string>>(
                moduleToAssembly.Select(x => Tuple.Create(x.Item1, x.Item2.GetName().Name)));

            var item = remaining.First;
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
        private static IDictionary<string, HashSet<string>> GetAssemblyReferences(ICollection<Assembly> assemblies)
        {
            var assemblyNames = new HashSet<string>(assemblies.Select(x => x.GetName().Name));
            return assemblies
                .Distinct(new AssemblyNameComparer())
                .ToDictionary(
                    x => x.GetName().Name,
                    x => new HashSet<string>(x.GetReferencedAssemblies().Select(a => a.Name).Where(a => assemblyNames.Contains(a))));
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