// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Schedule.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Events.Schedule
{
    /// <summary>
    /// Base module used for registering scheduled events.
    /// </summary>
    public sealed class ScheduledEventsModule : ModuleBase
    {
        #region Public and overriden methods
        protected override void InitializeModule(IInitializeContext context)
        {
            this.App.Container.Register(this);
            this.eventHub.Value.Initialize(context);
            this.App.Container.Register(this.eventHub.Value);
        }

        protected override void UninitializeModule()
        {
            this.eventHub.Value.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly Lazy<DefaultScheduledEventHub> eventHub = new Lazy<DefaultScheduledEventHub>();
        #endregion
    }
}
