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
        public IModule Module { get; }

        public ModuleConfigElement Config { get; }

        public ModuleStatus Status { get; }
        #endregion
    }
}
