// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Schedule.Timer.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Linq;

namespace AppBrix.Events.Schedule.Timer
{
    /// <summary>
    /// Module used for registering timer based scheduled events.
    /// </summary>
    public sealed class TimerScheduledEventsModule : ModuleBase
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
        private readonly Lazy<TimerScheduledEventHub> eventHub = new Lazy<TimerScheduledEventHub>();
        #endregion
    }
}