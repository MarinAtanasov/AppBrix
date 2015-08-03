// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Application;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Events
{
    /// <summary>
    /// Module used for registering an event hub.
    /// </summary>
    public sealed class EventsModule : ModuleBase
    {
        #region Properies
        public override int LoadPriority
        {
            get
            {
                return (int)ModuleLoadPriority.Events;
            }
        }
        #endregion

        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.GetResolver().Register(this);
            this.eventHub.Value.Initialize(context);
            this.App.GetResolver().Register(this.eventHub.Value);
        }

        protected override void UninitializeModule()
        {
            this.eventHub.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private Lazy<DefaultEventHub> eventHub = new Lazy<DefaultEventHub>();
        #endregion
    }
}
