// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Configuration
{
    /// <summary>
    /// Configuration which holds the modules collection.
    /// </summary>
    public sealed class AppConfig : IConfig
    {
        #region Properties
        /// <summary>
        /// Contains a collection of modules to be loaded by the application.
        /// </summary>
        public ICollection<ModuleConfigElement> Modules { get; } = new List<ModuleConfigElement>();
        #endregion
    }
}
