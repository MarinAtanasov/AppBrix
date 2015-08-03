// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Configuration.Memory
{
    /// <summary>
    /// Used for in-memory configuration.
    /// This module can be loaded instead of the default ConfigModule.
    /// Can be used for testing purposes.
    /// </summary>
    public sealed class MemoryConfigModule : ModuleBase
    {
        #region Properties
        public override int LoadPriority
        {
            get { return (int)ModuleLoadPriority.Config; }
        }
        #endregion

        #region Public methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetResolver().Register(this);
            this.App.GetResolver().Register(this.config.Value);
        }

        protected override void UninitializeModule()
        {
        }
        #endregion

        #region Private fields and constants
        private Lazy<MemoryConfig> config = new Lazy<MemoryConfig>();
        #endregion
    }
}
