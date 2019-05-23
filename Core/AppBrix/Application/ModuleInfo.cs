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
            var sortedModuleInfos = new List<ModuleInfo>();
            var loaded = new HashSet<Type>();
            var remaining = new LinkedList<(ModuleInfo info, Type type, List<Type> dependencies)>(
                modules.Select(x => (x, x.Module.GetType(), x.Module.Dependencies.ToList()))
            );

            var current = remaining.First;
            while (current != null)
            {
                if (current.Value.dependencies.All(loaded.Contains))
                {
                    sortedModuleInfos.Add(current.Value.info);
                    loaded.Add(current.Value.type);
                    remaining.Remove(current);
                    current = remaining.First;
                }
                else
                {
                    current = current.Next;
                }
            }

            if (remaining.Count > 0)
                throw new InvalidOperationException($@"The following modules have dependencies which have not been registered or enabled: {
                        string.Join(", ", remaining.Select(x => x.type.FullName))
                    }");

            return sortedModuleInfos;
        }
        #endregion
    }
}
