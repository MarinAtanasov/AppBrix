// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Events.Delayed.Impl;
using AppBrix.Lifecycle;
using AppBrix.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppBrix.Events.Delayed
{
    /// <summary>
    /// Module used for registering a delayed event hub.
    /// </summary>
    public sealed class DelayedEventsModule : ModuleBase
    {
        #region Properties
        /// <summary>
        /// Gets the types of the modules which are direct dependencies for the current module.
        /// This is used to determine the order in which the modules are loaded.
        /// </summary>
        public override IEnumerable<Type> Dependencies => new[] { typeof(EventsModule) };
        #endregion

        #region Public and overriden methods
        /// <summary>
        /// Initializes the module.
        /// Automatically called by <see cref="ModuleBase.Initialize"/>
        /// </summary>
        /// <param name="context">The initialization context.</param>
        protected override void Initialize(IInitializeContext context)
        {
            this.App.Container.Register(this);

            this.eventHub = this.App.GetEventHub();

            this.delayedEventHub.Initialize(context);
            this.App.Container.Register(this.delayedEventHub);
        }

        /// <summary>
        /// Uninitializes the module.
        /// Automatically called by <see cref="ModuleBase.Uninitialize"/>
        /// </summary>
        protected override void Uninitialize()
        {
            if (this.eventHub != null)
            {
                this.App.Container.Register(this.eventHub);
                this.eventHub = null;
            }

            this.delayedEventHub.Uninitialize();
        }
        #endregion

        #region Private fields and constants
        private readonly DefaultDelayedEventHub delayedEventHub = new DefaultDelayedEventHub();
        private IEventHub eventHub;
        #endregion
    }
}
