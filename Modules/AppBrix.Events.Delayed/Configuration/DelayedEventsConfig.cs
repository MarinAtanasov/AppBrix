// Copyright (c) MarinAtanasov. All rights reserved.
// Licensed under the MIT License (MIT). See License.txt in the project root for license information.
//
using AppBrix.Configuration;

namespace AppBrix.Events.Delayed.Configuration
{
    /// <summary>
    /// Configuration which sets how the delayed events should behave.
    /// </summary>
    public sealed class DelayedEventsConfig : IConfig
    {
        #region Construction
        /// <summary>
        /// Creates a new instance of <see cref="DelayedEventsConfig"/>.
        /// </summary>
        public DelayedEventsConfig()
        {
            this.DefaultBehavior = EventBehavior.Immediate;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the default behavior for the events.
        /// </summary>
        public EventBehavior DefaultBehavior { get; set; }
        #endregion
    }
}
