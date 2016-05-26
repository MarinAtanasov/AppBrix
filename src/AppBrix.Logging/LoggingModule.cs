// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Logging
{
    /// <summary>
    /// Modules used for registering a logs manager.
    /// </summary>
    public sealed class LoggingModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            var defaultLogHub = this.logHub.Value;
            defaultLogHub.Initialize(context);
            this.App.GetContainer().Register(defaultLogHub);
        }

        protected override void UninitializeModule()
        {
            this.logHub.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultLogHub> logHub = new Lazy<DefaultLogHub>();
        #endregion
    }
}
