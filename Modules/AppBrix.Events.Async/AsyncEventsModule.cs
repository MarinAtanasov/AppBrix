// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Events.Async
{
    /// <summary>
    /// Module used for registering an asynchronous event hub.
    /// </summary>
    public sealed class AsyncEventsModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetContainer().Register(this);
            this.eventHub.Value.Initialize(context);
            this.App.GetContainer().Register(this.eventHub.Value);
        }

        protected override void UninitializeModule()
        {
            this.eventHub.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultAsyncEventHub> eventHub = new Lazy<DefaultAsyncEventHub>();
        #endregion
    }
}
