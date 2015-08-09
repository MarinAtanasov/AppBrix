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
        public IModule Module { get; private set; }

        public ModuleConfigElement Config { get; private set; }

        public ModuleStatus Status { get; private set; }
        #endregion

        #region Public methods
        public static IEnumerable<ModuleInfo> SortByPriority(IEnumerable<ModuleInfo> modules)
        {
            var result = new List<ModuleInfo>();
            var allAssemblies = new HashSet<string>(modules.Select(m => m.Module.GetType().Assembly.GetName().Name));
            var previousWaves = new HashSet<string>();
            var remaining = new LinkedList<ModuleInfo>(modules);

            while (remaining.Count > 0)
            {
                var currentWave = new List<string>();
                LinkedListNode<ModuleInfo> previousItem = null;
                LinkedListNode<ModuleInfo> item = remaining.First; ;
                while (item != null)
                {
                    var assembly = item.Value.Module.GetType().Assembly;
                    if (assembly.GetReferencedAssemblies().All(a => !allAssemblies.Contains(a.Name) || previousWaves.Contains(a.Name)))
                    {
                        result.Add(item.Value);
                        currentWave.Add(assembly.GetName().Name);
                        remaining.Remove(item);
                    }
                    else
                    {
                        previousItem = item;
                    }
                    item = previousItem != null ? previousItem.Next : remaining.First;
                }

                foreach (var waveItem in currentWave)
                {
                    previousWaves.Add(waveItem);
                }
            }

            return result;
        }
        #endregion
    }
}
